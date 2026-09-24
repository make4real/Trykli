"""World 1 - Premiers rebonds (gravity, ramps, springs). Difficulty 1-2."""
from common import *


def level_001():
    L = Level(1, "Tutoriel du ressort", "Spring tutorial", 1, time_limit=10)
    L.tutorial = TUTO_DRAG
    L.spawn(-3.5, 7.0)
    L.ground(-5, -1.5, top=-4.0)
    L.ground(1.5, 5, top=-4.0)
    pit_spikes(L, -1.5, 1.5)
    L.platform(3.3, -0.2, 2.4)
    L.goal(3.3, 1.2, r=0.8)
    z = L.slot(-3.5, -3.75, ['spring'], default_rot=-30, allow_rot=False)
    L.items(spring=1)
    L.stars(MaxObjects(1), Time(5))
    L.solution(('spring', z, -30))
    L.tip("Place le ressort sous Trykli, puis appuie sur GO.", "Put the spring under Trykli, then press GO.")
    return L


def level_002():
    L = Level(2, "Choisir la position", "Pick the spot", 1, time_limit=12)
    L.tutorial = TUTO_RESTART
    L.bounds = (-6.5, -5.0, 13.0, 14.0)
    L.spawn(-5.6, 1.5)
    slope(L, -6.5, -0.6, -4.3, -4.05)
    slots = ground_with_recesses(L, -6.5, -0.8, -4.0, [-3.8, -2.6, -1.4])
    pit_spikes(L, -0.8, 2.4)
    L.ground(2.4, 3.6, top=-4.0)
    pit_spikes(L, 3.6, 6.5)
    L.goal(3.0, -3.2, r=0.7)
    zs = [L.slot(x, y, ['spring'], default_rot=-15, allow_rot=False) for x, y in slots]
    L.crystal(0.1, 2.2)
    L.items(spring=1)
    L.stars(Attempts(1, key=L.text('obj.l002.first', "Placer le ressort du premier coup", "Place the spring right the first time")),
            Crystals(1))
    L.solution(('spring', zs[1], -15))
    L.tip("Trois emplacements, une seule bonne portée.", "Three spots, only one has the right range.")
    return L


