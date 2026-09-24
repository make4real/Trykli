"""
Approximate re-implementation of the TRYKLI runtime simulation (Assets/Scripts/Gameplay + Mechanics) used to
design and pre-tune the 100 levels outside Unity.

It mirrors the gameplay rules exactly (fixed 0.02 s step, mechanisms stepped before physics, spring / fan /
magnet / bomb / bumper / portal / laser / button / door / gravity rules, stuck and out-of-bounds failures) and
approximates Box2D for the rigid body part (circle vs boxes / circles, restitution, Coulomb friction with
rolling). Final tuning must be confirmed in Unity with "Tools > TRYKLI > Verify Level Solutions".
"""
import math

DT = 0.02
GRAVITY_ACCEL = 9.81
TRYKLI_R = 0.35
TRYKLI_MASS = 1.0
TRYKLI_INERTIA = 0.5 * TRYKLI_MASS * TRYKLI_R * TRYKLI_R
TRYKLI_FRICTION = 0.4
TRYKLI_BOUNCE = 0.15
DEFAULT_FRICTION = 0.5
DEFAULT_BOUNCE = 0.05
RESTITUTION_THRESHOLD = 1.0
MAX_SPEED = 30.0
STUCK_SPEED = 0.15
STUCK_TIME = 3.0
IDLE_OBJECTIVE_SPEED = 0.5
OOB_MARGIN = 1.5
ANGULAR_DAMPING = 0.05

# Element type ids (must match ElementType in Assets/Scripts/Data/LevelEnums.cs)
BLOCK, SPIKES, CRYSTAL, SPRING, RAMP, FAN, PORTAL, MAGNET, BOMB, BUMPER, MOVING, ROTATING, BUTTON, DOOR, LASER, \
    GRAVITY, STICKY, SLIPPERY, CANNON, MARKER, MINIPLATFORM = range(21)
SURF_GROUND, SURF_WALL, SURF_CEILING, SURF_OBSTACLE, SURF_PLATFORM = range(5)
ACT_ALWAYS, ACT_ACTIVATED, ACT_DEACTIVATED, ACT_TOGGLED = range(4)
GRAV_TOGGLE, GRAV_INVERT, GRAV_NORMAL = range(3)


def v_add(a, b): return (a[0] + b[0], a[1] + b[1])
def v_sub(a, b): return (a[0] - b[0], a[1] - b[1])
def v_mul(a, s): return (a[0] * s, a[1] * s)
def v_dot(a, b): return a[0] * b[0] + a[1] * b[1]
def v_len(a): return math.hypot(a[0], a[1])


def v_norm(a):
    length = v_len(a)
    return (a[0] / length, a[1] / length) if length > 1e-9 else (0.0, 0.0)


def up_of(rotation):
    rad = math.radians(rotation + 90.0)
    return (math.cos(rad), math.sin(rad))


def right_of(rotation):
    rad = math.radians(rotation)
    return (math.cos(rad), math.sin(rad))


def to_local(p, origin, rotation):
    d = v_sub(p, origin)
    rad = math.radians(-rotation)
    c, s = math.cos(rad), math.sin(rad)
    return (d[0] * c - d[1] * s, d[0] * s + d[1] * c)


def to_world(local, origin, rotation):
    rad = math.radians(rotation)
    c, s = math.cos(rad), math.sin(rad)
    return (origin[0] + local[0] * c - local[1] * s, origin[1] + local[0] * s + local[1] * c)


def closest_on_segment(p, a, b):
    ab = v_sub(b, a)
    ll = v_dot(ab, ab)
    if ll < 1e-12:
        return a
    t = max(0.0, min(1.0, v_dot(v_sub(p, a), ab) / ll))
    return v_add(a, v_mul(ab, t))


def circle_overlaps_box(c, r, center, half, rot):
    lx, ly = to_local(c, center, rot)
    dx = max(abs(lx) - half[0], 0.0)
    dy = max(abs(ly) - half[1], 0.0)
    return dx * dx + dy * dy <= r * r


