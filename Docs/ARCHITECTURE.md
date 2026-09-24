# Architecture de TRYKLI

## Principes

1. **Piloté par les données** : un niveau est une donnée (`LevelData` / JSON), jamais une scène. La scène
   `Gameplay` construit le niveau au chargement (`LevelBuilder`) et le reconstruit à chaque essai.
2. **Déterministe** : la physique 2D est en mode `Script` (`Physics2D.simulationMode`). `SimulationRunner`
   avance par pas fixes de 0,02 s : d'abord les mécanismes (`ISimulationElement.Step`), puis
   `PhysicsScene2D.Simulate`. La vitesse x2 exécute plus de pas par frame, sans changer le pas. Les visuels sont
   interpolés entre deux pas.
3. **Détection mathématique** : ressorts, bumpers, portails, ventilateurs, aimants, lasers, switchs... testent
   Trykli par géométrie balayée (`ElementGeometry`) au lieu de triggers, pour ne rater aucun contact rapide.
4. **Aucun asset externe obligatoire** : sprites, sons et polices de secours sont générés (`PlaceholderArt`,
   `IconPainter`, `ProceduralAudio`). Des prefabs peuvent remplacer n'importe quel élément via
   `ElementPrefabCatalog`.
5. **Code testable** : logique pure (sauvegarde, progression, objectifs, inventaire, règles de placement,
   validation) sans dépendance aux objets de scène.

## Assemblies

| Assembly | Dossier | Dépendances |
|---|---|---|
| `Trykli.Runtime` | `Assets/Scripts` | UnityEngine.UI, Unity.TextMeshPro |
| `Trykli.Editor` | `Assets/Scripts/Editor` | Trykli.Runtime (éditeur uniquement) |
| `Trykli.Tests.EditMode` / `Trykli.Tests.PlayMode` | `Assets/Tests` | Trykli.Runtime, NUnit, Test Runner |

## Flux de l'application

```
Boot (BootController)
  └─ GameManager.EnsureExists()  (DontDestroyOnLoad)
       ├─ SaveManager  → FileSaveStorage (persistentDataPath/trykli_save.json + .bak, écriture atomique)
       ├─ LevelDatabase.Load()  (asset généré, sinon JSON de Resources/LevelDefinitions)
       ├─ ProgressionService, Loc (fr/en), AudioManager, Haptics
       └─ SceneFlow → MainMenu → WorldSelect → LevelSelect → Gameplay
```

