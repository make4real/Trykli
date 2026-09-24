using System.Collections.Generic;
using Trykli.Objectives;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Validates level definitions: missing spawn / exit, objectives, duplicates, invalid ids,
    /// inconsistent inventory, solutions and hints. Pure logic, used by the editor tools and the tests.
    /// </summary>
    public static class LevelValidator
    {
        public enum Severity
        {
            Warning,
            Error
        }

        public readonly struct Issue
        {
            public readonly int LevelId;
            public readonly Severity Severity;
            public readonly string Message;

            public Issue(int levelId, Severity severity, string message)
            {
                LevelId = levelId;
                Severity = severity;
                Message = message;
            }

            public override string ToString() => $"[{Severity}] Level {LevelId:000}: {Message}";
        }

        public static List<Issue> ValidateAll(IList<LevelDefinition> levels, int worldCount, int levelsPerWorld, ItemCatalog catalog = null)
        {
            var issues = new List<Issue>();
            var ids = new HashSet<int>();
            var slots = new HashSet<string>();
            foreach (LevelDefinition level in levels)
            {
                if (level == null) continue;
                if (!ids.Add(level.levelId)) issues.Add(new Issue(level.levelId, Severity.Error, "Duplicate levelId."));
                if (!slots.Add(level.worldId + "-" + level.levelNumber))
                {
                    issues.Add(new Issue(level.levelId, Severity.Error, $"Duplicate world/level number {level.worldId}-{level.levelNumber}."));
                }

                issues.AddRange(Validate(level, worldCount, levelsPerWorld, catalog));
            }

            return issues;
        }

        public static List<Issue> Validate(LevelDefinition level, int worldCount, int levelsPerWorld, ItemCatalog catalog = null)
        {
            var issues = new List<Issue>();
            int id = level.levelId;

            void Error(string message) => issues.Add(new Issue(id, Severity.Error, message));
            void Warning(string message) => issues.Add(new Issue(id, Severity.Warning, message));

            if (id <= 0) Error("levelId must be >= 1.");
            if (level.worldId < 1 || level.worldId > worldCount) Error($"Invalid worldId {level.worldId} (expected 1..{worldCount}).");
            if (level.levelNumber < 1 || level.levelNumber > levelsPerWorld) Error($"Invalid levelNumber {level.levelNumber} (expected 1..{levelsPerWorld}).");
            if (id > 0 && level.worldId >= 1 && (level.worldId - 1) * levelsPerWorld + level.levelNumber != id)
            {
                Warning("levelId does not match worldId / levelNumber ordering.");
            }

            if (level.difficulty < 1 || level.difficulty > 10) Error("Difficulty must be between 1 and 10.");
            if (level.timeLimit <= 0f) Error("Simulation time limit must be > 0.");
            if (string.IsNullOrEmpty(level.nameKey)) Warning("Missing display name key.");

            LevelLayout layout = level.layout;
            if (layout == null)
            {
                Error("Missing layout (no spawn, no exit).");
                return issues;
            }

            Rect bounds = layout.bounds;
            if (bounds.width <= 0f || bounds.height <= 0f) Error("Invalid level bounds.");
            if (!bounds.Contains(layout.spawn)) Error("Spawn missing or outside the level bounds.");
            if (layout.goalRadius <= 0f) Error("Level has no exit (goal radius <= 0).");
            else if (!bounds.Contains(layout.goal)) Error("Exit (goal) outside the level bounds.");
            if ((layout.spawn - layout.goal).sqrMagnitude < 0.01f) Error("Spawn and exit are at the same position.");

            ValidateObjectives(level, Error, Warning);
            var inventory = ValidateInventory(level, catalog, Error, Warning);
            ValidateElements(level, inventory, Error, Warning);
            ValidateSolution(level, inventory, Error, Warning);

            foreach (HintData hint in level.hints)
            {
                if (hint.zoneIndex >= layout.zones.Count) Warning($"Hint references missing zone {hint.zoneIndex}.");
            }

            return issues;
        }

        private static void ValidateObjectives(LevelDefinition level, System.Action<string> error, System.Action<string> warning)
        {
            if (level.objectives.Count != StarEvaluator.StarsPerLevel)
            {
                error($"Expected {StarEvaluator.StarsPerLevel} star objectives, found {level.objectives.Count}.");
            }

            if (level.objectives.Count > 0 && level.objectives[0].type != ObjectiveDefinition.Complete)
            {
                warning("The first star should be 'Complete' (GDD section 9).");
            }

            int crystals = level.layout.CountElements(ElementType.Crystal);
            foreach (ObjectiveDefinition objective in level.objectives)
            {
                if (System.Array.IndexOf(ObjectiveFactory.KnownTypes, objective.type) < 0) error($"Unknown objective type '{objective.type}'.");
                if (objective.type == ObjectiveDefinition.CollectCrystals)
                {
                    if (crystals == 0) error("Crystal objective but the level has no crystal.");
                    else if (objective.count > crystals) error($"Crystal objective requires {objective.count} crystals but only {crystals} exist.");
                }
            }

            if (crystals > 3) warning($"{crystals} crystals (the GDD recommends up to 3).");
        }

        private static Dictionary<string, int> ValidateInventory(LevelDefinition level, ItemCatalog catalog, System.Action<string> error, System.Action<string> warning)
        {
            var inventory = new Dictionary<string, int>();
            foreach (ItemCount count in level.inventory)
            {
                bool known = catalog != null ? catalog.Contains(count.itemId) : System.Array.IndexOf(BuiltInItems.AllIds, count.itemId) >= 0;
                if (!known) error($"Unknown inventory item '{count.itemId}'.");
                if (count.count <= 0) error($"Inventory count for '{count.itemId}' must be > 0.");
                inventory.TryGetValue(count.itemId, out int current);
                inventory[count.itemId] = current + count.count;
            }

            if (inventory.Count > 0 && level.layout.zones.Count == 0) error("Inventory is not empty but there is no placement zone.");
            if (inventory.Count == 0) warning("Empty inventory.");

            for (int i = 0; i < level.layout.zones.Count; i++)
            {
                foreach (string itemId in level.layout.zones[i].allowedItems)
                {
                    if (System.Array.IndexOf(BuiltInItems.AllIds, itemId) < 0 && (catalog == null || !catalog.Contains(itemId)))
                    {
                        error($"Zone {i} accepts unknown item '{itemId}'.");
                    }
                }
            }

            foreach (string itemId in inventory.Keys)
            {
                bool placeable = false;
                foreach (PlacementZoneData zone in level.layout.zones)
                {
                    if (zone.Accepts(itemId))
                    {
                        placeable = true;
                        break;
                    }
                }

                if (!placeable) warning($"No zone accepts the inventory item '{itemId}'.");
            }

            return inventory;
        }

        private static void ValidateElements(LevelDefinition level, Dictionary<string, int> inventory, System.Action<string> error, System.Action<string> warning)
        {
            var crystalIds = new HashSet<string>();
            var portalEndpoints = new Dictionary<int, int>();
            foreach (ElementData element in level.layout.elements)
            {
                if (!level.layout.bounds.Contains(element.position)) warning($"{element.type} at {element.position} is outside the bounds.");
                switch (element.type)
                {
                    case ElementType.Crystal:
                        if (!crystalIds.Add(element.id ?? string.Empty)) error($"Duplicate crystal id '{element.id}'.");
                        break;
                    case ElementType.Portal:
                        if (element.channel < 0) error("Fixed portal without pair channel.");
                        portalEndpoints.TryGetValue(element.channel, out int n);
                        portalEndpoints[element.channel] = n + 1;
                        break;
                    case ElementType.Button:
                        if (element.emitChannel < 0) warning("Button without emit channel.");
                        break;
                }
            }

            foreach (KeyValuePair<int, int> pair in portalEndpoints)
            {
                bool placeablePartner = (pair.Key == 0 && inventory.ContainsKey(BuiltInItems.PortalAB)) ||
                                        (pair.Key == 1 && inventory.ContainsKey(BuiltInItems.PortalCD));
                if (pair.Value == 1 && !placeablePartner) warning($"Fixed portal channel {pair.Key} has no partner.");
                if (pair.Value > 2) error($"Portal channel {pair.Key} has more than 2 endpoints.");
            }
        }

        private static void ValidateSolution(LevelDefinition level, Dictionary<string, int> inventory, System.Action<string> error, System.Action<string> warning)
        {
            if (level.solution.Count == 0)
            {
                if (inventory.Count > 0) warning("No reference solution (hints and the solution verifier will be limited).");
                return;
            }

            var used = new Dictionary<string, int>();
            var zoneUse = new Dictionary<int, int>();
            foreach (SolutionStep step in level.solution)
            {
                if (step.zoneIndex < 0 || step.zoneIndex >= level.layout.zones.Count)
                {
                    error($"Solution references missing zone {step.zoneIndex}.");
                    continue;
                }

                PlacementZoneData zone = level.layout.zones[step.zoneIndex];
                if (!zone.Accepts(step.itemId)) error($"Solution places '{step.itemId}' in zone {step.zoneIndex} which does not accept it.");
                used.TryGetValue(step.itemId, out int count);
                used[step.itemId] = count + 1;
                zoneUse.TryGetValue(step.zoneIndex, out int z);
                zoneUse[step.zoneIndex] = z + 1;
                if (zoneUse[step.zoneIndex] > Mathf.Max(1, zone.capacity)) error($"Solution exceeds the capacity of zone {step.zoneIndex}.");
            }

            foreach (KeyValuePair<string, int> pair in used)
            {
                inventory.TryGetValue(pair.Key, out int available);
                int pieces = available * (pair.Key == BuiltInItems.PortalAB || pair.Key == BuiltInItems.PortalCD ? 2 : 1);
                if (pair.Value > pieces) error($"Solution uses {pair.Value} x '{pair.Key}' but only {pieces} are available.");
            }
        }
    }
}
