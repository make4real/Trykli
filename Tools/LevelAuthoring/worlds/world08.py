"""World 8 - Gravité (gravity switches). Difficulty 6-8."""
from common import *

G = 'gravity_switch'
S = 'spring'
R = 'ramp'
F = 'fan'
P = 'portal_ab'


def ledge(L, x0, x1, top, h=0.6):
    return L.block((x0 + x1) / 2.0, top - h / 2.0, x1 - x0, h, surface='Platform')


def roll_start(L, y=-4.0):
    L.spawn(-4.5, y + 3.2)
    slope(L, -5.0, y + 2.0, -3.3, y - 0.02)


def level_071():
    L = Level(71, "Premier switch", "First switch", 6, time_limit=14)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.wall(5.0, -4.0, 1.6)
    L.goal(3.8, 1.0, r=0.6)
    zg = L.slot(-1.0, -3.6, [G])
    zg2 = L.slot(-3.8, 0.4, [G])
    L.path_crystal(0.8)
    L.items(gravity_switch=1)
    L.stars(Interaction('gravity_flip', 1, EXACTLY, key=L.text('obj.l071.one', "Une seule inversion", "A single flip")),
            Crystals(1, key=L.text('obj.l071.ceiling', "Cristal au plafond", "Ceiling crystal")))
    L.solution((G, zg, 0))
    L.tip("Traverse le switch : la gravité s'inverse.", "Roll through the switch: gravity flips.")
    return L


