"""World 5 - Réactions en chaîne (bombs, bumpers). Difficulty 5-6."""
from common import *

B = 'bomb'
U = 'bumper'
P = 'portal_ab'


def roll_start(L, y=-4.0):
    """Spawn above a slope: Trykli arrives rolling to the right on the floor at height y."""
    L.spawn(-4.5, y + 3.2)
    slope(L, -5.0, y + 2.0, -3.3, y - 0.02)


def level_041():
    L = Level(41, "Premier bumper", "First bumper", 5, time_limit=12)
    L.goal_on_path = True
    L.spawn(-3.2, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.platform(3.4, 1.4, 2.8)
    L.wall(5.0, 1.6, 9.0)
    z = [L.slot(x, -0.6, [U]) for x in (-3.6, -2.8, -1.8)]
    L.path_crystal('apex')
    L.items(bumper=1)
    L.stars(UseItem(U, 1, AT_MOST, key=L.text('obj.l041.one', "Un seul bumper", "A single bumper")), Crystals(1, key='obj.l013.top'))
    L.solution((U, z[0], 0))
    L.tip("Le bumper renvoie Trykli à l'opposé du point de contact.", "The bumper sends Trykli away from the contact point.")
    return L


def level_042():
    L = Level(42, "Ping-pong", "Ping-pong", 5, time_limit=12)
    L.goal_on_path = True
    L.tune_zones = {0: (0.6, 0.6), 2: (0.8, 0.8)}
    L.spawn(-3.4, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.wall(-5.0, -3.4, 9.0)
    L.wall(5.0, -3.4, 9.0)
    za = L.slot(-3.8, 0.4, [U])
    zb = L.slot(-1.4, 3.4, [U])
    zc = L.slot(1.2, 0.2, [U])
    zd = L.slot(3.0, 3.6, [U])
    L.path_crystal(0.45)
    L.path_crystal(0.75)
    L.items(bumper=2)
    L.stars(Bounces(2, EXACTLY, source='Bumper'), Crystals(2))
    L.solution((U, za, 0), (U, zc, 0))
    L.tip("Fais rebondir Trykli d'un bumper à l'autre.", "Bounce Trykli from one bumper to the other.")
    return L


def level_043():
    L = Level(43, "Première bombe", "First bomb", 5, time_limit=12)
    L.goal_on_path = True
    L.goal_region = (1.6, -4.0, 5.0, 9.0)
    L.tune_zones = {0: (0.3, 0.25)}
    roll_start(L)
    ground_with_recesses(L, -3.4, 1.0, -4.0, [-1.2], recess_width=0.9, depth=0.8)
    L.block(1.25, -2.4, 0.5, 4.2, surface='Wall')
    L.ground(1.5, 5.0, top=-4.0)
    L.wall(5.0, -4.0, 9.0)
    zb = L.slot(-1.2, -4.4, [B])
    zb2 = L.slot(0.4, -3.65, [B])
    L.path_crystal('apex')
    L.items(bomb=1)
    L.stars(Interaction('explosion', 1, EXACTLY, key=L.text('obj.l043.one', "Une seule explosion", "A single explosion")),
            Crystals(1, key=L.text('obj.l043.high', "Récupérer le cristal en hauteur", "Collect the high crystal")))
    L.solution((B, zb, 0))
    L.tip("L'explosion pousse Trykli à l'opposé de la bombe.", "The blast pushes Trykli away from the bomb.")
    return L


def level_044():
    L = Level(44, "Explosion orientée", "Aimed blast", 5, time_limit=12)
    L.goal_on_path = True
    L.goal_region = (0.8, -4.0, 5.0, 9.0)
    L.tune_zones = {1: (0.4, 0.4)}
    L.spawn(-3.6, 7.8)
    L.ground(-5, 0.0, top=-4.0)
    L.block(0.25, -1.4, 0.5, 5.2, surface='Wall')
    pit_spikes(L, 0.5, 5.0)
    L.platform(3.6, -0.6, 2.2)
    zr = L.area(-3.4, -2.6, 2.0, 1.0, ['ramp'], capacity=1, default_rot=0, min_rot=-45, max_rot=45)
    zb = L.area(-1.2, -3.5, 1.6, 0.6, [B], capacity=1)
    L.path_crystal(0.7)
    L.items(bomb=1, ramp=1)
    L.stars(Bounces(0, EXACTLY, key=L.text('obj.l044.clean', "Pas de rebond inutile", "No useless bounce")), Crystals(1))
    L.solution(('ramp', zr, -30, (-3.4, -2.6)), (B, zb, 0, (-0.9, -3.6)))
    L.tip("La position de la bombe choisit la direction.", "The bomb position chooses the direction.")
    return L


def level_045():
    L = Level(45, "Bombe et ressort", "Bomb and spring", 5, time_limit=12)
    L.goal_on_path = True
    L.goal_region = (1.6, -4.0, 5.0, 9.0)
    L.tune_zones = {1: (0.3, 0.2)}
    L.spawn(-3.8, 7.8)
    slots = ground_with_recesses(L, -5, -2.4, -4.0, [-3.8])
    ground_with_recesses(L, -2.4, 1.0, -4.0, [0.0], recess_width=0.9, depth=0.8)
    L.block(1.25, -1.8, 0.5, 5.4, surface='Wall')
    L.ground(1.5, 5.0, top=-4.0)
    L.wall(5.0, -4.0, 9.0)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    zb = L.slot(0.0, -4.4, [B])
    zb2 = L.slot(-3.6, 3.2, [B])
    L.path_crystal(0.6)
    L.items(bomb=1, spring=1)
    L.stars(UseAll(2, key='obj.l004.both'), Time(10))
    L.solution(('spring', zs, None), (B, zb, 0))
    L.tip("Le ressort envoie Trykli dans la zone d'explosion.", "The spring sends Trykli into the blast zone.")
    return L


def level_046():
    L = Level(46, "Bumper portal", "Bumper portal", 5, time_limit=12)
    L.goal_on_path = True
    L.tune_zones = {0: (0.5, 0.5)}
    L.spawn(-3.4, 7.8)
    pit_spikes(L, -5.0, 5.0, y=-3.8)
    L.ground(-5, 5, top=-4.2, h=0.8)
    L.obstacle(0.6, 2.4, 0.5, 13.0)
    zu = L.slot(-3.7, -0.4, [U])
    za = L.slot(-1.2, 1.6, [P])
    za2 = L.slot(-3.0, 4.6, [P])
    zb = L.slot(2.8, 4.2, [P])
    zb2 = L.slot(2.8, -2.0, [P])
    L.path_crystal(0.8)
    L.items(bumper=1, portal_ab=1)
    L.stars(MaxObjects(2, key=L.text('obj.l046.combo', "1 bumper + 1 portail", "1 bumper + 1 portal")),
            Crystals(1, key=L.text('obj.l046.after', "Récupérer le cristal après la téléportation", "Collect the crystal after teleporting")))
    L.solution((U, zu, 0), (P, za, 0), (P, zb, -45))
    L.tip("Le bumper envoie Trykli vers un portail inaccessible.", "The bumper sends Trykli to an unreachable portal.")
    return L


def level_047():
    L = Level(47, "Souffle explosif", "Explosive gust", 6, time_limit=12)
    L.goal_on_path = True
    L.goal_region = (1.0, -3.0, 5.0, 9.0)
    L.tune_zones = {1: (0.6, 0.6)}
    roll_start(L)
    ground_with_recesses(L, -3.4, 0.4, -4.0, [-1.4], recess_width=0.9, depth=0.8)
    pit_spikes(L, 0.4, 5.0)
    L.wall(5.0, -4.0, 9.0)
    zb = L.slot(-1.4, -4.4, [B])
    zf = L.slot(-2.4, 0.8, ['fan'], default_rot=0)
    zf2 = L.slot(2.6, -1.8, ['fan'], default_rot=0)
    L.path_crystal(0.5)
    L.path_crystal(0.8)
    L.items(bomb=1, fan=1)
    L.stars(MaxObjects(2), Crystals(2))
    L.solution((B, zb, 0), ('fan', zf, None))
    L.tip("Le ventilateur corrige la trajectoire après l'explosion.", "The fan corrects the path after the blast.")
    return L


def level_048():
    L = Level(48, "Déclencheur", "Trigger", 6, time_limit=14)
    roll_start(L)
    ground_with_recesses(L, -3.4, 5.0, -4.0, [-1.6], recess_width=0.9, depth=0.8)
    L.block(0.4, -2.6, 0.5, 2.8, surface='Wall')
    L.button(2.3, -3.95, emit=0, bid='b1')
    L.door(3.4, -2.8, channel=0, h=2.4)
    L.block(3.4, 2.4, 0.5, 7.6, surface='Wall')
    L.goal(4.4, -3.3, r=0.6)
    zb = L.slot(-1.6, -4.4, [B])
    zu = L.slot(-2.4, 2.6, [U])
    zu2 = L.slot(1.8, 1.6, [U])
    L.path_crystal(0.4)
    L.items(bomb=1, bumper=1)
    L.stars(Interaction('button', 1, key='obj.l036.button'), MaxObjects(1, key=L.text('obj.l048.one', "Un seul objet", "A single object")))
    L.solution((B, zb, 0))
    L.tip("La réaction doit finir sur le bouton.", "The chain reaction has to end on the button.")
    return L


def level_049():
    L = Level(49, "Trois chemins", "Three paths", 6, time_limit=14)
    L.goal_on_path = True
    L.goal_region = (2.9, -3.2, 5.0, 9.0)
    L.tune_zones = {1: (0.6, 0.6), 2: (0.6, 0.4)}
    roll_start(L)
    ground_with_recesses(L, -3.4, 0.6, -4.0, [-1.4], recess_width=0.9, depth=0.8)
    pit_spikes(L, 0.6, 2.8)
    L.ground(2.8, 5.0, top=-3.2, h=1.6)
    L.wall(5.0, -3.2, 9.0)
    zb = L.slot(-1.4, -4.4, [B])
    zu = L.slot(0.2, 1.6, [U])
    zr = L.area(1.6, -2.2, 1.6, 0.8, ['ramp'], capacity=1, default_rot=-20, min_rot=-45, max_rot=45)
    L.path_crystal(0.3)
    L.path_crystal(0.6)
    L.path_crystal(0.85)
    L.items(bomb=1, bumper=1, ramp=1)
    L.stars(MaxObjects(2), AllCrystals())
    L.solution((B, zb, 0), ('ramp', zr, None))
    L.tip("Trois routes : une seule prend tous les cristaux.", "Three routes: only one takes every crystal.")
    return L


def level_050():
    L = Level(50, "Boss réaction", "Reaction boss", 6, time_limit=18)
    L.goal_on_path = True
    L.goal_region = (2.0, -3.6, 5.0, 9.0)
    L.tune_zones = {1: (0.6, 0.6), 3: (0.6, 0.6)}
    roll_start(L)
    ground_with_recesses(L, -3.4, 1.4, -4.0, [-1.6], recess_width=0.9, depth=0.8)
    L.obstacle(1.6, 3.0, 0.5, 14.0)
    pit_spikes(L, 1.85, 5.0)
    L.ground(3.4, 5.0, top=-3.4, h=1.2)
    L.wall(5.0, -3.4, 9.0)
    zb = L.slot(-1.6, -4.4, [B])
    zu1 = L.slot(0.3, -0.2, [U])
    zu2 = L.slot(-3.8, 3.8, [U])
    za = L.slot(-2.4, -1.6, [P])
    zb2 = L.slot(3.0, 4.2, [P])
    L.path_crystal(0.3)
    L.path_crystal(0.55)
    L.path_crystal(0.85)
    L.items(bomb=1, bumper=2, portal_ab=1)
    L.stars(MaxObjects(4), AllCrystals())
    L.solution((B, zb, 0), (U, zu1, 0), (P, za, 0), (P, zb2, None))
    L.tip("Explosion, bumper, portail : une grande réaction en chaîne.", "Blast, bumper, portal: one big chain reaction.")
    return L


def levels():
    return [level_041(), level_042(), level_043(), level_044(), level_045(), level_046(), level_047(), level_048(),
            level_049(), level_050()]
