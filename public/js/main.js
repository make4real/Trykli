// Trykli V0 — jeu .io temps réel (simulation locale par défaut, WebSocket optionnel)
// Le code reste volontairement lisible et commenté pour faciliter l'évolution.

const CONFIG = window.TRYKLI_CONFIG;

const clamp = (v, min, max) => Math.max(min, Math.min(max, v));
const distance = (a, b, width = 1, height = 1) => {
  const dx = (a.x - b.x) * width;
  const dy = (a.y - b.y) * height;
  return Math.sqrt(dx * dx + dy * dy);
};
const randomPick = (arr) => arr[Math.floor(Math.random() * arr.length)];

class EventEmitter {
  constructor() { this.listeners = {}; }
  on(event, cb) { (this.listeners[event] = this.listeners[event] || []).push(cb); }
  emit(event, payload) { (this.listeners[event] || []).forEach((cb) => cb(payload)); }
}

/**
 * LocalHost simule un serveur temps réel : gestion des joueurs, routes,
 * production et combats. Le client bascule dessus si le WebSocket échoue.
 */
class LocalHost extends EventEmitter {
  constructor() {
    super();
    this.state = null;
    this.tickInterval = null;
    this.time = 0;
  }

  createState(mapId) {
    const map = CONFIG.MAPS[mapId];
    const points = map.points.map((p) => ({
      ...p,
      built: p.type !== "resource" ? true : false,
      connected: p.type === "capital",
      garrison: p.type === "capital" ? 14 : 6,
      encircled: false
    }));
    this.state = {
      mapId,
      map,
      availableCountries: map.defaultCountries.map((c) => ({ ...c })),
      points,
      players: {},
      routes: [],
      movements: [],
      notifications: []
    };
  }

  addPlayer(name, opts = {}) {
    const country = this.state.availableCountries.shift() || { id: `p${Date.now()}`, name: name || "Joueur", color: "#5ad1ff" };
    const playerId = country.id;
    const player = {
      id: playerId,
      name: name || country.name,
      color: country.color,
      population: 32,
      army: 18,
      resources: { wood: 80, metal: 60, stone: 60, oil: 40 },
      alive: true,
      bot: Boolean(opts.bot),
      capital: this.state.map.points.find((p) => p.owner === playerId && p.type === "capital")?.id,
      routes: []
    };
    // Appliquer propriété owner sur points
    this.state.points = this.state.points || this.state.map.points.map((p) => ({ ...p }));
    this.state.points.forEach((p) => {
      if (p.owner === playerId && p.type === "resource") {
        p.owner = playerId;
        p.built = false; // doit être placé par le joueur
      }
    });
    this.state.players[playerId] = player;
    this.emitState();
    return playerId;
  }

  emitState() {
    this.emit("message", { type: "state", payload: this.serializedState() });
  }

  serializedState() {
    return {
      ...this.state,
      // éviter référence circulaire
      map: { ...this.state.map, points: undefined },
      points: this.state.points,
      routes: this.state.routes,
      movements: this.state.movements,
      players: this.state.players,
      time: this.time
    };
  }

  startTick() {
    if (this.tickInterval) return;
    this.tickInterval = setInterval(() => this.tick(), 1000);
  }

  stop() { clearInterval(this.tickInterval); }

  tick() {
    this.time += 1;
    this.handleBots();
    this.handleProduction();
    this.progressMovements();
    this.handleEncirclement();
    this.handleVictory();
    this.emitState();
  }

  handleBots() {
    Object.values(this.state.players)
      .filter((p) => p.bot && p.alive)
      .forEach((bot) => {
        // Placer les ressources en priorité
        const ownResources = this.state.points.filter((pt) => pt.owner === bot.id && pt.type === "resource" && !pt.built);
        if (ownResources.length && Math.random() > 0.5) {
          this.placeResource(bot.id, randomPick(ownResources).id);
          return;
        }
        // Construire une route aléatoire régulièrement
        if (Math.random() > 0.7) {
          const owned = this.state.points.filter((p) => p.owner === bot.id);
          const targets = this.state.points.filter((p) => p.owner !== bot.id);
          if (owned.length && targets.length) {
            const from = randomPick(owned);
            const to = randomPick(targets);
            this.buildRoute(bot.id, from.id, to.id);
          }
        }
      });
  }

