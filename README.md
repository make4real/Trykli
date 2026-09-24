# TRYKLI

Jeu mobile de puzzle physique 2D (iOS / Android, portrait) : le joueur place des objets (ressorts, rampes,
ventilateurs, portails, aimants, bombes, bumpers, switchs de gravité...) puis appuie sur **GO** ; Trykli, une
petite boule, suit la physique jusqu'à la sortie. 100 niveaux répartis en 10 mondes, 3 étoiles par niveau.

Documents de conception : `TRYKLI_Game_Design_Document.md` et `TRYKLI_Level_Bible_100_Niveaux.md`.

## Version d'Unity

- **Unity 6 LTS — 6000.0.58f2** (`ProjectSettings/ProjectVersion.txt`). Toute version 6000.0.x récente convient.
- Paquets (voir `Packages/manifest.json`) : uGUI 2.0 (inclut TextMeshPro), Test Framework 1.4.5, modules intégrés.
- Aucun asset externe : graphismes, sons et polices de secours sont générés par le code (placeholders).

## Ouvrir et lancer le projet

1. Unity Hub > **Add project from disk** > choisir le dossier du dépôt. Ouvrir avec Unity 6000.0.x.
2. Au premier import, les *TMP Essential Resources* sont importées automatiquement (sinon :
   `Tools > TRYKLI > Advanced > Import TMP Essentials`) et une fenêtre de bienvenue s'affiche.
3. Menu **`Tools > TRYKLI > Setup Project`** (une seule fois) : régénère les scènes, les prefabs d'éléments,
   le catalogue d'objets, les assets de niveaux (`LevelData`/`WorldData`) depuis les JSON, les réglages joueur
   (portrait, IL2CPP, bundle id `com.make4real.trykli`) et les Build Settings.
4. **`Tools > TRYKLI > Play From Boot`** (ou ouvrir `Assets/Scenes/Boot.unity` et Play).

Le jeu fonctionne aussi sans l'étape 3 : les scènes sont déjà fournies et les niveaux sont alors chargés
directement depuis `Assets/Resources/LevelDefinitions/*.json`.

## Générer le contenu

| Menu | Rôle |
|---|---|
| `Tools > TRYKLI > Setup Project` | Tout (re)générer : scènes, prefabs, catalogue, niveaux, réglages. |
| `Tools > TRYKLI > Generate Levels` | JSON → assets `LevelData` / `WorldData` + `LevelDatabase` (seulement les manquants, ou tout réécrire). |
| `Tools > TRYKLI > Level Editor` | Éditeur de niveaux (voir `Docs/LEVEL_CREATION.md`). |
| `Tools > TRYKLI > Validate Levels` | Contrôles : spawn / sortie, objectifs, doublons, monde invalide, inventaire, solution... |
| `Tools > TRYKLI > Verify Level Solutions` | Joue chaque niveau avec sa solution de référence (physique Unity réelle, scène isolée). Rapport : `Logs/TrykliSolutionReport.txt`. |
| `Tools > TRYKLI > Export Levels to JSON` | Assets → JSON (source de vérité versionnée). |
| `Tools > TRYKLI > Save/...` | Ouvrir / supprimer la sauvegarde, tout débloquer (tests). |

Les 100 niveaux sont écrits en Python (`Tools/LevelAuthoring/worlds/world01..10.py`) et validés par un
simulateur approché ; `python3 Tools/LevelAuthoring/build_levels.py` régénère les JSON, les fichiers de
langue FR/EN et `Tools/LevelAuthoring/REPORT.md`. Après ajout de fichiers hors Unity :
`python3 Tools/UnityProject/unity_assets.py` (fichiers `.meta`).

## Architecture (résumé)

Détails dans `Docs/ARCHITECTURE.md`.

- `Assets/Scripts/Core` : `GameManager` (persistant), machine d'états (Menu, Placement, Simulation, Victory,
  Failure, Pause), navigation entre scènes, localisation (`Loc`), audio procédural, haptique.
- `Assets/Scripts/Data` : `LevelData`, `WorldData`, `ItemDefinition`, `LevelDatabase`, format JSON, validateur.
- `Assets/Scripts/Gameplay` : `LevelManager`, `LevelBuilder`, `TrykliController`, `SimulationRunner`
  (pas fixe 0,02 s, physique 2D pilotée par script), placement (drag & drop, zones), indices.
