"""World 9 - Chaos contrôlé (combinations of known mechanics). Difficulty 7-9."""
from common import *

S = 'spring'
R = 'ramp'
F = 'fan'
M = 'magnet'
B = 'bomb'
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


def level_081():
    L = Level(81, "Trio classique", "Classic trio", 7, time_limit=16)
    L.spawn(-4.4, 6.8)
    slots = ground_with_recesses(L, -5, -1.0, -4.0, [-3.4])
    pit_spikes(L, -1.0, 5.0, y=-3.8)
    L.ground(-1.0, 5.0, top=-4.2, h=0.8)
    L.ceiling(-5.0, 2.4, bottom=2.4, h=0.5)
    ledge(L, 3.4, 5.0, -0.4)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.3, 0.3, r=0.7)
    za = L.slot(-4.4, 4.6, [P])
    zb = L.slot(-3.4, 1.4, [P])
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(3.8, 2.0, [M])
    zm2 = L.slot(0.4, -2.4, [M])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, magnet=1, portal_ab=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((P, za, 0), (P, zb, 180), (S, zs, None), (M, zm, 0))
    L.tip("Portail, ressort, aimant : dans cet ordre.", "Portal, spring, magnet: in that order.")
    return L


def level_082():
    L = Level(82, "Explosion aérienne", "Mid-air blast", 7, time_limit=14)
    L.goal_on_path = True
    L.goal_region = (1.8, -2.8, 5.0, 9.0)
    L.spawn(-1.6, 0.8)
    ground_with_recesses(L, -5.0, -0.6, -4.0, [-1.6], depth=0.45)
    pit_spikes(L, -0.6, 5.0, y=-3.8)
    L.ground(-0.6, 5.0, top=-4.2, h=0.8)
    ledge(L, 1.6, 5.0, -2.8)
    L.wall(5.0, -2.8, 9.0)
    L.goal(3.6, -2.1, r=0.7)
    zf = L.slot(-1.6, -4.225, [F], default_rot=0, min_rot=-45, max_rot=45)
    zb = L.slot(-2.4, -3.4, [B])
    zb2 = L.slot(-0.6, -1.0, [B])
    L.path_crystal(0.4)
    L.path_crystal(0.75)
    L.items(fan=1, bomb=1)
    L.stars(Interaction('explosion', 1, EXACTLY, key='obj.l043.one'), Crystals(2))
    L.solution((F, zf, 15), (B, zb, 0))
    L.tip("Le vent retient Trykli ; la bombe le projette.", "The wind holds Trykli; the bomb throws him.")
    return L


def level_083():
    L = Level(83, "Portail mouvant", "Moving portal", 7, time_limit=16)
    L.spawn(-4.0, -1.0)
    slots = ground_with_recesses(L, -5.0, -3.05, -4.0, [-4.0])
    pit_spikes(L, -2.55, 5.0, y=-3.8)
    L.ground(-2.55, 5, top=-4.2, h=0.8)
    L.obstacle(-2.8, 1.0, 0.5, 15.0)
    basket(L, -1.4, -1.0, (3.0, 0.0), w=1.8, period=5.0, phase=0.8)
    L.obstacle(1.3, 4.1, 0.5, 8.0)
    L.wall(5.0, -3.8, 9.0)
    L.goal(2.2, -0.2, r=0.6)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    za = L.slot(-4.0, 1.6, [P])
    zb = L.slot(-1.2, 0.8, [P])
    L.path_crystal(0.75)
    L.items(portal_ab=1, spring=1)
    L.stars(MaxObjects(2), Crystals(1, key=L.text('obj.l083.moving', "Cristal mobile", "Moving crystal")))
    L.solution((S, zs, 0), (P, za, 0), (P, zb, 180))
    L.tip("Le ressort t'envoie au portail, la sortie tombe dans la nacelle.", "The spring sends you to the portal, the exit drops into the basket.")
    return L


