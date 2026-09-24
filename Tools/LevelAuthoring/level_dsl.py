"""
Small DSL used to author the TRYKLI levels. Each level is described with helper calls and exported to the JSON
format read by Unity (LevelDefinition / LevelLayout / ElementData / PlacementZoneData / ObjectiveDefinition).
Enum values are written as integers (JsonUtility format) and every field is written explicitly.
"""
import copy
import trykli_sim as sim

# Objective comparisons (Comparison enum)
AT_LEAST, AT_MOST, EXACTLY = 0, 1, 2
# Tutorial kinds
TUTO_NONE, TUTO_DRAG, TUTO_ROTATE, TUTO_RESTART = 0, 1, 2, 3
# Zone shapes
POINT, RECT, RAIL = 0, 1, 2

ITEM_PIECES = {'portal_ab': 2, 'portal_cd': 2}
ITEM_TYPE = {
    'spring': sim.SPRING, 'ramp': sim.RAMP, 'fan': sim.FAN, 'portal_ab': sim.PORTAL, 'portal_cd': sim.PORTAL,
    'magnet': sim.MAGNET, 'bomb': sim.BOMB, 'bumper': sim.BUMPER, 'mini_platform': sim.MINIPLATFORM,
    'gravity_switch': sim.GRAVITY,
}
ROTATABLE = {'spring': 15, 'ramp': 15, 'fan': 45, 'portal_ab': 45, 'portal_cd': 45, 'mini_platform': 15}


def vec(x, y):
    return {'x': round(float(x), 3), 'y': round(float(y), 3)}


def element(kind, x, y, **kw):
    e = {
        'type': kind, 'id': kw.get('id', ''), 'position': vec(x, y), 'rotation': float(kw.get('rot', 0.0)),
        'size': vec(*kw.get('size', (0, 0))), 'surface': kw.get('surface', sim.SURF_GROUND),
        'channel': kw.get('channel', -1), 'emitChannel': kw.get('emit', -1), 'activation': kw.get('activation', sim.ACT_ALWAYS),
        'power': float(kw.get('power', 0.0)), 'radius': float(kw.get('radius', 0.0)), 'length': float(kw.get('length', 0.0)),
        'travel': vec(*kw.get('travel', (0, 0))), 'period': float(kw.get('period', 0.0)),
        'onDuration': float(kw.get('on', 0.0)), 'offDuration': float(kw.get('off', 0.0)), 'phase': float(kw.get('phase', 0.0)),
        'delay': float(kw.get('delay', 0.0)), 'speed': float(kw.get('speed', 0.0)), 'lethal': bool(kw.get('lethal', True)),
        'loop': bool(kw.get('loop', True)), 'oneShot': bool(kw.get('one_shot', True)),
        'gravityMode': kw.get('gravity', sim.GRAV_TOGGLE), 'endpoint': kw.get('endpoint', ''),
    }
    return e


def placed_element(item_id, x, y, rot, endpoint=''):
    """Element created by the runtime for a placed item (ElementFactory.DataForItem)."""
    kind = ITEM_TYPE[item_id]
    kw = {'rot': rot, 'surface': sim.SURF_PLATFORM, 'id': 'placed_' + item_id}
    if kind == sim.PORTAL:
        kw['channel'] = 0 if item_id == 'portal_ab' else 1
        kw['endpoint'] = endpoint or 'A'
    if kind == sim.BOMB:
        kw['lethal'] = False
    if kind == sim.GRAVITY:
        kw['one_shot'] = False
    return element(kind, x, y, **kw)


def objective(kind, count=0, value=0.0, comparison=AT_LEAST, target='', invert=False, key=''):
    return {'type': kind, 'count': int(count), 'value': float(value), 'comparison': comparison, 'target': target,
            'invert': bool(invert), 'descriptionKey': key}


