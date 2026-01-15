// Serveur léger pour Trykli : sert les fichiers statiques et relaie les états de jeu
// en utilisant la même Simulation que le client (approche inspirée d'une boucle
// unique style openfront : serveur autoritaire, diffusions régulières).
const http = require('http');
const fs = require('fs');
const path = require('path');
const WebSocket = require('ws');
const CONFIG = require('./public/config/gameConfig.js');
const { Simulation } = require('./public/js/simulation.js');

const PORT = process.env.PORT || 3000;
const PUBLIC_DIR = path.join(__dirname, 'public');

const mime = {
  '.html': 'text/html',
  '.js': 'application/javascript',
  '.css': 'text/css',
  '.json': 'application/json',
  '.png': 'image/png',
  '.jpg': 'image/jpeg'
};

const server = http.createServer((req, res) => {
  const urlPath = req.url === '/' ? '/index.html' : req.url;
  const filePath = path.join(PUBLIC_DIR, urlPath.split('?')[0]);
  fs.readFile(filePath, (err, data) => {
    if (err) {
      res.writeHead(404);
      return res.end('Not found');
    }
    const type = mime[path.extname(filePath)] || 'text/plain';
    res.writeHead(200, { 'Content-Type': type });
    res.end(data);
  });
});

const wss = new WebSocket.Server({ server });
const simulation = new Simulation(CONFIG);
simulation.createState('europe');
simulation.startTick();

simulation.on('state', (state) => {
  broadcast({ type: 'state', payload: state });
});

const clientToPlayer = new Map();

function broadcast(payload) {
  const msg = JSON.stringify(payload);
  wss.clients.forEach((c) => c.readyState === WebSocket.OPEN && c.send(msg));
}

wss.on('connection', (ws) => {
  let playerId = null;

  const addPlayerIfNeeded = (mapId, name) => {
    if (!simulation.state || simulation.state.mapId !== mapId) simulation.createState(mapId);
    playerId = simulation.addPlayer(name || `Joueur ${Object.keys(simulation.state.players).length}`);
    clientToPlayer.set(ws, playerId);
    ws.send(JSON.stringify({ type: 'welcome', playerId }));
    ws.send(JSON.stringify({ type: 'state', payload: simulation.serializedState() }));
  };

  ws.on('message', (data) => {
    try {
      const msg = JSON.parse(data.toString());
      if (msg.type === 'join') {
        addPlayerIfNeeded(msg.mapId || 'europe', msg.name);
        return;
      }
      if (!playerId) return;
      if (msg.type === 'buildRoute') simulation.buildRoute(playerId, msg.fromId, msg.toId);
      if (msg.type === 'placeResource') simulation.placeResource(playerId, msg.pointId);
      if (msg.type === 'sendArmy') simulation.sendArmy(playerId, msg.routeId);
    } catch (e) {
      console.error('WS message error', e);
    }
  });

  ws.on('close', () => clientToPlayer.delete(ws));
});

server.listen(PORT, () => console.log(`Trykli serveur sur http://localhost:${PORT}`));
