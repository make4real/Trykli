# TRYKLI — Level Bible — 100 niveaux

Ce document complète le Game Design Document principal. Il sert de base de production pour construire les 100 niveaux dans Unity.

## Règles communes de production

- Chaque niveau doit être testable et reproductible.
- La solution ne doit jamais exiger une précision au pixel.
- Un joueur doit comprendre la cause de son échec.
- Les niveaux boss sont les niveaux 10, 20, 30… 100.
- Les objectifs 2 et 3 étoiles sont facultatifs : une étoile suffit pour progresser.
- Les cristaux servent d'objectifs de maîtrise et de collectibles.

## Échelle de difficulté

1–2 = initiation, 3–4 = facile/intermédiaire, 5–6 = intermédiaire, 7–8 = difficile, 9 = expert, 10 = final.


# Monde 1 — Premiers rebonds


## Niveau 001 — Tutoriel du ressort

**Difficulté :** 1/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Faire tomber Trykli sur un ressort placé dans l'unique zone autorisée pour atteindre directement la sortie.  

**Étoiles :**  

- ★ Terminer le niveau

- ★★ Utiliser 1 seul objet

- ★★★ Terminer en moins de 5 s

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 002 — Choisir la position

**Difficulté :** 1/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Trois emplacements sont proposés. Un seul donne la portée correcte pour franchir le vide.  

**Étoiles :**  

- ★ Terminer

- ★★ Placer le ressort du premier coup

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 003 — Premier angle

**Difficulté :** 1/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Le ressort doit être orienté vers une plateforme supérieure.  

**Étoiles :**  

- ★ Terminer

- ★★ Angle correct en 2 essais maximum

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 004 — Rampe et ressort

**Difficulté :** 1/10  

**Objets fournis :** Rampe x1, Ressort x1  

**Gameplay / solution logique attendue :** Utiliser la rampe pour donner de la vitesse avant le ressort.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les 2 objets

- ★★★ Récupérer le cristal haut

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 005 — Double rebond

**Difficulté :** 2/10  

**Objets fournis :** Ressort x2  

**Gameplay / solution logique attendue :** Deux ressorts doivent créer une chaîne de rebonds.  

**Étoiles :**  

- ★ Terminer

- ★★ Faire exactement 2 rebonds

- ★★★ Terminer en moins de 7 s

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 006 — Premier danger

**Difficulté :** 2/10  

**Objets fournis :** Rampe x1, Ressort x1  

**Gameplay / solution logique attendue :** Franchir une zone de pics en contrôlant la trajectoire.  

**Étoiles :**  

- ★ Terminer

- ★★ Ne toucher aucun mur latéral

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 007 — Le grand trou

**Difficulté :** 2/10  

**Objets fournis :** Ressort x2, Rampe x1  

**Gameplay / solution logique attendue :** Combiner vitesse et angle pour traverser un grand vide.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser maximum 2 objets

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 008 — Le détour

**Difficulté :** 2/10  

**Objets fournis :** Rampe x2, Ressort x1  

**Gameplay / solution logique attendue :** La sortie est visible mais une trajectoire directe échoue. Il faut passer par la plateforme gauche.  

**Étoiles :**  

- ★ Terminer

- ★★ Passer par la plateforme gauche

- ★★★ Récupérer les 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 009 — Deux solutions

**Difficulté :** 2/10  

**Objets fournis :** Ressort x2, Rampe x1  

**Gameplay / solution logique attendue :** Deux trajectoires valides : courte et risquée ou longue et sûre.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser maximum 2 objets

- ★★★ Prendre le chemin haut

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 010 — Boss rebonds

**Difficulté :** 2/10  

**Objets fournis :** Ressort x2, Rampe x2  

**Gameplay / solution logique attendue :** Puzzle combinant vide, hauteur et obstacle central.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser maximum 3 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 2 — Le vent


## Niveau 011 — Premier ventilateur

**Difficulté :** 2/10  

**Objets fournis :** Ventilateur x1  

**Gameplay / solution logique attendue :** Pousser Trykli horizontalement jusqu'à la sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser 1 ventilateur

- ★★★ Terminer en moins de 6 s

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 012 — Vent contraire

**Difficulté :** 2/10  

**Objets fournis :** Ventilateur x1, Rampe x1  