  handleProduction() {
    Object.values(this.state.players).forEach((player) => {
      if (!player.alive) return;
      player.population += CONFIG.POPULATION_GROWTH;
      player.army = Math.min(player.army + CONFIG.ARMY_TRAINING_FACTOR, player.population);

      const connected = this.computeConnectedPoints(player.id);
      this.state.routes.forEach((r) => {
        if (r.owner === player.id) {
          const isActive = connected.has(r.from) || connected.has(r.to);
          r.active = isActive;
          r.percentage = isActive ? Math.min(100, (r.percentage || 0) + 5) : 0;
        }
      });

      this.state.points.forEach((p) => {
        if (p.type === "resource" && p.owner === player.id && p.built) {
          p.connected = connected.has(p.id);
          if (p.connected) {
            const terrainYield = CONFIG.RESOURCE_TERRAIN_YIELD[p.terrain][p.resourceType];
            const produced = CONFIG.BASE_PRODUCTION * terrainYield * 0.15; // Vitesse réduite pour lisibilité
            player.resources[p.resourceType] += produced;
          }
        }
      });
    });
  }

  computeConnectedPoints(playerId) {
    const capitalId = this.state.players[playerId].capital;
    const visited = new Set();
    const queue = [capitalId];
    const adjacency = {};
    this.state.routes.forEach((r) => {
      if (r.owner === playerId && r.active) {
        adjacency[r.from] = adjacency[r.from] || [];
        adjacency[r.to] = adjacency[r.to] || [];
        adjacency[r.from].push(r.to);
        adjacency[r.to].push(r.from);
      }
    });
    while (queue.length) {
      const node = queue.shift();
      visited.add(node);
      (adjacency[node] || []).forEach((n) => {
        if (!visited.has(n)) queue.push(n);
      });
    }
    return visited;
  }

  buildRoute(playerId, fromId, toId) {
    if (fromId === toId) return { ok: false, message: "Sélectionnez deux points distincts" };
    const player = this.state.players[playerId];
    if (!player || !player.alive) return { ok: false, message: "Joueur inactif" };
    const from = this.state.points.find((p) => p.id === fromId);
    const to = this.state.points.find((p) => p.id === toId);
    if (!from || !to) return { ok: false, message: "Point introuvable" };
    if (from.owner !== playerId && to.owner !== playerId) return { ok: false, message: "Reliez depuis un point contrôlé" };

    const already = this.state.routes.find((r) => (r.from === fromId && r.to === toId) || (r.from === toId && r.to === fromId));
    if (already) return { ok: false, message: "Route déjà présente" };

    const dist = distance(from.position, to.position, this.state.map.size.width, this.state.map.size.height);
    const terrainFactor = (CONFIG.TERRAIN_ROUTE_COST[from.terrain] + CONFIG.TERRAIN_ROUTE_COST[to.terrain]) / 2;
    const cost = Math.round(dist * 0.35 * terrainFactor);

    const pay = (ratio) => {
      const wood = Math.ceil(cost * ratio * 1.1);
      const stone = Math.ceil(cost * ratio * 0.8);
      const metal = Math.ceil(cost * ratio * 0.9);
      const oil = Math.ceil(cost * ratio * 0.4);
      if (player.resources.wood < wood || player.resources.stone < stone || player.resources.metal < metal) {
        return false;
      }
      player.resources.wood -= wood;
      player.resources.stone -= stone;
      player.resources.metal -= metal;
      player.resources.oil = Math.max(0, player.resources.oil - oil);
      return true;
    };

    if (!pay(1)) return { ok: false, message: "Ressources insuffisantes" };

    const route = {
      id: `r_${Date.now()}_${Math.floor(Math.random() * 1000)}`,
      from: fromId,
      to: toId,
      owner: playerId,
      active: true,
      distance: dist,
      terrainFactor,
      percentage: 0,
      contested: false,
      armies: {}
    };
    player.routes.push(route.id);
    this.state.routes.push(route);

    // Notification pour l'ennemi si route offensive
    if (to.owner && to.owner !== playerId) {
      this.pushNotification(to.owner, `${player.name} construit une route vers ${to.label}`);
    }

    this.emitState();
    return { ok: true, route };
  }

