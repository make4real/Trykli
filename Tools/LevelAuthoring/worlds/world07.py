"""World 7 - Systèmes (buttons, doors, lasers). Difficulty 6-7."""
from common import *

S = 'spring'
R = 'ramp'
F = 'fan'
M = 'magnet'
P = 'portal_ab'


def ledge(L, x0, x1, top, h=0.6):
    return L.block((x0 + x1) / 2.0, top - h / 2.0, x1 - x0, h, surface='Platform')


def level_061():
    L = Level(61, "Ouvre la porte", "Open the door", 6, time_limit=14)
    L.spawn(-4.0, 5.4)
    pit_spikes(L, -5.0, -2.6, y=-3.8)
    L.ground(-5, -2.6, top=-4.2, h=0.8)
    L.ground(-2.6, 5.0, top=-4.0)
    L.button(-0.8, -3.95, emit=0, bid='b1')
    L.door(1.6, -2.8, channel=0, h=2.4)
    L.block(1.6, 2.6, 0.5, 8.4, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.6, -3.3, r=0.7)
    L.crystal(2.6, -3.5)
    zr = L.area(-3.8, 1.4, 1.6, 1.0, [R], capacity=1, default_rot=-30, min_rot=-45, max_rot=45)
    L.items(ramp=1)
    L.stars(Interaction('button', 1, key='obj.l036.button'),
            Crystals(1, key=L.text('obj.l061.behind', "Cristal derrière la porte", "Crystal behind the door")))
    L.solution((R, zr, None))
    L.tip("Le bouton ouvre la porte : passe dessus avant d'y arriver.", "The button opens the door: roll over it first.")
    return L


def level_062():
    L = Level(62, "Bouton détour", "Button detour", 6, time_limit=14)
    L.spawn(-3.4, -1.0)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 1.6, bottom=2.0, h=0.5)
    ledge(L, -1.6, 0.4, 0.0)
    L.button(0.0, 0.05, emit=0, bid='b1')
    slope(L, 0.4, 0.0, 2.3, -3.98)
    L.door(2.6, -2.8, channel=0, h=2.4)
    L.block(2.6, 2.6, 0.5, 8.4, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.0, -3.3, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.items(spring=1)
    L.stars(Interaction('button', 1, key='obj.l036.button'),
            Crystals(1))
    L.solution((S, zs, None))
    L.tip("Le chemin direct bute sur la porte : passe d'abord par le bouton.", "The direct path hits the door: go through the button first.")
    return L


def level_063():
    L = Level(63, "Premier laser", "First laser", 6, time_limit=14)
    L.spawn(-4.0, 6.4)
    L.ground(-5, 5.0, top=-4.0)
    ledge(L, -2.6, 1.0, 0.6)
    L.laser(1.7, -4.0, rot=0, length=4.0)
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, -3.3, r=0.7)
    L.crystal(1.7, 1.1)
    zr = L.area(-3.8, 2.4, 1.6, 1.0, [R], capacity=1, default_rot=-30, min_rot=-45, max_rot=45)
    L.items(ramp=1)
    L.stars(NoHazard('Laser', key=L.text('obj.l063.laser', "Aucun contact laser", "No laser contact")),
            Crystals(1, key=L.text('obj.l063.near', "Cristal proche du laser", "Crystal near the laser")))
    L.solution((R, zr, None))
    L.tip("Passe au-dessus du rayon.", "Go over the beam.")
    return L


def level_064():
    L = Level(64, "Laser intermittent", "Blinking laser", 6, time_limit=14)
    L.spawn(-3.4, -1.0)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 5.0, bottom=2.0, h=0.5)
    L.laser(0.6, -4.0, rot=0, length=5.8, on=1.0, off=1.0, phase=0.0)
    L.wall(5.0, -4.0, 2.0)
    L.goal(3.8, -3.3, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.6)
    L.items(spring=1)
    L.stars(NoHazard('Laser', key='obj.l063.laser'), Attempts(3, key='obj.l054.tries'))
    L.solution((S, zs, None))
    L.tip("Choisis l'angle qui arrive pendant l'extinction.", "Pick the angle that arrives while the beam is off.")
    return L