**Gameplay / solution logique attendue :** Modifier la trajectoire pour résister à une zone de vent fixe.  

**Étoiles :**  

- ★ Terminer

- ★★ Ne pas toucher le mur gauche

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 013 — Ascension

**Difficulté :** 2/10  

**Objets fournis :** Ventilateur x1  

**Gameplay / solution logique attendue :** Utiliser un flux vertical pour soulever Trykli.  

**Étoiles :**  

- ★ Terminer

- ★★ Un seul placement

- ★★★ Récupérer le cristal supérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 014 — Souffle et rebond

**Difficulté :** 2/10  

**Objets fournis :** Ventilateur x1, Ressort x1  

**Gameplay / solution logique attendue :** Le ressort lance Trykli dans un courant d'air latéral.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les 2 objets

- ★★★ Faire exactement 1 rebond

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 015 — Double courant

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2  

**Gameplay / solution logique attendue :** Créer une trajectoire en L avec deux ventilateurs.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les 2 ventilateurs

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 016 — Passage étroit

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2, Rampe x1  

**Gameplay / solution logique attendue :** Faire passer Trykli entre deux rangées de pics.  

**Étoiles :**  

- ★ Terminer

- ★★ Ne toucher aucun mur

- ★★★ Récupérer le cristal central

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 017 — Courbe aérienne

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2, Ressort x1  

**Gameplay / solution logique attendue :** Enchaîner propulsion verticale et poussée latérale.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 018 — Cristaux du ciel

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2, Rampe x1  

**Gameplay / solution logique attendue :** Faire une grande trajectoire aérienne passant par trois cristaux.  

**Étoiles :**  

- ★ Terminer

- ★★ Récupérer 2 cristaux

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 019 — Mauvaise orientation

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2, Ressort x1  

**Gameplay / solution logique attendue :** Plusieurs orientations semblent possibles, une seule maintient Trykli hors des pics.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact avec un obstacle

- ★★★ Moins de 3 essais

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 020 — Boss du vent

**Difficulté :** 3/10  

**Objets fournis :** Ventilateur x2, Ressort x1, Rampe x1  

**Gameplay / solution logique attendue :** Traversée complète avec montée, dérive et atterrissage précis.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 3 — Portails


## Niveau 021 — Premier portail

**Difficulté :** 3/10  

**Objets fournis :** Portail A/B  

**Gameplay / solution logique attendue :** Entrer dans le portail A et ressortir près de la sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser uniquement les portails

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 022 — Conserver l'élan

**Difficulté :** 3/10  

**Objets fournis :** Ressort x1, Portail A/B  

**Gameplay / solution logique attendue :** Comprendre que Trykli conserve sa vitesse à la sortie du portail.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 seul rebond

- ★★★ Récupérer le cristal après le portail

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 023 — Sortie verticale

**Difficulté :** 3/10  

**Objets fournis :** Portail A/B, Rampe x1  

**Gameplay / solution logique attendue :** Orienter la sortie du portail vers le haut pour atteindre une plateforme.  

**Étoiles :**  

- ★ Terminer

- ★★ Pas de contact avec le sol après téléportation

- ★★★ Cristal supérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 024 — Saut dimensionnel

**Difficulté :** 3/10  

**Objets fournis :** Ressort x1, Portail A/B  

**Gameplay / solution logique attendue :** Le ressort doit envoyer Trykli dans le portail avec le bon angle.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Terminer en moins de 7 s

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 025 — Deux paires

**Difficulté :** 4/10  

**Objets fournis :** Portail A/B, Portail C/D  

**Gameplay / solution logique attendue :** Choisir l'ordre correct des deux téléportations.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les 2 paires

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 026 — Portail dangereux

**Difficulté :** 4/10  

**Objets fournis :** Portail A/B, Rampe x1  

**Gameplay / solution logique attendue :** Éviter des pics placés juste après une mauvaise sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact avec les murs

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 027 — Portail dans le vent

**Difficulté :** 4/10  

**Objets fournis :** Portail A/B, Ventilateur x1  

**Gameplay / solution logique attendue :** Le courant d'air doit corriger la trajectoire après téléportation.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Cristal en plein vol

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 028 — Orientation libre

**Difficulté :** 4/10  

**Objets fournis :** Portail A/B, Ressort x1  

