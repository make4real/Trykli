# TRYKLI — Game Design Document (GDD)
Version 1.0

## 1. Vision du jeu

**Nom de travail : TRYKLI**

TRYKLI est un jeu mobile de puzzle-physique court, lisible et rejouable. Le joueur doit guider une petite créature appelée **Trykli** jusqu'à une sortie en plaçant un nombre limité d'objets dans le niveau avant de lancer la simulation.

Le joueur ne contrôle pas directement Trykli pendant la simulation. Toute la réflexion se fait **avant** le lancement.

Le concept repose sur trois phases simples :

1. **Observer**
2. **Placer**
3. **Lancer**

Le plaisir vient du fait d'imaginer une solution, de regarder la réaction en chaîne se produire, puis d'ajuster son plan si nécessaire.

### Promesse joueur

> "Je comprends le niveau en quelques secondes, mais trouver la solution parfaite me demande de réfléchir."

### Piliers du jeu

- Compréhensible immédiatement
- Parties très courtes
- Contrôles à un doigt
- Physique amusante et prévisible
- Plusieurs façons de réussir certains niveaux
- Difficulté progressive
- Satisfaction visuelle lors d'une bonne réaction en chaîne
- Échec rapide, redémarrage instantané
- Niveau parfait optionnel grâce aux 3 étoiles

---

# 2. Plateformes

- iOS
- Android
- Orientation portrait recommandée
- Fonctionnement hors ligne
- Unity
- Contrôles tactiles
- Souris fonctionnelle dans l'éditeur Unity

Résolution de référence :

- 1080 x 1920
- UI responsive avec Canvas Scaler

---

# 3. Public cible

Public principal :

- joueurs mobiles occasionnels
- 10 ans et plus
- joueurs aimant les puzzles simples
- sessions de 1 à 10 minutes

Le jeu doit être accessible sans tutoriel texte complexe.

---

# 4. Personnage principal : Trykli

Trykli est une petite créature ronde et expressive.

Caractéristiques visuelles recommandées :

- forme ronde ou légèrement gélatineuse
- deux yeux
- petites animations faciales
- déformation lors des impacts
- animations simples mais expressives

Trykli ne possède pas de jambes.

Il se déplace grâce à :

- gravité
- pentes
- ressorts
- plateformes
- explosions
- ventilateurs
- portails
- aimants
- autres mécanismes

## États possibles

Trykli peut être :

- Idle
- Falling
- Rolling
- Bouncing
- Flying
- Teleporting
- Stuck
- Dead
- Victory

---

# 5. Boucle principale de gameplay

## Phase 1 — Observation

Le niveau apparaît.

Le joueur voit :

- Trykli
- la sortie
- les obstacles
- les mécanismes
- les zones où les objets peuvent être placés
- les objets disponibles

Le temps est arrêté.

## Phase 2 — Placement

Le joueur fait glisser les objets disponibles depuis la barre d'outils vers les zones autorisées.

Le joueur peut :

- déplacer un objet
- tourner un objet
- supprimer un objet
- réinitialiser le placement
- zoomer légèrement si nécessaire

## Phase 3 — Simulation

Le joueur appuie sur :

**GO**

La simulation démarre.

Trykli est soumis aux lois physiques.

Le joueur regarde le résultat.

## Phase 4 — Résultat

Deux possibilités :

### Victoire

Trykli atteint la sortie.

Le jeu affiche :

- nombre d'étoiles
- objectifs réussis
- bouton Niveau suivant
- bouton Rejouer
- bouton Sélection des niveaux

### Échec

Trykli :

- tombe hors du niveau
- touche un danger
- reste bloqué
- dépasse le temps maximum

Le jeu affiche rapidement :

- Réessayer
- Réinitialiser
- éventuellement un petit conseil après plusieurs échecs

---

# 6. Contrôles

## Placement

Drag & Drop.

## Rotation

Deux possibilités :

- bouton rotation lorsque l'objet est sélectionné
- double tap sur l'objet

Rotation par incréments de 15° ou 30°.

## Suppression

Glisser l'objet vers une zone poubelle ou utiliser un bouton supprimer.

## Simulation

Bouton principal :

GO

Pendant la simulation :

- bouton vitesse x1 / x2
- bouton restart

---

# 7. Structure d'un niveau

Chaque niveau contient :

- Spawn Trykli
- Sortie
- Géométrie
- Obstacles
- Slots de placement
- Objets disponibles
- Objectifs bonus
- Temps maximum de simulation

La caméra est généralement fixe.

Certains niveaux avancés peuvent utiliser une caméra légèrement plus grande ou scrollable.

