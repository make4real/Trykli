"""World 3 - Portails (portal pairs, directional exits). Difficulty 3-4."""
from common import *

P = 'portal_ab'
Q = 'portal_cd'


def level_021():
    L = Level(21, "Premier portail", "First portal", 3, time_limit=10)
    L.spawn(-3.5, 7.8)
    L.ground(-5, -1.5, top=-4.0)
    pit_spikes(L, -5.0, -1.5, y=-3.8)
    L.ground(-1.5, 5.0, top=-4.0)
    L.obstacle(-1.2, 0.0, 0.5, 8.0)
    L.goal(3.2, -3.2, r=0.7)
    za = L.slot(-3.5, 0.5, [P], default_rot=0)
    zb = L.slot(3.2, 1.5, [P], default_rot=0)
    L.path_crystal(0.8)
    L.items(portal_ab=1)
    L.stars(OnlyItem(P, key=L.text('obj.l021.only', "Utiliser uniquement les portails", "Use only the portals")), Crystals(1))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("Trykli ressort du portail B dans la direction de sa flèche.", "Trykli leaves portal B the way its arrow points.")
    return L


def level_022():
    L = Level(22, "Conserver l'élan", "Keep the momentum", 3, time_limit=12)
    L.spawn(-4.0, 7.8)
    slots = ground_with_recesses(L, -5, -2.0, -4.0, [-4.0])
    pit_spikes(L, -2.0, 5.0)
    L.platform(3.8, 3.4, 2.4)
    L.wall(5.0, 3.6, 9.0)
    L.goal(4.0, 4.4, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    za = L.slot(-1.4, 1.2, [P], default_rot=0)
    zb = L.slot(1.6, -2.4, [P], default_rot=0)
    zc = L.slot(-3.0, 4.4, [P], default_rot=0)
    L.path_crystal(0.75)
    L.items(spring=1, portal_ab=1)
    L.stars(Bounces(1, EXACTLY, key=L.text('obj.l022.one', "1 seul rebond", "A single bounce")),
            Crystals(1, key=L.text('obj.l022.after', "Récupérer le cristal après le portail", "Collect the crystal after the portal")))
    L.solution(('spring', zs, None), (P, za, None), (P, zb, None))
    L.tip("La vitesse est conservée à la sortie du portail.", "Speed is kept when leaving a portal.")
    return L


def level_023():
    L = Level(23, "Sortie verticale", "Vertical exit", 3, time_limit=12)
    L.spawn(-3.6, 7.8)
    pit_spikes(L, -5.0, 1.4, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.platform(-0.2, 3.2, 3.4)
    L.goal(3.2, 3.1, r=0.7)
    za = L.slot(-3.6, 1.0, [P], default_rot=0)
    zb = L.slot(3.2, -2.6, [P], default_rot=0)
    zb2 = L.slot(1.8, -2.6, [P], default_rot=0)
    zr = L.area(1.6, 6.4, 2.4, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.85)
    L.items(portal_ab=1, ramp=1)
    L.stars(Avoid('Ground', key=L.text('obj.l023.ground', "Pas de contact avec le sol après la téléportation", "No ground contact after teleporting")),
            Crystals(1, key='obj.l013.top'))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("Oriente la sortie vers le haut.", "Point the exit upwards.")
    return L


def level_024():
    L = Level(24, "Saut dimensionnel", "Dimensional jump", 3, time_limit=12)
    L.spawn(-4.0, 7.8)
    slots = ground_with_recesses(L, -5, 0.0, -4.0, [-4.0])
    pit_spikes(L, 0.0, 5.0)
    L.obstacle(0.2, 2.0, 0.5, 12.0)
    L.platform(3.6, -0.6, 2.4)
    L.goal(3.8, 0.4, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    za = L.slot(-1.2, 1.8, [P], default_rot=0)
    za2 = L.slot(-2.6, 4.0, [P], default_rot=0)
    zb = L.slot(2.0, 3.0, [P], default_rot=0)
    L.path_crystal(0.6)
    L.items(spring=1, portal_ab=1)
    L.stars(MaxObjects(2), Time(7))
    L.solution(('spring', zs, None), (P, za, None), (P, zb, None))
    L.tip("Vise le portail avec le ressort.", "Aim at the portal with the spring.")
    return L


def level_025():
    L = Level(25, "Deux paires", "Two pairs", 4, time_limit=14)
    L.spawn(-3.8, 7.8)
    L.block(-1.4, 4.4, 0.5, 9.2, surface='Wall')
    L.block(1.8, 4.4, 0.5, 9.2, surface='Wall')
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.platform(3.4, 2.0, 2.4)
    L.goal(3.6, 3.0, r=0.7)
    za = L.slot(-3.8, 0.0, [P, Q], default_rot=0)
    zb = L.slot(0.2, 6.5, [P, Q], default_rot=0)
    zc = L.slot(0.2, 0.0, [P, Q], default_rot=0)
    zd = L.slot(3.4, 6.8, [P, Q], default_rot=0)
    L.path_crystal(0.4)
    L.path_crystal(0.75)
    L.items(portal_ab=1, portal_cd=1)
    L.stars(Interaction('portal', 2, AT_LEAST, key=L.text('obj.l025.pairs', "Utiliser les 2 paires", "Use both pairs")), Crystals(2))
    L.solution((P, za, 0), (P, zb, None), (Q, zc, 0), (Q, zd, None))
    L.tip("Enchaîne les deux paires, dans le bon ordre.", "Chain both pairs, in the right order.")
    return L


def level_026():
    L = Level(26, "Portail dangereux", "Dangerous portal", 4, time_limit=12)
    L.spawn(-3.6, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    L.wall(5.0, -4.0, 9.0)
    pit_spikes(L, -4.75, 4.75, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.platform(2.6, 1.2, 2.6)
    L.spikes(2.6, 2.95, 1.2, rot=180)
    L.block(2.6, 3.4, 1.4, 0.5, surface='Obstacle')
    L.spikes(4.4, 1.6, 0.6)
    L.goal(3.2, 2.0, r=0.6)
    za = L.slot(-3.6, 0.0, [P], default_rot=0)
    zb = L.slot(0.0, 2.2, [P], default_rot=0)
    zb2 = L.slot(0.0, -1.6, [P], default_rot=0)
    zr = L.area(1.0, 5.6, 2.4, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.8)
    L.items(portal_ab=1, ramp=1)
    L.stars(Avoid('Wall', key='obj.l016.walls'), Crystals(1))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("Une mauvaise sortie mène droit aux pics.", "A bad exit leads straight to the spikes.")
    return L


def level_027():
    L = Level(27, "Portail dans le vent", "Portal in the wind", 4, time_limit=12)
    L.spawn(-3.8, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.platform(3.9, 0.4, 1.8)
    L.wall(5.0, 0.6, 9.0)
    L.goal(4.0, 1.4, r=0.7)
    za = L.slot(-3.8, 0.6, [P], default_rot=0)
    zb = L.slot(-0.6, -2.4, [P], default_rot=0)
    zb2 = L.slot(1.6, -2.4, [P], default_rot=0)
    zf = L.slot(0.2, 3.6, ['fan'], default_rot=0)
    zf2 = L.slot(-2.4, 3.6, ['fan'], default_rot=0)
    L.path_crystal(0.7)
    L.items(portal_ab=1, fan=1)
    L.stars(MaxObjects(2), Crystals(1, key=L.text('obj.l027.air', "Récupérer le cristal en plein vol", "Collect the mid-air crystal")))
    L.solution((P, za, 0), (P, zb2, None), ('fan', zf, None))
    L.tip("Le vent corrige la sortie du portail.", "The wind corrects the portal exit.")
    return L


def level_028():
    L = Level(28, "Orientation libre", "Free orientation", 4, time_limit=12)
    L.spawn(-1.0, 7.8)
    pit_spikes(L, -5.0, 3.2, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.wall(-5.0, -4.0, 9.0)
    L.platform(4.1, -3.9, 1.8)
    L.obstacle(1.6, 2.6, 0.5, 5.0)
    L.goal(4.2, -3.0, r=0.7)
    slots = ground_with_recesses(L, -2.4, -1.2, -1.6, [-1.8], recess_width=1.1, depth=0.18, h=0.6)
    za = L.slot(-1.0, 1.0, [P], default_rot=0)
    zb = L.slot(-4.3, -1.0, [P], default_rot=0)
    zs = L.slot(-1.8, -1.7, ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.7, cid='hidden')
    L.items(portal_ab=1, spring=1)
    L.stars(Attempts(3, key=L.text('obj.l028.tries', "Trouver l'angle en 3 essais maximum", "Find the angle within 3 tries")),
            Crystals(1, key=L.text('obj.l028.hidden', "Récupérer le cristal caché", "Collect the hidden crystal")))
    L.solution((P, za, 0), (P, zb, None))
    L.tip("C'est l'orientation du portail de sortie qui compte.", "The orientation of the exit portal is what matters.")
    return L


def level_029():
    L = Level(29, "Fausse route", "Dead end", 4, time_limit=14)
    L.spawn(-4.0, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.portal(-1.6, 4.6, 180, 2, 'A')
    L.portal(-4.2, -2.6, 0, 2, 'B')
    L.block(-4.2, -3.2, 1.4, 0.4, surface='Ground')
    L.block(-3.4, -2.2, 0.3, 2.4, surface='Wall')
    L.platform(3.8, 0.8, 2.4)
    L.goal(4.0, 1.8, r=0.7)
    za = L.slot(-4.0, 3.0, [P, Q], default_rot=0)
    zb = L.slot(0.6, 6.4, [P, Q], default_rot=0)
    zc = L.slot(0.6, 2.4, [P, Q], default_rot=0)
    zd = L.slot(2.6, 4.6, [P, Q], default_rot=0)
    zr = L.area(-2.4, 1.2, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)
    L.path_crystal(0.8)
    L.items(portal_ab=1, portal_cd=1, ramp=1)
    L.stars(Interaction('portal:2', 0, AT_MOST, key=L.text('obj.l029.fake', "Ne jamais entrer dans le faux portail", "Never enter the fake portal")),
            Crystals(2))
    L.solution((P, za, 0), (P, zc, None))
    L.tip("Le portail fixe ne mène nulle part.", "The fixed portal leads nowhere.")
    return L


def level_030():
    L = Level(30, "Boss dimensionnel", "Dimension boss", 4, time_limit=18)
    L.spawn(-4.0, 7.8)
    slots = ground_with_recesses(L, -5, -2.0, -4.0, [-4.0])
    pit_spikes(L, -2.0, 5.0)
    L.obstacle(0.4, 3.0, 0.5, 11.0)
    L.platform(3.9, 1.6, 2.0)
    L.wall(5.0, 1.8, 9.0)
    L.goal(4.0, 2.6, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    za = L.slot(-3.1, -1.2, [P], default_rot=0)
    za2 = L.slot(-2.0, 3.6, [P], default_rot=0)
    zb = L.slot(1.7, -2.2, [P], default_rot=0)
    zb2 = L.slot(2.6, 6.8, [P], default_rot=0)
    zf = L.slot(0.9, 1.6, ['fan'], default_rot=0)
    zf2 = L.slot(3.2, -1.0, ['fan'], default_rot=0)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=1, fan=1, portal_ab=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution(('spring', zs, None), (P, za, None), (P, zb, None), ('fan', zf, None))
    L.tip("Ressort, portail, courant d'air : dans cet ordre.", "Spring, portal, air stream: in that order.")
    return L


def levels():
    return [level_021(), level_022(), level_023(), level_024(), level_025(), level_026(), level_027(), level_028(),
            level_029(), level_030()]