**Gameplay / solution logique attendue :** L'orientation du portail de sortie est la clé du niveau.  

**Étoiles :**  

- ★ Terminer

- ★★ Trouver l'angle en 3 essais maximum

- ★★★ Cristal caché

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 029 — Fausse route

**Difficulté :** 4/10  

**Objets fournis :** Portail A/B, Portail C/D, Rampe x1  

**Gameplay / solution logique attendue :** Une paire mène à une impasse. Il faut utiliser les portails dans un ordre précis.  

**Étoiles :**  

- ★ Terminer

- ★★ Ne jamais entrer dans le faux portail

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 030 — Boss dimensionnel

**Difficulté :** 4/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Portail A/B  

**Gameplay / solution logique attendue :** Réaction complète : ressort → portail → courant d'air → sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 4 — Magnétisme


## Niveau 031 — Premier aimant

**Difficulté :** 4/10  

**Objets fournis :** Aimant x1  

**Gameplay / solution logique attendue :** Attirer Trykli vers la plateforme de sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Un seul aimant

- ★★★ Cristal proche de l'aimant

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 032 — Courbe magnétique

**Difficulté :** 4/10  

**Objets fournis :** Aimant x1, Rampe x1  

**Gameplay / solution logique attendue :** Utiliser l'aimant pour courber une trajectoire initialement rectiligne.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Cristal extérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 033 — Au-dessus du vide

**Difficulté :** 4/10  

**Objets fournis :** Aimant x1  

**Gameplay / solution logique attendue :** Maintenir Trykli assez longtemps au-dessus d'un vide pour atteindre la plateforme opposée.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact avec le fond

- ★★★ Cristal central

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 034 — Rebond attiré

**Difficulté :** 4/10  

**Objets fournis :** Aimant x1, Ressort x1  

**Gameplay / solution logique attendue :** Combiner projection et attraction latérale.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 rebond

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 035 — Double aimant

**Difficulté :** 5/10  

**Objets fournis :** Aimant x2  

**Gameplay / solution logique attendue :** Créer une trajectoire en S entre deux champs magnétiques.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les deux aimants

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 036 — Aimant activable

**Difficulté :** 5/10  

**Objets fournis :** Aimant x1, Bouton fixe  

**Gameplay / solution logique attendue :** Trykli active un bouton qui déclenche ensuite l'aimant.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer le bouton

- ★★★ Cristal après activation

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 037 — Aimant et porte

**Difficulté :** 5/10  

**Objets fournis :** Aimant x1, Rampe x1  

**Gameplay / solution logique attendue :** L'aimant doit amener Trykli sur un bouton avant l'ouverture de la porte.  

**Étoiles :**  

- ★ Terminer

- ★★ Ouvrir la porte

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 038 — Orbite de cristaux

**Difficulté :** 5/10  

**Objets fournis :** Aimant x1, Ressort x1  

**Gameplay / solution logique attendue :** Créer une trajectoire circulaire partielle pour ramasser plusieurs cristaux.  

**Étoiles :**  

- ★ Terminer

- ★★ 2 cristaux

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 039 — Courbe précise

**Difficulté :** 5/10  

**Objets fournis :** Aimant x2, Ventilateur x1  

**Gameplay / solution logique attendue :** Combiner forces pour passer dans un corridor étroit.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun choc

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 040 — Boss magnétique

**Difficulté :** 5/10  

**Objets fournis :** Aimant x2, Ressort x1, Ventilateur x1  

**Gameplay / solution logique attendue :** Traverser trois zones magnétiques avec une sortie élevée.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 5 — Réactions en chaîne


## Niveau 041 — Premier bumper

**Difficulté :** 5/10  

**Objets fournis :** Bumper x1  

**Gameplay / solution logique attendue :** Utiliser le rebond puissant du bumper pour atteindre la sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Un seul bumper

- ★★★ Cristal supérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 042 — Ping-pong

**Difficulté :** 5/10  

**Objets fournis :** Bumper x2  

**Gameplay / solution logique attendue :** Faire rebondir Trykli entre deux bumpers.  

**Étoiles :**  

- ★ Terminer

- ★★ Exactement 2 contacts bumper

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 043 — Première bombe

**Difficulté :** 5/10  

**Objets fournis :** Bombe x1  

**Gameplay / solution logique attendue :** L'explosion propulse Trykli au-dessus d'un mur.  