---

# 8. Sortie

La sortie peut être représentée par :

- portail lumineux
- trou circulaire
- capsule énergétique

Condition de victoire :

Le centre de Trykli doit entrer dans la zone de sortie.

Animation :

- ralentissement très court
- aspiration
- particules
- son positif
- apparition du panneau de victoire

---

# 9. Système d'étoiles

Chaque niveau possède 3 étoiles.

## Étoile 1

Terminer le niveau.

## Étoile 2

Objectif bonus spécifique.

Exemples :

- utiliser maximum 2 objets
- récupérer une étoile placée dans le niveau
- ne toucher aucun danger
- effectuer 3 rebonds
- terminer en moins de 8 secondes

## Étoile 3

Objectif de maîtrise.

Exemples :

- utiliser exactement une certaine combinaison
- récupérer les 3 cristaux
- terminer avec un objet inutilisé
- réaliser un trick spécifique

Le joueur peut donc terminer le jeu sans obtenir toutes les étoiles.

---

# 10. Philosophie de difficulté

Le jeu ne doit jamais devenir brutalement plus difficile.

Chaque nouvelle mécanique suit ce cycle :

1. introduction
2. pratique
3. combinaison
4. maîtrise

Exemple :

Niveau 11 :
découverte du ventilateur

Niveau 12 :
ventilateur simple

Niveau 13 :
ventilateur + ressort

Niveau 14 :
ventilateur + obstacle

Niveau 15 :
puzzle complet avec ventilateur

---

# 11. Courbe de difficulté

Échelle interne :

- 1 = très facile
- 10 = très difficile

Progression recommandée :

Niveaux 1–10 : difficulté 1–2  
Niveaux 11–20 : difficulté 2–3  
Niveaux 21–30 : difficulté 3–4  
Niveaux 31–40 : difficulté 4–5  
Niveaux 41–50 : difficulté 5–6  
Niveaux 51–60 : difficulté 6  
Niveaux 61–70 : difficulté 6–7  
Niveaux 71–80 : difficulté 7–8  
Niveaux 81–90 : difficulté 8  
Niveaux 91–100 : difficulté 8–10  

Important :

Un niveau particulièrement difficile doit être suivi d'un niveau légèrement plus simple.

Cela évite la frustration.

---

# 12. Mécaniques principales

## 12.1 Ressort

Projette Trykli.

Paramètres :

- puissance
- direction
- angle

Couleur visuelle claire.

---

## 12.2 Rampe

Permet de modifier la trajectoire.

Le joueur peut parfois régler son angle.

---

## 12.3 Ventilateur

Crée une force constante.

Peut pousser Trykli :

- gauche
- droite
- haut
- diagonale

---

## 12.4 Portail

Deux portails reliés.

Entrée A -> sortie B.

Conservation partielle de la vitesse.

---

## 12.5 Aimant

Attire Trykli.

Uniquement dans un rayon défini.

Peut être :

- fixe
- activé par bouton

---

## 12.6 Bombe

Explose au contact ou après activation.

Projette Trykli.

Ne tue pas automatiquement Trykli.

---

## 12.7 Bloc mobile

Se déplace entre deux points.

Peut :

- transporter Trykli
- bloquer une trajectoire

---

## 12.8 Plateforme rotative

Tourne continuellement ou lorsqu'elle est activée.

---

## 12.9 Bouton

Active :

- porte
- plateforme
- ventilateur
- aimant
- laser

---

## 12.10 Porte

Bloque un passage.

S'ouvre avec un bouton.

---

## 12.11 Zone collante

Réduit fortement la vitesse.

---

## 12.12 Zone glissante

Réduit les frottements.

---

## 12.13 Pic

Danger.

Contact = échec.

---

## 12.14 Laser

Danger périodique.

Peut être désactivé.

---

## 12.15 Canon

Capture Trykli puis le tire.

Le joueur peut parfois régler l'angle.

---

## 12.16 Téléporteur directionnel

Portail avancé modifiant la direction de sortie.

---

## 12.17 Gravity Switch

Inverse la gravité.

Peut affecter :

- Trykli
- certains objets

---

## 12.18 Bumper

Objet circulaire qui repousse Trykli fortement.

---

# 13. Objets plaçables

Objets utilisables par le joueur :

- ressort
- rampe
- ventilateur
- portail
- aimant
- bombe
- bumper
- mini plateforme

Chaque niveau fournit uniquement certains objets.

Exemple :

Ressort x2  
Rampe x1  
Ventilateur x1

Le joueur ne choisit pas librement tous les objets du jeu.

Cela permet de contrôler la difficulté.