- `Assets/Scripts/Mechanics` : 17 mécanismes (ressort, rampe, ventilateur, portail, aimant, bombe, bumper,
  plateformes mobiles / rotatives, bouton, porte, laser, switch de gravité, zones collante / glissante, pics, canon).
- `Assets/Scripts/Objectives` : objectifs d'étoiles extensibles + `StarEvaluator`.
- `Assets/Scripts/Save` : sauvegarde JSON versionnée (écriture atomique + backup), progression.
- `Assets/Scripts/UI` : menus, HUD, panneaux (pause, victoire, échec, paramètres, crédits, skins), safe area.
- `Assets/Scripts/Editor` : outils `Tools > TRYKLI`.

## Scènes

`Boot` → `MainMenu` → `WorldSelect` → `LevelSelect` → `Gameplay`. Les 100 niveaux sont des données chargées
dans la scène `Gameplay` (pas une scène par niveau). Chaque scène ne contient qu'un contrôleur qui construit
son interface au lancement.

## Contrôles

| Action | Mobile | Éditeur / PC |
|---|---|---|
| Placer un objet | glisser depuis la barre d'inventaire vers une zone | clic-glisser |
| Déplacer | glisser l'objet posé | clic-glisser |
| Sélectionner | toucher l'objet | clic |
| Tourner | bouton ⟳ / ⟲ ou double-tap | boutons ou double-clic |
| Supprimer | bouton corbeille ou glisser vers l'inventaire | idem |
| Lancer | **GO** | **GO** |
| Recommencer (même placement) / Réinitialiser (objets rendus) | boutons du HUD | idem |
| Pause | bouton pause | bouton ou `Échap` |

## Structure d'un niveau

Un niveau (`LevelDefinition` JSON / `LevelData` asset) contient : identifiants (monde, numéro), difficulté,
boss, limite de simulation, disposition (`spawn`, `goal`, limites, éléments fixes : blocs, pics, cristaux,
mécanismes), zones de placement (point, rectangle, rail : objets autorisés, rotation, capacité, canal),
inventaire, 3 objectifs d'étoiles (le 1er = terminer), solution de référence, indices, astuce et caméra.
Voir `Docs/LEVEL_CREATION.md`.

## Build Android / iOS

`Setup Project` configure : orientation portrait, IL2CPP, ARM64, identifiant `com.make4real.trykli`.
- **Android** : `File > Build Profiles` > Android > *Switch Platform* > Build (module Android installé via le Hub).
  Pour le Play Store : créer un keystore (Player Settings > Publishing) et produire un AAB.
- **iOS** : plateforme iOS > Build, puis ouvrir le projet Xcode généré, choisir l'équipe de signature et lancer
  sur appareil (macOS + Xcode requis).
- Avant publication : remplir `AppLinks` (URL de politique de confidentialité, conditions, support), les
  icônes et l'écran de démarrage.

## Tests

- EditMode (`Assets/Tests/EditMode`) : sauvegarde, migration, progression / déblocages, étoiles, chaque type
  d'objectif, inventaire, règles de placement, contenu (100 niveaux valides, mondes, boss, clés FR/EN).
- PlayMode (`Assets/Tests/PlayMode`) : chaque niveau joué avec sa solution de référence dans la physique Unity.
- `Window > General > Test Runner`. Liste de vérification manuelle : `Docs/TESTING.md`.

## Limitations connues

- Le projet a été produit **sans éditeur Unity** : le code C# (runtime, éditeur, tests) a été compilé contre les
  DLL Unity/uGUI/TMP hors Unity, mais le jeu **n'a jamais été lancé** et aucun test Unity n'a été exécuté.
- Les solutions des 100 niveaux ont été validées dans un **simulateur Python approché** de la physique, pas
  dans Box2D : certains niveaux (surtout ceux liés au timing : plateformes mobiles, lasers intermittents,
  bombes) peuvent demander un réglage. Lancer `Verify Level Solutions` puis corriger dans le Level Editor.
- Certains objets d'inventaire sont volontairement optionnels (niveaux « plusieurs solutions » ou finales) ;
  voir `Tools/LevelAuthoring/REPORT.md`.
- Pas de monétisation, pas de services en ligne ; liens légaux à compléter.

## Assets temporaires

Tout le visuel (sprites SDF générés, dégradés, icônes), l'audio (sons synthétisés) et les animations (tweens
code) sont des placeholders générés à l'exécution ou par `Rebake Placeholder Art`. Ils peuvent être remplacés
via `SpriteLibrary`, `AudioLibrary` et `ElementPrefabCatalog` sans toucher au gameplay.