def level_003():
    L = Level(3, "Premier angle", "First angle", 1, time_limit=12)
    L.tutorial = TUTO_ROTATE
    L.spawn(-3.0, 7.5)
    L.ground(-5, 1.5, top=-4.0)
    pit_spikes(L, 1.5, 5.0)
    L.platform(3.4, -0.4, 3.2)
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.8, 0.55, r=0.7)
    L.crystal(0.1, 2.6)
    z = L.slot(-3.0, -3.75, ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.items(spring=1)
    L.stars(Attempts(2, key=L.text('obj.l003.angle', "Angle correct en 2 essais maximum", "Right angle within 2 tries")), Crystals(1))
    L.solution(('spring', z, -15))
    L.tip("Tourne le ressort pour viser la plateforme.", "Rotate the spring to aim at the platform.")
    return L


def level_004():
    L = Level(4, "Rampe et ressort", "Ramp and spring", 1, time_limit=12)
    L.spawn(-3.6, 7.8)
    slots = ground_with_recesses(L, -5, 1.4, -4.0, [0.2])
    pit_spikes(L, 1.4, 5.0)
    L.platform(3.6, 0.0, 2.8)
    L.wall(5.0, -4.0, 9.0)
    L.goal(3.9, 1.0, r=0.7)
    zr = L.area(-3.4, -2.6, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal('apex', 0.1)
    L.items(ramp=1, spring=1)
    L.stars(UseAll(2, key=L.text('obj.l004.both', "Utiliser les 2 objets", "Use both objects")),
            Crystals(1, key=L.text('obj.l004.high', "Récupérer le cristal haut", "Collect the high crystal")))
    L.solution(('ramp', zr, -30, (-3.4, -2.6)), ('spring', zs, -15))
    L.tip("Trykli doit rouler jusqu'au ressort.", "Trykli has to roll to the spring.")
    return L


def level_005():
    L = Level(5, "Double rebond", "Double bounce", 2, time_limit=12)
    L.spawn(-3.8, 7.5)
    low = ground_with_recesses(L, -5, 0.0, -4.0, [-3.8, -1.9])
    ledge = ground_with_recesses(L, 0.0, 3.2, 0.0, [1.3, 2.5], h=0.8)
    L.ground(3.2, 5.0, top=-4.0)
    L.platform(4.2, 4.6, 1.6)
    L.wall(5.0, -4.0, 9.0)
    L.goal(4.2, 5.7, r=0.7)
    z = [L.slot(x, y, ['spring'], default_rot=0, min_rot=-45, max_rot=45) for x, y in low + ledge]
    L.items(spring=2)
    L.stars(Bounces(2, EXACTLY), Time(7))
    L.solution(('spring', z[0], -15), ('spring', z[2], -15))
    L.tip("Le premier ressort doit t'envoyer sur le second.", "The first spring must send you onto the second one.")
    return L


def level_006():
    L = Level(6, "Premier danger", "First danger", 2, time_limit=12)
    L.spawn(-3.6, 7.8)
    L.wall(-5.0, -4.0, 9.0)
    L.wall(5.0, -4.0, 9.0)
    slots = ground_with_recesses(L, -4.75, -1.0, -4.0, [-1.8])
    pit_spikes(L, -1.0, 3.0)
    L.ground(3.0, 4.75, top=-4.0)
    L.goal(3.9, -3.2, r=0.7)
    zr = L.area(-3.4, -2.6, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal('apex')
    L.items(ramp=1, spring=1)
    L.stars(Avoid('Wall', key=L.text('obj.l006.walls', "Ne toucher aucun mur latéral", "Never touch a side wall")), Crystals(1))
    L.solution(('ramp', zr, None), ('spring', zs, None))
    L.tip("Vise juste au-dessus des pics.", "Aim just above the spikes.")
    return L


def level_007():
    L = Level(7, "Le grand trou", "The big gap", 2, time_limit=12)
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -2.2, -4.0, [-2.9])
    pit_spikes(L, -2.2, 5.0)
    L.platform(4.3, -0.6, 1.6)
    L.goal(4.3, 0.4, r=0.7)
    zr = L.area(-3.8, -2.6, 1.6, 1.0, ['ramp', 'spring'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    za = L.slot(0.9, -1.3, ['spring', 'ramp'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.45)
    L.path_crystal(0.7)
    L.items(spring=2, ramp=1)
    L.stars(MaxObjects(2), Crystals(2))
    L.solution(('ramp', zr, None), ('spring', zs, -30))
    L.tip("Un angle plus ouvert porte plus loin.", "A wider angle carries further.")
    return L


def level_008():
    L = Level(8, "Le détour", "The detour", 2, time_limit=14)
    L.spawn(1.2, 7.8)
    L.ground(-5.0, 5.0, top=-4.0)
    L.obstacle(1.0, 3.2, 3.6, 0.5)
    L.platform(-3.6, 1.0, 2.8)
    L.wall(-5.0, 1.2, 4.0)
    L.marker(-3.6, 2.0, 2.8, 1.6, 'left')
    L.platform(4.1, 3.2, 1.8)
    L.goal(4.1, 4.2, r=0.7)
    zr = L.area(1.0, 3.9, 3.0, 0.6, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(-3.8, 1.1, ['spring'], default_rot=0, min_rot=-60, max_rot=60)
    zs2 = L.slot(1.2, -3.75, ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zr2 = L.area(-2.8, 1.6, 1.0, 0.4, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.35)
    L.path_crystal('apex')
    L.items(ramp=2, spring=1)
    L.stars(Interaction('zone:left', key=L.text('obj.l008.left', "Passer par la plateforme gauche", "Go through the left platform")),
            Crystals(2))
    L.solution(('ramp', zr, None), ('spring', zs, None))
    L.tip("La sortie se prend en faisant le tour par la gauche.", "Reach the exit by going around on the left.")
    return L


def level_009():
    L = Level(9, "Deux solutions", "Two solutions", 2, time_limit=14)
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, 5, -4.0, [-3.8, -1.6])
    L.spikes(1.4, -3.8, 2.0)
    L.spikes(1.2, 2.6, 1.2, rot=180)
    L.obstacle(1.2, 3.1, 1.2, 0.5)
    L.platform(-0.4, 1.4, 1.0)
    L.platform(4.2, -0.4, 1.6)
    L.wall(5.0, -0.2, 9.0)
    L.marker(3.0, 6.5, 3.6, 3.4, 'high')
    L.goal(4.2, 0.6, r=0.7)
    z = [L.slot(x, y, ['spring'], default_rot=0, min_rot=-45, max_rot=45) for x, y in slots]
    zh = L.slot(-0.4, 1.5, ['spring', 'ramp'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal('apex')
    L.items(spring=2, ramp=1)
    L.stars(MaxObjects(2),
            Interaction('zone:high', key=L.text('obj.l009.high', "Prendre le chemin haut", "Take the high road")))
    L.solution(('spring', z[0], None), ('spring', zh, None))
    L.tip("Le chemin du haut est plus long mais plus sûr.", "The high road is longer but safer.")
    return L


def level_010():
    L = Level(10, "Boss rebonds", "Bounce boss", 2, time_limit=18)
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -0.6, -4.0, [-1.4])
    pit_spikes(L, -0.6, 5.0)
    L.obstacle(0.4, -1.6, 0.6, 3.6)
    L.platform(2.6, 0.6, 1.6)
    L.platform(4.2, 3.6, 1.6)
    L.wall(5.0, 3.8, 9.0)
    L.goal(4.3, 4.6, r=0.7)
    zr = L.area(-3.9, -3.0, 1.6, 0.8, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zr2 = L.area(2.2, 6.0, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zs2 = L.slot(2.6, 0.7, ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.35)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(spring=2, ramp=2)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution(('ramp', zr, None), ('spring', zs, None), ('spring', zs2, None))
    L.tip("Franchis l'obstacle central en deux sauts.", "Clear the central obstacle in two jumps.")
    return L


def levels():
    return [level_001(), level_002(), level_003(), level_004(), level_005(), level_006(), level_007(), level_008(),
            level_009(), level_010()]