**Étoiles :**  

- ★ Terminer

- ★★ Une seule explosion

- ★★★ Cristal en hauteur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 044 — Explosion orientée

**Difficulté :** 5/10  

**Objets fournis :** Bombe x1, Rampe x1  

**Gameplay / solution logique attendue :** Positionner la bombe pour produire la bonne direction de poussée.  

**Étoiles :**  

- ★ Terminer

- ★★ Pas de rebond inutile

- ★★★ Récupérer le cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 045 — Bombe et ressort

**Difficulté :** 5/10  

**Objets fournis :** Bombe x1, Ressort x1  

**Gameplay / solution logique attendue :** Le ressort place Trykli dans la zone d'explosion.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser les 2 objets

- ★★★ Terminer en moins de 8 s

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 046 — Bumper portal

**Difficulté :** 5/10  

**Objets fournis :** Bumper x1, Portail A/B  

**Gameplay / solution logique attendue :** Utiliser un bumper pour atteindre un portail placé hors de portée.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 bumper + 1 portail

- ★★★ Cristal après téléportation

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 047 — Souffle explosif

**Difficulté :** 6/10  

**Objets fournis :** Bombe x1, Ventilateur x1  

**Gameplay / solution logique attendue :** Le ventilateur corrige la trajectoire après explosion.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Récupérer 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 048 — Déclencheur

**Difficulté :** 6/10  

**Objets fournis :** Bombe x1, Bumper x1  

**Gameplay / solution logique attendue :** Une réaction pousse Trykli sur un bouton qui ouvre la suite.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer le bouton

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 049 — Trois chemins

**Difficulté :** 6/10  

**Objets fournis :** Bombe x1, Bumper x1, Rampe x1  

**Gameplay / solution logique attendue :** Trois routes sont possibles, une seule permet les trois étoiles.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Récupérer 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 050 — Boss réaction

**Difficulté :** 6/10  

**Objets fournis :** Bombe x1, Bumper x2, Portail A/B  

**Gameplay / solution logique attendue :** Grande chaîne : explosion → bumper → portail → sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 4 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 6 — Machines


## Niveau 051 — Première plateforme

**Difficulté :** 5/10  

**Objets fournis :** Rampe x1  

**Gameplay / solution logique attendue :** Atterrir sur une plateforme mobile et se laisser transporter.  

**Étoiles :**  

- ★ Terminer

- ★★ Rester sur la plateforme jusqu'au bout

- ★★★ Cristal sur la plateforme

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 052 — Ascenseur

**Difficulté :** 5/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Rebondir sur une plateforme verticale au bon moment.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 seul ressort

- ★★★ Cristal au sommet

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 053 — Plateforme rotative

**Difficulté :** 6/10  

**Objets fournis :** Rampe x1  

**Gameplay / solution logique attendue :** Utiliser l'inclinaison momentanée de la plateforme tournante.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact avec le danger

- ★★★ Cristal extérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 054 — Machine à rebond

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1, Rampe x1  

**Gameplay / solution logique attendue :** Synchroniser un rebond avec une plateforme mobile.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Moins de 3 essais

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 055 — Deux plateformes

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Passer d'une plateforme mobile à l'autre.  

**Étoiles :**  

- ★ Terminer

- ★★ Ne pas toucher le sol

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 056 — Transport aérien

**Difficulté :** 6/10  

**Objets fournis :** Ventilateur x1  

**Gameplay / solution logique attendue :** Utiliser le ventilateur pour rester sur une plateforme mobile.  

**Étoiles :**  

- ★ Terminer

- ★★ Un seul ventilateur

- ★★★ Cristal haut

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 057 — Portail mobile

**Difficulté :** 6/10  

**Objets fournis :** Portail A/B, Rampe x1  

**Gameplay / solution logique attendue :** Atteindre un portail positionné sur une plateforme mobile.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Cristal après portail

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 058 — Activation mécanique

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Toucher un bouton pour démarrer la plateforme menant à la sortie.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer le bouton

- ★★★ 1 cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 059 — Timing indirect

**Difficulté :** 6/10  

**Objets fournis :** Ventilateur x1, Ressort x1  

**Gameplay / solution logique attendue :** Le placement doit produire un délai naturel avant l'arrivée de la plateforme.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Terminer sans attendre bloqué

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 060 — Boss machines

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Portail A/B  

