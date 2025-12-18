# Trykli

Prototype .io temps réel respectant le GDD fourni. Frontend HTML5 + Canvas, backend Node.js léger (WebSocket) avec repli automatique sur une simulation locale pour rester jouable sur Netlify.

## Fonctionnalités V0
- Trois cartes inspirées (Europe, Amériques, Asie) avec terrains et points (capitales, ressources, postes de défense).
- Ressources (bois, métal, pierre, pétrole) placées par le joueur et rendement dépendant du terrain.
- Routes point-à-point avec coût distance/terrain, animations, état actif/inactif, acheminement des ressources et de l'armée.
- Population et armée liées à la capitale ; armée se déplace uniquement via les routes, combats simplifiés et encerclement.
- Notifications (attaque, prise de capitale), bots basiques si peu de joueurs.

## Lancement local
```bash
npm install
npm start
```
Puis ouvrir http://localhost:3000. Le serveur sert les fichiers statiques et expose un WebSocket. En cas d'absence de serveur (ex. déploiement Netlify), le client bascule sur la simulation locale automatique.

## Déploiement Netlify
- Build command : `npm run build` (noop, assets déjà prêts)
- Publish directory : `public`
- Aucune variable d'environnement requise
- Le jeu reste jouable grâce au fallback simulation locale si aucun WebSocket n'est disponible.

## Utilisation en jeu
1. Choisissez une carte dans la barre supérieure.
2. Placez vos zones de ressources via le bouton « Placer ressource » puis cliquez sur un point ressource qui vous appartient.
3. Construisez des routes : clic sur un point possédé puis sur un autre point. Les routes affichent un point mobile et un pourcentage d'activité.
4. Envoyez 10% de votre armée sur une route sélectionnée via le bouton « Envoyer armée ».
5. Attaquez en créant une route vers un point ennemi (notification envoyée), surveillez l'encerclement de votre capitale.
6. Victoire : dernier pays en vie ou contrôle majoritaire de la carte.

## Structure
- `public/index.html` : canvas + UI minimale
- `public/js/main.js` : moteur de jeu, simulation locale, gestion réseau
- `public/config/gameConfig.js` : cartes, terrains, rendements
- `public/styles/styles.css` : styles
- `server.js` : serveur Node.js statique + WebSocket léger
- `netlify.toml` : configuration de publication