---

# 14. Zones de placement

Pour éviter des solutions impossibles à prévoir, le joueur ne peut pas placer des objets partout.

Utiliser des **Placement Zones**.

Types :

- emplacement précis
- zone rectangulaire
- rail
- surface

Exemple :

Un ventilateur peut être placé uniquement dans trois emplacements prévus.

Cela rend les niveaux plus faciles à concevoir et à équilibrer.

---

# 15. Timing

Certains niveaux avancés utilisent le timing.

Exemples :

- plateforme mobile
- laser intermittent
- porte temporisée

Cependant le joueur ne doit pas avoir besoin d'appuyer pendant la simulation.

Le timing dépend uniquement du placement initial.

---

# 16. Système anti-frustration

Après 3 échecs :

petit message facultatif.

Exemple :

"Essaie de modifier l'angle du ressort."

Après 5 échecs :

bouton :

"Indice"

L'indice peut montrer :

- zone importante
- objet recommandé
- trajectoire approximative

Jamais afficher directement la solution complète au premier indice.

---

# 17. Monde et progression

Le jeu contient 10 mondes.

Chaque monde contient 10 niveaux.

Total :

100 niveaux.

Chaque monde introduit une mécanique principale.

---

# 18. Monde 1 — Premiers rebonds

Thème :
laboratoire clair.

Mécaniques :

- gravité
- rampes
- ressorts

## Niveau 1
Tutoriel.
Un ressort.
Ligne droite.
Très facile.

## Niveau 2
Choisir la bonne position du ressort.

## Niveau 3
Modifier l'angle du ressort.

## Niveau 4
Rampe + ressort.

## Niveau 5
Deux ressorts.

## Niveau 6
Obstacle simple.

## Niveau 7
Trou à franchir.

## Niveau 8
Récupérer un cristal bonus.

## Niveau 9
Deux solutions possibles.

## Niveau 10
Puzzle final :
rampe + deux ressorts + obstacle.

---

# 19. Monde 2 — Le vent

Nouvelle mécanique :
ventilateur.

## Niveau 11
Introduction ventilateur.

## Niveau 12
Ventilateur horizontal.

## Niveau 13
Ventilateur vertical.

## Niveau 14
Ressort + ventilateur.

## Niveau 15
Deux ventilateurs.

## Niveau 16
Éviter des pics.

## Niveau 17
Ventilateur avec rampe.

## Niveau 18
Cristaux bonus en hauteur.

## Niveau 19
Choisir orientation optimale.

## Niveau 20
Puzzle complet du monde.

---

# 20. Monde 3 — Portails

Nouvelle mécanique :
portails.

## Niveau 21
Portail simple.

## Niveau 22
Conservation de vitesse.

## Niveau 23
Portail vertical.

## Niveau 24
Ressort vers portail.

## Niveau 25
Deux paires de portails.

## Niveau 26
Portail + danger.

## Niveau 27
Portail + ventilateur.

## Niveau 28
Choisir orientation portail.

## Niveau 29
Fausse route possible.

## Niveau 30
Puzzle chaîne :
ressort -> portail -> ventilateur -> sortie.

---

# 21. Monde 4 — Magnétisme

Nouvelle mécanique :
aimant.

## Niveau 31
Aimant simple.

## Niveau 32
Modifier trajectoire avec aimant.

## Niveau 33
Aimant au-dessus du vide.

## Niveau 34
Aimant + ressort.

## Niveau 35
Deux aimants.

## Niveau 36
Aimant activable.

## Niveau 37
Aimant + bouton.

## Niveau 38
Cristaux autour d'un aimant.

## Niveau 39
Trajectoire courbe complexe.

## Niveau 40
Puzzle final magnétique.

---

# 22. Monde 5 — Réactions en chaîne

Nouvelle mécanique :
bombes et bumpers.

## Niveau 41
Bumper simple.

## Niveau 42
Deux bumpers.

## Niveau 43
Bombe simple.

## Niveau 44
Explosion directionnelle.

## Niveau 45
Bombe + ressort.

## Niveau 46
Bumper + portail.

## Niveau 47
Bombe + ventilateur.

## Niveau 48
Réaction en chaîne avec boutons.

## Niveau 49
Plusieurs chemins possibles.

## Niveau 50
Grand puzzle réaction en chaîne.

---

# 23. Monde 6 — Machines

Nouvelle mécanique :
plateformes mobiles et rotatives.

## Niveau 51
Plateforme mobile.

## Niveau 52
Plateforme verticale.

## Niveau 53
Plateforme rotative.

## Niveau 54
Plateforme + ressort.

