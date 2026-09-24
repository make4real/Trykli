"""World 2 - Le vent (fans). Difficulty 2-3."""
from common import *


def level_011():
    L = Level(11, "Premier ventilateur", "First fan", 2, time_limit=10)
    L.spawn(-3.2, 4.5)
    L.ground(-5, 5, top=-4.0)
    L.wall(-5.0, -4.0, 9.0)
    L.obstacle(0.6, -3.9, 0.8, 0.2)
    L.goal(3.9, -3.2, r=0.7)
    z = L.slot(-4.3, -3.55, ['fan'], default_rot=-90, allow_rot=False)
    L.items(fan=1)
    L.stars(UseItem('fan', 1, EXACTLY, key=L.text('obj.l011.fan', "Utiliser 1 ventilateur", "Use 1 fan")), Time(6))
    L.solution(('fan', z, -90))
    L.tip("Le vent pousse Trykli dans la direction du ventilateur.", "The wind pushes Trykli the way the fan points.")
    return L


def level_012():
    L = Level(12, "Vent contraire", "Headwind", 2, time_limit=12)
    L.spawn(0.0, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    L.wall(5.0, -1.0, 9.0)
    L.fan(4.4, 2.6, rot=90, length=6.0)
    L.ground(-4.75, -1.8, top=-4.0)
    pit_spikes(L, -1.8, 1.6)
    L.ground(1.6, 5.0, top=-4.0)
    L.goal(3.6, -3.2, r=0.7)
    zr = L.area(0.0, 0.6, 3.0, 1.2, ['ramp', 'fan'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zf = L.slot(-4.3, 2.6, ['fan', 'ramp'], default_rot=0)
    L.path_crystal(0.7)
    L.items(fan=1, ramp=1)
    L.stars(Avoid('Wall', key=L.text('obj.l012.left', "Ne pas toucher le mur gauche", "Do not touch the left wall")), Crystals(1))
    L.solution(('ramp', zr, None))
    L.tip("Une rampe bien inclinée résiste au vent.", "A well tilted ramp beats the wind.")
    return L


def level_013():
    L = Level(13, "Ascension", "Ascent", 2, time_limit=12)
    L.spawn(-4.4, 3.0)
    slope(L, -5.0, 1.4, -3.4, -2.02)
    upper = ground_with_recesses(L, -3.6, -0.8, -2.0, [-2.4], recess_width=1.0, depth=0.45, h=2.0)
    shaft = ground_with_recesses(L, -0.8, 0.8, -4.0, [0.0], recess_width=1.0, depth=0.45)
    L.block(1.05, -1.75, 0.5, 4.5, surface='Wall')
    L.ground(1.3, 5.0, top=-1.0, h=3.0)
    L.wall(5.0, -1.0, 9.0)
    L.goal(0.0, 0.7, r=0.7)
    z_up = L.slot(-2.4, -2.225, ['fan'], default_rot=0, allow_rot=False)
    z_shaft = L.slot(0.0, -4.225, ['fan'], default_rot=0, allow_rot=False)
    z_right = L.slot(3.0, -0.775, ['fan'], default_rot=0, allow_rot=False)
    L.block(3.0, -1.1, 1.0, 0.2, surface='Ground')
    L.path_crystal(0.85)
    L.items(fan=1)
    L.stars(MaxObjects(1, key=L.text('obj.l013.one', "Un seul placement", "A single placement")),
            Crystals(1, key=L.text('obj.l013.top', "Récupérer le cristal supérieur", "Collect the top crystal")))
    L.solution(('fan', z_shaft, 0))
    L.tip("Un courant vertical peut soulever Trykli.", "A vertical stream can lift Trykli.")
    return L


def level_014():
    L = Level(14, "Souffle et rebond", "Blow and bounce", 2, time_limit=12)
    L.auto_goal = True
    L.spawn(-3.4, 7.8)
    slots = ground_with_recesses(L, -5, 1.0, -4.0, [-3.4])
    pit_spikes(L, 1.0, 5.0)
    L.wall(-5.0, -4.0, 9.0)
    L.platform(3.9, 0.4, 2.2)
    L.wall(5.0, 0.6, 9.0)
    L.goal(4.0, 1.4, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-15, max_rot=15)
    zf = L.slot(-2.2, 1.4, ['fan'], default_rot=0)
    zf2 = L.slot(-4.3, 4.0, ['fan'], default_rot=0)
    L.path_crystal(0.6)
    L.items(fan=1, spring=1)
    L.stars(UseAll(2, key='obj.l004.both'), Bounces(1, EXACTLY))
    L.solution(('spring', zs, None), ('fan', zf, None))
    L.tip("Lance Trykli dans le courant d'air.", "Launch Trykli into the air stream.")
    return L


def level_015():
    L = Level(15, "Double courant", "Double stream", 3, time_limit=14)
    L.spawn(-3.9, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    L.block(-2.4, 2.6, 0.5, 11.0, surface='Wall')
    floor = ground_with_recesses(L, -5, 5, -4.0, [0.2, 4.0], recess_width=1.0, depth=0.45)
    L.block(2.9, 0.1, 0.5, 5.8, surface='Wall')
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.0, 0.6, r=0.7)
    zf1 = L.slot(-4.3, -3.55, ['fan'], default_rot=0)
    zf_mid = L.slot(0.2, -4.225, ['fan'], default_rot=0)
    zf2 = L.slot(4.0, -4.225, ['fan'], default_rot=0)
    L.path_crystal(0.35)
    L.path_crystal(0.8)
    L.items(fan=2)
    L.stars(UseItem('fan', 2, AT_LEAST, key=L.text('obj.l015.fans', "Utiliser les 2 ventilateurs", "Use both fans")), Crystals(2))
    L.solution(('fan', zf1, -90), ('fan', zf2, 0))
    L.tip("Un ventilateur pour avancer, un autre pour monter.", "One fan to move forward, another one to climb.")
    return L


def level_016():
    L = Level(16, "Passage étroit", "Narrow passage", 3, time_limit=14)
    L.spawn(-3.7, 3.0)
    L.wall(-5.0, -4.0, 9.0)
    L.wall(5.0, -4.0, 9.0)
    ground_with_recesses(L, -4.75, 4.75, -4.0, [-2.6, 0.0], recess_width=1.0, depth=0.45)
    L.spikes(1.7, -3.8, 1.8)
    L.block(1.2, -1.15, 4.4, 0.5, surface='Ceiling')
    L.spikes(1.2, -1.6, 4.2, rot=180)
    L.goal(4.0, -3.2, r=0.7)
    zf0 = L.slot(-4.5, -3.55, ['fan'], default_rot=-90)
    zf1 = L.slot(-2.6, -4.225, ['fan'], default_rot=0)
    zf2 = L.slot(0.0, -4.225, ['fan'], default_rot=0)
    zr = L.area(1.7, -3.3, 2.0, 0.6, ['ramp'], capacity=1, default_rot=0, min_rot=-30, max_rot=30)
    L.path_crystal(0.6)
    L.items(fan=2, ramp=1)
    L.stars(Avoid('Wall', key=L.text('obj.l016.walls', "Ne toucher aucun mur", "Never touch a wall")),
            Crystals(1, key=L.text('obj.l016.center', "Récupérer le cristal central", "Collect the central crystal")))
    L.solution(('fan', zf0, -90), ('fan', zf1, 0))
    L.tip("Un petit saut au bon moment évite les deux rangées.", "A small hop at the right time avoids both rows.")
    return L


def level_017():
    L = Level(17, "Courbe aérienne", "Air curve", 3, time_limit=14)
    L.spawn(-3.4, 7.8)
    slots = ground_with_recesses(L, -5, -1.0, -4.0, [-3.4])
    pit_spikes(L, -1.0, 5.0)
    L.wall(-5.0, -4.0, 9.0)
    L.obstacle(0.6, -1.0, 0.5, 5.0)
    L.platform(3.6, 0.6, 2.4)
    L.goal(3.9, 1.6, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zf1 = L.slot(-2.4, 1.6, ['fan'], default_rot=0)
    zf2 = L.slot(0.6, 6.2, ['fan'], default_rot=0)
    zf3 = L.slot(-4.3, 0.4, ['fan'], default_rot=0)
    L.path_crystal(0.4)
    L.path_crystal(0.8)
    L.items(fan=2, spring=1)
    L.stars(MaxObjects(2), Crystals(2))
    L.solution(('spring', zs, None), ('fan', zf1, None))
    L.tip("Monte d'abord, glisse ensuite.", "Go up first, then drift.")
    return L


def level_018():
    L = Level(18, "Cristaux du ciel", "Sky crystals", 3, time_limit=14)
    L.spawn(-4.0, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    L.ground(-4.75, -1.0, top=-4.0)
    pit_spikes(L, -1.0, 2.8)
    L.ground(2.8, 5.0, top=-4.0)
    L.goal(3.9, -3.2, r=0.7)
    zr = L.area(-3.8, 3.4, 1.6, 1.2, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zf1 = L.slot(-4.3, -3.55, ['fan'], default_rot=0)
    zf2 = L.slot(-1.6, -3.55, ['fan'], default_rot=0)
    zf3 = L.slot(0.9, 6.5, ['fan'], default_rot=0)
    L.path_crystal(0.35)
    L.path_crystal(0.55)
    L.path_crystal(0.75)
    L.items(fan=2, ramp=1)
    L.stars(Crystals(2), AllCrystals())
    L.solution(('fan', zf1, -90), ('fan', zf2, 0))
    L.tip("Le vent peut porter Trykli très loin.", "The wind can carry Trykli very far.")
    return L


def level_019():
    L = Level(19, "Mauvaise orientation", "Wrong way", 3, time_limit=14)
    L.auto_goal = True
    L.spawn(-4.5, -0.8)
    slope(L, -5.0, -2.2, -4.0, -4.02)
    slots = ground_with_recesses(L, -4.1, -2.4, -4.0, [-3.3])
    L.wall(-5.0, -2.2, 9.0)
    pit_spikes(L, -2.4, 5.0)
    L.spikes(-3.3, 5.6, 2.4, rot=180)
    L.obstacle(-3.3, 6.05, 2.4, 0.5)
    L.spikes(-0.6, 3.4, 1.2, rot=-90)
    L.platform(3.4, 1.0, 2.6)
    L.wall(5.0, 1.2, 9.0)
    L.goal(3.6, 2.0, r=0.7)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, allow_rot=False)
    zf1 = L.slot(-4.4, 1.6, ['fan'], default_rot=0)
    zf2 = L.slot(-1.8, -1.0, ['fan'], default_rot=0)
    L.path_crystal(0.5)
    L.items(fan=2, spring=1)
    L.stars(Avoid('Obstacle', key=L.text('obj.l019.clean', "Aucun contact avec un obstacle", "No contact with an obstacle")),
            Attempts(3, key=L.text('obj.l019.tries', "Moins de 3 essais", "Fewer than 3 tries")))
    L.solution(('spring', zs, 0), ('fan', zf1, None))
    L.tip("Une seule orientation évite les pics.", "Only one orientation avoids the spikes.")
    return L


def level_020():
    L = Level(20, "Boss du vent", "Wind boss", 3, time_limit=18)
    L.spawn(-4.0, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    slots = ground_with_recesses(L, -4.75, -0.4, -4.0, [-1.6])
    pit_spikes(L, -0.4, 5.0)
    L.obstacle(1.0, -1.6, 0.5, 5.2)
    L.spikes(1.0, 1.2, 0.5)
    L.platform(4.0, 0.2, 1.8)
    L.wall(5.0, 0.4, 9.0)
    L.block(3.05, 0.7, 0.2, 0.8, surface='Obstacle')
    L.goal(4.1, 1.1, r=0.6)
    zr = L.area(-3.8, -3.0, 1.6, 0.8, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zf1 = L.slot(-0.8, 2.4, ['fan'], default_rot=0)
    zf2 = L.slot(2.6, 5.0, ['fan'], default_rot=0)
    L.path_crystal(0.3)
    L.path_crystal(0.55)
    L.path_crystal(0.8)
    L.items(fan=2, spring=1, ramp=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution(('ramp', zr, None), ('spring', zs, None), ('fan', zf1, None))
    L.tip("Monte, laisse-toi dériver, puis atterris.", "Climb, drift, then land.")
    return L


def levels():
    return [level_011(), level_012(), level_013(), level_014(), level_015(), level_016(), level_017(), level_018(),
            level_019(), level_020()]