def Complete(): return objective('Complete')
def MaxObjects(n, key=''): return objective('MaxObjects', count=n, key=key)
def UseItem(item, n, cmp=AT_LEAST, key=''): return objective('ItemUsage', count=n, comparison=cmp, target=item, key=key)
def UseAll(n, key=''): return objective('ItemUsage', count=n, comparison=AT_LEAST, target='', key=key)
def OnlyItem(item, key=''): return objective('ItemUsage', count=0, comparison=AT_MOST, target=item, invert=True, key=key)
def Crystals(n=1, crystal_id='', key=''): return objective('CollectCrystals', count=n, target=crystal_id, key=key)
def AllCrystals(key=''): return objective('CollectCrystals', count=0, key=key)
def Time(seconds, key=''): return objective('Time', value=seconds, key=key)
def Bounces(n, cmp=EXACTLY, source='Any', key=''): return objective('BounceCount', count=n, comparison=cmp, target=source, key=key)
def NoHazard(category='Any', key=''): return objective('NoHazard', target=category, key=key)
def Avoid(surface='Wall', key=''): return objective('AvoidContact', target=surface, key=key)
def Interaction(counter, n=1, cmp=AT_LEAST, key=''): return objective('Interaction', count=n, comparison=cmp, target=counter, key=key)
def Attempts(n, key=''): return objective('Attempts', count=n, key=key)
def NoStop(seconds=1.0, key=''): return objective('NoStop', value=seconds, key=key)


SURFACES = {'Ground': sim.SURF_GROUND, 'Wall': sim.SURF_WALL, 'Ceiling': sim.SURF_CEILING, 'Obstacle': sim.SURF_OBSTACLE,
            'Platform': sim.SURF_PLATFORM}


