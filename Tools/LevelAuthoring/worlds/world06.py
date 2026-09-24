"""World 6 - Machines (moving and rotating platforms). Difficulty 5-6."""
from common import *

S = 'spring'
R = 'ramp'
F = 'fan'
P = 'portal_ab'


def ledge(L, x0, x1, top, h=0.6):
    return L.block((x0 + x1) / 2.0, top - h / 2.0, x1 - x0, h, surface='Platform')


def pad(L, x0, x1, top, center, width=1.1, depth=0.18, h=0.6):
    """Platform (not ground) with a recess where a spring fits flush."""
    ledge(L, x0, center - width / 2.0, top, h)
    ledge(L, center + width / 2.0, x1, top, h)
    ledge(L, center - width / 2.0, center + width / 2.0, top - depth, h - depth)
    return (center, top - SPRING_FLUSH)


def level_051():
    L = Level(51, "Première plateforme", "First platform", 5, time_limit=16)
    L.spawn(-4.3, 1.2)
    ledge(L, -5.0, -2.6, 0.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.moving(-1.4, -0.4, 1.8, (3.4, 0.0), period=4.0, phase=0.55)
    ledge(L, 2.9, 5.0, -0.2)
    L.goal(4.1, 0.5, r=0.7)
    zr = L.area(-3.8, 0.9, 1.6, 0.6, [R], capacity=1, default_rot=-15, min_rot=-30, max_rot=30)
    L.path_crystal(0.6)
    L.items(ramp=1)
    L.stars(NoHazard(key=L.text('obj.l051.ride', "Rester sur la plateforme jusqu'au bout", "Stay on the platform until the end")),
            Crystals(1, key=L.text('obj.l051.platform', "Cristal sur la plateforme", "Crystal on the platform")))
    L.solution((R, zr, None))
    L.tip("Attends que la plateforme t'emmène.", "Let the platform carry you.")
    return L


def level_052():
    L = Level(52, "Ascenseur", "Lift", 5, time_limit=16)
    L.spawn(-3.8, -1.2)
    slots = ground_with_recesses(L, -5, -1.2, -4.0, [-3.8])
    L.moving(-0.2, -4.2, 1.6, (0.0, 5.0), period=6.0)
    L.wall(0.85, -4.6, 0.4)
    ledge(L, 0.6, 5.0, 0.4)
    L.wall(-1.2, -4.0, -2.6)
    L.ceiling(-5.0, -1.0, bottom=-0.4, h=0.5)
    pit_spikes(L, -1.0, 0.6, y=-4.8)
    L.goal(1.3, 1.2, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.85)
    L.items(spring=1)
    L.stars(Bounces(1, EXACTLY, key=L.text('obj.l052.one', "Un seul rebond", "A single bounce")),
            Crystals(1, key=L.text('obj.l052.top', "Cristal au sommet", "Crystal at the top")))
    L.solution((S, zs, None))
    L.tip("Rebondis sur l'ascenseur au bon moment.", "Bounce onto the lift at the right time.")
    return L


def level_053():
    L = Level(53, "Plateforme rotative", "Rotating platform", 6, time_limit=16)
    L.spawn(-4.3, 3.2)
    ledge(L, -5.0, -2.8, 2.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.rotating(0.0, -0.6, 3.0, speed=60.0, oscillation=25.0)
    ledge(L, 2.8, 5.0, -1.6)
    L.goal(4.0, -0.9, r=0.7)
    zr = L.area(-3.8, 2.9, 1.6, 0.6, [R], capacity=1, default_rot=-15, min_rot=-30, max_rot=30)
    L.path_crystal(0.55)
    L.items(ramp=1)
    L.stars(NoHazard(), Crystals(1, key=L.text('obj.l053.outer', "Cristal extérieur", "Outer crystal")))
    L.solution((R, zr, None))
    L.tip("La plateforme penche : profite de son inclinaison.", "The platform tilts: use its slope.")
    return L


def level_054():
    L = Level(54, "Machine à rebond", "Bounce machine", 6, time_limit=16)
    L.spawn(-3.4, 0.2)
    slots = ground_with_recesses(L, -5, -2.4, -4.0, [-3.4])
    pit_spikes(L, -2.4, 5.0, y=-4.4)
    L.ground(-2.4, 5.0, top=-4.8, h=0.4)
    L.ceiling(-5.0, -1.4, bottom=1.4, h=0.5)
    L.moving(-1.0, -0.6, 1.8, (3.0, 0.0), period=5.0, phase=0.62)
    L.moving(-0.2, -0.3, 0.2, (3.0, 0.0), period=5.0, phase=0.62, h=0.6)
    L.obstacle(1.1, 4.5, 0.5, 8.2)
    L.wall(5.0, -4.4, 9.0)
    L.goal(2.4, 0.1, r=0.6)
    zr = L.area(-1.8, 0.6, 1.2, 0.6, [R], capacity=1, default_rot=-20, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.7)
    L.items(spring=1, ramp=1)
    L.stars(MaxObjects(2), Attempts(3, key=L.text('obj.l054.tries', "Moins de 3 essais", "Fewer than 3 attempts")))
    L.solution((S, zs, -15))
    L.tip("Le rebond doit tomber sur la plateforme au bon moment.", "The bounce has to land on the platform at the right time.")
    return L


def level_055():
    L = Level(55, "Deux plateformes", "Two platforms", 6, time_limit=16)
    L.spawn(-4.5, 4.4)
    slope(L, -5.0, 3.4, -3.3, 2.42)
    ledge(L, -3.3, -2.6, 2.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.moving(-1.6, 1.2, 1.8, (2.2, 0.0), period=4.0, phase=0.5)
    L.moving(0.6, -1.2, 1.8, (2.0, 0.0), period=4.0, phase=0.35)
    sp = pad(L, 3.4, 5.0, -2.6, 4.2)
    L.wall(5.0, -2.6, 9.0)
    L.goal(4.0, 2.4, r=0.7)
    zs = L.slot(sp[0], sp[1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.35)
    L.path_crystal(0.7)
    L.items(spring=1)
    L.stars(Avoid('Ground', key=L.text('obj.l055.air', "Ne pas toucher le sol", "Never touch the ground")), Crystals(2))
    L.solution((S, zs, None))
    L.tip("Passe d'une plateforme à l'autre.", "Hop from one platform to the other.")
    return L


def level_056():
    L = Level(56, "Transport aérien", "Air transport", 6, time_limit=16)
    L.spawn(-4.5, 1.8)
    slope(L, -5.0, 1.0, -3.3, 0.42)
    ledge(L, -3.3, -2.6, 0.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.moving(-1.4, -0.4, 1.8, (3.0, 0.0), period=4.0, phase=0.55)
    ledge(L, 3.9, 5.0, 0.5)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.4, 1.2, r=0.7)
    zf = L.slot(3.2, -1.4, [F], default_rot=0, min_rot=-45, max_rot=45)
    zf2 = L.slot(-4.0, 3.4, [F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal('apex')
    L.items(fan=1)
    L.stars(UseItem(F, 1, AT_MOST, key=L.text('obj.l056.one', "Un seul ventilateur", "A single fan")),
            Crystals(1, key=L.text('obj.l056.high', "Cristal en hauteur", "High crystal")))
    L.solution((F, zf, -15))
    L.tip("Le vent soulève Trykli au bout du trajet.", "The wind lifts Trykli at the end of the ride.")
    return L


def level_057():
    L = Level(57, "Portail mobile", "Moving portal", 6, time_limit=16)
    L.spawn(-4.0, 3.6)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.obstacle(-2.8, 1.0, 0.5, 15.0)
    L.moving(-1.4, -1.0, 1.8, (3.0, 0.0), period=5.0, phase=0.8)
    L.moving(-0.6, -0.7, 0.2, (3.0, 0.0), period=5.0, phase=0.8, h=0.6)
    L.moving(-2.2, -0.7, 0.2, (3.0, 0.0), period=5.0, phase=0.8, h=0.6)
    L.obstacle(1.3, 4.1, 0.5, 8.0)
    L.wall(5.0, -3.8, 9.0)
    L.goal(2.2, -0.2, r=0.6)
    za = L.slot(-4.0, 2.0, [P])
    zb = L.slot(-1.2, 0.8, [P])
    zr = L.area(-4.0, -1.6, 1.6, 0.6, [R], capacity=1, default_rot=-20, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.items(portal_ab=1, ramp=1)
    L.stars(MaxObjects(2), Crystals(1, key=L.text('obj.l057.after', "Cristal après le portail", "Crystal after the portal")))
    L.solution((P, za, 0), (P, zb, 180))
    L.tip("La sortie du portail doit tomber sur la plateforme.", "The portal exit has to drop onto the platform.")
    return L


def basket(L, x, y, travel, w=1.6, **kw):
    """Moving platform with two small lips so that Trykli stays on it while it travels."""
    L.moving(x, y, w, travel, **kw)
    kw = dict(kw)
    kw['h'] = 0.6
    L.moving(x - w / 2.0 + 0.1, y + 0.3, 0.2, travel, **kw)
    L.moving(x + w / 2.0 - 0.1, y + 0.3, 0.2, travel, **kw)


def level_058():
    L = Level(58, "Activation mécanique", "Mechanical trigger", 6, time_limit=16)
    L.spawn(-3.8, -0.6)
    L.ceiling(-5.0, 0.8, bottom=0.4, h=0.5)
    slots = ground_with_recesses(L, -5, -2.4, -4.0, [-3.8])
    pit_spikes(L, -2.4, 5.0, y=-4.4)
    L.ground(-2.4, 5.0, top=-4.8, h=0.4)
    ledge(L, -1.8, 0.2, -1.0)
    L.button(-0.8, -0.95, emit=0, bid='b1')
    basket(L, 1.0, -2.4, (2.4, 0.0), period=4.0, delay=0.8, loop=False, channel=0, activation=sim.ACT_ACTIVATED)
    L.wall(5.0, -4.4, 9.0)
    L.goal(3.4, -1.7, r=0.6)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zs2 = L.slot(-2.8, -0.2, [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.45)
    L.items(spring=1)
    L.stars(Interaction('button', 1, key='obj.l036.button'), Crystals(1))
    L.solution((S, zs, -30))
    L.tip("Le bouton démarre la plateforme.", "The button starts the platform.")
    return L


def level_059():
    L = Level(59, "Timing indirect", "Indirect timing", 6, time_limit=16)
    L.spawn(-4.5, 1.8)
    slope(L, -5.0, 1.0, -3.3, 0.42)
    ledge(L, -3.3, -2.8, 0.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    sp = pad(L, -2.6, -0.6, -2.4, -1.2)
    L.moving(0.2, -0.4, 1.8, (1.8, 0.0), period=4.0, phase=0.8)
    ledge(L, 3.9, 5.0, 0.5)
    L.obstacle(2.6, 4.8, 0.5, 8.4)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.4, 1.2, r=0.7)
    zs = L.slot(sp[0], sp[1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zf = L.slot(3.2, -1.4, [F], default_rot=0, min_rot=-45, max_rot=45)
    zf2 = L.slot(-1.4, 2.6, [F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.items(fan=1, spring=1)
    L.stars(MaxObjects(2), NoStop(2.5, key=L.text('obj.l059.flow', "Terminer sans attendre bloqué", "Finish without waiting stuck")))
    L.solution((S, zs, -30), (F, zf, -15))
    L.tip("Le bon placement crée le bon délai.", "The right placement creates the right delay.")
    return L


def level_060():
    L = Level(60, "Boss machines", "Machines boss", 6, time_limit=20)
    L.spawn(-4.4, 5.4)
    slope(L, -5.0, 4.6, -3.6, 3.62)
    ledge(L, -3.6, -3.0, 3.6)
    L.rotating(-1.6, 2.4, 2.6, speed=60.0, oscillation=25.0)
    pit_spikes(L, -5.0, 1.6, y=-3.8)
    L.ground(-5, 1.6, top=-4.2, h=0.8)
    L.wall(1.6, -4.2, 1.6)
    L.moving(3.2, -4.0, 2.6, (0.0, 4.4), period=6.0, phase=0.1)
    ledge(L, 4.5, 5.0, 0.6)
    L.wall(5.0, -4.2, 9.0)
    L.ground(1.6, 5.0, top=-4.6, h=0.6)
    L.ceiling(1.35, 5.0, bottom=2.2, h=0.5)
    L.goal(3.4, 1.3, r=0.7)
    za = L.slot(0.8, -0.8, [P])
    zb = L.slot(3.2, -1.0, [P])
    zs = L.slot(-2.8, -1.0, [S], default_rot=0, min_rot=-45, max_rot=45)
    zf = L.slot(0.4, -2.8, [F], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, fan=1, portal_ab=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((P, za, 0), (P, zb, 0))
    L.tip("Plateforme, portail, ascenseur : une vraie usine.", "Platform, portal, lift: a real factory.")
    return L


def levels():
    return [level_051(), level_052(), level_053(), level_054(), level_055(), level_056(), level_057(), level_058(), level_059(),
            level_060()]
