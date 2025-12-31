# Couple New Year Games

Site web statique (HTML/CSS/JS) prêt pour GitHub Pages ou Netlify. Aucun backend, aucune dépendance externe, tout fonctionne hors ligne.

## Lancement local
Deux options simples :

1. **Ouvrir directement** `index.html` dans votre navigateur.
2. **Lancer un petit serveur local** (recommandé) :
   ```bash
   python -m http.server 8080
   ```
   Puis ouvrir http://localhost:8080.

## Déploiement Netlify (drag & drop)
1. Allez sur Netlify.
2. Glissez-déposez le dossier racine du projet.
3. Aucun build requis, le site est prêt.

## Déploiement GitHub Pages
1. Poussez le repo sur GitHub (branche `main`).
2. Dans **Settings → Pages**, sélectionnez :
   - Source : `main`
   - Dossier : `/root` (racine)
3. Enregistrez, GitHub Pages publie automatiquement.

## Structure
- `/index.html` : page unique (SPA)
- `/assets/style.css` : styles
- `/assets/app.js` : logique interactive
- `/assets/data.js` : toutes les cartes/questions/jeux
- `/favicon.svg` : favicon optionnelle