def level_065():
    L = Level(65, "Couper le laser", "Cut the laser", 7, time_limit=14)
    L.spawn(-3.4, -1.0)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 5.0, bottom=2.4, h=0.5)
    ledge(L, -1.4, 0.4, 0.4)
    L.button(0.0, 0.45, emit=0, bid='b1')
    slope(L, 0.4, 0.4, 2.3, -3.98)
    L.laser(2.9, -4.0, rot=0, length=6.2, channel=0, activation=sim.ACT_DEACTIVATED)
    L.wall(5.0, -4.0, 2.4)
    L.goal(4.0, -3.3, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zr = L.area(-2.2, 1.6, 1.4, 0.6, [R], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.4)
    L.items(spring=1, ramp=1)
    L.stars(Interaction('laser_off', 1, key=L.text('obj.l065.off', "Désactiver le laser", "Switch the laser off")), MaxObjects(2))
    L.solution((S, zs, None))
    L.tip("Un bouton peut couper le laser.", "A button can switch the laser off.")
    return L


def level_066():
    L = Level(66, "Porte dimensionnelle", "Dimensional door", 7, time_limit=14)
    L.spawn(-4.2, 6.4)
    L.ground(-5, 5.0, top=-4.0)
    slope(L, -0.2, -2.2, 1.7, -3.98)
    L.door(1.95, -2.8, channel=0, h=2.4)
    slope(L, 1.7, -1.4, 5.0, 0.2)
    L.block(3.5, -1.6, 3.4, 0.4, surface='Wall')
    L.button(3.0, -0.63, emit=0, bid='b1', rot=25.9)
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, -3.3, r=0.7)
    za = L.slot(-4.2, 1.6, [P])
    zb = L.slot(4.3, 2.0, [P])
    zb2 = L.slot(-1.8, 3.0, [P])
    L.path_crystal(0.5)
    L.items(portal_ab=1)
    L.stars(Interaction('portal', 1, key=L.text('obj.l066.portal', "Utiliser le portail", "Use the portal")),
            Crystals(1, key=L.text('obj.l066.secret', "Cristal secret", "Secret crystal")))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("Le bouton est derrière la porte... mais pas pour un portail.", "The button is behind the door... but not for a portal.")
    return L


def level_067():
    L = Level(67, "Double commande", "Double switch", 7, time_limit=16)
    L.spawn(-3.4, -1.0)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 1.6, bottom=2.0, h=0.5)
    ledge(L, -1.6, 0.4, 0.0)
    L.button(0.0, 0.05, emit=0, bid='b1')
    slope(L, 0.4, 0.0, 2.1, -3.98)
    L.door(2.4, -2.8, channel=0, h=2.4)
    L.block(2.4, 2.6, 0.5, 8.4, surface='Wall')
    L.button(3.1, -3.95, emit=1, bid='b2')
    L.door(3.8, -2.8, channel=1, h=2.4)
    L.block(3.8, 2.6, 0.5, 8.4, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.45, -3.4, r=0.5)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zf = L.slot(-2.4, -3.775, [F], default_rot=-90, min_rot=-90, max_rot=90)
    L.path_crystal(0.3)
    L.items(spring=1, fan=1)
    L.stars(Interaction('button', 2, key=L.text('obj.l067.both', "Activer les 2 boutons", "Press both buttons")), MaxObjects(2))
    L.solution((S, zs, -15))
    L.tip("Chaque bouton ouvre la porte suivante.", "Each button opens the next door.")
    return L


