"""World 10 - Master Trykli (synthesis levels). Difficulty 8-10."""
from common import *

S = 'spring'
R = 'ramp'
F = 'fan'
M = 'magnet'
B = 'bomb'
U = 'bumper'
P = 'portal_ab'
G = 'gravity_switch'


def ledge(L, x0, x1, top, h=0.6):
    return L.block((x0 + x1) / 2.0, top - h / 2.0, x1 - x0, h, surface='Platform')


def roll_start(L, y=-4.0):
    L.spawn(-4.5, y + 3.2)
    slope(L, -5.0, y + 2.0, -3.3, y - 0.02)


def basket(L, x, y, travel, w=1.6, **kw):
    L.moving(x, y, w, travel, **kw)
    kw = dict(kw)
    kw['h'] = 0.6
    L.moving(x - w / 2.0 + 0.1, y + 0.3, 0.2, travel, **kw)
    L.moving(x + w / 2.0 - 0.1, y + 0.3, 0.2, travel, **kw)


def level_091():
    L = Level(91, "Précision contrôlée", "Controlled precision", 8, time_limit=16)
    L.spawn(-3.6, -1.0)
    slots = ground_with_recesses(L, -5, -1.6, -4.0, [-3.6])
    pit_spikes(L, -1.6, 5.0, y=-3.8)
    L.ground(-1.6, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 2.8, bottom=2.0, h=0.5)
    L.obstacle(1.0, -1.9, 0.5, 3.0)
    L.obstacle(2.6, 1.7, 0.5, 0.6)
    ledge(L, 3.4, 5.0, -0.8)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.3, -0.1, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.area(3.2, 0.0, 2.0, 2.0, [M], capacity=1)
    zm2 = L.slot(-0.6, -2.6, [M])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, magnet=1)
    L.stars(MaxObjects(2), AllCrystals())
    L.solution((S, zs, None), (M, zm, None))
    L.tip("Le ressort lance, l'aimant rattrape : deux zones d'effet.", "The spring launches, the magnet catches: two areas of effect.")
    return L


