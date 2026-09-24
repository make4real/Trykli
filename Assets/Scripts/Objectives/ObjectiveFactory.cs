using System;
using Trykli.Data;
using UnityEngine;

namespace Trykli.Objectives
{
    /// <summary>Creates <see cref="StarObjective"/> instances from their JSON definition.</summary>
    public static class ObjectiveFactory
    {
        public static readonly string[] KnownTypes =
        {
            ObjectiveDefinition.Complete, ObjectiveDefinition.MaxObjects, ObjectiveDefinition.ItemUsage,
            ObjectiveDefinition.CollectCrystals, ObjectiveDefinition.Time, ObjectiveDefinition.BounceCount,
            ObjectiveDefinition.NoHazard, ObjectiveDefinition.AvoidContact, ObjectiveDefinition.Interaction,
            ObjectiveDefinition.Attempts, ObjectiveDefinition.NoStop
        };

        public static StarObjective Create(ObjectiveDefinition definition)
        {
            if (definition == null) return new CompleteLevelObjective();
            StarObjective objective = CreateUnconfigured(definition);
            objective.descriptionKey = definition.descriptionKey ?? string.Empty;
            return objective;
        }

        /// <summary>Creates a default instance of the given objective type name (Level Editor "Add objective" menu).</summary>
        public static StarObjective CreateDefault(string typeName)
        {
            return Create(new ObjectiveDefinition { type = typeName, count = 1, value = 8f });
        }

        private static StarObjective CreateUnconfigured(ObjectiveDefinition d)
        {
            switch (d.type)
            {
                case ObjectiveDefinition.Complete:
                    return new CompleteLevelObjective();
                case ObjectiveDefinition.MaxObjects:
                    return new MaxObjectsObjective { maxObjects = Mathf.Max(0, d.count) };
                case ObjectiveDefinition.ItemUsage:
                    return new ItemUsageObjective { itemId = d.target ?? string.Empty, excludeItem = d.invert, comparison = d.comparison, count = d.count };
                case ObjectiveDefinition.CollectCrystals:
                    return new CollectCrystalObjective { requiredCount = Mathf.Max(0, d.count), crystalId = d.target ?? string.Empty };
                case ObjectiveDefinition.Time:
                    return new TimeObjective { maxSeconds = d.value > 0f ? d.value : 8f };
                case ObjectiveDefinition.BounceCount:
                    return new BounceCountObjective { source = ParseEnum(d.target, BounceSource.Any), comparison = d.comparison, count = d.count };
                case ObjectiveDefinition.NoHazard:
                    return new NoHazardObjective { category = ParseEnum(d.target, HazardCategory.Any) };
                case ObjectiveDefinition.AvoidContact:
                    return new AvoidContactObjective { surface = ParseEnum(d.target, SurfaceKind.Wall) };
                case ObjectiveDefinition.Interaction:
                    return new SpecificInteractionObjective { counterKey = d.target ?? string.Empty, comparison = d.comparison, count = d.count };
                case ObjectiveDefinition.Attempts:
                    return new AttemptsObjective { maxAttempts = Mathf.Max(1, d.count) };
                case ObjectiveDefinition.NoStop:
                    return new NoStopObjective { maxIdleSeconds = d.value > 0f ? d.value : 1f };
                default:
                    Debug.LogWarning($"[TRYKLI] Unknown objective type '{d.type}', using CompleteLevelObjective.");
                    return new CompleteLevelObjective();
            }
        }

        private static T ParseEnum<T>(string value, T fallback) where T : struct
        {
            return !string.IsNullOrEmpty(value) && Enum.TryParse(value, true, out T parsed) ? parsed : fallback;
        }
    }
}