  placeResource(playerId, pointId) {
    const point = this.state.points.find((p) => p.id === pointId);
    if (!point || point.owner !== playerId || point.type !== "resource") return { ok: false, message: "Point non valable" };
    if (point.built) return { ok: false, message: "Déjà placé" };
    const player = this.state.players[playerId];
    const cost = 12;
    if (player.resources.wood < cost || player.resources.stone < cost) return { ok: false, message: "Ressources insuffisantes" };
    player.resources.wood -= cost;
    player.resources.stone -= cost;
    point.built = true;
    this.emitState();
    return { ok: true };
  }

  sendArmy(playerId, routeId) {
    const route = this.state.routes.find((r) => r.id === routeId && r.owner === playerId);
    if (!route) return { ok: false, message: "Route introuvable" };
    const player = this.state.players[playerId];
    if (player.army < 1) return { ok: false, message: "Armée trop faible" };
    const amount = Math.max(1, Math.floor(player.army * 0.1));
    player.army -= amount;
    const movement = {
      id: `m_${Date.now()}`,
      routeId,
      owner: playerId,
      from: route.from,
      to: route.to,
      amount,
      progress: 0
    };
    this.state.movements.push(movement);
    this.emitState();
    return { ok: true };
  }

  progressMovements() {
    const finished = [];
    this.state.movements.forEach((m) => {
      m.progress += 0.25;
      const route = this.state.routes.find((r) => r.id === m.routeId);
      if (route) route.percentage = Math.min(100, (m.progress / 1) * 100);
      if (m.progress >= 1) finished.push(m);
    });

    finished.forEach((m) => {
      const target = this.state.points.find((p) => p.id === m.to);
      if (!target) return;
      if (!target.owner || target.owner === m.owner) {
        // Renfort ou capture de point neutre
        target.owner = m.owner;
        target.garrison = (target.garrison || 0) + m.amount;
      } else {
        // Combat simple
        const defenseBonus = target.terrain === "mountain" ? 1.4 : target.terrain === "forest" ? 1.1 : 1;
        const defense = (target.garrison || 0) * defenseBonus;
        const result = defense - m.amount;
        if (result <= 0) {
          // Capture
          const previousOwner = target.owner;
          target.owner = m.owner;
          target.garrison = Math.max(2, Math.abs(result));
          if (previousOwner) this.pushNotification(previousOwner, `${this.state.players[m.owner]?.name || m.owner} prend ${target.label}`);
          if (target.type === "capital" && this.state.players[previousOwner]) {
            this.state.players[previousOwner].alive = false;
            this.pushNotification(previousOwner, "Votre capitale est tombée !");
          }
        } else {
          target.garrison = Math.round(result);
        }
      }
    });

    this.state.movements = this.state.movements.filter((m) => !finished.includes(m));
  }

  handleEncirclement() {
    Object.values(this.state.players).forEach((player) => {
      if (!player.alive) return;
      const capital = this.state.points.find((p) => p.id === player.capital);
      if (!capital) return;
      const hostileRoutes = this.state.routes.filter((r) => {
        if (r.owner === player.id) return false;
        const from = this.state.points.find((p) => p.id === r.from);
        const to = this.state.points.find((p) => p.id === r.to);
        return (from && distance(capital.position, from.position, this.state.map.size.width, this.state.map.size.height) < 180) ||
               (to && distance(capital.position, to.position, this.state.map.size.width, this.state.map.size.height) < 180);
      });
      const ratio = clamp(hostileRoutes.length / 4, 0, 1);
      capital.encircled = ratio > 0;
      if (ratio > 0) {
        player.population = Math.max(0, player.population - ratio * 1.5);
        player.army = Math.max(0, player.army - ratio * 1.2);
      }
    });
  }

  handleVictory() {
    const alive = Object.values(this.state.players).filter((p) => p.alive);
    if (alive.length === 1) {
      this.pushNotification(alive[0].id, `${alive[0].name} contrôle la carte`);
    }
  }

  pushNotification(targetId, text) {
    this.state.notifications.push({ id: `n_${Date.now()}`, targetId, text });
  }
}

/**
 * NetworkClient tente une connexion WebSocket, sinon crée un LocalHost.
 */