def level_084():
    L = Level(84, "Gravité magnétique", "Magnetic gravity", 8, time_limit=16)
    L.gravity_hint = -1
    roll_start(L)
    L.ground(-3.3, 5.0, top=-4.0)
    L.ceiling(-5.0, 5.0, bottom=1.6, h=0.6)
    L.block(1.2, 0.3, 0.5, 2.6, surface='Obstacle')
    L.wall(5.0, -4.0, 1.6)
    L.goal(3.6, 1.0, r=0.6)
    zg = L.slot(-2.0, -3.6, [G])
    zm = L.slot(1.4, -2.8, [M])
    zm2 = L.slot(-0.6, 0.6, [M])
    zm3 = L.slot(3.4, -1.2, [M])
    L.path_crystal(0.4)
    L.path_crystal(0.75)
    L.items(gravity_switch=1, magnet=1)
    L.stars(MaxObjects(2), Crystals(2))
    L.solution((G, zg, 0), (M, zm, 0))
    L.tip("L'aimant tire Trykli sous le mur pendant qu'il tombe vers le haut.", "The magnet pulls Trykli under the wall while he falls upwards.")
    return L


def level_085():
    L = Level(85, "Sécurité maximale", "Maximum security", 8, time_limit=16)
    L.spawn(-3.4, -1.0)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 5.0, bottom=2.0, h=0.5)
    ledge(L, -1.6, 0.4, 0.0)
    L.button(0.0, 0.05, emit=0, bid='b1')
    slope(L, 0.4, 0.0, 2.1, -3.98)
    L.laser(2.4, -4.0, rot=0, length=5.8, channel=0, activation=sim.ACT_DEACTIVATED)
    L.button(3.1, -3.95, emit=1, bid='b2')
    L.laser(3.8, -4.0, rot=0, length=5.8, channel=1, activation=sim.ACT_DEACTIVATED)
    L.wall(5.0, -4.0, 2.0)
    L.goal(4.45, -3.4, r=0.5)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zr = L.area(-2.4, -3.7, 1.2, 0.4, [R], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.9)
    L.items(spring=1, ramp=1)
    L.stars(NoHazard('Laser', key='obj.l063.laser'), AllCrystals())
    L.solution((S, zs, -15))
    L.tip("Deux boutons, deux lasers : coupe-les dans l'ordre.", "Two buttons, two lasers: switch them off in order.")
    return L


def level_086():
    L = Level(86, "Explosion dimensionnelle", "Dimensional blast", 8, time_limit=14)
    roll_start(L)
    ground_with_recesses(L, -3.4, 1.0, -4.0, [-1.2], recess_width=0.9, depth=0.8)
    L.block(1.25, 2.5, 0.5, 13.0, surface='Wall')
    L.ground(1.5, 5.0, top=-4.0)
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, -3.3, r=0.7)
    zb = L.slot(-1.2, -4.4, [B])
    za = L.area(-0.2, -0.6, 1.6, 1.6, [P], capacity=1)
    zp = L.slot(3.2, 3.0, [P])
    L.path_crystal(0.8)
    L.items(bomb=1, portal_ab=1)
    L.stars(Interaction('explosion', 1, EXACTLY, key='obj.l043.one'),
            Crystals(1, key=L.text('obj.l086.exit', "Cristal à la sortie du portail", "Crystal at the portal exit")))
    L.solution((B, zb, 0), (P, za, 0), (P, zp, None))
    L.tip("L'explosion doit viser le portail.", "The blast has to aim at the portal.")
    return L


