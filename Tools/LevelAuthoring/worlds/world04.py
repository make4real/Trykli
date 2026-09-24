"""World 4 - Magnétisme (magnets, first buttons). Difficulty 4-5."""
from common import *

M = 'magnet'


def ledge_start(L, y=0.4, end=-1.5):
    """Spawn above a slope that makes Trykli roll right along a ledge ending at x=end."""
    L.spawn(-4.4, y + 2.6)
    slope(L, -5.0, y + 1.4, -3.4, y + 0.02)
    L.block((-3.5 + end) / 2.0, y - 0.3, end + 3.5, 0.6, surface='Platform')


def level_031():
    L = Level(31, "Premier aimant", "First magnet", 4, time_limit=12)
    ledge_start(L, 0.4, -1.5)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.ground(1.3, 5.0, top=0.4, h=0.6)
    L.wall(5.0, 0.4, 9.0)
    L.goal(1.2, 0.9, r=0.7)
    zm = L.slot(2.1, 1.3, [M])
    zm2 = L.slot(-0.2, 4.2, [M])
    zm3 = L.slot(-0.2, -2.4, [M])
    L.path_crystal(0.9)
    L.items(magnet=1)
    L.stars(UseItem(M, 1, AT_MOST, key=L.text('obj.l031.one', "Un seul aimant", "A single magnet")),
            Crystals(1, key=L.text('obj.l031.near', "Récupérer le cristal proche de l'aimant", "Collect the crystal near the magnet")))
    L.solution((M, zm, 0))
    L.tip("L'aimant attire Trykli dans son rayon.", "The magnet pulls Trykli within its radius.")
    return L