def level_068():
    L = Level(68, "Ordre logique", "Logical order", 7, time_limit=16)
    L.spawn(-4.2, 4.4)
    L.ground(-5, 5.0, top=-4.0)
    L.wall(-3.0, -4.0, 5.0)
    L.button(-1.9, -3.95, emit=0, bid='b1')
    L.door(-0.9, -2.8, channel=0, h=2.4)
    L.button(0.2, -3.95, emit=1, bid='b2')
    L.door(1.2, -2.8, channel=1, h=2.4)
    L.door(2.4, -2.8, channel=0, h=2.4)
    L.ceiling(-3.0, 5.0, bottom=-1.6, h=0.5)
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.0, -3.3, r=0.6)
    za = L.slot(-4.2, 0.0, [P])
    zb = L.slot(-2.4, -3.4, [P])
    zb2 = L.slot(-4.2, -2.8, [P])
    zr = L.area(-2.4, -3.8, 0.8, 0.3, [R], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.path_crystal(0.8)
    L.items(portal_ab=1, ramp=1)
    L.stars(Attempts(1, key=L.text('obj.l068.first', "Bon ordre du premier coup", "Right order on the first try")), Crystals(2))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("Le bouton de gauche ouvre deux portes, celui de droite une seule.", "The left button opens two doors, the right one only one.")
    return L


def level_069():
    L = Level(69, "Petit réseau", "Small network", 7, time_limit=16)
    L.spawn(-4.5, -0.8)
    slope(L, -5.0, -1.8, -3.4, -3.98)
    slots = ground_with_recesses(L, -3.4, 1.2, -4.0, [0.0])
    pit_spikes(L, 1.2, 5.0, y=-3.8)
    L.ground(1.2, 5.0, top=-4.2, h=0.8)
    L.button(-2.4, -3.95, emit=0, bid='b1')
    L.door(-1.3, -2.8, channel=0, h=2.4)
    L.block(-1.3, 2.6, 0.5, 8.4, surface='Wall')
    ledge(L, 1.2, 2.6, -0.6)
    L.ceiling(-1.05, 3.0, bottom=1.1, h=0.4)
    L.button(1.9, -0.55, emit=1, bid='b2')
    ledge(L, 3.6, 5.0, -0.2)
    L.wall(5.0, -3.8, 9.0)
    L.goal(4.3, 0.5, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(3.6, 1.8, [M], linked=1)
    zf = L.slot(-0.2, 2.0, [F], default_rot=0)
    L.path_crystal(0.35)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, fan=1, magnet=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution((S, zs, -15), (M, zm, 0))
    L.tip("Bouton, ressort, bouton, aimant : une cascade.", "Button, spring, button, magnet: a cascade.")
    return L


def level_070():
    L = Level(70, "Boss systèmes", "Systems boss", 7, time_limit=18)
    L.spawn(-4.4, 6.8)
    slots = ground_with_recesses(L, -5, 5.0, -4.0, [-3.4])
    L.ceiling(-5.0, 5.0, bottom=2.4, h=0.5)
    ledge(L, -1.4, 0.4, 0.4)
    L.button(0.0, 0.45, emit=0, bid='b1')
    slope(L, 0.4, 0.4, 2.3, -3.98)
    L.laser(2.7, -4.0, rot=0, length=6.2, channel=0, activation=sim.ACT_DEACTIVATED)
    L.button(3.3, -3.95, emit=1, bid='b2')
    L.door(3.95, -2.8, channel=1, h=2.4)
    L.block(3.95, 0.6, 0.5, 4.4, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.5, -3.4, r=0.5)
    za = L.slot(-4.4, 4.6, [P])
    zb = L.slot(-3.4, 1.4, [P])
    zb2 = L.slot(1.4, 1.6, [P])
    zs = L.slot(slots[0][0], slots[0][1], [S], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(-1.6, 1.8, [M])
    L.path_crystal(0.2)
    L.path_crystal(0.5)
    L.path_crystal(0.8)
    L.items(portal_ab=1, magnet=1, spring=1)
    L.stars(NoHazard('Laser', key='obj.l063.laser'), AllCrystals())
    L.solution((P, za, 0), (P, zb, 180), (S, zs, -15))
    L.tip("Portail, ressort, bouton, laser, porte : dans cet ordre.", "Portal, spring, button, laser, door: in that order.")
    return L


def levels():
    return [level_061(), level_062(), level_063(), level_064(), level_065(), level_066(), level_067(), level_068(), level_069(),
            level_070()]
