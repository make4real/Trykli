using System;
using System.Collections.Generic;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>Converts between the JSON level format and the LevelData / WorldData assets.</summary>
    public static class LevelDefinitionConverter
    {
        public static LevelDefinition ParseLevel(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                LevelDefinition definition = JsonUtility.FromJson<LevelDefinition>(json);
                Sanitize(definition);
                return definition;
            }
            catch (Exception exception)
            {
                Debug.LogError("[TRYKLI] Invalid level JSON: " + exception.Message);
                return null;
            }
        }

        public static WorldCollectionDefinition ParseWorlds(string json)
        {
            if (string.IsNullOrEmpty(json)) return new WorldCollectionDefinition();
            try
            {
                return JsonUtility.FromJson<WorldCollectionDefinition>(json) ?? new WorldCollectionDefinition();
            }
            catch (Exception exception)
            {
                Debug.LogError("[TRYKLI] Invalid worlds JSON: " + exception.Message);
                return new WorldCollectionDefinition();
            }
        }

        public static string ToJson(LevelDefinition definition)
        {
            return JsonUtility.ToJson(definition, true);
        }

        /// <summary>Fills <paramref name="target"/> with the content of <paramref name="definition"/>.</summary>
        public static void Apply(LevelDefinition definition, LevelData target, ItemCatalog catalog)
        {
            target.levelId = definition.levelId;
            target.worldId = definition.worldId;
            target.levelNumber = definition.levelNumber;
            target.displayNameKey = definition.nameKey;
            target.difficulty = Mathf.Clamp(definition.difficulty, 1, 10);
            target.isBoss = definition.isBoss;
            target.tutorial = definition.tutorial;
            target.simulationTimeLimit = definition.timeLimit > 0f ? definition.timeLimit : 15f;
            target.layout = definition.layout != null ? definition.layout.Clone() : new LevelLayout();
            target.cameraSettings = definition.camera ?? new CameraSettingsData();
            target.tipKey = definition.tipKey ?? string.Empty;
            target.designNotes = definition.notes ?? string.Empty;

            target.availableItems = new List<ItemStack>();
            foreach (ItemCount count in definition.inventory)
            {
                ItemDefinition item = catalog != null ? catalog.Get(count.itemId) : null;
                if (item == null)
                {
                    Debug.LogWarning($"[TRYKLI] Level {definition.levelId}: unknown item '{count.itemId}'.");
                    continue;
                }

                target.availableItems.Add(new ItemStack { item = item, count = count.count });
            }

            target.starObjectives = new List<StarObjective>();
            foreach (ObjectiveDefinition objective in definition.objectives) target.starObjectives.Add(ObjectiveFactory.Create(objective));

            target.solution = new List<SolutionStep>();
            foreach (SolutionStep step in definition.solution)
            {
                target.solution.Add(new SolutionStep { itemId = step.itemId, zoneIndex = step.zoneIndex, position = step.position, rotation = step.rotation });
            }

            target.hintData = new List<HintData>();
            foreach (HintData hint in definition.hints)
            {
                target.hintData.Add(new HintData
                {
                    textKey = hint.textKey, zoneIndex = hint.zoneIndex, itemId = hint.itemId, showDirection = hint.showDirection, rotation = hint.rotation
                });
            }
        }

        public static LevelDefinition ToDefinition(LevelData data)
        {
            var definition = new LevelDefinition
            {
                levelId = data.levelId,
                worldId = data.worldId,
                levelNumber = data.levelNumber,
                nameKey = data.displayNameKey,
                difficulty = data.difficulty,
                isBoss = data.isBoss,
                tutorial = data.tutorial,
                timeLimit = data.simulationTimeLimit,
                layout = data.layout != null ? data.layout.Clone() : new LevelLayout(),
                camera = data.cameraSettings ?? new CameraSettingsData(),
                tipKey = data.tipKey,
                notes = data.designNotes
            };

            foreach (ItemStack stack in data.availableItems)
            {
                if (stack.item != null) definition.inventory.Add(new ItemCount(stack.item.itemId, stack.count));
            }

            foreach (StarObjective objective in data.starObjectives)
            {
                definition.objectives.Add(objective != null ? objective.ToDefinition() : new ObjectiveDefinition());
            }

            foreach (SolutionStep step in data.solution)
            {
                definition.solution.Add(new SolutionStep { itemId = step.itemId, zoneIndex = step.zoneIndex, position = step.position, rotation = step.rotation });
            }

            foreach (HintData hint in data.hintData)
            {
                definition.hints.Add(new HintData
                {
                    textKey = hint.textKey, zoneIndex = hint.zoneIndex, itemId = hint.itemId, showDirection = hint.showDirection, rotation = hint.rotation
                });
            }

            return definition;
        }

        public static void Apply(WorldDefinition definition, WorldData target)
        {
            target.worldId = definition.worldId;
            target.displayNameKey = definition.nameKey;
            target.themeKey = definition.themeKey;
            target.mechanicKey = definition.mechanicKey;
            target.backgroundTop = ColorUtils.Hex(definition.backgroundTop, target.backgroundTop);
            target.backgroundBottom = ColorUtils.Hex(definition.backgroundBottom, target.backgroundBottom);
            target.blockColor = ColorUtils.Hex(definition.blockColor, target.blockColor);
            target.accentColor = ColorUtils.Hex(definition.accentColor, target.accentColor);
        }

        public static WorldDefinition ToDefinition(WorldData world)
        {
            return new WorldDefinition
            {
                worldId = world.worldId,
                nameKey = world.displayNameKey,
                themeKey = world.themeKey,
                mechanicKey = world.mechanicKey,
                backgroundTop = "#" + ColorUtility.ToHtmlStringRGB(world.backgroundTop),
                backgroundBottom = "#" + ColorUtility.ToHtmlStringRGB(world.backgroundBottom),
                blockColor = "#" + ColorUtility.ToHtmlStringRGB(world.blockColor),
                accentColor = "#" + ColorUtility.ToHtmlStringRGB(world.accentColor)
            };
        }

        /// <summary>Makes sure no list is null after JSON parsing (missing fields).</summary>
        public static void Sanitize(LevelDefinition definition)
        {
            if (definition == null) return;
            if (definition.layout == null) definition.layout = new LevelLayout();
            if (definition.layout.elements == null) definition.layout.elements = new List<ElementData>();
            if (definition.layout.zones == null) definition.layout.zones = new List<PlacementZoneData>();
            foreach (PlacementZoneData zone in definition.layout.zones)
            {
                if (zone.allowedItems == null) zone.allowedItems = new List<string>();
            }

            if (definition.inventory == null) definition.inventory = new List<ItemCount>();
            if (definition.objectives == null) definition.objectives = new List<ObjectiveDefinition>();
            if (definition.solution == null) definition.solution = new List<SolutionStep>();
            if (definition.hints == null) definition.hints = new List<HintData>();
            if (definition.camera == null) definition.camera = new CameraSettingsData();
        }
    }
}