`GameStateMachine` : `Boot → Menu → Placement ⇄ Simulation → Victory | Failure`, avec `Pause` empilable.
Les événements globaux passent par `GameEvents` (changement d'état, victoire, échec, langue...).

## Gameplay (scène Gameplay)

- `GameplayController` : crée le HUD et le `LevelManager` pour le niveau choisi (`SceneFlow`).
- `LevelManager` + `LevelSession` : cycle d'un niveau.
  - **Chargement** : `LevelBuilder.Build` crée blocs, dangers, cristaux, mécanismes, zones et la sortie ;
    `TrykliController.Create` place Trykli au spawn.
  - **GO** : placement figé → `Simulation`. `RunTracker` remplit `LevelRunStats` (objets, rebonds, contacts,
    dangers, compteurs, cristaux, immobilité).
  - **Victoire** : `GoalZone` → `StarEvaluator` → `ProgressionService.RecordResult` → sauvegarde → panneau.
  - **Échec** : pics / laser / explosion létale, sortie des limites (+1,5 m), blocage (< 0,15 m/s pendant 3 s),
    limite de temps.
  - **Restart** : reconstruit le niveau en gardant le même placement. **Reset** : rend tous les objets.
  - **Niveau suivant** : `LevelDatabase.GetNextLevelId`.
- `TrykliController` : `Rigidbody2D` + états `Idle, Falling, Rolling, Bouncing, Flying, Teleporting, Stuck,
  Dead, Victory` ; gère la gravité inversée (`GravityService`).
- Placement (`Gameplay/Placement`) : `PointerInput` (tactile + souris), `PlacementManager` (drag & drop,
  sélection, déplacement, rotation, suppression, double-tap), `PlacementRules` (aimantation aux zones point /
  rectangle / rail, capacité, espacement, rotation par pas et bornes), `InventoryModel` (un portail A/B =
  1 objet de 2 pièces), `PlacedItem` (lien de canal : un objet posé dans une zone « liée » attend un bouton).
- `HintController` : astuce après 3 échecs ; bouton INDICE après 5 échecs qui révèle progressivement la zone,
  l'objet puis la direction du **premier** pas de la solution — jamais la solution complète.
- `CameraController` : caméra orthographique cadrée sur les limites du niveau (`layout.bounds`), en tenant compte des barres d'interface et de la safe area ; suivi de Trykli optionnel, borné aux limites.
- `SolutionVerifier` : joue un niveau avec sa solution dans une scène physique isolée (outil éditeur + test).

## Mécanismes

Tous héritent de `MechanismBase` (`Configure(ElementData)`, activation par canal : toujours / activé /
désactivé / basculé par `SwitchBus`). `ElementFactory` crée un élément depuis `ElementData` (ou un objet
d'inventaire via `DataForItem`), avec valeurs par défaut centralisées (`ElementDefaults`).

| Type | Fichier | Remarques |
|---|---|---|
| Ressort | `Spring` | vitesse 11 m/s selon son axe |
| Rampe | `Ramp` | collider statique incliné |
| Ventilateur | `Fan` | courant 4 m, largeur 1,6 m, accélération 16 m/s² |
| Portail | `Portal` | paires par canal, conserve 95 % de la vitesse, sortie min. 3 m/s |
| Aimant | `Magnet` | rayon 3,5 m, force 22 |
| Bombe | `Bomb` | rayon 2,5 m, impulsion 11, réaction en chaîne 0,12 s |
| Bumper | `Bumper` | renvoie à 10 m/s |
| Plateformes | `MovingPlatform`, `RotatingPlatform` | cinématiques, boucle ou aller simple, activables |
| Bouton / Porte / Laser | `ButtonTrigger`, `Door`, `Laser` | canaux `SwitchBus` ; laser intermittent possible |
| Gravité | `GravitySwitch` | inverse / rétablit / bascule |
| Zones | `StickyZone`, `SlipperyZone`, `HazardSpikes` | |
| Canon | `Cannon` | capture puis tire après un délai |

## Objectifs d'étoiles

`ObjectiveDefinition` (JSON) → `ObjectiveFactory` → `StarObjective`. Types : `Complete`, `MaxObjects`,
`ItemUsage`, `CollectCrystals`, `Time`, `BounceCount`, `NoHazard`, `AvoidContact`, `Interaction` (compteurs :
bouton, portail, explosion, inversion de gravité, porte, laser coupé, canon, zone marquée), `Attempts`, `NoStop`.
Ajouter un type : une classe dans `Objectives.cs`, un `case` dans `ObjectiveFactory`, les textes FR/EN.

## Sauvegarde

`SaveData` (version 1) : niveau max débloqué, mondes, progression par niveau (étoiles, cristaux, meilleur temps,
objets, essais), réglages, skin. `SaveMigrator` met à niveau / répare. Écriture atomique (fichier temporaire
puis remplacement) + backup relu si le fichier principal est corrompu.

## Localisation / audio / haptique

- `Loc` : tables `Resources/Localization/{fr,en}.json`, repli sur le français, `LocalizedText` pour l'UI.
- `AudioManager` : bus musique / effets, sons synthétisés si aucun `AudioClip` n'est fourni.
- `Haptics` (`Trykli.Feedback`) : abstraction vibration (Android / iOS), désactivable.

## Outils éditeur

`Tools > TRYKLI` (voir README) : génération (scènes, prefabs, art, items, niveaux), Level Editor,
validation, vérification des solutions, export JSON, sauvegarde.

## Pipeline de contenu (hors Unity)

`Tools/LevelAuthoring` : DSL Python (`level_dsl.py`), simulateur approché (`trykli_sim.py`), recherche de
solutions (`design_search.py`, `finalize.py`), rendu ASCII (`render.py`), textes (`strings.py`) et
`build_levels.py`, qui écrit `Assets/Resources/LevelDefinitions/*.json`, `Assets/Resources/Localization/*.json`
et `REPORT.md`.