## Niveau 55
Deux plateformes synchronisées.

## Niveau 56
Plateforme + ventilateur.

## Niveau 57
Plateforme + portail.

## Niveau 58
Bouton contrôlant plateforme.

## Niveau 59
Timing indirect.

## Niveau 60
Puzzle mécanique complet.

---

# 24. Monde 7 — Systèmes

Nouvelles mécaniques :

- boutons
- portes
- lasers

## Niveau 61
Bouton + porte.

## Niveau 62
Bouton à activer avec Trykli.

## Niveau 63
Laser simple.

## Niveau 64
Laser intermittent.

## Niveau 65
Bouton désactive laser.

## Niveau 66
Portail derrière porte.

## Niveau 67
Deux boutons.

## Niveau 68
Ordre d'activation.

## Niveau 69
Mini réseau de mécanismes.

## Niveau 70
Puzzle logique complet.

---

# 25. Monde 8 — Gravité

Nouvelle mécanique :
gravity switch.

## Niveau 71
Inversion simple.

## Niveau 72
Marcher au plafond.

## Niveau 73
Gravity + ressort.

## Niveau 74
Gravity + portail.

## Niveau 75
Deux inversions.

## Niveau 76
Gravity + laser.

## Niveau 77
Gravity + ventilateur.

## Niveau 78
Parcours plafond/sol.

## Niveau 79
Plusieurs trajectoires.

## Niveau 80
Puzzle complet gravité.

---

# 26. Monde 9 — Chaos contrôlé

Le joueur utilise toutes les mécaniques précédentes.

## Niveau 81
Ressort + aimant + portail.

## Niveau 82
Ventilateur + bombe.

## Niveau 83
Portail + plateforme mobile.

## Niveau 84
Gravity + aimant.

## Niveau 85
Laser + boutons + ressort.

## Niveau 86
Bombe + portail.

## Niveau 87
Plateformes + ventilateurs.

## Niveau 88
3 chemins possibles.

## Niveau 89
Puzzle multi-étapes.

## Niveau 90
Mega puzzle.

---

# 27. Monde 10 — Master Trykli

Objectif :
tester la maîtrise complète.

## Niveau 91
Placement précis.

## Niveau 92
Utilisation minimale d'objets.

## Niveau 93
Puzzle à plusieurs solutions.

## Niveau 94
Longue réaction en chaîne.

## Niveau 95
Timing avancé.

## Niveau 96
Portails + gravité + laser.

## Niveau 97
Machine complexe.

## Niveau 98
Puzzle presque libre.

## Niveau 99
Ultimate Challenge.

## Niveau 100 — Final

Le niveau final utilise :

- ressort
- portail
- ventilateur
- aimant
- bombe
- plateforme mobile
- gravity switch
- boutons

La réaction finale doit être spectaculaire.

Une fois réussi :

animation spéciale.

Message :

"TRYKLI MASTER"

---

# 28. Structure de difficulté détaillée

La difficulté ne doit pas uniquement dépendre du nombre d'objets.

Variables :

- nombre d'objets disponibles
- nombre d'emplacements
- précision nécessaire
- timing
- nombre de mécaniques
- dangers
- fausses pistes
- objectifs bonus

Exemple :

Niveau facile :

3 emplacements possibles
1 ressort
1 solution évidente

Niveau moyen :

5 emplacements
2 objets
2 interactions

Niveau difficile :

6 à 8 emplacements
3 ou 4 objets
3 mécaniques
plusieurs trajectoires

---

# 29. Limites de difficulté

Éviter :

- précision au pixel
- hasard important
- physique imprévisible
- niveaux nécessitant plus de 20 essais
- timing nécessitant des millisecondes

Une solution correcte doit fonctionner de manière reproductible.

---

# 30. Durée des niveaux

Objectif :

Début :
15 à 30 secondes

Milieu :
30 secondes à 2 minutes

Fin :
1 à 5 minutes

La simulation elle-même ne devrait généralement pas dépasser 15 secondes.

---

# 31. Écran d'accueil

Éléments :

Logo TRYKLI

Boutons :

JOUER  
NIVEAUX  
PARAMÈTRES  

Optionnel :

SKINS

Version du jeu en bas.

Fond animé léger avec Trykli.

---

# 32. Écran de sélection des mondes

Afficher :

Monde 1
Monde 2
...
Monde 10

Chaque monde affiche :

- progression
- étoiles obtenues
- niveau maximum atteint

Exemple :

MONDE 4  
23 / 30 étoiles

---

# 33. Sélection des niveaux

Grille 1 à 10.

Chaque niveau affiche :