def level_087():
    L = Level(87, "Usine à vent", "Wind factory", 8, time_limit=18)
    L.spawn(-4.5, 1.8)
    slope(L, -5.0, 1.0, -3.3, 0.42)
    ledge(L, -3.3, -2.9, 0.4)
    L.wall(-2.9, -4.0, 0.4, w=0.1)
    ground_with_recesses(L, -2.85, -1.0, -4.0, [-2.0], depth=0.45)
    pit_spikes(L, -1.0, 5.0, y=-3.8)
    L.ground(-1.0, 5.0, top=-4.2, h=0.8)
    L.moving(-0.6, -0.9, 1.8, (2.6, 0.0), period=5.0, phase=0.6)
    ledge(L, 3.9, 5.0, 0.5)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.4, 1.2, r=0.7)
    zf1 = L.slot(-2.0, -4.225, [F, S], default_rot=0, min_rot=-45, max_rot=45)
    zf2 = L.slot(3.2, -1.8, [F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(fan=2, spring=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((F, zf1, -15), (F, zf2, -15))
    L.tip("Deux courants d'air et une plateforme : synchronise-les.", "Two air streams and a platform: sync them.")
    return L


def level_088():
    L = Level(88, "Trois routes", "Three routes", 8, time_limit=16)
    L.spawn(-4.2, 6.4)
    L.ground(-5.0, 5.0, top=-4.0)
    L.block(1.5, 3.4, 0.5, 11.2, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, -3.3, r=0.7)
    za = L.slot(-4.2, 2.4, [P])
    zb = L.slot(3.4, 4.6, [P])
    zr = L.area(-3.8, -2.4, 1.6, 1.0, [R], capacity=1, default_rot=-30, min_rot=-45, max_rot=45)
    zm = L.slot(-0.6, -2.4, [M])
    L.path_crystal(0.8)
    L.path_crystal(0.88)
    L.path_crystal(0.96)
    L.items(magnet=1, portal_ab=1, ramp=1)
    L.stars(MaxObjects(2), AllCrystals(key=L.text('obj.l088.high', "Route haute et 3 cristaux", "High route and 3 crystals")))
    L.solution((P, za, 0), (P, zb, 180))
    L.tip("Trois routes mènent à la sortie ; celle du haut est la meilleure.", "Three routes lead to the exit; the high one is the best.")
    return L


def level_089():
    L = Level(89, "Multi-étapes", "Multi-stage", 8, time_limit=18)
    roll_start(L)
    ground_with_recesses(L, -3.3, -1.0, -4.0, [-2.1], depth=0.45)
    slots = ground_with_recesses(L, -1.0, 3.6, -4.0, [2.4])
    L.block(0.1, -2.9, 2.2, 2.2, surface='Platform')
    L.button(0.3, -1.75, emit=0, bid='b1')
    L.door(1.05, -0.8, channel=0, h=1.4)
    ledge(L, 3.6, 5.0, 0.8)
    L.wall(3.6, -4.0, 0.2, w=0.2)
    L.wall(5.0, 0.8, 9.0)
    L.goal(4.3, 1.5, r=0.7)
    zf = L.slot(-2.1, -4.225, [F], default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(3.9, 2.8, [M])
    zm2 = L.slot(1.8, 1.4, [M])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, fan=1, magnet=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((F, zf, -15), (S, zs, -15), (M, zm, 0))
    L.tip("Vent, bouton, ressort, aimant : quatre étapes.", "Wind, button, spring, magnet: four stages.")
    return L


def level_090():
    L = Level(90, "Mega puzzle", "Mega puzzle", 9, time_limit=20)
    L.spawn(-4.4, 6.8)
    slope(L, -5.0, -2.0, -3.3, -4.02)
    L.ceiling(-5.0, -0.8, bottom=2.4, h=0.5)
    ground_with_recesses(L, -3.3, -1.0, -4.0, [-2.1], depth=0.45)
    slots = ground_with_recesses(L, -1.0, 3.6, -4.0, [2.4])
    L.block(0.1, -2.9, 2.2, 2.2, surface='Platform')
    L.button(0.3, -1.75, emit=0, bid='b1')
    L.door(1.05, -0.8, channel=0, h=1.4)
    ledge(L, 3.6, 5.0, 0.8)
    L.wall(3.6, -4.0, 0.2, w=0.2)
    L.wall(5.0, 0.8, 9.0)
    L.goal(4.3, 1.5, r=0.7)
    zf = L.slot(-2.1, -4.225, [F], default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(3.9, 2.8, [M])
    zm2 = L.slot(1.8, 1.4, [M])
    za = L.slot(-4.4, 4.6, [P])
    zb = L.slot(-4.5, 0.4, [P])
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, fan=1, portal_ab=1, magnet=1)
    L.stars(MaxObjects(4), AllCrystals())
    L.solution((P, za, 0), (P, zb, 180), (F, zf, -15), (S, zs, -15), (M, zm, 0))
    L.tip("Portail, vent, bouton, ressort, aimant : tout s'enchaîne.", "Portal, wind, button, spring, magnet: everything chains.")
    return L


def levels():
    return [level_081(), level_082(), level_083(), level_084(), level_085(), level_086(), level_087(), level_088(), level_089(),
            level_090()]
