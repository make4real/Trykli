"""
Design helpers: explore placements of a level with the approximate simulator.

- search_intent(level, intent): tries every rotation / position of the given (item, zone) skeleton and returns
  the successful placements sorted by stars then time. Used to tune geometry and to pick reference solutions.
- random_success_rate(level): fraction of random placements that succeed (rough difficulty indicator).
"""
import itertools
import random

from level_dsl import POINT, RECT, RAIL, ROTATABLE, vec
import trykli_sim as sim


def zone_candidates(level, item, zone_index, grid=0.5):
    z = level.zones[zone_index]
    if z['allowedItems'] and item not in z['allowedItems']:
        return []
    if z['allowRotation'] and item in ROTATABLE:
        step = ROTATABLE[item]
        lo, hi = z['minRotation'], z['maxRotation']
        if hi - lo >= 359:
            lo, hi = -180 + step, 180
        rotations = sorted({int(round(r)) for r in frange(lo, hi, step)})
    else:
        rotations = [int(z['defaultRotation'])]
    px, py = z['position']['x'], z['position']['y']
    positions = []
    if z['shape'] == POINT:
        positions = [(px, py)]
    elif z['shape'] == RECT:
        hw, hh = z['size']['x'] / 2.0, z['size']['y'] / 2.0
        for dx in frange(-hw, hw, grid):
            for dy in frange(-hh, hh, grid):
                positions.append(sim.to_world((dx, dy), (px, py), z['rotation']))
    else:
        import math
        half = z['size']['x'] / 2.0
        axis = (math.cos(math.radians(z['rotation'])), math.sin(math.radians(z['rotation'])))
        for d in frange(-half, half, grid):
            positions.append((px + axis[0] * d, py + axis[1] * d))
    return [(zone_index, (round(p[0], 2), round(p[1], 2)), float(r)) for p in positions for r in rotations]


def frange(lo, hi, step):
    values = []
    v = lo
    while v <= hi + 1e-6:
        values.append(round(v, 3))
        v += step
    if not values:
        values = [lo]
    return values


def steps_from(combo, items):
    return [{'itemId': item, 'zoneIndex': c[0], 'position': vec(*c[1]), 'rotation': c[2]} for item, c in zip(items, combo)]


def _run_chunk(args):
    level, items, combos = args
    out = []
    best_miss = None
    for combo in combos:
        steps = steps_from(combo, items)
        result = level.simulate(steps)
        if result.success:
            result.trace = []
            out.append((sum(level.evaluate(result, steps)), result.time, steps, result))
        elif best_miss is None or result.min_goal_dist < best_miss[0]:
            best_miss = (result.min_goal_dist, result.closest_point, steps, result.failure)
    if not out and best_miss is not None:
        out.append(('miss', best_miss))
    return out


_POOL = None


def _pool():
    global _POOL
    if _POOL is None:
        import multiprocessing
        _POOL = multiprocessing.Pool()
    return _POOL


def search_intent(level, intent, grid=0.5, limit=20000, stop_after=None, parallel=True):
    """intent: list of (item, zone_index). Returns [(stars, time, steps, result)] for successful placements."""
    candidate_lists = [zone_candidates(level, item, zi, grid) for item, zi in intent]
    items = [item for item, _ in intent]
    combos = list(itertools.islice(itertools.product(*candidate_lists), limit))
    count = len(combos)
    if parallel and count > 64:
        size = max(16, count // 32)
        chunks = [(level, items, combos[i:i + size]) for i in range(0, count, size)]
        found = [f for part in _pool().map(_run_chunk, chunks) for f in part]
    else:
        found = _run_chunk((level, items, combos))
    misses = [f[1] for f in found if f[0] == 'miss']
    found = [f for f in found if f[0] != 'miss']
    if not found and misses:
        level.best_miss = min(misses, key=lambda m: m[0])
    found.sort(key=lambda f: (-f[0], f[1]))
    if stop_after:
        found = found[:stop_after]
    return found, count


def random_success_rate(level, samples=200, seed=1):
    rng = random.Random(seed)
    pool = []
    for entry in level.inventory:
        pieces = entry['count'] * (2 if entry['itemId'].startswith('portal') else 1)
        pool += [entry['itemId']] * pieces
    success = 0
    for _ in range(samples):
        steps = []
        used = {}
        for item in pool:
            if rng.random() < 0.25:
                continue
            options = []
            for zi in range(len(level.zones)):
                if used.get(zi, 0) >= level.zones[zi]['capacity']:
                    continue
                options += zone_candidates(level, item, zi, grid=0.5)
            if not options:
                continue
            c = rng.choice(options)
            used[c[0]] = used.get(c[0], 0) + 1
            steps.append({'itemId': item, 'zoneIndex': c[0], 'position': vec(*c[1]), 'rotation': c[2]})
        if level.simulate(steps).success:
            success += 1
    return success / float(samples)


def all_intents(level, max_items=None):
    """Every assignment of a subset of the inventory to zones (respecting zone acceptance and capacity)."""
    pool = []
    for entry in level.inventory:
        pieces = entry['count'] * (2 if entry['itemId'].startswith('portal') else 1)
        pool += [entry['itemId']] * pieces
    max_items = max_items or len(pool)
    seen = set()
    results = []
    for k in range(1, min(len(pool), max_items) + 1):
        for subset in set(itertools.combinations(pool, k)):
            options = []
            for item in subset:
                zones = [zi for zi, z in enumerate(level.zones) if not z['allowedItems'] or item in z['allowedItems']]
                options.append(zones)
            for assignment in itertools.product(*options):
                usage = {}
                ok = True
                for zi in assignment:
                    usage[zi] = usage.get(zi, 0) + 1
                    if usage[zi] > level.zones[zi]['capacity']:
                        ok = False
                if not ok:
                    continue
                key = tuple(sorted(zip(subset, assignment)))
                if key in seen:
                    continue
                seen.add(key)
                results.append(list(key))
    return results


def explore(level, grid=1.0, limit_per_intent=3000, max_items=None):
    summary = []
    for intent in all_intents(level, max_items):
        found, count = search_intent(level, intent, grid=grid, limit=limit_per_intent)
        if found:
            summary.append((intent, len(found), count, found[0]))
    return summary
