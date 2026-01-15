(function (global) {
  const RESOURCE_TYPES = ["wood", "metal", "stone", "oil"];

  const TERRAIN_ROUTE_COST = {
    plains: 1,
    forest: 1.15,
    mountain: 1.4,
    desert: 1.25
  };

  const RESOURCE_TERRAIN_YIELD = {
    plains: { wood: 0.9, metal: 0.8, stone: 1, oil: 0.85 },
    forest: { wood: 1.35, metal: 0.8, stone: 0.95, oil: 0.75 },
    mountain: { wood: 0.5, metal: 1.35, stone: 1.25, oil: 0.6 },
    desert: { wood: 0.35, metal: 0.9, stone: 0.65, oil: 1.4 }
  };

  const BASE_PRODUCTION = 12; // per minute equivalent, scaled down in simulation
  const POPULATION_GROWTH = 0.18; // per tick base
  const ARMY_TRAINING_FACTOR = 0.45; // portion of available population convertable to army per second

  const MAPS = {
    europe: {
      name: "Europe",
      size: { width: 1200, height: 800 },
      defaultCountries: [
        { id: "fr", name: "Gallia", color: "#4CAF50" },
        { id: "de", name: "Teutonia", color: "#2196F3" },
        { id: "it", name: "Romana", color: "#FFC107" }
      ],
      points: [
        { id: "fr_cap", label: "Gallia", type: "capital", terrain: "plains", position: { x: 0.34, y: 0.55 }, owner: "fr" },
        { id: "de_cap", label: "Teutonia", type: "capital", terrain: "forest", position: { x: 0.55, y: 0.35 }, owner: "de" },
        { id: "it_cap", label: "Romana", type: "capital", terrain: "mountain", position: { x: 0.48, y: 0.68 }, owner: "it" },
        { id: "fr_wood", label: "Bois du Nord", type: "resource", resourceType: "wood", terrain: "forest", position: { x: 0.25, y: 0.38 }, owner: "fr" },
        { id: "fr_metal", label: "Minerai Ouest", type: "resource", resourceType: "metal", terrain: "plains", position: { x: 0.28, y: 0.63 }, owner: "fr" },
        { id: "de_stone", label: "Carrière", type: "resource", resourceType: "stone", terrain: "mountain", position: { x: 0.63, y: 0.32 }, owner: "de" },
        { id: "de_oil", label: "Puits Est", type: "resource", resourceType: "oil", terrain: "plains", position: { x: 0.72, y: 0.42 }, owner: "de" },
        { id: "it_wood", label: "Forêt Alp", type: "resource", resourceType: "wood", terrain: "mountain", position: { x: 0.53, y: 0.75 }, owner: "it" },
        { id: "it_oil", label: "Puits Sud", type: "resource", resourceType: "oil", terrain: "desert", position: { x: 0.6, y: 0.82 }, owner: "it" }
      ]
    },
    americas: {
      name: "Ameriques",
      size: { width: 1200, height: 900 },
      defaultCountries: [
        { id: "na", name: "Nordia", color: "#9C27B0" },
        { id: "sa", name: "Sudoria", color: "#FF5722" }
      ],
      points: [
        { id: "na_cap", label: "Nordia", type: "capital", terrain: "forest", position: { x: 0.42, y: 0.25 }, owner: "na" },
        { id: "sa_cap", label: "Sudoria", type: "capital", terrain: "plains", position: { x: 0.52, y: 0.65 }, owner: "sa" },
        { id: "na_metal", label: "Roche Nord", type: "resource", resourceType: "metal", terrain: "mountain", position: { x: 0.35, y: 0.18 }, owner: "na" },
        { id: "na_wood", label: "Forêt Centrale", type: "resource", resourceType: "wood", terrain: "forest", position: { x: 0.48, y: 0.32 }, owner: "na" },
        { id: "sa_oil", label: "Pétrole Amazonie", type: "resource", resourceType: "oil", terrain: "forest", position: { x: 0.45, y: 0.6 }, owner: "sa" },
        { id: "sa_stone", label: "Roche Andes", type: "resource", resourceType: "stone", terrain: "mountain", position: { x: 0.58, y: 0.7 }, owner: "sa" },
        { id: "isthmus", label: "Isthme", type: "defense", terrain: "plains", position: { x: 0.49, y: 0.48 }, owner: null }
      ]
    },
    asia: {
      name: "Asie",
      size: { width: 1400, height: 900 },
      defaultCountries: [
        { id: "east", name: "Orient", color: "#00BCD4" },
        { id: "steppe", name: "Steppe", color: "#8BC34A" },
        { id: "south", name: "Sud", color: "#E91E63" }
      ],
      points: [
        { id: "east_cap", label: "Orient", type: "capital", terrain: "plains", position: { x: 0.72, y: 0.42 }, owner: "east" },
        { id: "steppe_cap", label: "Steppe", type: "capital", terrain: "plains", position: { x: 0.45, y: 0.28 }, owner: "steppe" },
        { id: "south_cap", label: "Sud", type: "capital", terrain: "forest", position: { x: 0.55, y: 0.65 }, owner: "south" },
        { id: "east_oil", label: "Mer Caspienne", type: "resource", resourceType: "oil", terrain: "desert", position: { x: 0.68, y: 0.55 }, owner: "east" },
        { id: "steppe_metal", label: "Minerai Steppe", type: "resource", resourceType: "metal", terrain: "mountain", position: { x: 0.38, y: 0.32 }, owner: "steppe" },
        { id: "south_wood", label: "Jungle", type: "resource", resourceType: "wood", terrain: "forest", position: { x: 0.6, y: 0.78 }, owner: "south" },
        { id: "chokepoint", label: "Col", type: "defense", terrain: "mountain", position: { x: 0.55, y: 0.5 }, owner: null },
        { id: "tradehub", label: "Route de soie", type: "defense", terrain: "plains", position: { x: 0.6, y: 0.36 }, owner: null }
      ]
    }
  };

  const CONFIG = {
    RESOURCE_TYPES,
    TERRAIN_ROUTE_COST,
    RESOURCE_TERRAIN_YIELD,
    BASE_PRODUCTION,
    POPULATION_GROWTH,
    ARMY_TRAINING_FACTOR,
    MAPS
  };

  if (typeof module !== "undefined") {
    module.exports = CONFIG;
  } else {
    global.TRYKLI_CONFIG = CONFIG;
  }
})(typeof window !== "undefined" ? window : globalThis);