- numéro
- étoiles
- verrouillé ou débloqué

Déblocage :

Terminer un niveau débloque le suivant.

---

# 34. Écran de jeu

## Haut

- bouton pause
- numéro niveau
- étoiles/objectifs

## Centre

terrain de jeu.

## Bas

inventaire des objets.

Bouton central :

GO

---

# 35. Menu Pause

Boutons :

CONTINUER  
RECOMMENCER  
PARAMÈTRES  
QUITTER LE NIVEAU

---

# 36. Écran de victoire

Afficher :

NIVEAU TERMINÉ

Étoiles :
★ ★ ☆

Objectifs :

✓ Atteindre la sortie  
✓ Maximum 2 objets  
✗ Récupérer le cristal

Boutons :

NIVEAU SUIVANT  
REJOUER  
NIVEAUX

---

# 37. Écran d'échec

Petit panneau.

Texte aléatoire :

"Oups."

"Presque."

"Trykli avait un autre plan."

Boutons :

RÉESSAYER  
MODIFIER

Le bouton Modifier ramène directement au mode placement.

---

# 38. Paramètres

Options :

Musique ON/OFF

Sons ON/OFF

Vibrations ON/OFF

Langue

Réinitialiser progression

Crédits

Politique de confidentialité

---

# 39. Audio

Musique :

- légère
- minimaliste
- relaxante
- légèrement dynamique

Effets :

- ressort
- portail
- explosion
- collision
- victoire
- bouton
- échec

Trykli peut produire de petits sons non verbaux.

---

# 40. Feedback visuel

Chaque action doit être claire.

Exemple :

Ressort :
compression + étirement

Bumper :
flash

Portail :
particules

Aimant :
ondes

Ventilateur :
particules d'air

---

# 41. Trajectoire prévisionnelle

Option importante.

Lorsqu'un objet est sélectionné, afficher éventuellement une indication légère de direction.

Ne pas montrer toute la solution.

Exemple :

un ressort affiche une flèche indiquant sa direction.

---

# 42. Système de cristaux

Optionnel.

Chaque niveau peut contenir jusqu'à 3 cristaux.

Ils servent principalement au scoring.

Ils peuvent aussi débloquer :

- skins
- couleurs
- effets visuels

Aucun élément pay-to-win.

---

# 43. Skins

Trykli peut avoir différents skins.

Exemples :

- classique
- ninja
- robot
- slime
- astronaute
- pirate

Uniquement cosmétique.

---

# 44. Sauvegarde

Sauvegarde locale.

Données :

- niveau maximum débloqué
- étoiles par niveau
- cristaux
- paramètres
- skin sélectionné

Utiliser JSON ou PlayerPrefs structurés.

Prévoir une architecture pouvant ensuite recevoir du Cloud Save.

---

# 45. Architecture Unity recommandée

Scenes :

Boot  
MainMenu  
WorldSelect  
LevelSelect  
Gameplay

Le Gameplay doit charger les niveaux depuis des données.

Ne pas créer 100 scènes Unity différentes.

---

# 46. Système de niveaux data-driven

Chaque niveau doit être défini par des données.

Créer par exemple :

LevelData ScriptableObject

Contenu :

levelID  
worldID  
levelNumber  
difficulty  
availableItems  
starObjectives  
timeLimit  
levelPrefab

Les géométries peuvent utiliser un prefab par niveau.

---

# 47. Level Manager

Responsabilités :

- charger niveau
- lancer simulation
- détecter victoire
- détecter échec
- gérer restart
- calculer étoiles
- sauvegarder progression

---

# 48. Game Manager

Responsabilités :

- état global
- progression
- navigation
- sauvegarde

---

# 49. Object Placement Manager

Responsabilités :

- drag & drop
- slots autorisés
- rotation
- suppression
- inventaire

---

# 50. Physics Manager

Utiliser Unity Physics 2D.

Composants :

Rigidbody2D  
Collider2D  
PhysicsMaterial2D

La physique doit être stable.

Fixed Timestep conseillé :

0.02

---

# 51. Caméra

Orthographic Camera.

La majorité des niveaux tient sur un écran.

Pour les niveaux plus grands :

CameraBounds.

Éviter une caméra libre compliquée.

---

# 52. Tutorial

Le tutorial doit être visuel.

Niveau 1 :

animation doigt.

1. déplacer ressort
2. appuyer GO

Niveau 2 :

montrer rotation.

Niveau 3 :

montrer restart.

Après cela :

plus de tutorial obligatoire.

---

# 53. Méthode de création des 100 niveaux

Pour éviter 100 niveaux répétitifs :