class Level:
    def __init__(self, level_id, name_fr, name_en, difficulty, time_limit=15.0, boss=False):
        self.id = level_id
        self.world = (level_id - 1) // 10 + 1
        self.number = (level_id - 1) % 10 + 1
        self.name_fr = name_fr
        self.name_en = name_en
        self.difficulty = difficulty
        self.time_limit = time_limit
        self.boss = boss or self.number == 10
        self.tutorial = TUTO_NONE
        self.spawn_pos = (-3.5, 7.5)
        self.goal_pos = (3.5, -3.0)
        self.goal_radius = 0.7
        self.bounds = (-5.0, -5.0, 10.0, 14.0)
        self.elements = []
        self.zones = []
        self.inventory = []
        self.objectives = [Complete()]
        self.solution_steps = []
        self.hints = []
        self.tip_fr = ''
        self.tip_en = ''
        self.notes = ''
        self.custom_texts = {}
        self._crystal_count = 0
        self.path_crystals = []
        self.gravity_hint = 1  # -1 when the interesting apex is a lowest point (inverted gravity)
        self.auto_goal = False  # allow the build to move the exit onto the best intended trajectory
        self.goal_on_path = False  # place the exit on the reference trajectory so that every item is required
        self.tune_zones = {}  # zone index -> (dx, dy) range explored to make every item necessary
        self.goal_region = None  # (xmin, ymin, xmax, ymax) where goal_on_path may put the exit
        self.report = {}

    # ---------------------------------------------------------------- geometry
    def spawn(self, x, y):
        self.spawn_pos = (x, y)
        return self

    def goal(self, x, y, r=0.7):
        self.goal_pos = (x, y)
        self.goal_radius = r
        return self

    def add(self, kind, x, y, **kw):
        self.elements.append(element(kind, x, y, **kw))
        return self.elements[-1]

    def block(self, x, y, w, h, rot=0.0, surface='Ground'):
        return self.add(sim.BLOCK, x, y, size=(w, h), rot=rot, surface=SURFACES[surface])

    def ground(self, x0, x1, top=-4.0, h=1.0):
        return self.block((x0 + x1) / 2.0, top - h / 2.0, x1 - x0, h, surface='Ground')

    def ceiling(self, x0, x1, bottom=8.5, h=0.6):
        return self.block((x0 + x1) / 2.0, bottom + h / 2.0, x1 - x0, h, surface='Ceiling')

    def wall(self, x, y0, y1, w=0.5):
        return self.block(x, (y0 + y1) / 2.0, w, y1 - y0, surface='Wall')

    def obstacle(self, x, y, w, h, rot=0.0):
        return self.block(x, y, w, h, rot=rot, surface='Obstacle')

    def platform(self, x, y, w, h=0.4, rot=0.0):
        return self.block(x, y, w, h, rot=rot, surface='Platform')

    def spikes(self, x, y, w, rot=0.0, lethal=True, h=0.4):
        return self.add(sim.SPIKES, x, y, size=(w, h), rot=rot, lethal=lethal)

    def crystal(self, x, y, cid=None):
        self._crystal_count += 1
        return self.add(sim.CRYSTAL, x, y, id=cid or 'c%d' % self._crystal_count)

    def path_crystal(self, where, dy=0.0, cid=None):
        """Crystal placed on the reference trajectory: 'apex' or a fraction (0..1) of the run time."""
        self.path_crystals.append((where, dy, cid))
        return self

    def spring(self, x, y, rot=0.0, power=0.0, **kw):
        return self.add(sim.SPRING, x, y, rot=rot, power=power, **kw)

    def ramp(self, x, y, rot=0.0, w=2.4):
        return self.add(sim.RAMP, x, y, rot=rot, size=(w, 0.22))

    def fan(self, x, y, rot=0.0, power=0.0, length=0.0, width=0.0, **kw):
        return self.add(sim.FAN, x, y, rot=rot, power=power, length=length, size=(width, 0.45) if width else (0, 0), **kw)

    def portal(self, x, y, rot, channel, endpoint):
        return self.add(sim.PORTAL, x, y, rot=rot, channel=channel, endpoint=endpoint)

    def magnet(self, x, y, radius=0.0, power=0.0, **kw):
        return self.add(sim.MAGNET, x, y, radius=radius, power=power, **kw)

    def bomb(self, x, y, radius=0.0, power=0.0, lethal=False, **kw):
        return self.add(sim.BOMB, x, y, radius=radius, power=power, lethal=lethal, **kw)

    def bumper(self, x, y, power=0.0, radius=0.0):
        return self.add(sim.BUMPER, x, y, power=power, radius=radius)

    def moving(self, x, y, w, travel, period=4.0, phase=0.0, delay=0.0, loop=True, h=0.3, **kw):
        return self.add(sim.MOVING, x, y, size=(w, h), travel=travel, period=period, phase=phase, delay=delay, loop=loop, **kw)

    def rotating(self, x, y, w, speed=45.0, rot=0.0, oscillation=0.0, h=0.3, **kw):
        return self.add(sim.ROTATING, x, y, size=(w, h), speed=speed, rot=rot, length=oscillation, **kw)

    def button(self, x, y, emit, bid='', rot=0.0, one_shot=True):
        return self.add(sim.BUTTON, x, y, emit=emit, id=bid, rot=rot, one_shot=one_shot)

    def door(self, x, y, channel, h=2.4, w=0.35, rot=0.0):
        return self.add(sim.DOOR, x, y, size=(w, h), channel=channel, activation=sim.ACT_ACTIVATED, rot=rot)

    def laser(self, x, y, rot=0.0, length=4.0, on=0.0, off=0.0, phase=0.0, lethal=True, channel=-1, activation=sim.ACT_ALWAYS):
        return self.add(sim.LASER, x, y, rot=rot, length=length, on=on, off=off, phase=phase, lethal=lethal, channel=channel,
                        activation=activation)

    def gravity(self, x, y, mode=sim.GRAV_TOGGLE, one_shot=False):
        return self.add(sim.GRAVITY, x, y, gravity=mode, one_shot=one_shot)

    def sticky(self, x, y, w, h, power=0.0):
        return self.add(sim.STICKY, x, y, size=(w, h), power=power)

    def slippery(self, x, y, w, h=0.4, rot=0.0):
        return self.add(sim.SLIPPERY, x, y, size=(w, h), rot=rot)

    def cannon(self, x, y, rot=0.0, power=0.0, delay=0.0):
        return self.add(sim.CANNON, x, y, rot=rot, power=power, delay=delay)

    def marker(self, x, y, w, h, mid):
        return self.add(sim.MARKER, x, y, size=(w, h), id=mid)

    # ---------------------------------------------------------------- zones & inventory
    def zone(self, shape, x, y, items=None, size=(0, 0), rot=0.0, default_rot=0.0, allow_rot=True, min_rot=-180.0,
             max_rot=180.0, capacity=1, zid='', linked=-1):
        self.zones.append({
            'id': zid or 'z%d' % len(self.zones), 'shape': shape, 'position': vec(x, y), 'size': vec(*size),
            'rotation': float(rot), 'allowedItems': list(items or []), 'allowRotation': bool(allow_rot),
            'minRotation': float(min_rot), 'maxRotation': float(max_rot), 'defaultRotation': float(default_rot),
            'capacity': int(capacity), 'linkedChannel': int(linked),
        })
        return len(self.zones) - 1

    def slot(self, x, y, items=None, default_rot=0.0, allow_rot=True, min_rot=-180.0, max_rot=180.0, linked=-1):
        return self.zone(POINT, x, y, items, default_rot=default_rot, allow_rot=allow_rot, min_rot=min_rot, max_rot=max_rot,
                         linked=linked)

    def area(self, x, y, w, h, items=None, capacity=2, default_rot=0.0, allow_rot=True, min_rot=-180.0, max_rot=180.0):
        return self.zone(RECT, x, y, items, size=(w, h), capacity=capacity, default_rot=default_rot, allow_rot=allow_rot,
                         min_rot=min_rot, max_rot=max_rot)

    def rail(self, x, y, length, rot=0.0, items=None, capacity=2, default_rot=0.0, allow_rot=True, min_rot=-180.0, max_rot=180.0):
        return self.zone(RAIL, x, y, items, size=(length, 0), rot=rot, capacity=capacity, default_rot=default_rot,
                         allow_rot=allow_rot, min_rot=min_rot, max_rot=max_rot)

    def items(self, **counts):
        self.inventory = [{'itemId': k, 'count': int(v)} for k, v in counts.items()]
        return self

    def stars(self, second, third):
        self.objectives = [Complete(), second, third]
        return self

    def solution(self, *steps):
        """steps: (item, zone_index, rotation) or (item, zone_index, rotation, (x, y))."""
        self.solution_steps = []
        for step in steps:
            item, zone_index = step[0], step[1]
            rotation = None if step[2] is None else float(step[2])
            z = self.zones[zone_index]
            pos = step[3] if len(step) > 3 else (z['position']['x'], z['position']['y'])
            self.solution_steps.append({'itemId': item, 'zoneIndex': zone_index, 'position': vec(*pos), 'rotation': rotation})
        return self

    def tip(self, fr, en):
        self.tip_fr, self.tip_en = fr, en
        return self

    def text(self, key, fr, en):
        """Custom localized text (objective descriptions specific to this level)."""
        self.custom_texts[key] = (fr, en)
        return key

    # ---------------------------------------------------------------- export
    def placed_elements(self, steps=None):
        steps = self.solution_steps if steps is None else steps
        placed = []
        pieces = {}
        for step in steps:
            item = step['itemId']
            n = pieces.get(item, 0)
            pieces[item] = n + 1
            endpoint = ('A' if n % 2 == 0 else 'B') if item in ITEM_PIECES else ''
            zone = self.zones[step['zoneIndex']]
            pos = (zone['position']['x'], zone['position']['y']) if zone['shape'] == POINT else (step['position']['x'], step['position']['y'])
            rot = step['rotation'] if zone['allowRotation'] and item in ROTATABLE else zone['defaultRotation']
            e = placed_element(item, pos[0], pos[1], rot, endpoint)
            if zone.get('linkedChannel', -1) >= 0 and item not in ITEM_PIECES:
                e['activation'] = sim.ACT_ACTIVATED
                e['channel'] = zone['linkedChannel']
            placed.append(e)
        return placed

    def units_used(self, steps=None):
        steps = self.solution_steps if steps is None else steps
        pieces = {}
        for step in steps:
            pieces[step['itemId']] = pieces.get(step['itemId'], 0) + 1
        return {k: (v + ITEM_PIECES.get(k, 1) - 1) // ITEM_PIECES.get(k, 1) for k, v in pieces.items()}

    def to_definition(self):
        b = self.bounds
        return {
            'levelId': self.id, 'worldId': self.world, 'levelNumber': self.number,
            'nameKey': 'level.%03d.name' % self.id, 'difficulty': self.difficulty, 'isBoss': self.boss,
            'timeLimit': float(self.time_limit), 'tutorial': self.tutorial,
            'layout': {
                'spawn': vec(*self.spawn_pos), 'goal': vec(*self.goal_pos), 'goalRadius': self.goal_radius,
                'bounds': {'x': b[0], 'y': b[1], 'width': b[2], 'height': b[3]},
                'elements': copy.deepcopy(self.elements), 'zones': copy.deepcopy(self.zones),
            },
            'inventory': copy.deepcopy(self.inventory),
            'objectives': copy.deepcopy(self.objectives),
            'solution': copy.deepcopy(self.solution_steps),
            'hints': copy.deepcopy(self.hints),
            'tipKey': ('level.%03d.tip' % self.id) if self.tip_fr else '',
            'camera': {'autoFit': True, 'orthographicSize': 10.0, 'followTrykli': False},
            'notes': self.notes,
        }

    # ---------------------------------------------------------------- simulation helpers
    def simulate(self, steps=None, trace=False):
        definition = self.to_definition()
        return sim.simulate(definition, self.placed_elements(steps), record_trace=trace)

    def evaluate(self, result, steps=None):
        units = self.units_used(steps)
        total_units = sum(units.values())
        crystals_total = sum(1 for e in self.elements if e['type'] == sim.CRYSTAL)
        met = []
        for o in self.objectives:
            met.append(evaluate_objective(o, result, units, total_units, crystals_total))
        return met


def compare(cmp, value, target):
    if cmp == AT_LEAST:
        return value >= target
    if cmp == AT_MOST:
        return value <= target
    return value == target


def evaluate_objective(o, r, units, total_units, crystals_total):
    t = o['type']
    if not r.success:
        return False
    if t == 'Complete':
        return True
    if t == 'MaxObjects':
        return total_units <= o['count']
    if t == 'ItemUsage':
        if not o['target']:
            used = total_units
        elif o['invert']:
            used = total_units - units.get(o['target'], 0)
        else:
            used = units.get(o['target'], 0)
        return compare(o['comparison'], used, o['count'])
    if t == 'CollectCrystals':
        if o['target']:
            return o['target'] in r.crystals
        need = crystals_total if o['count'] <= 0 else o['count']
        return len(r.crystals) >= need
    if t == 'Time':
        return r.time <= o['value']
    if t == 'BounceCount':
        return compare(o['comparison'], r.bounces[o['target'].lower()], o['count'])
    if t == 'NoHazard':
        return r.hazards[o['target'].lower()] == 0
    if t == 'AvoidContact':
        return r.contacts.get(SURFACES[o['target']], 0) == 0
    if t == 'Interaction':
        return compare(o['comparison'], r.counters.get(o['target'], 0), o['count'])
    if t == 'Attempts':
        return True
    if t == 'NoStop':
        return r.longest_idle <= o['value']
    raise ValueError(t)
