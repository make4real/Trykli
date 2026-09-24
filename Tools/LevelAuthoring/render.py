"""ASCII preview of a level and of the reference trajectory (design aid)."""
import trykli_sim as sim

CHARS = {sim.SPIKES: '^', sim.SPRING: 's', sim.FAN: 'f', sim.PORTAL: 'O', sim.MAGNET: 'm', sim.BOMB: 'b', sim.BUMPER: 'B',
         sim.MOVING: '=', sim.ROTATING: '%', sim.BUTTON: '_', sim.DOOR: '|', sim.LASER: 'L', sim.GRAVITY: 'g',
         sim.STICKY: '~', sim.SLIPPERY: '-', sim.CANNON: 'C', sim.RAMP: '/', sim.MINIPLATFORM: '='}


def render(level, steps=None, cell=0.25, row=0.5):
    d = level.to_definition()
    b = d['layout']['bounds']
    cols = int(b['width'] / cell)
    rows = int(b['height'] / row)
    grid = [[' '] * cols for _ in range(rows)]

    def put(x, y, ch, force=True):
        c = int((x - b['x']) / cell)
        r = rows - 1 - int((y - b['y']) / row)
        if 0 <= c < cols and 0 <= r < rows and (force or grid[r][c] == ' '):
            grid[r][c] = ch

    s = sim.Simulation(d, level.placed_elements(steps))
    for shape in s.shapes:
        ch = '#'
        if shape.owner is not None and shape.owner['type'] in CHARS:
            ch = CHARS[shape.owner['type']]
        for ci in range(cols):
            for ri in range(rows):
                x = b['x'] + (ci + 0.5) * cell
                y = b['y'] + (rows - 1 - ri + 0.5) * row
                if shape.kind == 'box' and sim.point_in_box((x, y), shape.center, shape.half, shape.rot):
                    grid[ri][ci] = ch
                elif shape.kind == 'circle' and sim.v_len(sim.v_sub((x, y), shape.center)) <= shape.radius:
                    grid[ri][ci] = ch
    for e in s.elements:
        if e['type'] in CHARS and e['type'] not in (sim.RAMP, sim.MINIPLATFORM):
            put(e['_p'][0], e['_p'][1], CHARS[e['type']].upper() if e['type'] != sim.SPIKES else '^')
        if e['type'] == sim.CRYSTAL:
            put(e['_p'][0], e['_p'][1], '*')
    for z in d['layout']['zones']:
        put(z['position']['x'], z['position']['y'], 'Z', force=False)
    result = sim.Simulation(d, level.placed_elements(steps), record_trace=True).run()
    for p in result.trace[::2]:
        put(p[0], p[1], '.', force=False)
    put(d['layout']['goal']['x'], d['layout']['goal']['y'], 'G')
    put(d['layout']['spawn']['x'], d['layout']['spawn']['y'], 'S')
    print('+' + '-' * cols + '+  L%03d %s -> %s' % (level.id, level.name_en, result))
    for r in grid:
        print('|' + ''.join(r) + '|')
    print('+' + '-' * cols + '+')
