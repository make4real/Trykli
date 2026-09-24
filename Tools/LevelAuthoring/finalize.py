"""Finalization of an authored level: reference solution search, crystals on the trajectory, star report."""
import design_search as ds
import trykli_sim as sim


def _placements_valid(level, steps):
    return steps and all(s['rotation'] is not None for s in steps)


def _trace(level, steps, max_time=None):
    """Full trajectory with the exit moved away (so the run is not cut short by the current goal)."""
    saved = level.goal_pos
    level.goal_pos = (999.0, 999.0)
    try:
        definition = level.to_definition()
        result = sim.Simulation(definition, level.placed_elements(steps), record_trace=True).run(max_time)
    finally:
        level.goal_pos = saved
    return result.trace


def place_goal_on_path(level, steps, start_fraction=0.3, min_score=1.1):
    """
    Puts the exit on the intended trajectory, at the point that is the furthest from every trajectory obtained
    when one of the placed items is removed (so each item is required). Returns the score (distance) or None.
    """
    path = _trace(level, steps)
    if len(path) < 10:
        return None
    others = [_trace(level, steps[:i] + steps[i + 1:]) for i in range(len(steps))]
    others.append(_trace(level, []))
    spawn = level.spawn_pos
    best = None
    first = int(len(path) * start_fraction)
    for k in range(first, len(path) - 5, 2):
        p = path[k]
        if sim.v_len(sim.v_sub(p, spawn)) < 2.5:
            continue
        score = min(min(sim.v_len(sim.v_sub(p, q)) for q in other[::2]) if other else 99.0 for other in others)
        if best is None or score > best[0] + 0.05:
            best = (score, p, k)
    if best is None or best[0] < min_score:
        return None
    level.goal_pos = (round(best[1][0], 2), round(best[1][1], 2))
    level.goal_radius = 0.6
    return best[0]


def _necessity_score(level, steps):
    path = _trace(level, steps)
    if len(path) < 10:
        return 0.0
    others = [_trace(level, steps[:i] + steps[i + 1:]) for i in range(len(steps))]
    others.append(_trace(level, []))
    spawn = level.spawn_pos
    best = 0.0
    for k in range(int(len(path) * 0.3), len(path) - 5, 3):
        p = path[k]
        if sim.v_len(sim.v_sub(p, spawn)) < 2.5:
            continue
        score = min(min(sim.v_len(sim.v_sub(p, q)) for q in other[::3]) if other else 99.0 for other in others)
        best = max(best, score)
    return best


def tune_zones(level, samples=120):
    """Randomly moves the tunable zones (and their solution steps) to maximize the item necessity score."""
    import random
    rng = random.Random(level.id * 7919)
    tunable = level.tune_zones
    base = {zi: (level.zones[zi]['position']['x'], level.zones[zi]['position']['y']) for zi in tunable}

    def apply(positions):
        for zi, (x, y) in positions.items():
            level.zones[zi]['position'] = {'x': round(x, 2), 'y': round(y, 2)}
            for step in level.solution_steps:
                if step['zoneIndex'] == zi:
                    step['position'] = {'x': round(x, 2), 'y': round(y, 2)}

    def objective():
        if getattr(level, 'goal_on_path', False):
            return _necessity_score(level, level.solution_steps)
        result = level.simulate(level.solution_steps)
        if not result.success:
            return -result.min_goal_dist
        return 10.0 + _necessity_score(level, level.solution_steps)

    best = (objective(), dict(base))
    for _ in range(samples):
        candidate = {zi: (base[zi][0] + rng.uniform(-r[0], r[0]), base[zi][1] + rng.uniform(-r[1], r[1])) for zi, r in tunable.items()}
        apply(candidate)
        score = objective()
        if score > best[0]:
            best = (score, candidate)
    apply(best[1])
    return best[0]


def finalize(level, allow_search=True, grid=0.5):
    report = {'searched': False, 'success': False, 'stars': [False] * 3, 'time': 0.0, 'alternatives': None}
    steps = level.solution_steps
    if getattr(level, 'tune_zones', None) and _placements_valid(level, steps):
        report['tuned'] = tune_zones(level)
    if getattr(level, 'goal_on_path', False) and _placements_valid(level, steps):
        score = place_goal_on_path(level, steps)
        report['goal_on_path'] = score
    ok = False
    if _placements_valid(level, steps):
        result = level.simulate(steps)
        ok = result.success
    if not ok and allow_search and steps:
        intent = [(s['itemId'], s['zoneIndex']) for s in steps]
        found, count = ds.search_intent(level, intent, grid=grid, limit=30000)
        report['searched'] = True
        report['alternatives'] = (len(found), count)
        if found:
            # prefer solutions fulfilling the objectives where every placed item is required, then the fastest
            chosen = found[0][2]
            for stars, _, candidate, _ in found[:60]:
                if all(not level.simulate(candidate[:i] + candidate[i + 1:]).success for i in range(len(candidate))):
                    chosen = candidate
                    break
            level.solution_steps = chosen
            ok = True
    attempts = 0
    while not ok and allow_search and getattr(level, 'auto_goal', False) and getattr(level, 'best_miss', None) and attempts < 3:
        # Move the exit onto the closest point of the best intended trajectory (design aid, reported).
        attempts += 1
        _, point, steps, _ = level.best_miss
        if point is None:
            break
        report.setdefault('goal_moved', []).append((round(level.goal_pos[0], 2), round(level.goal_pos[1], 2)))
        level.goal_pos = (round(point[0], 2), round(point[1], 2))
        result = level.simulate(steps)
        if result.success:
            level.solution_steps = steps
            ok = True
        else:
            level.best_miss = (result.min_goal_dist, result.closest_point, steps, result.failure)
    if not ok:
        for step in level.solution_steps:
            if step['rotation'] is None:
                step['rotation'] = level.zones[step['zoneIndex']]['defaultRotation']
    if ok and level.path_crystals:
        result = level.simulate(trace=True)
        trace = result.trace
        level.path_crystals_done = True
        for where, dy, cid in level.path_crystals:
            if where == 'apex':
                index = max(range(len(trace)), key=lambda i: trace[i][1] * (1 if level.gravity_hint >= 0 else -1))
            else:
                index = min(len(trace) - 1, max(0, int(float(where) * (len(trace) - 1))))
            x, y = trace[index]
            level.crystal(round(x, 2), round(y + dy, 2), cid)
    if not ok:
        for where, dy, cid in level.path_crystals:
            level.crystal(level.goal_pos[0], level.goal_pos[1] + 1.2, cid)
    final = level.simulate()
    if final.success and len(level.solution_steps) > 0:
        # necessity check: which placed items are required by the reference solution
        report['unnecessary'] = []
        for i, step in enumerate(level.solution_steps):
            reduced = level.solution_steps[:i] + level.solution_steps[i + 1:]
            if level.simulate(reduced).success:
                report['unnecessary'].append(step['itemId'])
        report['trivial'] = level.simulate([]).success
    report['success'] = final.success
    report['failure'] = final.failure
    report['time'] = final.time
    report['stars'] = level.evaluate(final)
    report['crystals'] = (len(final.crystals), sum(1 for e in level.elements if e['type'] == sim.CRYSTAL))
    level.report = report
    return report