**Gameplay / solution logique attendue :** Chaîne avec ascenseur, plateforme rotative et portail.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 7 — Systèmes


## Niveau 061 — Ouvre la porte

**Difficulté :** 6/10  

**Objets fournis :** Rampe x1  

**Gameplay / solution logique attendue :** Trykli doit passer sur un bouton avant d'atteindre la porte.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer 1 bouton

- ★★★ Cristal derrière porte

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 062 — Bouton détour

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** La trajectoire directe échoue ; il faut d'abord rebondir vers le bouton.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer le bouton

- ★★★ Maximum 1 ressort

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 063 — Premier laser

**Difficulté :** 6/10  

**Objets fournis :** Rampe x1  

**Gameplay / solution logique attendue :** Contourner un laser permanent.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact laser

- ★★★ Cristal proche du laser

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 064 — Laser intermittent

**Difficulté :** 6/10  

**Objets fournis :** Ressort x1  

**Gameplay / solution logique attendue :** Le trajet doit naturellement faire passer Trykli pendant l'extinction du laser.  

**Étoiles :**  

- ★ Terminer

- ★★ Passer sans contact

- ★★★ Moins de 3 essais

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 065 — Couper le laser

**Difficulté :** 7/10  

**Objets fournis :** Ressort x1, Rampe x1  

**Gameplay / solution logique attendue :** Activer un bouton qui désactive le laser.  

**Étoiles :**  

- ★ Terminer

- ★★ Désactiver le laser

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 066 — Porte dimensionnelle

**Difficulté :** 7/10  

**Objets fournis :** Portail A/B  

**Gameplay / solution logique attendue :** Le portail permet d'atteindre le bouton derrière une porte.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser le portail

- ★★★ Cristal secret

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 067 — Double commande

**Difficulté :** 7/10  

**Objets fournis :** Ressort x1, Ventilateur x1  

**Gameplay / solution logique attendue :** Activer deux boutons dans le bon ordre.  

**Étoiles :**  

- ★ Terminer

- ★★ Activer les 2 boutons

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 068 — Ordre logique

**Difficulté :** 7/10  

**Objets fournis :** Portail A/B, Rampe x1  

**Gameplay / solution logique attendue :** Trois portes reliées à deux boutons imposent un ordre précis.  

**Étoiles :**  

- ★ Terminer

- ★★ Bon ordre du premier coup

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 069 — Petit réseau

**Difficulté :** 7/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Aimant x1  

**Gameplay / solution logique attendue :** Plusieurs mécanismes doivent être activés en cascade.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 070 — Boss systèmes

**Difficulté :** 7/10  

**Objets fournis :** Portail A/B, Aimant x1, Ressort x1  

**Gameplay / solution logique attendue :** Boutons, portes et lasers combinés dans un puzzle logique complet.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact laser

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 8 — Gravité


## Niveau 071 — Premier switch

**Difficulté :** 6/10  

**Objets fournis :** Gravity Switch x1  

**Gameplay / solution logique attendue :** Inverser la gravité pour atteindre une sortie au plafond.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 inversion

- ★★★ Cristal plafond

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 072 — Plafond

**Difficulté :** 6/10  

**Objets fournis :** Gravity Switch x1, Rampe x1  

**Gameplay / solution logique attendue :** Parcourir une section entière tête en bas.  

**Étoiles :**  

- ★ Terminer

- ★★ Rester au plafond

- ★★★ Cristal supérieur

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 073 — Rebond inversé

**Difficulté :** 7/10  

**Objets fournis :** Gravity Switch x1, Ressort x1  

**Gameplay / solution logique attendue :** Le ressort est utilisé après inversion de gravité.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 rebond inversé

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 074 — Portail inversé

**Difficulté :** 7/10  

**Objets fournis :** Gravity Switch x1, Portail A/B  

**Gameplay / solution logique attendue :** Sortir d'un portail alors que la gravité est inversée.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser portail et switch

- ★★★ Cristal dimensionnel

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 075 — Deux inversions

**Difficulté :** 7/10  

**Objets fournis :** Gravity Switch x2  

**Gameplay / solution logique attendue :** Alterner plafond et sol.  

**Étoiles :**  

- ★ Terminer