Chaque niveau doit répondre à au moins une question nouvelle.

Exemples :

"Comment franchir ce trou ?"

"Comment changer ma vitesse ?"

"Comment activer cette porte avant d'arriver ?"

"Comment utiliser le portail pour conserver mon élan ?"

---

# 54. Matrice de variété

Chaque niveau peut varier selon 5 axes :

1. mécanique
2. topologie
3. danger
4. contraintes
5. objectif bonus

Cela permet des centaines de combinaisons.

---

# 55. Ratio recommandé

Sur 100 niveaux :

30 niveaux faciles  
40 niveaux moyens  
25 niveaux difficiles  
5 niveaux très difficiles

Les très difficiles doivent être :

50  
70  
90  
99  
100

---

# 56. Rythme recommandé

Après un gros niveau :

mettre un niveau plus fun ou spectaculaire.

Exemple :

49 difficile  
50 boss puzzle  
51 découverte simple

---

# 57. Boss Puzzles

Chaque niveau 10, 20, 30, etc. agit comme un mini-boss.

Il combine les mécaniques apprises dans le monde.

Il ne doit pas introduire une nouvelle mécanique.

---

# 58. Replay

Le joueur doit pouvoir recommencer instantanément.

Temps entre échec et nouvel essai :

moins de 2 secondes.

C'est essentiel pour éviter la frustration.

---

# 59. Temps de chargement

Objectif :

moins de 2 secondes entre niveaux sur téléphone récent.

Les niveaux doivent être légers.

---

# 60. UI Design

Style recommandé :

- minimal
- moderne
- formes arrondies
- gros boutons
- très peu de texte
- animations fluides

Palette :

chaque monde peut avoir sa propre couleur dominante.

---

# 61. Identité des mondes

Monde 1 : laboratoire  
Monde 2 : ciel / air  
Monde 3 : dimension portail  
Monde 4 : magnétique  
Monde 5 : usine explosive  
Monde 6 : machines  
Monde 7 : sécurité / lasers  
Monde 8 : espace / gravité  
Monde 9 : chaos dimensionnel  
Monde 10 : nexus Trykli  

---

# 62. Conditions d'échec

Échec si :

Trykli touche un danger.

Trykli quitte la zone.

Trykli reste immobile trop longtemps.

Le temps maximum est dépassé.

---

# 63. Détection d'immobilité

Si vitesse < seuil pendant 3 secondes :

afficher :

"Trykli est coincé."

Puis échec.

---

# 64. Accessibilité

Prévoir :

- couleurs très contrastées
- icônes en plus des couleurs
- taille de texte lisible
- vibrations désactivables

---

# 65. Monétisation possible plus tard

Version initiale :

aucune monétisation obligatoire.

Option future :

- suppression publicité
- pack skins
- publicité volontaire pour indice

Ne jamais bloquer la progression derrière un paiement.

---

# 66. Analytics futures

Prévoir la possibilité de mesurer :

- niveau commencé
- niveau terminé
- nombre d'essais
- temps de résolution
- étoiles obtenues
- niveau abandonné

Cela permettra d'équilibrer les niveaux.

---

# 67. Indicateur d'équilibrage

Objectifs approximatifs :

Niveaux 1–10 :
80–95 % de réussite

11–30 :
70–90 %

31–60 :
60–80 %

61–80 :
50–70 %

81–100 :
35–60 %

Ces valeurs concernent les joueurs ayant atteint ces niveaux.

---

# 68. Tests utilisateurs

Pour chaque niveau mesurer :

- nombre moyen d'essais
- temps moyen
- taux d'abandon

Si un niveau demande plus de 8–10 essais moyens :

réexaminer le design.

---

# 69. Objectif de session

Une session moyenne doit permettre :

3 à 8 niveaux.

Un joueur doit pouvoir jouer une minute ou vingt minutes.

---

# 70. Premier lancement

Sequence :

Logo studio

Logo TRYKLI

Bouton Jouer

Niveau 1

Pas de création de compte obligatoire.

---

# 71. Fin du jeu

Après niveau 100 :

animation spéciale.

Trykli traverse un portail géant.

Écran :

TRYKLI MASTER

Statistiques :

Niveaux : 100/100  
Étoiles : XXX/300  
Cristaux : XXX/300

Boutons :

REJOUER LES NIVEAUX  
COMPLÉTER LES ÉTOILES

---

# 72. Direction artistique

Le jeu doit être très lisible.

Éviter des décors qui gênent la compréhension.

Premier plan :
éléments interactifs très visibles.

Arrière-plan :
simple et légèrement animé.

---

# 73. Animations principales

Trykli :

