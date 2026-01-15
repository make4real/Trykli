# Trykli — Jeux de couple

Site web statique 100% HTML/CSS/JS. Tout fonctionne hors ligne, sans dépendance, compatible Netlify et GitHub Pages.

## Lancer localement
- Ouvrir `index.html` directement dans un navigateur.
- Ou lancer un petit serveur local :
  ```bash
  python -m http.server 8080
  ```
  Puis ouvrir http://localhost:8080.

## Déploiement Netlify (auto)
1. Glisser-déposer le dossier racine du projet dans Netlify.
2. Le fichier `_redirects` est déjà fourni pour la SPA.
3. Associez le domaine **trykli.com** dans la configuration Netlify si besoin.

## Déploiement GitHub Pages
1. Poussez le repo sur GitHub (branche `main`).
2. Dans **Settings → Pages**, sélectionnez :
   - Source : `main`
   - Dossier : `/root`
3. Le site est publié automatiquement.

## Structure
- `/index.html` : SPA principale
- `/assets/style.css` : styles
- `/assets/app.js` : logique
- `/assets/data.js` : toutes les cartes/questions/jeux
- `/_redirects` : redirection SPA
- `/favicon.svg` : icône