def level_092():
    L = Level(92, "Minimaliste", "Minimalist", 8, time_limit=16)
    roll_start(L)
    slots = ground_with_recesses(L, -3.3, -1.4, -4.0, [-2.3])
    pit_spikes(L, -1.4, 5.0, y=-3.8)
    L.ground(-1.4, 5.0, top=-4.2, h=0.8)
    L.block(0.8, -3.2, 1.4, 1.2, surface='Platform')
    L.ceiling(-5.0, 5.0, bottom=2.4, h=0.5)
    L.obstacle(2.3, 1.1, 0.5, 2.6)
    ledge(L, 3.3, 5.0, -1.6)
    L.wall(5.0, -1.6, 2.4)
    L.goal(4.2, -0.9, r=0.7)
    zs1 = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-30, max_rot=30)
    zs2 = L.slot(0.8, -2.5, [S, F], default_rot=0, min_rot=-45, max_rot=45)
    zr = L.area(-0.4, 0.6, 1.6, 1.0, [R], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.items(spring=2, fan=1, ramp=1)
    L.stars(MaxObjects(3), MaxObjects(2, key=L.text('obj.l092.two', "Maximum 2 objets", "At most 2 objects")))
    L.solution((S, zs1, -30), (S, zs2, -45))
    L.tip("Tout n'est pas utile : la meilleure solution est la plus simple.", "Not everything is useful: the best solution is the simplest one.")
    return L


def level_093():
    L = Level(93, "Solutions multiples", "Many solutions", 8, time_limit=16)
    L.spawn(-4.2, 6.4)
    pit_spikes(L, -5.0, -2.6, y=-3.8)
    L.ground(-5.0, -2.6, top=-4.2, h=0.8)
    L.ground(-2.6, 5.0, top=-4.0)
    L.block(1.5, 4.4, 0.5, 9.2, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, -3.3, r=0.7)
    za = L.slot(-4.2, 2.4, [P])
    zb = L.slot(3.4, 4.6, [P])
    zu = L.area(-4.2, -1.2, 1.6, 2.0, [U], capacity=1)
    zm = L.area(-2.0, -1.8, 2.4, 2.4, [M], capacity=1)
    L.path_crystal(0.3)
    L.path_crystal(0.85)
    L.path_crystal(0.95)
    L.items(portal_ab=1, magnet=1, bumper=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((P, za, 0), (P, zb, 180))
    L.tip("Portail, bumper ou aimant : plusieurs chemins, plusieurs risques.", "Portal, bumper or magnet: several paths, several risks.")
    return L


def level_094():
    L = Level(94, "Grande chaîne", "Great chain", 9, time_limit=18)
    roll_start(L)
    ground_with_recesses(L, -3.4, 1.4, -4.0, [-1.6], recess_width=0.9, depth=0.8)
    L.obstacle(1.6, 3.0, 0.5, 14.0)
    pit_spikes(L, 1.85, 5.0, y=-3.8)
    L.ground(1.85, 5.0, top=-4.2, h=0.8)
    L.block(3.0, -3.3, 1.2, 1.0, surface='Platform')
    ledge(L, 4.1, 5.0, -1.4)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.5, -0.7, r=0.6)
    zb = L.slot(-1.6, -4.4, [B])
    zu = L.slot(0.58, -0.57, [U])
    za = L.slot(-2.5, -1.29, [P])
    zb2 = L.slot(3.0, 2.2, [P], default_rot=180, allow_rot=False)
    zf = L.slot(3.0, -2.575, [F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(bomb=1, bumper=1, portal_ab=1, fan=1)
    L.stars(NoStop(1.0, key=L.text('obj.l094.chain', "Chaîne complète sans arrêt", "Full chain without stopping")), AllCrystals())
    L.solution((B, zb, 0), (U, zu, 0), (P, za, -135), (P, zb2, 180), (F, zf, -30))
    L.tip("Bombe, bumper, portail, vent : une réaction en chaîne complète.", "Bomb, bumper, portal, wind: a full chain reaction.")
    return L


def pad(L, x0, x1, top, center, width=1.1, depth=0.18, h=0.6):
    ledge(L, x0, center - width / 2.0, top, h)
    ledge(L, center + width / 2.0, x1, top, h)
    ledge(L, center - width / 2.0, center + width / 2.0, top - depth, h - depth)
    return (center, top - SPRING_FLUSH)


def level_095():
    L = Level(95, "Timing expert", "Expert timing", 9, time_limit=18)
    L.spawn(-4.3, 3.2)
    ledge(L, -5.0, -2.6, 2.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.moving(-1.6, 1.2, 1.8, (2.2, 0.0), period=4.0, phase=0.5)
    L.moving(0.6, -1.2, 1.8, (2.0, 0.0), period=4.0, phase=0.35)
    L.laser(3.05, -3.8, rot=0, length=3.4, on=1.0, off=1.0, phase=1.75)
    sp = pad(L, 3.4, 5.0, -2.6, 4.2)
    L.wall(5.0, -2.6, 9.0)
    L.goal(4.0, 2.4, r=0.7)
    zr = L.area(-3.8, 2.9, 1.6, 0.6, [R], capacity=1, default_rot=-15, min_rot=-30, max_rot=30)
    zs = L.slot(sp[0], sp[1], [S, F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.35)
    L.path_crystal(0.7)
    L.items(spring=1, ramp=1, fan=1)
    L.stars(NoHazard('Laser', key='obj.l063.laser'), Attempts(3, key=L.text('obj.l095.tries', "Moins de 4 essais", "Fewer than 4 attempts")))
    L.solution((R, zr, -15, (-4.2, 2.6)), (S, zs, -15))
    L.tip("Deux plateformes, un laser qui clignote : tout est une question de rythme.", "Two platforms, a blinking laser: it is all about rhythm.")
    return L


def level_096():
    L = Level(96, "Dimension inversée", "Inverted dimension", 9, time_limit=18)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.wall(1.5, -4.0, 1.6)
    L.wall(5.0, -4.0, 1.6)
    L.laser(3.1, 1.6, rot=180, length=2.8)
    L.goal(4.2, 1.0, r=0.6)
    zg = L.slot(-1.8, -3.6, [G])
    za = L.slot(0.2, 0.8, [P])
    zb = L.slot(2.2, -3.2, [P], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.area(3.4, -2.6, 2.0, 1.6, [M], capacity=1)
    zb2 = L.slot(-3.8, 0.6, [P])
    L.path_crystal(0.4)
    L.path_crystal(0.7)
    L.path_crystal(0.9)
    L.items(portal_ab=1, gravity_switch=1, magnet=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((G, zg, 0), (P, za, -135), (P, zb, -45), (M, zm, 0, (4.0, -2.2)))
    L.tip("Tête en bas, l'aimant fait passer Trykli sous le laser.", "Upside down, the magnet pulls Trykli under the laser.")
    return L


def level_097():
    L = Level(97, "Machine complexe", "Complex machine", 9, time_limit=20)
    roll_start(L)
    L.ground(-3.3, -0.35, top=-4.0)
    L.button(-2.0, -3.95, emit=0, bid='b1')
    L.moving(0.45, -4.15, 1.5, (0.0, 4.5), period=6.0, delay=1.0, loop=False, channel=0, activation=sim.ACT_ACTIVATED)
    L.ground(-0.35, 1.3, top=-4.6, h=0.6)
    L.wall(1.35, -4.6, 0.4, w=0.3)
    ledge(L, 1.2, 5.0, 0.4)
    L.button(2.4, 0.45, emit=1, bid='b2')
    L.door(3.6, 1.6, channel=1, h=2.4)
    L.ceiling(1.2, 5.0, bottom=2.8, h=0.4)
    L.wall(5.0, 0.4, 2.8)
    L.goal(4.4, 1.1, r=0.6)
    zf = L.slot(-0.9, 1.2, [F, S, U], default_rot=-90, min_rot=-135, max_rot=-45)
    zs = L.slot(-0.8, -2.4, [F, S, U], default_rot=0)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.9)
    L.items(spring=1, fan=1, bumper=1)
    L.stars(MaxObjects(3), Interaction('button', 2, key=L.text('obj.l097.all', "Activer tous les boutons", "Press every button")))
    L.solution((F, zf, -90))
    L.tip("Bouton, ascenseur, vent, bouton : chaque machine lance la suivante.", "Button, lift, wind, button: each machine starts the next one.")
    return L


def level_098():
    L = Level(98, "Presque libre", "Almost free", 9, time_limit=18)
    roll_start(L)
    slots = ground_with_recesses(L, -3.3, -1.4, -4.0, [-2.3], depth=0.45)
    pit_spikes(L, -1.4, 5.0, y=-3.8)
    L.ground(-1.4, 5.0, top=-4.2, h=0.8)
    L.block(0.8, -3.2, 1.4, 1.2, surface='Platform')
    L.block(0.8, -2.825, 1.4, 0.45, surface='Platform')
    L.ceiling(-5.0, 5.0, bottom=4.4, h=0.5)
    ledge(L, 3.3, 5.0, 0.2)
    L.wall(5.0, 0.2, 4.4)
    L.goal(4.2, 0.9, r=0.7)
    z1 = L.slot(-2.3, -4.225, [S, F], default_rot=0, min_rot=-45, max_rot=45)
    z2 = L.slot(0.8, -2.825, [S, F], default_rot=0, min_rot=-45, max_rot=45)
    z3 = L.area(1.4, 1.6, 2.4, 1.6, [M, F], capacity=1, default_rot=0)
    za = L.slot(-0.4, -1.2, [P])
    zb = L.slot(2.6, 3.0, [P])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=2, fan=2, portal_ab=1, magnet=1)
    L.stars(MaxObjects(4), AllCrystals())
    L.solution((S, z1, -45), (F, z2, -45))
    L.tip("Beaucoup de libertés : plusieurs machines fonctionnent.", "Lots of freedom: several machines work.")
    return L


def level_099():
    L = Level(99, "Ultimate Challenge", "Ultimate Challenge", 10, time_limit=20)
    L.gravity_hint = -1
    roll_start(L)
    ground_with_recesses(L, -3.4, 1.0, -4.0, [-1.2], recess_width=0.9, depth=0.8)
    L.block(1.25, 0.5, 0.5, 9.0, surface='Wall')
    slots = ground_with_recesses(L, 1.5, 5.0, -4.0, [3.0])
    L.ceiling(-5.0, 5.0, bottom=5.0, h=0.5)
    L.ceiling(1.5, 5.0, bottom=2.4, h=0.4)
    L.wall(5.0, -4.0, 2.4)
    L.obstacle(3.9, 1.3, 0.3, 2.2)
    L.goal(4.4, 1.8, r=0.6)
    zb = L.slot(-1.2, -4.4, [B])
    za = L.slot(-0.5, -1.4, [P])
    zp = L.slot(3.0, 0.2, [P], default_rot=180, allow_rot=False)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zg = L.area(3.2, 0.8, 2.4, 1.6, [G], capacity=1)
    zm = L.area(4.3, -1.6, 1.0, 2.4, [M], capacity=1)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.9)
    L.items(spring=1, bomb=1, portal_ab=1, gravity_switch=1, magnet=1)
    L.stars(MaxObjects(5), AllCrystals())
    L.solution((B, zb, 0), (P, za, -135), (P, zp, 180), (S, zs, -15), (G, zg, 0, (3.6, 0.0)))
    L.tip("Bombe, portail, ressort, gravité, aimant : chacun a son rôle.", "Bomb, portal, spring, gravity, magnet: each one has its role.")
    return L


def level_100():
    L = Level(100, "TRYKLI MASTER", "TRYKLI MASTER", 10, time_limit=24)
    L.gravity_hint = -1
    L.spawn(-4.2, -1.0)
    slots = ground_with_recesses(L, -5.0, 1.5, -4.0, [-4.2])
    L.ceiling(-5.0, 1.5, bottom=1.4, h=0.5)
    L.wall(-1.0, -4.0, -0.6)
    L.wall(1.5, -4.0, 1.4)
    L.ground(1.5, 5.0, top=-4.0)
    L.ceiling(1.5, 5.0, bottom=2.4, h=0.4)
    L.button(3.4, 2.35, emit=0, bid='b1', rot=180)
    L.door(4.1, 1.5, channel=0, h=1.8)
    L.wall(5.0, -4.0, 2.4)
    L.goal(4.6, 1.8, r=0.5)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    za = L.area(-2.4, 0.2, 1.6, 1.2, [P], capacity=1)
    zb = L.slot(2.2, -0.6, [P], default_rot=-135, min_rot=-180, max_rot=-90)
    zg = L.area(3.0, -2.0, 1.6, 1.2, [G], capacity=1)
    zx = L.area(2.8, 1.8, 1.0, 0.8, [B, F, U, M], capacity=1)
    zm = L.area(-0.2, -2.6, 1.6, 1.2, [M, F, U], capacity=1)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.9)
    L.items(spring=1, portal_ab=1, fan=1, magnet=1, bomb=1, bumper=1, gravity_switch=1)
    L.stars(MaxObjects(7), AllCrystals())
    L.solution((S, zs, -30), (P, za, 0, (-2.6, -0.4)), (P, zb, -135), (G, zg, 0, (2.8, -2.0)))
    L.tip("Lancement, téléportation, inversion, bouton, explosion : la grande finale.", "Launch, teleport, flip, button, blast: the grand finale.")
    return L


def levels():
    return [level_091(), level_092(), level_093(), level_094(), level_095(), level_096(), level_097(), level_098(), level_099(),
            level_100()]