class NetworkClient extends EventEmitter {
  constructor(onState) {
    super();
    this.socket = null;
    this.host = null;
    this.playerId = null;
    this.isLocal = false;
    this.onState = onState;
  }

  connect(mapId) {
    // Essayer WebSocket local, sinon fallback
    const wsUrl = `${location.origin.replace(/^http/, "ws")}`;
    try {
      const socket = new WebSocket(wsUrl);
      socket.onopen = () => {
        this.socket = socket;
        this.socket.send(JSON.stringify({ type: "join", mapId }));
      };
      socket.onerror = () => this.startLocal(mapId);
      socket.onclose = () => {
        if (!this.host) this.startLocal(mapId);
      };
      socket.onmessage = (evt) => {
        const msg = JSON.parse(evt.data);
        if (msg.type === "state") this.onState(msg.payload);
        if (msg.type === "welcome") this.playerId = msg.playerId;
      };
    } catch (e) {
      this.startLocal(mapId);
    }
  }

  startLocal(mapId) {
    this.isLocal = true;
    this.host = new LocalHost();
    this.host.createState(mapId);
    const me = this.host.addPlayer("Vous");
    this.playerId = me;
    // Ajouter bots simples
    const bots = ["Bot Nord", "Bot Est"].slice(0, 2);
    bots.forEach((name) => this.host.addPlayer(name, { bot: true }));
    this.host.on("message", (msg) => {
      if (msg.type === "state") this.onState(msg.payload);
    });
    this.host.startTick();
  }

  buildRoute(fromId, toId) {
    if (this.host) return this.host.buildRoute(this.playerId, fromId, toId);
    this.socket?.send(JSON.stringify({ type: "buildRoute", fromId, toId }));
  }

  placeResource(pointId) {
    if (this.host) return this.host.placeResource(this.playerId, pointId);
    this.socket?.send(JSON.stringify({ type: "placeResource", pointId }));
  }

  sendArmy(routeId) {
    if (this.host) return this.host.sendArmy(this.playerId, routeId);
    this.socket?.send(JSON.stringify({ type: "sendArmy", routeId }));
  }
}

/**
 * GameView gère le rendu Canvas.
 */
class GameView {
  constructor(canvas, ui) {
    this.canvas = canvas;
    this.ctx = canvas.getContext("2d");
    this.ui = ui;
    this.state = null;
    this.selected = null;
    this.hover = null;
  }

  setState(state) { this.state = state; }
  setSelection(sel) { this.selected = sel; }
  setHover(h) { this.hover = h; }

  render() {
    if (!this.state) return;
    const { ctx, canvas } = this;
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Fond quadrillé subtil
    ctx.strokeStyle = "rgba(255,255,255,0.03)";
    ctx.lineWidth = 1;
    for (let x = 0; x < canvas.width; x += 80) {
      ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, canvas.height); ctx.stroke();
    }
    for (let y = 0; y < canvas.height; y += 80) {
      ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(canvas.width, y); ctx.stroke();
    }

    // Routes
    this.state.routes.forEach((r) => this.drawRoute(r));

    // Points
    this.state.points.forEach((p) => this.drawPoint(p));

