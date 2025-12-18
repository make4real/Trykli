// Serveur léger pour Trykli : sert les fichiers statiques et relaie les états de jeu.
const http = require('http');
const fs = require('fs');
const path = require('path');
const WebSocket = require('ws');
const CONFIG = require('./public/config/gameConfig.js');

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

// État minimal partagé
const world = {
  mapId: 'europe',
  players: {},
  points: CONFIG.MAPS['europe'].points.map((p) => ({ ...p, built: p.type !== 'resource' })),
  routes: [],
  movements: []
};

function broadcast(payload) {
  const msg = JSON.stringify(payload);
  wss.clients.forEach((c) => c.readyState === WebSocket.OPEN && c.send(msg));
}

function computeConnected(playerId) {
  const capitalId = world.points.find((p) => p.owner === playerId && p.type === 'capital')?.id;
  const visited = new Set();
  const queue = capitalId ? [capitalId] : [];
  const adjacency = {};
  world.routes.forEach((r) => {
    if (r.owner === playerId && r.active) {
      (adjacency[r.from] = adjacency[r.from] || []).push(r.to);
      (adjacency[r.to] = adjacency[r.to] || []).push(r.from);
    }
  });
  while (queue.length) {
    const n = queue.shift(); visited.add(n);
    (adjacency[n] || []).forEach((d) => { if (!visited.has(d)) queue.push(d); });
  }
  return visited;
}

function tick() {
  Object.values(world.players).forEach((player) => {
    player.population += CONFIG.POPULATION_GROWTH;
    player.army = Math.min(player.army + CONFIG.ARMY_TRAINING_FACTOR, player.population);
    const connected = computeConnected(player.id);
    world.points.forEach((p) => {
      if (p.owner === player.id && p.type === 'resource' && p.built && connected.has(p.id)) {
        const amount = CONFIG.BASE_PRODUCTION * CONFIG.RESOURCE_TERRAIN_YIELD[p.terrain][p.resourceType] * 0.15;
        player.resources[p.resourceType] += amount;
      }
    });
  });
  broadcast({ type: 'state', payload: serializeState() });
}

function serializeState() {
  return { mapId: world.mapId, points: world.points, routes: world.routes, movements: world.movements, players: world.players };
}

function buildRoute(playerId, fromId, toId) {
  if (fromId === toId) return;
  const player = world.players[playerId];
  const from = world.points.find((p) => p.id === fromId);
  const to = world.points.find((p) => p.id === toId);
  if (!from || !to || (!from.owner && !to.owner)) return;
  const dist = Math.hypot(from.position.x - to.position.x, from.position.y - to.position.y) * 1000;
  const cost = Math.round(dist * 0.35);
  if (player.resources.wood < cost) return;
  player.resources.wood -= cost;
  player.resources.stone -= Math.min(player.resources.stone, Math.round(cost * 0.5));
  world.routes.push({ id: `r_${Date.now()}`, from: fromId, to: toId, owner: playerId, active: true, percentage: 0 });
}

wss.on('connection', (ws) => {
  const playerId = `p_${Date.now()}_${Math.floor(Math.random() * 1000)}`;
  world.players[playerId] = {
    id: playerId,
    name: `Joueur ${Object.keys(world.players).length}`,
    color: '#5ad1ff',
    resources: { wood: 90, metal: 70, stone: 70, oil: 50 },
    population: 30,
    army: 15
  };
  ws.send(JSON.stringify({ type: 'welcome', playerId }));
  ws.send(JSON.stringify({ type: 'state', payload: serializeState() }));

  ws.on('message', (data) => {
    try {
      const msg = JSON.parse(data.toString());
      if (msg.type === 'buildRoute') buildRoute(playerId, msg.fromId, msg.toId);
      if (msg.type === 'placeResource') {
        const point = world.points.find((p) => p.id === msg.pointId && p.owner === playerId);
        if (point && point.type === 'resource') point.built = true;
      }
      if (msg.type === 'sendArmy') {
        // Simulation minimale : consommer armée
        const route = world.routes.find((r) => r.id === msg.routeId && r.owner === playerId);
        if (route) world.players[playerId].army = Math.max(0, world.players[playerId].army - 2);
      }
      broadcast({ type: 'state', payload: serializeState() });
    } catch (e) {
      console.error('WS message error', e);
    }
  });
});

setInterval(tick, 1000);

server.listen(PORT, () => console.log(`Trykli serveur sur http://localhost:${PORT}`));