idle  
squash/stretch  
impact  
surprise  
victory  
death

Objets :

activation  
idle  
feedback

UI :

buttons scale  
panels slide  
stars pop

---

# 74. Effets haptiques

Optionnels.

Petit feedback :

placement

GO

impact important

victoire

---

# 75. Structure de projet Unity

Assets/

Art/
Audio/
Materials/
Prefabs/
Scripts/
Scenes/
Levels/
UI/
ScriptableObjects/
Animations/
VFX/

---

# 76. Scripts principaux recommandés

GameManager.cs  
LevelManager.cs  
SaveManager.cs  
UIManager.cs  
PlacementManager.cs  
DraggableObject.cs  
PlacementSlot.cs  
TrykliController.cs  
GoalController.cs  
Hazard.cs  
Spring.cs  
Fan.cs  
Portal.cs  
Magnet.cs  
Bomb.cs  
Bumper.cs  
MovingPlatform.cs  
GravitySwitch.cs  
ButtonTrigger.cs  
Door.cs  
Laser.cs  

---

# 77. State Machine

GameState :

Menu  
Placement  
Simulation  
Victory  
Failure  
Pause

Cela évite les bugs d'interaction.

---

# 78. Règle importante

Lorsque Simulation commence :

les objets placés deviennent verrouillés.

Le joueur ne peut plus les déplacer.

Restart :

restaure exactement les positions de placement.

Reset :

supprime les placements.

---

# 79. Preview Mode

Optionnel.

Le joueur peut sélectionner un objet.

Afficher :

direction

zone d'effet

rayon aimant

zone ventilateur

Cela améliore fortement la compréhension.

---

# 80. Génération des niveaux

Ne pas utiliser de génération procédurale pour les 100 niveaux principaux.

Les niveaux doivent être conçus manuellement.

La physique doit produire des puzzles fiables.

---

# 81. Level Editor interne

Prévoir idéalement un outil Unity simple.

Fonctions :

Créer niveau

Placer spawn

Placer sortie

Ajouter terrain

Ajouter slots

Définir inventaire

Définir objectifs

Tester niveau

Cela accélérera énormément la création des 100 niveaux.

---

# 82. Critères pour considérer un niveau terminé

Un niveau est valide si :

- solution testée
- solution reproductible
- aucune précision extrême
- aucun bug physique
- temps raisonnable
- au moins une solution

---

# 83. Design des objectifs bonus

Ne pas utiliser toujours :

"terminer rapidement".

Varier :

collecte

objets limités

interaction spéciale

trajet

nombre de rebonds

ordre

---

# 84. Exemple niveau complet

Niveau 34

Objectif :
atteindre une plateforme haute.

Objets :

Ressort x1  
Aimant x1

Slots :

5

Solution :

placer ressort sous Trykli.

placer aimant à droite.

Le ressort propulse Trykli.

L'aimant courbe la trajectoire.

Trykli atteint la sortie.

Étoiles :

1 : terminer  
2 : récupérer cristal  
3 : terminer avec aimant placé dans zone supérieure  

---

# 85. Exemple niveau avancé

Niveau 87

Mécaniques :

portail

plateforme mobile

ventilateur

laser

Objets :

ventilateur x1  
portail x2

Le joueur doit :

propulser Trykli

entrer portail

sortir sur plateforme

attendre déplacement

éviter laser

atteindre sortie

Objectif 3 étoiles :

terminer sans toucher bumper de sécurité.

---

# 86. Priorité MVP

Pour une première version jouable :

Créer :

menus

sauvegarde

système niveaux

Trykli

sortie

ressort

rampe

ventilateur

portail

10 niveaux

Une fois cela stable :

ajouter les autres mécaniques.

---

# 87. Ordre de développement recommandé

Phase 1 :
core physics

Phase 2 :
placement

Phase 3 :
level system

Phase 4 :
UI

Phase 5 :
premier monde

Phase 6 :
sauvegarde

Phase 7 :
nouvelles mécaniques

Phase 8 :
100 niveaux

Phase 9 :
polish

Phase 10 :
mobile build

---

# 88. Ce que Claude doit produire

Claude doit générer un projet Unity structuré.

Il doit fournir :

- tous les scripts complets
- structure dossiers
- prefabs nécessaires
- scènes
- UI
- sauvegarde
- système niveaux
- systèmes physiques
- outils de création de niveaux

Il doit éviter de coder les 100 niveaux directement dans les scripts.

Les niveaux doivent être pilotés par données.

---

# 89. Contraintes techniques pour Claude

Unity 2D.

Code C#.

Architecture claire.