def swept_circle_box(a, b, r, center, half, rot):
    dist = v_len(v_sub(b, a))
    samples = max(1, min(32, int(math.ceil(dist / max(0.05, r * 0.5)))))
    for i in range(samples + 1):
        p = v_add(a, v_mul(v_sub(b, a), i / samples))
        if circle_overlaps_box(p, r, center, half, rot):
            return True
    return False


def swept_circle_circle(a, b, r, center, radius):
    closest = closest_on_segment(center, a, b)
    return v_len(v_sub(closest, center)) <= r + radius


def point_in_box(p, center, half, rot):
    lx, ly = to_local(p, center, rot)
    return abs(lx) <= half[0] and abs(ly) <= half[1]


class Shape:
    """Physical collider seen by Trykli."""

    def __init__(self, kind, center, half=(0, 0), rot=0.0, radius=0.0, friction=DEFAULT_FRICTION, bounce=DEFAULT_BOUNCE,
                 surface=None, owner=None):
        self.kind = kind  # 'box' or 'circle'
        self.center = center
        self.half = half
        self.rot = rot
        self.radius = radius
        self.friction = friction
        self.bounce = bounce
        self.surface = surface
        self.owner = owner
        self.enabled = True
        self.velocity = (0.0, 0.0)
        self.angular = 0.0  # rad/s (rotating platforms)
        self.pivot = center

    def contact(self, p, r):
        """Returns (normal, penetration) when the circle (p, r) touches the shape, else None."""
        if self.kind == 'circle':
            d = v_sub(p, self.center)
            dist = v_len(d)
            reach = r + self.radius
            if dist >= reach:
                return None
            n = v_norm(d) if dist > 1e-6 else (0.0, 1.0)
            return n, reach - dist
        lx, ly = to_local(p, self.center, self.rot)
        hx, hy = self.half
        cx = max(-hx, min(hx, lx))
        cy = max(-hy, min(hy, ly))
        if abs(lx) <= hx and abs(ly) <= hy:
            # center inside the box: push along the smallest penetration axis
            px, py = hx - abs(lx), hy - abs(ly)
            if px < py:
                nl = (1.0 if lx >= 0 else -1.0, 0.0)
                pen = px + r
            else:
                nl = (0.0, 1.0 if ly >= 0 else -1.0)
                pen = py + r
            n = v_sub(to_world(nl, (0, 0), self.rot), (0, 0))
            return n, pen
        d = (lx - cx, ly - cy)
        dist = v_len(d)
        if dist >= r:
            return None
        nl = v_norm(d)
        n = to_world(nl, (0, 0), self.rot)
        return n, r - dist

    def surface_velocity(self, point):
        vx, vy = self.velocity
        if self.angular != 0.0:
            rx, ry = v_sub(point, self.pivot)
            vx += -self.angular * ry
            vy += self.angular * rx
        return (vx, vy)


class Result:
    def __init__(self):
        self.success = False
        self.failure = None
        self.time = 0.0
        self.crystals = set()
        self.bounces = {'any': 0, 'spring': 0, 'bumper': 0}
        self.counters = {}
        self.contacts = {}
        self.hazards = {'any': 0, 'spikes': 0, 'laser': 0, 'explosion': 0}
        self.longest_idle = 0.0
        self.trace = []
        self.min_goal_dist = 99.0
        self.closest_point = None

    def inc(self, key, amount=1):
        self.counters[key] = self.counters.get(key, 0) + amount

    def __repr__(self):
        if self.success:
            return "OK t=%.2f crystals=%d bounces=%s" % (self.time, len(self.crystals), self.bounces)
        return "FAIL(%s) t=%.2f" % (self.failure, self.time)


