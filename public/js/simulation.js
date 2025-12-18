(function (global) {
  const CONFIG = typeof module !== "undefined" ? require("../config/gameConfig.js") : global.TRYKLI_CONFIG;

  class EventEmitter {
    constructor() { this.listeners = {}; }
    on(event, cb) { (this.listeners[event] = this.listeners[event] || []).push(cb); }
    emit(event, payload) { (this.listeners[event] || []).forEach((cb) => cb(payload)); }
  }

  /**
   * Simulation partageable (navigateur ou Node) inspirée d'une boucle serveur
   * autoritaire : tick unique, ordres appliqués, état sérialisé diffusé.
   */
  class Simulation extends EventEmitter {
    constructor(config = CONFIG) {
      super();
      this.config = config;
      this.state = null;
      this.tickInterval = null;
      this.time = 0;
    }

    createState(mapId) {
      const map = this.config.MAPS[mapId];
      if (!map) throw new Error("Map inconnue");
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
      this.time = 0;
      this.emitState();
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
      this.emit("state", this.serializedState());
    }

    serializedState() {
      return {
        ...this.state,
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

    stop() {
      if (this.tickInterval) clearInterval(this.tickInterval);
    }

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
          const ownResources = this.state.points.filter((pt) => pt.owner === bot.id && pt.type === "resource" && !pt.built);
          if (ownResources.length && Math.random() > 0.5) {
            this.placeResource(bot.id, ownResources[Math.floor(Math.random() * ownResources.length)].id);
            return;
          }
          if (Math.random() > 0.7) {
            const owned = this.state.points.filter((p) => p.owner === bot.id);
            const targets = this.state.points.filter((p) => p.owner !== bot.id);
            if (owned.length && targets.length) {
              const from = owned[Math.floor(Math.random() * owned.length)];
              const to = targets[Math.floor(Math.random() * targets.length)];
              this.buildRoute(bot.id, from.id, to.id);
            }
          }
        });
    }

    handleProduction() {
      Object.values(this.state.players).forEach((player) => {
        if (!player.alive) return;
        player.population += this.config.POPULATION_GROWTH;
        player.army = Math.min(player.army + this.config.ARMY_TRAINING_FACTOR, player.population);

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
              const terrainYield = this.config.RESOURCE_TERRAIN_YIELD[p.terrain][p.resourceType];
              const produced = this.config.BASE_PRODUCTION * terrainYield * 0.15;
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
      const terrainFactor = (this.config.TERRAIN_ROUTE_COST[from.terrain] + this.config.TERRAIN_ROUTE_COST[to.terrain]) / 2;
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
          target.owner = m.owner;
          target.garrison = (target.garrison || 0) + m.amount;
        } else {
          const defenseBonus = target.terrain === "mountain" ? 1.4 : target.terrain === "forest" ? 1.1 : 1;
          const defense = (target.garrison || 0) * defenseBonus;
          const result = defense - m.amount;
          if (result <= 0) {
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
      if (this.state.notifications.length > 50) {
        this.state.notifications = this.state.notifications.slice(-50);
      }
    }
  }

  const clamp = (v, min, max) => Math.max(min, Math.min(max, v));
  const distance = (a, b, width = 1, height = 1) => {
    const dx = (a.x - b.x) * width;
    const dy = (a.y - b.y) * height;
    return Math.sqrt(dx * dx + dy * dy);
  };

  const api = { Simulation, EventEmitter };
  if (typeof module !== "undefined") {
    module.exports = api;
  } else {
    global.TrykliSimulation = api;
  }
})(typeof window !== "undefined" ? window : globalThis);