- ★★ Exactement 2 inversions

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 076 — Gravité laser

**Difficulté :** 7/10  

**Objets fournis :** Gravity Switch x1, Rampe x1  

**Gameplay / solution logique attendue :** Éviter un laser en passant au plafond.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact laser

- ★★★ Cristal

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 077 — Vent ascendant

**Difficulté :** 7/10  

**Objets fournis :** Gravity Switch x1, Ventilateur x1  

**Gameplay / solution logique attendue :** Le vent agit différemment lorsque Trykli est inversé.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Cristal central

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 078 — Sol-plafond-sol

**Difficulté :** 8/10  

**Objets fournis :** Gravity Switch x2, Ressort x1  

**Gameplay / solution logique attendue :** Parcours alternant trois orientations.  

**Étoiles :**  

- ★ Terminer

- ★★ 2 inversions

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 079 — Choix de gravité

**Difficulté :** 8/10  

**Objets fournis :** Gravity Switch x2, Portail A/B  

**Gameplay / solution logique attendue :** Plusieurs routes mais seule une orientation permet la sortie parfaite.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 switches

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 080 — Boss gravité

**Difficulté :** 8/10  

**Objets fournis :** Gravity Switch x2, Portail A/B, Ventilateur x1  

**Gameplay / solution logique attendue :** Long puzzle combinant gravité, portails et courant d'air.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 4 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 9 — Chaos contrôlé


## Niveau 081 — Trio classique

**Difficulté :** 7/10  

**Objets fournis :** Ressort x1, Aimant x1, Portail A/B  

**Gameplay / solution logique attendue :** Combiner trois mécaniques connues dans un ordre évident mais précis.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 082 — Explosion aérienne

**Difficulté :** 7/10  

**Objets fournis :** Ventilateur x1, Bombe x1  

**Gameplay / solution logique attendue :** Créer une explosion alors que Trykli est porté par le vent.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 explosion

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 083 — Portail mouvant

**Difficulté :** 7/10  

**Objets fournis :** Portail A/B, Ressort x1  

**Gameplay / solution logique attendue :** Atteindre un portail sur une plateforme mobile.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ Cristal mobile

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 084 — Gravité magnétique

**Difficulté :** 8/10  

**Objets fournis :** Gravity Switch x1, Aimant x1  

**Gameplay / solution logique attendue :** Aimant et gravité produisent une courbe complexe.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ 2 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 085 — Sécurité maximale

**Difficulté :** 8/10  

**Objets fournis :** Ressort x1, Rampe x1  

**Gameplay / solution logique attendue :** Passage entre lasers après activation de deux boutons.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact laser

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 086 — Explosion dimensionnelle

**Difficulté :** 8/10  

**Objets fournis :** Bombe x1, Portail A/B  

**Gameplay / solution logique attendue :** Une explosion doit projeter Trykli directement dans un portail.  

**Étoiles :**  

- ★ Terminer

- ★★ 1 explosion

- ★★★ Cristal sortie portail

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 087 — Usine à vent

**Difficulté :** 8/10  

**Objets fournis :** Ventilateur x2, Ressort x1  

**Gameplay / solution logique attendue :** Plateformes mobiles et deux courants d'air.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 088 — Trois routes

**Difficulté :** 8/10  

**Objets fournis :** Aimant x1, Portail A/B, Rampe x1  

**Gameplay / solution logique attendue :** Le niveau propose 3 routes valides ; la route haute est la plus efficace.  

**Étoiles :**  

- ★ Terminer

- ★★ Utiliser maximum 2 objets

- ★★★ Route haute + 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 089 — Multi-étapes

**Difficulté :** 8/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Aimant x1  

**Gameplay / solution logique attendue :** Puzzle en quatre phases avec bouton intermédiaire.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 090 — Mega puzzle

**Difficulté :** 9/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Portail A/B, Aimant x1  

**Gameplay / solution logique attendue :** Grand niveau synthèse avec plusieurs interactions et aucune nouvelle mécanique.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 4 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Monde 10 — Master Trykli


## Niveau 091 — Précision contrôlée

**Difficulté :** 8/10  

**Objets fournis :** Ressort x1, Aimant x1  

**Gameplay / solution logique attendue :** Placement précis mais tolérant ; le joueur doit exploiter deux zones d'effet.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 2 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 092 — Minimaliste