class Simulation:
    """One run of a level (layout elements + placed items)."""

    def __init__(self, level, placed_elements, record_trace=False):
        self.level = level
        self.layout = level['layout']
        self.elements = [dict(e) for e in self.layout['elements']] + [dict(e) for e in placed_elements]
        self.result = Result()
        self.record_trace = record_trace
        self.time = 0.0
        self.pos = (self.layout['spawn']['x'], self.layout['spawn']['y'])
        self.prev = self.pos
        self.vel = (0.0, 0.0)
        self.omega = 0.0
        self.force = (0.0, 0.0)
        self.gravity_sign = 1.0
        self.damping = 0.0
        self.sticky = 0
        self.alive = True
        self.captured = False
        self.stuck_timer = 0.0
        self.idle_timer = 0.0
        self.next_teleport = 0.0
        self.switch_listeners = {}
        self.shapes = []
        self.done = False
        b = self.layout['bounds']
        m = OOB_MARGIN
        self.kill_box = (b['x'] - m, b['y'] - m, b['x'] + b['width'] + m, b['y'] + b['height'] + m)
        self.goal = (self.layout['goal']['x'], self.layout['goal']['y'])
        self.goal_radius = self.layout['goalRadius']
        self.limit = level.get('timeLimit', 15.0)
        for e in self.elements:
            self._setup(e)

    # ------------------------------------------------------------------ setup
    def _setup(self, e):
        t = e['type']
        p = (e['position']['x'], e['position']['y'])
        rot = e['rotation']
        size = (e['size']['x'], e['size']['y'])
        e['_p'] = p
        e['_rot'] = rot
        e['_active'] = e['activation'] != ACT_ACTIVATED
        e['_state'] = {}
        if e['channel'] >= 0 and e['activation'] != ACT_ALWAYS and t not in (PORTAL, BUTTON):
            self.switch_listeners.setdefault(e['channel'], []).append(e)
        if t == BLOCK:
            e['_shape'] = self._box(p, size, rot, surface=e['surface'], owner=e)
        elif t == SLIPPERY:
            s = size if size[0] > 0 else (3.0, 0.4)
            e['_shape'] = self._box(p, s, rot, friction=0.0, bounce=0.0, surface=e['surface'], owner=e)
        elif t in (RAMP, MINIPLATFORM):
            s = size if size[0] > 0 else ((2.4, 0.22) if t == RAMP else (1.4, 0.25))
            e['_shape'] = self._box(p, s, rot, surface=SURF_PLATFORM, owner=e)
        elif t == SPRING:
            s = size if size[0] > 0 else (1.0, 0.5)
            e['_size'] = s
            e['_speed'] = e['power'] if e['power'] > 0 else 11.0
            center = to_world((0.0, -s[1] * 0.15), p, rot)
            e['_shape'] = self._box(center, (s[0], s[1] * 0.7), rot, owner=e)
            e['_state']['last'] = -10.0
        elif t == FAN:
            e['_accel'] = e['power'] if e['power'] > 0 else 16.0
            e['_len'] = e['length'] if e['length'] > 0 else 4.0
            e["_width"] = size[0] if size[0] > 0 else 1.6
            e['_shape'] = self._box(p, (0.9, 0.45), rot, owner=e)
        elif t == MAGNET:
            e['_radius'] = e['radius'] if e['radius'] > 0 else 3.5
            e['_strength'] = e['power'] if e['power'] > 0 else 22.0
            e['_shape'] = self._circle(p, 0.35, owner=e)
        elif t == BOMB:
            e['_radius'] = e['radius'] if e['radius'] > 0 else 2.5
            e['_impulse'] = e['power'] if e['power'] > 0 else 11.0
            e['_shape'] = self._circle(p, 0.35, owner=e)
            e['_state'].update({'exploded': False, 'scheduled': -1.0})
        elif t == BUMPER:
            e['_radius'] = e['radius'] if e['radius'] > 0 else 0.45
            e['_speed'] = e['power'] if e['power'] > 0 else 10.0
            e['_shape'] = self._circle(p, e['_radius'], owner=e)
            e['_state']['last'] = -10.0
        elif t == MOVING:
            s = size if size[0] > 0 else (2.0, 0.3)
            e['_shape'] = self._box(p, s, rot, surface=SURF_PLATFORM, owner=e)
            e['_state'].update({'start': p, 'time': 0.0})
            e['_period'] = e['period'] if e['period'] > 0 else 4.0
        elif t == ROTATING:
            s = size if size[0] > 0 else (2.4, 0.3)
            e['_shape'] = self._box(p, s, rot, surface=SURF_PLATFORM, owner=e)
            e['_state'].update({'start': rot, 'time': 0.0})
            e['_speed'] = e['speed'] if abs(e['speed']) > 0 else 45.0
        elif t == BUTTON:
            s = size if size[0] > 0 else (0.9, 0.2)
            e['_size'] = s
            center = to_world((0.0, -s[1] * 0.25), p, rot)
            e['_shape'] = self._box(center, (s[0], s[1] * 0.5), rot, owner=e)
            e['_state'].update({'pressed': False, 'on': False})
        elif t == DOOR:
            s = size if size[0] > 0 else (0.35, 2.4)
            if e['activation'] == ACT_ALWAYS:
                e['activation'] = ACT_ACTIVATED
                e['_active'] = False
                if e['channel'] >= 0:
                    self.switch_listeners.setdefault(e['channel'], []).append(e)
            e['_shape'] = self._box(p, s, rot, surface=SURF_OBSTACLE, owner=e)
            e['_shape'].enabled = not e['_active']
        elif t == LASER:
            e['_len'] = e['length'] if e['length'] > 0 else 4.0
            e['_state']['last'] = -10.0
        elif t == SPIKES:
            s = size if size[0] > 0 else (1.0, 0.4)
            e['_size'] = s
            center = to_world((0.0, -s[1] * 0.2), p, rot)
            e['_shape'] = self._box(center, (s[0], s[1] * 0.6), rot, owner=e)
            e['_state']['last'] = -10.0
        elif t == GRAVITY:
            e['_radius'] = e['radius'] if e['radius'] > 0 else 0.4
            e['_state'].update({'inside': False, 'used': False})
        elif t == STICKY:
            e['_size'] = size if size[0] > 0 else (2.0, 1.0)
            e['_damping'] = e['power'] if e['power'] > 0 else 5.0
            e['_state']['inside'] = False
        elif t == CANNON:
            e['_speed'] = e['power'] if e['power'] > 0 else 12.0
            e['_delay'] = e['delay'] if e['delay'] > 0 else 0.6
            e['_radius'] = e['radius'] if e['radius'] > 0 else 0.5
            e['_state'].update({'mode': 'ready', 'loaded': 0.0})
        elif t == PORTAL:
            e['_radius'] = e['radius'] if e['radius'] > 0 else 0.45
            e['_retention'] = e['power'] if e['power'] > 0 else 0.95
            e['_state']['ignore'] = False
        elif t == MARKER:
            e['_size'] = size if size[0] > 0 else (1.0, 1.0)
            e['_state']['reached'] = False
        elif t == CRYSTAL:
            e['_state']['collected'] = False

    def _box(self, center, size, rot, friction=DEFAULT_FRICTION, bounce=DEFAULT_BOUNCE, surface=None, owner=None):
        shape = Shape('box', center, (size[0] / 2.0, size[1] / 2.0), rot, friction=friction, bounce=bounce, surface=surface, owner=owner)
        self.shapes.append(shape)
        return shape

    def _circle(self, center, radius, owner=None):
        shape = Shape('circle', center, radius=radius, owner=owner)
        self.shapes.append(shape)
        return shape

    # ------------------------------------------------------------------ helpers
    def speed(self):
        return v_len(self.vel)

    def launch(self, velocity, source):
        if not self.alive or self.captured:
            return
        self.vel = velocity
        self.omega = 0.0
        self.stuck_timer = 0.0
        if source:
            self.result.bounces[source] += 1
            self.result.bounces['any'] += 1

    def accelerate(self, a):
        self.force = v_add(self.force, a)

    def fail(self, reason):
        if self.done:
            return
        self.done = True
        self.result.failure = reason

    def kill(self, reason):
        if not self.alive:
            return
        self.alive = False
        self.fail(reason)

    def emit(self, channel):
        for e in list(self.switch_listeners.get(channel, [])):
            mode = e['activation']
            if mode == ACT_ACTIVATED:
                self._set_active(e, True)
            elif mode == ACT_DEACTIVATED:
                self._set_active(e, False)
            elif mode == ACT_TOGGLED:
                self._set_active(e, not e['_active'])

    def _set_active(self, e, active):
        if e['_active'] == active:
            return
        e['_active'] = active
        t = e['type']
        if t == DOOR:
            e['_shape'].enabled = not active
            if active:
                self.result.inc('door_open')
        elif t == LASER and not active:
            self.result.inc('laser_off')
        elif t == BOMB and active and e['activation'] == ACT_ACTIVATED:
            st = e['_state']
            if st['scheduled'] < 0 or self.time < st['scheduled']:
                st['scheduled'] = self.time

    # ------------------------------------------------------------------ run
    def run(self, max_time=None):
        limit = max_time or self.limit
        steps = int(math.ceil(limit / DT)) + 1
        for _ in range(steps):
            if self.done:
                break
            self.step()
            if self.record_trace:
                self.result.trace.append(self.pos)
        if not self.done:
            self.result.failure = 'timeout'
        self.result.time = self.time
        return self.result

    def step(self):
        # 1. elements, in hierarchy order: layout elements, goal, placed items, then Trykli
        for e in self.elements:
            if self.done:
                break
            self._step_element(e)
        if not self.done:
            self._step_goal()
        if not self.done:
            self._step_trykli()
        self.time += DT
        if self.done:
            return
        if self.time >= self.limit - 1e-9:
            self.fail('timeout')
            return
        # 2. physics
        self.prev = self.pos
        self._physics()

    def _step_goal(self):
        closest = closest_on_segment(self.goal, self.prev, self.pos)
        dist = v_len(v_sub(closest, self.goal))
        if dist < self.result.min_goal_dist:
            self.result.min_goal_dist = dist
            self.result.closest_point = closest
        if dist <= self.goal_radius and self.alive:
            self.done = True
            self.result.success = True

    def _step_trykli(self):
        if not self.alive or self.captured:
            self.stuck_timer = 0.0
            return
        sp = self.speed()
        if sp > MAX_SPEED:
            self.vel = v_mul(self.vel, MAX_SPEED / sp)
            sp = MAX_SPEED
        # idle tracking for objectives
        if sp < IDLE_OBJECTIVE_SPEED:
            self.idle_timer += DT
            self.result.longest_idle = max(self.result.longest_idle, self.idle_timer)
        else:
            self.idle_timer = 0.0
        x, y = self.pos
        k = self.kill_box
        if not (k[0] <= x <= k[2] and k[1] <= y <= k[3]):
            self.kill('out')
            return
        if sp < STUCK_SPEED:
            self.stuck_timer += DT
            if self.stuck_timer >= STUCK_TIME:
                self.fail('stuck')
        else:
            self.stuck_timer = 0.0

    def _step_element(self, e):
        t = e['type']
        st = e['_state']
        p = e['_p']
        rot = e['_rot']
        if t == SPRING:
            if not e['_active'] or not self.alive or self.captured or self.time - st['last'] < 0.25:
                return
            s = e['_size']
            lx, ly = to_local(self.pos, p, rot)
            top = s[1] * 0.5
            if abs(lx) <= s[0] * 0.5 + TRYKLI_R * 0.3 and top - 0.15 <= ly <= top + TRYKLI_R + 0.18:
                st['last'] = self.time
                self.launch(v_mul(up_of(rot), e['_speed']), 'spring')
        elif t == FAN:
            if not e['_active'] or not self.alive:
                return
            lx, ly = to_local(self.pos, p, rot)
            start = 0.225 - 0.1
            if start <= ly <= start + e['_len'] and abs(lx) <= e['_width'] * 0.5:
                self.accelerate(v_mul(up_of(rot), e['_accel']))
        elif t == MAGNET:
            if not e['_active'] or not self.alive:
                return
            d = v_sub(p, self.pos)
            dist = v_len(d)
            if 0.05 < dist <= e['_radius']:
                f = 0.4 + 0.6 * (1.0 - dist / e['_radius'])
                self.accelerate(v_mul(d, e['_strength'] * f / dist))
        elif t == BOMB:
            if st['exploded']:
                return
            if st['scheduled'] >= 0 and self.time >= st['scheduled']:
                self._explode(e)
                return
            if e['delay'] > 0 and e['activation'] != ACT_ACTIVATED and self.time >= e['delay']:
                self._explode(e)
                return
            if self.alive and swept_circle_circle(self.prev, self.pos, TRYKLI_R, p, 0.4):
                self._explode(e)
        elif t == BUMPER:
            if not e['_active'] or not self.alive or self.time - st['last'] < 0.15:
                return
            if swept_circle_circle(self.prev, self.pos, TRYKLI_R, p, e['_radius'] + 0.04):
                d = v_sub(self.pos, p)
                direction = v_norm(d) if v_len(d) > 1e-4 else up_of(rot)
                st['last'] = self.time
                self.launch(v_mul(direction, e['_speed']), 'bumper')
        elif t == PORTAL:
            self._step_portal(e)
        elif t == MOVING:
            if not e['_active']:
                return
            st['time'] += DT
            tt = st['time'] - e['delay']
            if tt < 0:
                return
            if e['loop']:
                s = 0.5 - 0.5 * math.cos(2 * math.pi * (tt / e['_period'] + e['phase']))
            else:
                u = max(0.0, min(1.0, tt / (e['_period'] * 0.5)))
                s = u * u * (3 - 2 * u)
            target = v_add(st['start'], v_mul((e['travel']['x'], e['travel']['y']), s))
            shape = e['_shape']
            shape.velocity = v_mul(v_sub(target, shape.center), 1.0 / DT)
            shape.target = target
        elif t == ROTATING:
            if not e['_active']:
                return
            st['time'] += DT
            if e['length'] > 0:
                angle = st['start'] + math.sin(math.radians(st['time'] * e['_speed'])) * e['length']
            else:
                angle = st['start'] + e['_speed'] * st['time']
            shape = e['_shape']
            shape.angular = math.radians(angle - shape.rot) / DT
            shape.target_rot = angle
        elif t == BUTTON:
            if not self.alive:
                return
            s = e['_size']
            lx, ly = to_local(self.pos, p, rot)
            touching = abs(lx) <= s[0] * 0.5 + TRYKLI_R * 0.3 and -s[1] * 0.5 <= ly <= s[1] * 0.5 + TRYKLI_R + 0.1
            if touching and not st['on'] and (not st['pressed'] or not e['oneShot']):
                st['pressed'] = (not st['pressed']) if not e['oneShot'] else True
                self.result.inc('button')
                if e['id']:
                    self.result.inc('button:' + e['id'])
                self.emit(e['emitChannel'])
            st['on'] = touching
        elif t == LASER:
            if not self.alive:
                return
            on = e['_active']
            if on and e['onDuration'] > 0 and e['offDuration'] > 0:
                cycle = e['onDuration'] + e['offDuration']
                on = ((self.time + e['phase']) % cycle) < e['onDuration']
            if not on:
                return
            up = up_of(rot)
            center = v_add(p, v_mul(up, 0.2 + e['_len'] * 0.5))
            if swept_circle_box(self.prev, self.pos, TRYKLI_R * 0.9, center, (0.07, e['_len'] * 0.5), rot):
                self.result.hazards['laser'] += 1
                self.result.hazards['any'] += 1
                if e['lethal']:
                    self.kill('laser')
                elif self.time - st['last'] >= 0.3:
                    st['last'] = self.time
                    lx, _ = to_local(self.pos, center, rot)
                    self.launch(v_mul(right_of(rot), 5.0 if lx >= 0 else -5.0), None)
        elif t == SPIKES:
            if not self.alive:
                return
            s = e['_size']
            if swept_circle_box(self.prev, self.pos, TRYKLI_R + 0.02, p, (s[0] * 0.5, s[1] * 0.5), rot):
                self.result.hazards['spikes'] += 1
                self.result.hazards['any'] += 1
                if e['lethal']:
                    self.kill('spikes')
                elif self.time - st['last'] >= 0.3:
                    st['last'] = self.time
                    self.launch(v_mul(up_of(rot), 6.0), None)
        elif t == GRAVITY:
            if not e['_active'] or not self.alive:
                return
            inside = swept_circle_circle(self.prev, self.pos, TRYKLI_R * 0.6, p, e['_radius'])
            if inside and not st['inside'] and not (e['oneShot'] and st['used']):
                st['used'] = True
                mode = e['gravityMode']
                current = self.gravity_sign < 0
                nxt = (not current) if mode == GRAV_TOGGLE else (mode == GRAV_INVERT)
                if nxt != current:
                    self.gravity_sign = -1.0 if nxt else 1.0
                    self.result.inc('gravity_flip')
            st['inside'] = inside
        elif t == STICKY:
            if not self.alive:
                return
            inside = e['_active'] and point_in_box(self.pos, p, (e['_size'][0] / 2, e['_size'][1] / 2), rot)
            if inside != st['inside']:
                st['inside'] = inside
                self.sticky += 1 if inside else -1
                self.damping = e['_damping'] if self.sticky > 0 else 0.0
        elif t == CANNON:
            if not e['_active'] or not self.alive:
                return
            mode = st['mode']
            if mode == 'ready':
                if swept_circle_circle(self.prev, self.pos, TRYKLI_R * 0.5, p, e['_radius']):
                    self.captured = True
                    self.pos = p
                    self.prev = p
                    self.vel = (0.0, 0.0)
                    st['mode'] = 'loaded'
                    st['loaded'] = self.time
                    self.result.inc('cannon')
            elif mode == 'loaded':
                if self.time - st['loaded'] >= e['_delay']:
                    up = up_of(rot)
                    self.captured = False
                    self.pos = v_add(p, v_mul(up, e['_radius'] + TRYKLI_R + 0.35))
                    self.prev = self.pos
                    st['mode'] = 'cooldown'
                    self.launch(v_mul(up, e['_speed']), None)
            elif v_len(v_sub(self.pos, p)) > e['_radius'] + TRYKLI_R + 0.5:
                st['mode'] = 'ready'
        elif t == CRYSTAL:
            if st['collected'] or not self.alive:
                return
            closest = closest_on_segment(p, self.prev, self.pos)
            if v_len(v_sub(closest, p)) <= 0.32 + TRYKLI_R:
                st['collected'] = True
                self.result.crystals.add(e['id'])
        elif t == MARKER:
            if st['reached'] or not self.alive:
                return
            s = e['_size']
            if point_in_box(self.pos, p, (s[0] / 2, s[1] / 2), rot):
                st['reached'] = True
                self.result.inc('zone:' + e['id'])

    def _step_portal(self, e):
        st = e['_state']
        if not self.alive or self.captured:
            return
        p = e['_p']
        dist = v_len(v_sub(self.pos, p))
        if st['ignore']:
            if dist > e['_radius'] + TRYKLI_R + 0.1:
                st['ignore'] = False
            return
        if self.time < self.next_teleport:
            return
        if not swept_circle_circle(self.prev, self.pos, TRYKLI_R * 0.5, p, e['_radius']):
            return
        partner = None
        for other in self.elements:
            if other is not e and other['type'] == PORTAL and other['channel'] == e['channel']:
                partner = other
                break
        if partner is None:
            return
        up = up_of(partner['_rot'])
        speed = max(self.speed() * e['_retention'], 3.0)
        partner['_state']['ignore'] = True
        self.next_teleport = self.time + 0.1
        self.pos = v_add(partner['_p'], v_mul(up, 0.7))
        self.prev = self.pos
        self.vel = v_mul(up, speed)
        self.stuck_timer = 0.0
        self.result.inc('portal')
        self.result.inc('portal:%d' % e['channel'])

    def _explode(self, e):
        st = e['_state']
        st['exploded'] = True
        e['_shape'].enabled = False
        self.result.inc('explosion')
        p = e['_p']
        if self.alive:
            d = v_sub(self.pos, p)
            dist = v_len(d)
            if dist <= e['_radius']:
                if e['lethal'] and dist < e['_radius'] * 0.5:
                    self.result.hazards['explosion'] += 1
                    self.result.hazards['any'] += 1
                    self.kill('explosion')
                else:
                    direction = v_mul(d, 1.0 / dist) if dist > 0.01 else up_of(e['_rot'])
                    falloff = 1.0 - 0.5 * (dist / e['_radius'])
                    self.vel = v_add(self.vel, v_mul(direction, e['_impulse'] * falloff))
                    self.stuck_timer = 0.0
        for other in self.elements:
            if other is e or other['type'] != BOMB or other['_state']['exploded']:
                continue
            if v_len(v_sub(other['_p'], p)) <= e['_radius']:
                sched = other['_state']['scheduled']
                when = self.time + 0.12
                if sched < 0 or when < sched:
                    other['_state']['scheduled'] = when

    # ------------------------------------------------------------------ physics (Box2D approximation)
    def _physics(self):
        # move kinematic shapes to their target (MovePosition / MoveRotation)
        for s in self.shapes:
            if hasattr(s, 'target'):
                s.center = s.target
                del s.target
            elif s.kind == 'box' and s.owner is not None and s.owner['type'] == MOVING:
                s.velocity = (0.0, 0.0)
            if hasattr(s, 'target_rot'):
                s.rot = s.target_rot
                del s.target_rot
            elif s.owner is not None and s.owner['type'] == ROTATING:
                s.angular = 0.0
        if not self.alive or self.captured:
            self.force = (0.0, 0.0)
            return
        g = (0.0, -GRAVITY_ACCEL * self.gravity_sign)
        self.vel = v_add(self.vel, v_mul(v_add(g, v_mul(self.force, 1.0 / TRYKLI_MASS)), DT))
        self.force = (0.0, 0.0)
        if self.damping > 0:
            self.vel = v_mul(self.vel, 1.0 / (1.0 + DT * self.damping))
        self.omega *= 1.0 / (1.0 + DT * ANGULAR_DAMPING)

        # velocity constraints with resting / approaching contacts
        self._solve_contacts(self.pos, apply_position=False)

        # integrate position with sub steps (continuous collision approximation)
        disp = v_mul(self.vel, DT)
        n = max(1, int(math.ceil(v_len(disp) / (TRYKLI_R * 0.4))))
        for _ in range(n):
            self.pos = v_add(self.pos, v_mul(self.vel, DT / n))
            self._solve_contacts(self.pos, apply_position=True)

    def _solve_contacts(self, p, apply_position):
        for s in self.shapes:
            if not s.enabled:
                continue
            c = s.contact(p, TRYKLI_R + (0.0 if apply_position else 0.01))
            if c is None:
                continue
            n, pen = c
            if apply_position:
                p = v_add(p, v_mul(n, max(0.0, pen - 0.005)))
                self.pos = p
            contact_point = v_sub(p, v_mul(n, TRYKLI_R))
            rc = v_mul(n, -TRYKLI_R)
            surf = s.surface_velocity(contact_point)
            vc = v_sub(v_add(self.vel, (-self.omega * rc[1], self.omega * rc[0])), surf)
            vn = v_dot(vc, n)
            if vn >= 0:
                continue
            if apply_position and s.surface is not None and -vn > 0.3:
                # new impact: record surface contact for objectives
                if not getattr(s, '_touching', False):
                    self.result.contacts[s.surface] = self.result.contacts.get(s.surface, 0) + 1
            if apply_position:
                s._touching = True
            e = max(TRYKLI_BOUNCE, s.bounce) if -vn > RESTITUTION_THRESHOLD else 0.0
            jn = -(1.0 + e) * vn * TRYKLI_MASS
            tangent = (-n[1], n[0])
            vt = v_dot(vc, tangent)
            mu = math.sqrt(TRYKLI_FRICTION * s.friction)
            kt = 1.0 / TRYKLI_MASS + (TRYKLI_R * TRYKLI_R) / TRYKLI_INERTIA
            jt = -vt / kt
            jt = max(-mu * jn, min(mu * jn, jt))
            impulse = v_add(v_mul(n, jn), v_mul(tangent, jt))
            self.vel = v_add(self.vel, v_mul(impulse, 1.0 / TRYKLI_MASS))
            # torque = rc x (jt * t)
            cross = rc[0] * (tangent[1] * jt) - rc[1] * (tangent[0] * jt)
            self.omega += cross / TRYKLI_INERTIA
        if not apply_position:
            for s in self.shapes:
                if getattr(s, '_touching', False) and s.contact(self.pos, TRYKLI_R + 0.02) is None:
                    s._touching = False


def simulate(level, placed_elements, record_trace=False, max_time=None):
    return Simulation(level, placed_elements, record_trace).run(max_time)