Chaque fichier doit être complet.

Pas de pseudo-code.

Pas de méthodes volontairement incomplètes.

Pas de TODO essentiel.

Le projet doit compiler.

Les références doivent être configurables dans l'Inspector.

---

# 90. Exigence importante : LevelData

Créer une classe ou ScriptableObject :

LevelData

Exemple de champs :

int levelId  
int worldId  
int levelNumber  
float simulationTimeLimit  
GameObject levelPrefab  
List<ItemDefinition> availableItems  
List<StarObjective> objectives  

---

# 91. Objectifs extensibles

Créer une interface ou classe abstraite pour les objectifs.

Exemples :

CompleteLevelObjective  
MaxObjectsObjective  
CollectCrystalObjective  
TimeObjective  
BounceCountObjective  

Cela permet d'ajouter facilement de nouveaux objectifs.

---

# 92. Interface mobile

Tous les boutons doivent être assez grands.

Minimum conseillé :

80–100 pixels sur résolution référence.

Ne jamais nécessiter un clic précis.

---

# 93. Performance

Objectif :

60 FPS.

Éviter :

particules excessives

scripts Update inutiles

allocations répétées

---

# 94. Sauvegarde sécurisée minimale

Créer un SaveData.

Exemple :

highestUnlockedLevel  
starsPerLevel  
crystalsPerLevel  
selectedSkin  
settings

Sauvegarder automatiquement après victoire.

---

# 95. Localisation

Tous les textes doivent passer par un système de localisation.

Langues initiales possibles :

Français

Anglais

Prévoir l'ajout d'autres langues.

---

# 96. Noms UI de base

JOUER

NIVEAUX

PARAMÈTRES

GO

REJOUER

NIVEAU SUIVANT

CONTINUER

QUITTER

INDICE

---

# 97. Sensation recherchée

Le joueur doit souvent penser :

"Ahhhh, évidemment !"

puis :

"Je veux essayer le suivant."

Le jeu doit favoriser cette boucle.

---

# 98. Résumé du gameplay

Observer.

Comprendre.

Placer.

Tester.

Échouer rapidement.

Modifier.

Réussir.

Optimiser.

Continuer.

---

# 99. Objectif produit

TRYKLI doit pouvoir fonctionner comme un jeu mobile complet avec :

100 niveaux

300 étoiles

progression claire

mécaniques progressivement introduites

sessions courtes

bonne rejouabilité

architecture extensible

---

# 100. Prompt recommandé à donner à Claude

Tu es un développeur senior Unity spécialisé en jeux mobiles 2D.

Je veux que tu développes le jeu décrit dans le Game Design Document TRYKLI fourni.

Contraintes principales :

- Unity 2D
- C#
- iOS et Android
- orientation portrait
- projet propre et modulaire
- architecture data-driven
- aucun pseudo-code
- tous les scripts doivent être complets
- aucun TODO bloquant
- le projet doit pouvoir compiler
- les niveaux doivent être configurables sans modifier le code
- utiliser ScriptableObjects et Prefabs
- créer un système de Level Editor aussi simple que possible
- sauvegarde locale
- système de 3 étoiles
- 10 mondes
- support de 100 niveaux
- menus complets
- écran de victoire
- écran d'échec
- écran paramètres
- écran sélection mondes
- écran sélection niveaux
- système placement
- simulation physique
- restart instantané

Commence par produire :

1. l'architecture complète du projet
2. la liste de tous les fichiers
3. les scènes nécessaires
4. les prefabs nécessaires
5. les ScriptableObjects
6. les systèmes principaux
7. ensuite tous les scripts complets

Ne saute aucune dépendance nécessaire au fonctionnement du jeu.

Si une partie nécessite une configuration manuelle dans Unity, donne des instructions précises étape par étape.

Le jeu doit être conçu pour permettre l'ajout rapide de nouveaux niveaux sans modifier le code.

---

# 101. Améliorations possibles après la version 1

Une fois les 100 niveaux terminés :

Daily Challenge

Niveaux communautaires

Level Editor joueur

Ghost replay

Classements

Nouveaux mondes

Événements saisonniers

Skins

Achievements

---

# Conclusion

TRYKLI est conçu autour d'une mécanique simple :

**préparer une réaction en chaîne puis regarder Trykli tenter de survivre à ton plan.**

La profondeur vient de la combinaison progressive de mécanismes physiques simples.

La structure en 10 mondes de 10 niveaux permet d'introduire les mécaniques progressivement et d'obtenir une courbe de difficulté contrôlée tout en conservant suffisamment de variété pour maintenir l'intérêt pendant 100 niveaux.