    // Sélection
    if (this.selected) {
      const p = this.state.points.find((pt) => pt.id === this.selected.id);
      if (p) {
        const pos = this.toCanvas(p.position);
        ctx.strokeStyle = "#5ad1ff";
        ctx.lineWidth = 3;
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, 18, 0, Math.PI * 2);
        ctx.stroke();
      }
    }
  }

  toCanvas(pos) {
    return { x: pos.x * this.canvas.width, y: pos.y * this.canvas.height };
  }

  drawRoute(route) {
    const { ctx } = this;
    const from = this.state.points.find((p) => p.id === route.from);
    const to = this.state.points.find((p) => p.id === route.to);
    if (!from || !to) return;
    const a = this.toCanvas(from.position);
    const b = this.toCanvas(to.position);
    const color = this.state.players[route.owner]?.color || "#888";
    ctx.lineWidth = 3;
    ctx.strokeStyle = route.active ? color : "#555";
    ctx.beginPath();
    ctx.moveTo(a.x, a.y);
    ctx.lineTo(b.x, b.y);
    ctx.stroke();

    // point mobile
    if (route.active) {
      const t = ((Date.now() / 500) % 1);
      const px = a.x + (b.x - a.x) * t;
      const py = a.y + (b.y - a.y) * t;
      ctx.fillStyle = color;
      ctx.beginPath();
      ctx.arc(px, py, 5, 0, Math.PI * 2);
      ctx.fill();
    }

    ctx.fillStyle = "#cde7ff";
    ctx.font = "12px Arial";
    ctx.fillText(`${Math.round(route.percentage || 0)}%`, (a.x + b.x) / 2, (a.y + b.y) / 2);
  }

  drawPoint(point) {
    const { ctx } = this;
    const pos = this.toCanvas(point.position);
    const ownerColor = point.owner ? this.state.players[point.owner]?.color || "#aaa" : "#777";
    const terrainColors = { plains: "#3f5c9e", forest: "#2f6a44", mountain: "#555a6f", desert: "#7a5b32" };

    // Terrain marker
    ctx.fillStyle = terrainColors[point.terrain] || "#444";
    ctx.beginPath();
    ctx.arc(pos.x, pos.y, 12, 0, Math.PI * 2);
    ctx.fill();

    // Point
    ctx.strokeStyle = ownerColor;
    ctx.lineWidth = 3;
    ctx.beginPath();
    ctx.arc(pos.x, pos.y, 9, 0, Math.PI * 2);
    ctx.stroke();
    ctx.fillStyle = ownerColor;
    ctx.beginPath();
    ctx.arc(pos.x, pos.y, 6, 0, Math.PI * 2);
    ctx.fill();

    // Label
    ctx.fillStyle = "#e7ecf7";
    ctx.font = "11px Arial";
    ctx.fillText(point.label, pos.x + 12, pos.y - 8);

    // Type
    const typeIcon = point.type === "capital" ? "★" : point.type === "resource" ? "⬢" : "⛨";
    ctx.fillText(typeIcon, pos.x + 12, pos.y + 6);

    if (point.encircled) {
      ctx.strokeStyle = "#ff5252";
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(pos.x, pos.y, 16, 0, Math.PI * 2);
      ctx.stroke();
    }
  }
}

/**
 * UIManager met à jour les panneaux HTML.
 */
class UIManager {
  constructor() {
    this.resourceList = document.getElementById("resourceList");
    this.popInfo = document.getElementById("popInfo");
    this.notifications = document.getElementById("notifications");
    this.hint = document.getElementById("hint");
  }

  updateResources(player) {
    if (!player) return;
    this.resourceList.innerHTML = CONFIG.RESOURCE_TYPES.map((r) => {
      const val = player.resources[r] || 0;
      return `<div class="resource-entry"><span>${r}</span><strong>${val.toFixed(1)}</strong></div>`;
    }).join("");
  }

  updatePop(player) {
    if (!player) return;
    this.popInfo.innerHTML = `Population : ${player.population.toFixed(1)}<br>Armée : ${player.army.toFixed(1)}`;
  }

  pushNotifications(state, playerId) {
    const msgs = state.notifications.filter((n) => !playerId || !n.targetId || n.targetId === playerId);
    const recent = msgs.slice(-8);
    this.notifications.innerHTML = recent.map((n) => `<div>${n.text}</div>`).join("");
  }

  setHint(text) { this.hint.textContent = text; }
}

class GameController {
  constructor() {
    this.canvas = document.getElementById("gameCanvas");
    this.ui = new UIManager();
    this.view = new GameView(this.canvas, this.ui);
    this.network = new NetworkClient((state) => this.sync(state));
    this.state = null;
    this.playerId = null;
    this.mapId = "europe";
    this.pendingAction = "route";
    this.selection = null;
    this.installUi();
    requestAnimationFrame(() => this.loop());
  }

  installUi() {
    document.getElementById("mapEurope").onclick = () => this.start("europe");
    document.getElementById("mapAmericas").onclick = () => this.start("americas");
    document.getElementById("mapAsia").onclick = () => this.start("asia");
    document.getElementById("toggleHelp").onclick = () => document.getElementById("helpModal").classList.add("active");
    document.getElementById("closeHelp").onclick = () => document.getElementById("helpModal").classList.remove("active");
    document.getElementById("actionPlaceResource").onclick = () => {
      this.pendingAction = "resource";
      this.ui.setHint("Cliquez sur une zone de ressource que vous possédez");
    };
    document.getElementById("actionSendArmy").onclick = () => {
      this.pendingAction = "army";
      this.ui.setHint("Cliquez sur une route pour envoyer 10% de l'armée");
    };

    this.canvas.addEventListener("click", (e) => this.handleClick(e));
    this.canvas.addEventListener("mousemove", (e) => this.handleHover(e));
  }

