# SoundKey Tasks — Static Offline Tracker

Projet reconstruit à zéro en **site statique ultra léger** :
- 0 framework
- 0 dépendance
- 0 build
- stockage local via `localStorage` (clé: `soundkey_tasks_v2`)

## Structure finale

```text
/
  index.html
  README.md
  netlify.toml
  /assets
    app.js
    app.css
```

## Fonctionnalités

- CRUD complet des tâches : création, édition, suppression, done/undone
- Changement de catégorie via select
- Changement de priorité P0/P1/P2 via select
- CRUD catégories : création, renommage, suppression, réordonnancement (↑ ↓)
- Suppression catégorie avec choix MOVE (vers Inbox) ou DELETE (supprimer les tâches)
- Filtres : priorité, statut, recherche texte
- Tri : priorité, statut, date de création
- Progression globale (% et done/total)
- Export JSON / Import JSON / Reset seed
- Autosave après chaque action

## Test local

1. Aller dans le repo:
   ```bash
   cd /workspace/Trykli
   ```
2. Lancer un serveur statique:
   ```bash
   python -m http.server 8000
   ```
3. Ouvrir:
   - `http://localhost:8000`

## Déploiement Netlify

- `publish = "."`
- aucune commande de build
- cache long pour `/assets/*`
- revalidation pour `/index.html`
