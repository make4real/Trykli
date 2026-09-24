# Créer un niveau

Un niveau est une **donnée**, jamais une scène. Il existe sous deux formes équivalentes :

- `Assets/Resources/LevelDefinitions/level_XXX.json` : format source, versionné, lisible (`LevelDefinition`) ;
- `Assets/ScriptableObjects/Levels/Level_XXX.asset` : asset `LevelData` éditable dans Unity, généré par
  `Tools > TRYKLI > Generate Levels`. Les `WorldData` sont dans `Assets/ScriptableObjects/Worlds`.

`Tools > TRYKLI > Export Levels to JSON` réécrit les JSON depuis les assets, `Generate Levels` fait l'inverse.
Le jeu utilise la `LevelDatabase` générée, ou les JSON si elle n'existe pas.

## Contenu d'un niveau

| Champ | Rôle |
|---|---|
| `levelId`, `worldId`, `levelNumber` | identifiant global (1..N), monde, numéro dans le monde (1..10) |
| `nameKey`, `tipKey` | clés de localisation (`level.XXX.name`, `level.XXX.tip`) |
| `difficulty` (1-10), `isBoss`, `timeLimit` | réglages ; le 10e niveau d'un monde est un boss |
| `layout.spawn`, `layout.goal`, `goalRadius`, `bounds` | départ de Trykli, sortie, limites (sortie des limites = échec) |
| `layout.elements` | géométrie et mécanismes fixes (`ElementData` : type, position, rotation, taille, surface, canal, puissance, rayon, trajet, période, phase, délai...) |
| `layout.zones` | zones de placement : `Point`, `Rect` ou `Rail`, objets autorisés, capacité, rotation autorisée / bornes / par défaut, `linkedChannel` |
| `inventory` | objets fournis (`itemId`, `count`) — un `portal_ab` = une paire A/B |
| `objectives` | exactement 3 objectifs d'étoiles, le premier `Complete` |
| `solution` | solution de référence (objet, zone, position, rotation) — utilisée par la vérification et les indices |
| `hints` | indices optionnels (sinon ils sont déduits du premier pas de la solution) |
| `camera` | cadrage automatique ou taille fixe / suivi |

Repères : l'écran de jeu fait ~10 m de large (x de -5 à 5), le sol est en général à y = -4, le plafond
vers y = 8,5. Trykli a un rayon de 0,35 m.

## Exemple complet : ajouter le niveau 101

Le niveau 101 n'appartient à aucun des 10 mondes existants (10 niveaux par monde) : il ouvre un **monde 11**.

### Méthode A — dans Unity (Level Editor)

1. Créer le monde 11 : dans `Assets/ScriptableObjects/Worlds`, clic droit > `Create > TRYKLI > World Data`,
   nommer `World_11`, régler `worldId = 11`, les clés `world.11.name` / `world.11.theme` /
   `world.11.mechanic` et les couleurs.
2. `Tools > TRYKLI > Level Editor` > **New Level** : l'asset `Level_101` est créé avec `worldId = 11`,
   `levelNumber = 1`, une disposition de départ (sol, spawn, sortie), un ressort et 3 objectifs.
3. Remplir l'inspecteur du Level Editor : difficulté, limite de temps, inventaire, objectifs
   (**Add objective**), indices.
4. **Edit layout in scene** : une scène de travail affiche le niveau ; déplacer spawn / sortie / éléments /
   zones avec les outils Unity, ajouter des éléments (**Add element**) et des zones (**Add point / rect / rail
   zone**), puis **Apply scene to LevelData**.
5. Placer la solution de référence (liste `solution`), puis **Verify solution** : la simulation Unity doit
   atteindre la sortie. **Validate** doit afficher 0 erreur.
6. Ajouter les textes dans `Assets/Resources/Localization/fr.json` et `en.json` :
   `level.101.name`, `level.101.tip`, `world.11.name`, `world.11.theme`, `world.11.mechanic`, et les clés
   d'objectifs personnalisés éventuelles.
7. **Export JSON** (pour versionner `level_101.json`) et `Tools > TRYKLI > Refresh Level Database`.
8. Ajouter le monde 11 à `LevelDefinitions/worlds.json` (même structure que les autres) pour que la
   génération depuis les JSON le recrée.
9. **Play level** pour tester directement, puis vérifier le déblocage : après le niveau 100, le monde 11
   s'ouvre (la progression lit le nombre de niveaux dans la base).

### Méthode B — en Python (pipeline des 100 niveaux)

Créer `Tools/LevelAuthoring/worlds/world11.py` :

```python
from common import *

def level_101():
    L = Level(101, "Le grand saut", "The big leap", 6, time_limit=14)
    L.spawn(-3.4, -1.0)                                   # Trykli tombe sur l'emplacement du ressort
    slots = ground_with_recesses(L, -5, -1.6, -4.0, [-3.4])
    pit_spikes(L, -1.6, 5.0, y=-3.8)
    L.ground(-1.6, 5.0, top=-4.2, h=0.8)
    L.block(4.2, -1.3, 1.6, 0.6, surface='Platform')     # plateforme d'arrivée
    L.goal(4.2, -0.4, r=0.6)
    zs = L.slot(slots[0][0], slots[0][1], ['spring'], default_rot=0, min_rot=-45, max_rot=45)
    L.path_crystal(0.5)                                   # cristal posé sur la trajectoire de référence
    L.items(spring=1)
    L.stars(MaxObjects(1), Crystals(1))
    L.solution(('spring', zs, None))                      # None = rotation cherchée automatiquement
    L.tip("Incline le ressort vers la plateforme.", "Tilt the spring towards the platform.")
    return L

def levels():
    return [level_101()]
```

Puis :

```bash
python3 Tools/LevelAuthoring/build_levels.py            # tous les mondes : JSON + fr/en + REPORT.md
python3 Tools/UnityProject/unity_assets.py              # .meta des nouveaux fichiers
```

Ajouter le monde 11 dans `Tools/LevelAuthoring/worlds_meta.py` (nom, couleurs, mécanique) avant le build.
Le build cherche la solution dans le simulateur, place les cristaux « sur la trajectoire », signale les objets
inutiles et écrit le rapport. Dans Unity : `Generate Levels` puis `Verify Level Solutions`
(le simulateur Python est approché : la vérification Unity fait foi).

Outils de mise au point : `render.py` (rendu ASCII d'une trajectoire), `design_search.py` (exploration de
toutes les combinaisons de placement).

## Règles de conception

- Chaque niveau doit être résoluble avec l'inventaire, la solution de référence doit passer `Verify`.
- Solution robuste : une zone point ou une rotation par pas de 15° doit suffire ; éviter les réglages au
  pixel. Tester aussi la solution à vitesse x2 (même pas de physique, résultat identique).
- Étoiles : 1 = terminer, 2 et 3 = objectifs d'optimisation / cristaux (3 cristaux maximum).
- Un nouveau mécanisme = un `ElementType`, une classe `MechanismBase`, un cas dans `ElementFactory`, un
  visuel (placeholder) et, si besoin, un type d'objet d'inventaire dans `BuiltInItems`.
