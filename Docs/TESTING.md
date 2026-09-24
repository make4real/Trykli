# Tests de TRYKLI

## Tests automatiques (Unity Test Runner)

`Window > General > Test Runner`.

### EditMode (`Assets/Tests/EditMode`)

| Fichier | Couvre |
|---|---|
| `SaveSystemTests` | nouvelle sauvegarde, aller-retour JSON, backup si fichier corrompu, fichiers illisibles, migration de version, reset |
| `ProgressionTests` | déblocage du niveau suivant, du monde suivant après le boss, meilleures étoiles / cristaux / temps, fin du jeu au niveau 100, totaux |
| `ObjectiveTests`, `StarEvaluatorTests` | chaque type d'objectif, comparaisons, fabrique, calcul des étoiles (échec = 0, victoire >= 1) |
| `PlacementTests` | inventaire (paire de portails = 1 objet), zones point / rectangle / rail, capacité, espacement, rotation par pas et bornes |
| `LevelContentTests` | 100 niveaux présents et numérotés, validateur sans erreur, 3 objectifs, solution cohérente avec l'inventaire, boss tous les 10 niveaux, dispositions uniques, 10 mondes, clés FR = EN, textes des niveaux traduits, conversion JSON ↔ LevelData, détection de niveaux cassés |

### PlayMode (`Assets/Tests/PlayMode`)

`LevelSolutionTests` joue les 10 niveaux de chaque monde avec leur solution de référence, dans une scène
physique 2D isolée (même code que `Tools > TRYKLI > Verify Level Solutions`). Un échec indique un niveau à
régler dans le Level Editor.

## État de la vérification (honnête)

- Le code runtime, éditeur et tests **compile** contre les DLL Unity 2021.3 / uGUI / TextMeshPro hors éditeur
  (les API spécifiques à Unity 6 sont protégées par `#if UNITY_6000_0_OR_NEWER`).
- **Aucun test n'a été exécuté dans Unity** : l'éditeur n'était pas disponible pendant le développement.
- Les 100 solutions de référence atteignent la sortie dans le simulateur Python approché
  (`Tools/LevelAuthoring/REPORT.md`) ; elles doivent être confirmées avec la physique Unity (PlayMode ou
  `Verify Level Solutions`).
- Les contrôles de contenu (numérotation, clés de localisation, parité FR/EN, solutions, boss, dispositions
  uniques) ont été rejoués en Python sur les fichiers générés.

## Liste de vérification manuelle

À faire dans l'éditeur puis sur un téléphone Android et un iPhone.

### Démarrage
- [ ] Le projet s'ouvre dans Unity 6 sans erreur de compilation ; les TMP Essentials sont importées.
- [ ] `Tools > TRYKLI > Setup Project` se termine sans erreur ; `Validate Levels` : 0 erreur.
- [ ] `Verify Level Solutions` : noter les niveaux en échec dans `Logs/TrykliSolutionReport.txt`.
- [ ] `Play From Boot` : Boot → menu principal sans erreur dans la console.

### Menus
- [ ] Menu principal : Jouer (continue au bon niveau), Mondes, Paramètres, Crédits, Skins.
- [ ] Sélection des mondes : seul le monde 1 est ouvert au départ ; compteurs d'étoiles.
- [ ] Sélection des niveaux : niveaux verrouillés / débloqués / étoiles ; boss mis en évidence.
- [ ] Paramètres : musique, effets, vibration, langue FR/EN (textes mis à jour immédiatement), vitesse, reset.
- [ ] Crédits : liens (politique de confidentialité...) ouvrent le navigateur quand l'URL est renseignée.

### Placement
- [ ] Glisser un objet de l'inventaire vers une zone : il s'aimante ; hors zone : il revient.
- [ ] Déplacer, sélectionner, tourner (boutons et double-tap), supprimer (corbeille ou retour à l'inventaire).
- [ ] Objets interdits refusés par une zone ; capacité respectée ; paire de portails A puis B.
- [ ] Souris (éditeur) et tactile (appareil) ; boutons assez grands au pouce.

### Simulation
- [ ] GO lance la simulation, le placement est figé ; x2 accélère sans changer le résultat.
- [ ] Recommencer garde le placement ; Réinitialiser rend les objets.
- [ ] Victoire : panneau, étoiles, cristaux, niveau suivant ; la progression est sauvegardée (relancer l'app).
- [ ] Échecs : pics, laser, chute hors écran, blocage (3 s immobile), temps écoulé → message adapté.
- [ ] Indices : astuce après 3 échecs, bouton INDICE après 5 échecs, jamais la solution complète.
- [ ] Pause (bouton / Échap) : reprendre, recommencer, quitter vers la sélection.

### Mécanismes (au moins un niveau chacun)
- [ ] Ressort (1), rampe (4), ventilateur (11), portail (21), aimant (31), bumper (41), bombe (43),
      plateformes mobiles (51) et rotatives (53), bouton / porte (61), laser (63), laser intermittent (64),
      switch de gravité (71), finale (100).

### Appareil
- [ ] Portrait uniquement ; safe area (encoche) respectée ; différentes tailles d'écran.
- [ ] Performance fluide (60 i/s visés) ; audio et vibrations ; mise en arrière-plan / retour.
- [ ] Sauvegarde conservée après fermeture forcée.