def level_032():
    L = Level(32, "Courbe magnétique", "Magnetic curve", 4, time_limit=12)
    L.goal_on_path = True
    L.spawn(-3.8, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.obstacle(0.0, -0.5, 0.5, 6.0)
    L.platform(3.8, -1.4, 2.0)
    zr = L.area(-3.6, 2.6, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(1.0, 3.8, [M])
    zm2 = L.slot(-1.6, -1.4, [M])
    zm3 = L.slot(2.6, 6.0, [M])
    L.path_crystal(0.55)
    L.items(magnet=1, ramp=1)
    L.stars(MaxObjects(2), Crystals(1, key=L.text('obj.l032.outer', "Récupérer le cristal extérieur", "Collect the outer crystal")))
    L.solution(('ramp', zr, -15, (-3.1, 3.1)), (M, zm, 0))
    L.tip("L'aimant courbe la trajectoire au-dessus du mur.", "The magnet bends the path over the wall.")
    return L


def level_033():
    L = Level(33, "Au-dessus du vide", "Above the void", 4, time_limit=12)
    ledge_start(L, 1.0, -2.2)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.block(0.0, -4.4, 10.0, 0.6, surface='Ground')
    L.ground(2.4, 5.0, top=1.0, h=0.6)
    L.wall(5.0, 1.0, 9.0)
    L.goal(3.6, 1.7, r=0.7)
    zm = L.slot(0.2, 3.4, [M])
    zm2 = L.slot(0.2, -1.6, [M])
    zm3 = L.slot(4.4, 3.6, [M])
    L.path_crystal(0.5)
    L.items(magnet=1)
    L.stars(Avoid('Ground', key=L.text('obj.l033.bottom', "Aucun contact avec le fond", "Never touch the bottom")),
            Crystals(1, key='obj.l016.center'))
    L.solution((M, zm, 0))
    L.tip("Un aimant au-dessus du vide ralentit la chute.", "A magnet above the void slows the fall.")
    return L


def level_034():
    L = Level(34, "Rebond attiré", "Attracted bounce", 4, time_limit=12)
    L.goal_on_path = True
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -1.4, -4.0, [-3.8, -2.4])
    pit_spikes(L, -1.4, 5.0)
    L.platform(1.6, 1.8, 1.2)
    zs = [L.slot(x, y, ['spring'], default_rot=0, min_rot=-15, max_rot=15) for x, y in slots]
    zm = L.slot(0.9, 3.4, [M])
    zm2 = L.slot(2.4, 0.0, [M])
    zm3 = L.slot(-3.8, 5.8, [M])
    L.path_crystal(0.45)
    L.path_crystal(0.75)
    L.items(magnet=1, spring=1)
    L.stars(Bounces(1, EXACTLY, key='obj.l022.one'), Crystals(2))
    L.solution(('spring', zs[0], -15), (M, zm, 0))
    L.tip("Le ressort propulse, l'aimant courbe la trajectoire.", "The spring launches, the magnet bends the path.")
    return L


def level_035():
    L = Level(35, "Double aimant", "Double magnet", 5, time_limit=14)
    L.goal_on_path = True
    L.tune_zones = {0: (1.0, 1.0), 1: (1.0, 1.0)}
    ledge_start(L, 2.6, -2.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.obstacle(0.6, 3.0, 0.5, 5.0)
    L.obstacle(0.6, -2.6, 0.5, 2.4)
    zm1 = L.slot(-1.2, 1.4, [M])
    zm2 = L.slot(2.4, -0.6, [M])
    zm3 = L.slot(-4.2, -1.2, [M])
    zm4 = L.slot(3.2, 6.2, [M])
    L.path_crystal(0.4)
    L.path_crystal(0.7)
    L.items(magnet=2)
    L.stars(UseItem(M, 2, AT_LEAST, key=L.text('obj.l035.both', "Utiliser les deux aimants", "Use both magnets")), Crystals(2))
    L.solution((M, zm1, 0), (M, zm2, 0))
    L.tip("Deux aimants dessinent un S.", "Two magnets draw an S.")
    return L


def level_036():
    L = Level(36, "Aimant activable", "Switchable magnet", 5, time_limit=14)
    L.goal_on_path = True
    L.spawn(-4.3, 5.0)
    slope(L, -5.0, 3.6, -3.2, 2.02)
    L.block(-2.05, 1.7, 2.5, 0.6, surface='Platform')
    L.button(-1.8, 2.1, emit=0, bid='b1')
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.ground(2.4, 5.0, top=-1.2, h=0.6)
    L.wall(5.0, -1.2, 9.0)
    zm = L.slot(1.8, 1.6, [M], linked=0)
    zm2 = L.slot(0.8, 5.4, [M], linked=0)
    L.path_crystal(0.8)
    L.items(magnet=1)
    L.stars(Interaction('button', 1, key=L.text('obj.l036.button', "Activer le bouton", "Press the button")),
            Crystals(1, key=L.text('obj.l036.after', "Récupérer le cristal après l'activation", "Collect the crystal after the activation")))
    L.solution((M, zm, 0))
    L.tip("Le bouton allume l'aimant posé dans la zone rouge.", "The button switches on the magnet placed in the red zone.")
    return L


def level_037():
    L = Level(37, "Aimant et porte", "Magnet and door", 5, time_limit=14)
    L.tune_zones = {1: (0.8, 0.8)}
    L.spawn(-3.8, 7.8)
    L.ground(-5, -0.8, top=-4.0)
    slope(L, -1.0, -4.02, 0.4, -2.6)
    L.ground(0.3, 5.0, top=-2.6, h=2.0)
    L.button(0.8, -2.5, emit=0, bid='b1')
    L.door(1.6, -1.3, channel=0, h=2.6)
    L.block(1.6, 3.9, 0.5, 7.8, surface='Wall')
    L.goal(2.6, -1.9, r=0.6)
    zr = L.area(-3.6, -2.6, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(3.6, -1.8, [M])
    zm2 = L.slot(-3.4, 2.4, [M])
    L.path_crystal(0.8)
    L.items(magnet=1, ramp=1)
    L.stars(Interaction('door_open', 1, key=L.text('obj.l037.door', "Ouvrir la porte", "Open the door")), MaxObjects(2))
    L.solution(('ramp', zr, -45, (-3.6, -2.6)), (M, zm, 0))
    L.tip("L'aimant aide Trykli à monter jusqu'au bouton.", "The magnet helps Trykli climb up to the button.")
    return L


def level_038():
    L = Level(38, "Orbite de cristaux", "Crystal orbit", 5, time_limit=14)
    L.goal_on_path = True
    L.tune_zones = {1: (1.5, 1.5)}
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -1.8, -4.0, [-3.8])
    pit_spikes(L, -1.8, 5.0)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zm = L.slot(1.8, 0.2, [M])
    zm2 = L.slot(3.4, 4.6, [M])
    L.path_crystal(0.35)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(magnet=1, spring=1)
    L.stars(Crystals(2), AllCrystals())
    L.solution(('spring', zs, -30), (M, zm, 0))
    L.tip("Tourne autour de l'aimant.", "Swing around the magnet.")
    return L


def level_039():
    L = Level(39, "Courbe précise", "Precise curve", 5, time_limit=14)
    L.goal_on_path = True
    ledge_start(L, 3.4, -2.4)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.block(0.4, 5.6, 0.5, 2.8, surface='Obstacle')
    L.block(0.4, -0.2, 0.5, 3.6, surface='Obstacle')
    L.platform(3.8, -1.0, 2.0)
    L.wall(5.0, -0.8, 9.0)
    zm1 = L.slot(2.0, 2.4, [M])
    zm2 = L.slot(-1.2, -1.4, [M])
    zm3 = L.slot(3.4, 5.2, [M])
    zf = L.slot(-1.8, 6.4, ['fan'], default_rot=0)
    L.path_crystal(0.6)
    L.items(magnet=2, fan=1)
    L.stars(Avoid('Obstacle', key=L.text('obj.l039.clean', "Aucun choc", "No collision")), MaxObjects(2))
    L.solution((M, zm1, 0))
    L.tip("Passe par la fenêtre du mur, sans le toucher.", "Go through the window in the wall, without touching it.")
    return L


def level_040():
    L = Level(40, "Boss magnétique", "Magnet boss", 5, time_limit=18)
    L.goal_on_path = True
    L.tune_zones = {1: (1.2, 1.2), 2: (1.2, 1.2)}
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -2.0, -4.0, [-3.8])
    pit_spikes(L, -2.0, 5.0)
    L.obstacle(-0.6, 4.6, 0.5, 4.4)
    L.obstacle(1.8, -0.6, 0.5, 4.0)
    L.wall(5.0, -4.0, 9.0)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zm1 = L.slot(0.4, 1.6, [M])
    zm2 = L.slot(3.6, 4.4, [M])
    zm3 = L.slot(-2.4, 2.4, [M])
    zf = L.slot(-1.8, -1.0, ['fan'], default_rot=0)
    zf2 = L.slot(2.8, 7.4, ['fan'], default_rot=0)
    L.path_crystal(0.3)
    L.path_crystal(0.55)
    L.path_crystal(0.8)
    L.items(magnet=2, spring=1, fan=1)
    L.stars(MaxObjects(3), AllCrystals())
    L.solution(('spring', zs, -15), (M, zm1, 0), (M, zm2, 0))
    L.tip("Trois champs magnétiques, une sortie très haute.", "Three magnetic fields, a very high exit.")
    return L


def levels():
    return [level_031(), level_032(), level_033(), level_034(), level_035(), level_036(), level_037(), level_038(),
            level_039(), level_040()]