def level_072():
    L = Level(72, "Plafond", "Ceiling", 6, time_limit=14)
    L.gravity_hint = -1
    L.spawn(-4.2, 0.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.ceiling(-5.0, 5.0, bottom=2.6, h=0.6)
    L.wall(5.0, -3.8, 2.6)
    L.goal(4.2, 2.0, r=0.6)
    zr = L.area(-4.0, -0.4, 1.6, 0.8, [R], capacity=1, default_rot=-30, min_rot=-45, max_rot=45)
    zg = L.area(-1.6, -0.8, 1.6, 0.8, [G], capacity=1)
    L.path_crystal(0.7)
    L.items(gravity_switch=1, ramp=1)
    L.stars(Avoid('Ground', key=L.text('obj.l072.stay', "Rester au plafond", "Stay on the ceiling")),
            Crystals(1, key=L.text('obj.l072.upper', "Cristal supérieur", "Upper crystal")))
    L.solution((R, zr, None), (G, zg, 0))
    L.tip("Tête en bas, le plafond devient le sol.", "Upside down, the ceiling becomes the floor.")
    return L


def level_073():
    L = Level(73, "Rebond inversé", "Inverted bounce", 7, time_limit=14)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 1.0, top=-4.0)
    pit_spikes(L, 1.0, 5.0, y=-3.8)
    L.ground(1.0, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 5.0, bottom=2.0, h=0.6)
    ledge(L, 3.2, 5.0, -1.6)
    L.wall(5.0, -1.6, 2.0)
    L.goal(4.2, -0.9, r=0.7)
    zg = L.slot(-1.8, -3.6, [G])
    zs = L.rail(0.8, 1.9, 2.4, items=[S], capacity=1, default_rot=180, min_rot=135, max_rot=225)
    zs2 = L.slot(2.4, -3.9, [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.4)
    L.path_crystal(0.75)
    L.items(gravity_switch=1, spring=1)
    L.stars(Bounces(1, EXACTLY, source='Spring', key=L.text('obj.l073.bounce', "Un rebond inversé", "One inverted bounce")), Crystals(2))
    L.solution((G, zg, 0), (S, zs, None))
    L.tip("Au plafond, le ressort renvoie vers le bas.", "On the ceiling, the spring sends you down.")
    return L


def level_074():
    L = Level(74, "Portail inversé", "Inverted portal", 7, time_limit=14)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.wall(1.5, -4.0, 1.6)
    L.wall(5.0, -4.0, 1.6)
    L.goal(3.8, 1.0, r=0.6)
    zg = L.slot(-1.8, -3.6, [G])
    za = L.slot(0.2, 0.8, [P])
    zb = L.slot(3.2, -3.2, [P])
    zb2 = L.slot(-3.8, 0.6, [P])
    L.path_crystal(0.8)
    L.items(gravity_switch=1, portal_ab=1)
    L.stars(UseAll(2, key=L.text('obj.l074.both', "Utiliser portail et switch", "Use the portal and the switch")),
            Crystals(1, key=L.text('obj.l074.dim', "Cristal dimensionnel", "Dimensional crystal")))
    L.solution((G, zg, 0), (P, za, 0), (P, zb, None))
    L.tip("Même tête en bas, un portail garde la vitesse.", "Even upside down, a portal keeps the speed.")
    return L


def level_075():
    L = Level(75, "Deux inversions", "Two flips", 7, time_limit=14)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, -0.6, top=-4.0)
    pit_spikes(L, -0.6, 3.2, y=-3.8)
    L.ground(-0.6, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.block(1.4, 0.4, 3.0, 0.4, surface='Obstacle')
    ledge(L, 3.2, 5.0, -3.4)
    L.wall(5.0, -3.4, 1.6)
    L.goal(4.2, -2.7, r=0.7)
    zg1 = L.slot(-1.8, -3.6, [G])
    zg2 = L.area(3.4, -0.6, 1.4, 1.6, [G], capacity=1)
    L.path_crystal(0.35)
    L.path_crystal(0.7)
    L.items(gravity_switch=2)
    L.stars(Interaction('gravity_flip', 2, EXACTLY, key=L.text('obj.l075.two', "Exactement 2 inversions", "Exactly 2 flips")), Crystals(2))
    L.solution((G, zg1, 0), (G, zg2, 0))
    L.tip("Monte au plafond, puis redescends au bon moment.", "Go up to the ceiling, then come back down at the right time.")
    return L


def level_076():
    L = Level(76, "Gravité laser", "Laser gravity", 7, time_limit=14)
    L.gravity_hint = -1
    L.spawn(-4.2, -3.4)
    L.ground(-5.0, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.laser(1.2, -4.0, rot=0, length=3.8)
    L.wall(5.0, -4.0, 1.6)
    L.goal(4.0, 1.0, r=0.6)
    zr = L.area(-4.0, -3.7, 1.4, 0.4, [R], capacity=1, default_rot=-15, min_rot=-45, max_rot=45)
    zg = L.area(-1.2, -2.6, 2.0, 1.6, [G], capacity=1)
    L.path_crystal(0.6)
    L.items(gravity_switch=1, ramp=1)
    L.stars(NoHazard('Laser', key='obj.l063.laser'), Crystals(1))
    L.solution((R, zr, None), (G, zg, 0))
    L.tip("Le laser ne touche pas le plafond.", "The laser does not reach the ceiling.")
    return L


def level_077():
    L = Level(77, "Vent ascendant", "Updraft", 7, time_limit=14)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 0.6, top=-4.0)
    pit_spikes(L, 0.6, 5.0, y=-3.8)
    L.ground(0.6, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 1.65, bottom=1.6, h=0.6)
    L.ceiling(1.65, 2.75, bottom=2.05, h=0.3)
    L.ceiling(2.75, 5.0, bottom=1.6, h=0.6)
    ledge(L, 2.6, 5.0, -1.8)
    L.wall(5.0, -1.8, 1.6)
    L.goal(3.8, -1.1, r=0.7)
    zg = L.slot(-1.8, -3.6, [G])
    zf = L.slot(2.2, 1.825, [F], default_rot=180, min_rot=135, max_rot=225)
    L.path_crystal(0.6)
    L.items(gravity_switch=1, fan=1)
    L.stars(MaxObjects(2), Crystals(1, key=L.text('obj.l077.center', "Cristal central", "Central crystal")))
    L.solution((G, zg, 0), (F, zf, None))
    L.tip("Tête en bas, un ventilateur tourné vers le sol te décroche du plafond.", "Upside down, a fan facing the floor pulls you off the ceiling.")
    return L


def level_078():
    L = Level(78, "Sol-plafond-sol", "Floor-ceiling-floor", 8, time_limit=16)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, -0.6, top=-4.0)
    pit_spikes(L, -0.6, 5.0, y=-3.8)
    L.ground(-0.6, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 5.0, bottom=2.4, h=0.6)
    sp = ground_with_recesses(L, 0.8, 2.6, -2.2, [1.7], h=0.6)
    L.wall(2.85, -2.8, 0.6)
    ledge(L, 3.1, 5.0, -0.2)
    L.wall(5.0, -0.2, 2.4)
    L.goal(4.2, 0.5, r=0.7)
    zg1 = L.slot(-1.8, -3.6, [G])
    zg2 = L.slot(1.2, 2.05, [G])
    zg3 = L.slot(-3.6, 0.8, [G])
    zs = L.slot(sp[0][0], sp[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(gravity_switch=2, spring=1)
    L.stars(Interaction('gravity_flip', 2, EXACTLY, key='obj.l075.two'), AllCrystals())
    L.solution((G, zg1, 0), (G, zg2, 0), (S, zs, -15))
    L.tip("Sol, plafond, sol : trois orientations.", "Floor, ceiling, floor: three orientations.")
    return L


def level_079():
    L = Level(79, "Choix de gravité", "Gravity choice", 8, time_limit=16)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.wall(1.5, -4.0, 1.6)
    L.wall(5.0, -4.0, 1.6)
    ledge(L, 1.75, 3.4, -1.2, h=0.4)
    L.goal(4.3, -3.3, r=0.6)
    zg = L.slot(-1.8, -3.6, [G])
    za = L.slot(0.2, 0.8, [P])
    zb = L.slot(2.4, 0.6, [P])
    zb2 = L.slot(-3.8, 0.6, [P])
    zg2 = L.area(3.9, 0.6, 1.4, 1.6, [G], capacity=1)
    zg3 = L.slot(-0.6, -2.0, [G])
    L.path_crystal(0.55)
    L.path_crystal(0.85)
    L.items(gravity_switch=2, portal_ab=1)
    L.stars(UseItem(G, 2, AT_MOST, key=L.text('obj.l079.two', "Maximum 2 switches", "At most 2 switches")), Crystals(2))
    L.solution((G, zg, 0), (P, za, 0), (P, zb, None), (G, zg2, 0))
    L.tip("Seule une orientation mène à la sortie parfaite.", "Only one orientation leads to the perfect exit.")
    return L


def level_080():
    L = Level(80, "Boss gravité", "Gravity boss", 8, time_limit=18)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 1.5, top=-4.0)
    pit_spikes(L, 1.75, 5.0, y=-3.8)
    L.ground(1.75, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 2.95, bottom=1.6, h=0.6)
    L.ceiling(2.95, 4.05, bottom=2.05, h=0.3)
    L.ceiling(4.05, 5.0, bottom=1.6, h=0.6)
    L.wall(1.5, -4.0, 1.6)
    L.wall(5.0, -4.0, 1.6)
    ledge(L, 3.2, 5.0, -2.2)
    L.goal(4.3, -1.5, r=0.6)
    zg = L.slot(-1.8, -3.6, [G])
    za = L.slot(0.2, 0.8, [P])
    zb = L.slot(2.2, -2.0, [P])
    zf = L.slot(3.5, 1.825, [F], default_rot=180, min_rot=135, max_rot=225)
    zg2 = L.slot(-3.8, 0.6, [G])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(gravity_switch=2, portal_ab=1, fan=1)
    L.stars(MaxObjects(4), AllCrystals())
    L.solution((G, zg, 0), (P, za, 0), (P, zb, None), (F, zf, None))
    L.tip("Gravité, portail et vent : un long voyage tête en bas.", "Gravity, portal and wind: a long upside-down trip.")
    return L


def levels():
    return [level_071(), level_072(), level_073(), level_074(), level_075(), level_076(), level_077(), level_078(), level_079(),
            level_080()]
