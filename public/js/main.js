// Trykli V0 — jeu .io temps réel (simulation locale par défaut, WebSocket optionnel)
// Le code reste volontairement lisible et commenté pour faciliter l'évolution.

const CONFIG = window.TRYKLI_CONFIG;
const { Simulation, EventEmitter } = window.TrykliSimulation;
const clamp = (v, min, max) => Math.max(min, Math.min(max, v));

/**
 * NetworkClient tente une connexion WebSocket, sinon crée une Simulation locale.
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
    this.host = new Simulation(CONFIG);
    this.host.createState(mapId);
    const me = this.host.addPlayer("Vous");
    this.playerId = me;
    // Ajouter bots simples
    const bots = ["Bot Nord", "Bot Est"].slice(0, 2);
    bots.forEach((name) => this.host.addPlayer(name, { bot: true }));
    this.host.on("state", (state) => this.onState(state));
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