**Difficulté :** 8/10  

**Objets fournis :** Ressort x2, Ventilateur x1, Rampe x1  

**Gameplay / solution logique attendue :** Beaucoup d'objets fournis mais la meilleure solution n'en utilise que deux.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Maximum 2 objets

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 093 — Solutions multiples

**Difficulté :** 8/10  

**Objets fournis :** Portail A/B, Aimant x1, Bumper x1  

**Gameplay / solution logique attendue :** Au moins trois solutions intentionnelles avec risques différents.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 094 — Grande chaîne

**Difficulté :** 9/10  

**Objets fournis :** Bombe x1, Bumper x1, Portail A/B, Ventilateur x1  

**Gameplay / solution logique attendue :** Une longue réaction en chaîne spectaculaire doit fonctionner sans action du joueur.  

**Étoiles :**  

- ★ Terminer

- ★★ Chaîne complète sans arrêt

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 095 — Timing expert

**Difficulté :** 9/10  

**Objets fournis :** Ressort x1, Rampe x1, Ventilateur x1  

**Gameplay / solution logique attendue :** Synchronisation indirecte avec deux plateformes mobiles et un laser intermittent.  

**Étoiles :**  

- ★ Terminer

- ★★ Aucun contact laser

- ★★★ Moins de 4 essais

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 096 — Dimension inversée

**Difficulté :** 9/10  

**Objets fournis :** Portail A/B, Gravity Switch x1, Aimant x1  

**Gameplay / solution logique attendue :** Portail, gravité et laser combinés.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 097 — Machine complexe

**Difficulté :** 9/10  

**Objets fournis :** Ressort x1, Ventilateur x1, Bumper x1  

**Gameplay / solution logique attendue :** Suite de plateformes, boutons et mécanismes interdépendants.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 3 objets

- ★★★ Activer tous les boutons

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 098 — Presque libre

**Difficulté :** 9/10  

**Objets fournis :** Ressort x2, Ventilateur x2, Portail A/B, Aimant x1  

**Gameplay / solution logique attendue :** Grande aire avec plusieurs slots et plusieurs solutions fiables.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 4 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 099 — Ultimate Challenge

**Difficulté :** 10/10  

**Objets fournis :** Ressort x1, Bombe x1, Portail A/B, Gravity Switch x1, Aimant x1  

**Gameplay / solution logique attendue :** Avant-dernier niveau : chaque mécanique a un rôle clair et aucune solution brute.  

**Étoiles :**  

- ★ Terminer

- ★★ Maximum 5 objets

- ★★★ 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


## Niveau 100 — TRYKLI MASTER

**Difficulté :** 10/10  

**Objets fournis :** Ressort x1, Portail A/B, Ventilateur x1, Aimant x1, Bombe x1, Bumper x1, Gravity Switch x1  

**Gameplay / solution logique attendue :** Final spectaculaire en plusieurs étapes : lancement, téléportation, inversion, activation de bouton, explosion contrôlée et entrée dans un portail géant.  

**Étoiles :**  

- ★ Terminer le jeu

- ★★ Maximum 7 objets

- ★★★ Récupérer les 3 cristaux

**Règle d'équilibrage :** prévoir au moins une solution robuste avec une marge suffisante sur les angles/positions pour qu'un placement visuellement correct fonctionne de façon répétable.


# Consignes de construction pour Claude / Unity


Pour chaque niveau, créer un prefab ou une définition de niveau comportant :
- spawn de Trykli ;
- Goal ;
- géométrie fixe ;
- dangers ;
- mécanismes fixes ;
- PlacementZones / PlacementSlots ;
- inventaire d'objets ;
- cristaux ;
- données des 3 objectifs ;
- limites caméra ;
- limite de temps de simulation ;
- identifiant monde/niveau ;
- difficulté.

Ne pas coder la solution directement dans le gameplay. La solution doit émerger de la physique et du placement.

Chaque niveau doit être testé au minimum avec :
1. solution principale ;
2. restart immédiat ;
3. reset des objets ;
4. réussite 1 étoile ;
5. réussite 3 étoiles ;
6. chute hors niveau ;
7. immobilité ;
8. interaction avec tous les dangers présents.

Les niveaux 1 à 10 doivent servir de référence de qualité avant de produire les 90 suivants.
