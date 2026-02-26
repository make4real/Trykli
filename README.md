# SoundKey — Suivi d’avancement (statique)

Page ultra légère en **HTML/CSS/JS natifs** (aucune dépendance runtime).

## Objectif
Suivre l’avancement SoundKey V2 avec :
- checklist par section,
- progression globale (% + compteur),
- export/import JSON,
- reset au seed,
- sauvegarde automatique dans `localStorage` (`soundkey_tasks_v2`).

## Lancer localement
```bash
python -m http.server 8080
```
Puis ouvrir http://localhost:8080.

## Déploiement Netlify
Le repo inclut `netlify.toml` :
- **publish dir**: `.`
- **build command**: aucune (site statique direct)
- **cache headers**:
  - `/assets/*` -> cache long immuable
  - `/index.html` -> revalidation

## Structure
- `/index.html` : page principale
- `/assets/app.css` : styles
- `/assets/app.js` : logique + seed des tâches SoundKey V2
- `/netlify.toml` : config de déploiement et cache