  start(mapId) {
    this.mapId = mapId;
    this.network.connect(mapId);
    document.getElementById("connectionStatus").textContent = `Carte ${CONFIG.MAPS[mapId].name} - connexion...`;
    this.selection = null;
    this.pendingAction = "route";
  }

  sync(state) {
    this.state = state;
    if (!this.playerId && state.players) {
      // dev fallback : prendre le premier joueur s'il manque le welcome
      this.playerId = Object.keys(state.players)[0];
    }
    this.view.setState(state);
    document.getElementById("connectionStatus").textContent = this.network.isLocal ? "Simulation locale (fallback)" : "Connecté serveur";
    const me = state.players?.[this.playerId];
    this.ui.updateResources(me);
    this.ui.updatePop(me);
    this.ui.pushNotifications(state, this.playerId);
  }

  loop() {
    this.view.render();
    requestAnimationFrame(() => this.loop());
  }

  canvasPos(evt) {
    const rect = this.canvas.getBoundingClientRect();
    return { x: (evt.clientX - rect.left) / rect.width, y: (evt.clientY - rect.top) / rect.height };
  }

  findPointAt(pos) {
    if (!this.state) return null;
    return this.state.points.find((p) => {
      const screen = this.view.toCanvas(p.position);
      const dx = screen.x - pos.x * this.canvas.width;
      const dy = screen.y - pos.y * this.canvas.height;
      return Math.sqrt(dx * dx + dy * dy) < 14;
    });
  }

  findRouteAt(pos) {
    if (!this.state) return null;
    const click = { x: pos.x * this.canvas.width, y: pos.y * this.canvas.height };
    return this.state.routes.find((r) => {
      const a = this.view.toCanvas(this.state.points.find((p) => p.id === r.from).position);
      const b = this.view.toCanvas(this.state.points.find((p) => p.id === r.to).position);
      // distance point-ligne
      const t = ((click.x - a.x) * (b.x - a.x) + (click.y - a.y) * (b.y - a.y)) / ((b.x - a.x) ** 2 + (b.y - a.y) ** 2);
      const clamped = clamp(t, 0, 1);
      const proj = { x: a.x + (b.x - a.x) * clamped, y: a.y + (b.y - a.y) * clamped };
      const d = Math.sqrt((proj.x - click.x) ** 2 + (proj.y - click.y) ** 2);
      return d < 8;
    });
  }

  handleClick(evt) {
    const pos = this.canvasPos(evt);
    const point = this.findPointAt(pos);
    const route = this.findRouteAt(pos);

    if (!this.state || !this.playerId) return;

    if (this.pendingAction === "resource") {
      if (point) {
        const res = this.network.placeResource(point.id);
        if (res?.ok) {
          this.pendingAction = "route";
          this.ui.setHint("Raccorder deux points pour créer une route");
        } else {
          this.ui.setHint(res?.message || "Impossible");
        }
      }
      return;
    }

    if (this.pendingAction === "army") {
      if (route && route.owner === this.playerId) {
        const res = this.network.sendArmy(route.id);
        this.ui.setHint(res?.ok ? "Armée en route" : res?.message || "");
        this.pendingAction = "route";
      }
      return;
    }

    // Mode route par défaut : deux clics
    if (point) {
      if (!this.selection) {
        this.selection = point;
        this.view.setSelection(point);
        this.ui.setHint(`Point ${point.label} sélectionné`);
      } else {
        const res = this.network.buildRoute(this.selection.id, point.id);
        this.ui.setHint(res?.message || "Route construite");
        this.selection = null;
        this.view.setSelection(null);
      }
    }
  }

  handleHover(evt) {
    const pos = this.canvasPos(evt);
    const point = this.findPointAt(pos);
    this.view.setHover(point);
  }
}

window.addEventListener("DOMContentLoaded", () => {
  const game = new GameController();
  game.ui.setHint("Choisissez une carte et connectez deux points pour commencer.");
});
