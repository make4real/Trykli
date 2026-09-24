using System;
using Trykli.Data;
using Trykli.Localization;
using UnityEngine;

namespace Trykli.Objectives
{
    /// <summary>★ Reach the exit.</summary>
    [Serializable]
    public sealed class CompleteLevelObjective : StarObjective
    {
        public override bool Evaluate(LevelRunStats stats) => stats.ReachedGoal;
        protected override string BuildDescription() => Loc.Get("obj.complete");
        protected override ObjectiveDefinition CreateDefinition() => new ObjectiveDefinition { type = ObjectiveDefinition.Complete };
    }

    /// <summary>Use at most N objects (a portal pair counts as one object).</summary>
    [Serializable]
    public sealed class MaxObjectsObjective : StarObjective
    {
        [Min(0)] public int maxObjects = 2;

        public override bool Evaluate(LevelRunStats stats) => stats.TotalItemsUsed <= maxObjects;

        protected override string BuildDescription()
        {
            return maxObjects <= 1 ? Loc.Get("obj.max_objects_one") : Loc.Format("obj.max_objects", maxObjects);
        }

        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.MaxObjects, count = maxObjects };
    }

    /// <summary>
    /// Constraint on the use of a specific item (or of every item except one when <see cref="excludeItem"/> is set).
    /// Examples: "utiliser les 2 objets", "utiliser uniquement les portails".
    /// </summary>
    [Serializable]
    public sealed class ItemUsageObjective : StarObjective
    {
        [Tooltip("Item id. Empty = every item.")]
        public string itemId = "";
        [Tooltip("Count the items that are NOT itemId instead.")]
        public bool excludeItem;
        public Comparison comparison = Comparison.AtLeast;
        [Min(0)] public int count = 1;

        public override bool Evaluate(LevelRunStats stats)
        {
            int used;
            if (string.IsNullOrEmpty(itemId)) used = stats.TotalItemsUsed;
            else if (excludeItem) used = stats.TotalItemsUsed - stats.GetItemUnits(itemId);
            else used = stats.GetItemUnits(itemId);
            return comparison.Evaluate(used, count);
        }

        protected override string BuildDescription()
        {
            string itemName = string.IsNullOrEmpty(itemId) ? Loc.Get("obj.items_any") : Loc.Get("item." + itemId);
            if (excludeItem) return Loc.Format("obj.only_item", itemName);
            return Loc.Format("obj.use_item." + comparison.ToString().ToLowerInvariant(), count, itemName);
        }

        protected override ObjectiveDefinition CreateDefinition() => new ObjectiveDefinition
        {
            type = ObjectiveDefinition.ItemUsage, target = itemId, invert = excludeItem, comparison = comparison, count = count
        };
    }

    /// <summary>Collect crystals: N crystals, all crystals (count 0) or one specific crystal.</summary>
    [Serializable]
    public sealed class CollectCrystalObjective : StarObjective
    {
        [Tooltip("Number of crystals required. 0 = every crystal of the level.")]
        [Min(0)] public int requiredCount = 1;
        [Tooltip("Optional id of a specific crystal that must be collected.")]
        public string crystalId = "";

        public override bool Evaluate(LevelRunStats stats)
        {
            if (!string.IsNullOrEmpty(crystalId)) return stats.HasCrystal(crystalId);
            int required = requiredCount <= 0 ? stats.TotalCrystalsInLevel : requiredCount;
            return stats.CrystalCount >= required;
        }

        protected override string BuildDescription()
        {
            if (!string.IsNullOrEmpty(crystalId) || requiredCount == 1) return Loc.Get("obj.crystal_one");
            if (requiredCount <= 0) return Loc.Get("obj.crystal_all");
            return Loc.Format("obj.crystals", requiredCount);
        }

        protected override ObjectiveDefinition CreateDefinition() => new ObjectiveDefinition
        {
            type = ObjectiveDefinition.CollectCrystals, count = requiredCount, target = crystalId
        };
    }

    /// <summary>Reach the exit in less than N seconds of simulation.</summary>
    [Serializable]
    public sealed class TimeObjective : StarObjective
    {
        [Min(0.1f)] public float maxSeconds = 8f;

        public override bool Evaluate(LevelRunStats stats) => stats.ElapsedTime <= maxSeconds;
        protected override string BuildDescription() => Loc.Format("obj.time", maxSeconds.ToString("0.#"));
        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.Time, value = maxSeconds };
    }

    /// <summary>Bounce constraint (spring launches, bumper hits or both).</summary>
    [Serializable]
    public sealed class BounceCountObjective : StarObjective
    {
        public BounceSource source = BounceSource.Any;
        public Comparison comparison = Comparison.Exactly;
        [Min(0)] public int count = 2;

        public override bool Evaluate(LevelRunStats stats) => comparison.Evaluate(stats.GetBounces(source), count);

        protected override string BuildDescription()
        {
            string key = source == BounceSource.Bumper ? "obj.bumper." : "obj.bounce.";
            return Loc.Format(key + comparison.ToString().ToLowerInvariant(), count);
        }

        protected override ObjectiveDefinition CreateDefinition() => new ObjectiveDefinition
        {
            type = ObjectiveDefinition.BounceCount, target = source.ToString(), comparison = comparison, count = count
        };
    }

    /// <summary>Never touch a (non lethal) hazard: lasers, spikes...</summary>
    [Serializable]
    public sealed class NoHazardObjective : StarObjective
    {
        public HazardCategory category = HazardCategory.Any;

        public override bool Evaluate(LevelRunStats stats) => stats.GetHazardContacts(category) == 0;
        protected override string BuildDescription() => Loc.Get("obj.no_hazard." + category.ToString().ToLowerInvariant());
        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.NoHazard, target = category.ToString() };
    }

    /// <summary>Never touch a surface category (walls, ground...).</summary>
    [Serializable]
    public sealed class AvoidContactObjective : StarObjective
    {
        public SurfaceKind surface = SurfaceKind.Wall;

        public override bool Evaluate(LevelRunStats stats) => stats.GetContacts(surface) == 0;
        protected override string BuildDescription() => Loc.Get("obj.avoid." + surface.ToString().ToLowerInvariant());
        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.AvoidContact, target = surface.ToString() };
    }

    /// <summary>
    /// Generic interaction objective based on run counters: press buttons, use a portal pair,
    /// avoid a fake portal, trigger N explosions, flip gravity N times, pass through a marker zone...
    /// </summary>
    [Serializable]
    public sealed class SpecificInteractionObjective : StarObjective
    {
        [Tooltip("Counter key, e.g. button, button:<id>, portal, portal:<channel>, explosion, gravity_flip, door_open, laser_off, zone:<id>.")]
        public string counterKey = LevelRunStats.Counters.ButtonPressed;
        public Comparison comparison = Comparison.AtLeast;
        [Min(0)] public int count = 1;

        public override bool Evaluate(LevelRunStats stats) => comparison.Evaluate(stats.GetCounter(counterKey), count);

        protected override string BuildDescription()
        {
            string baseKey = counterKey ?? string.Empty;
            int separator = baseKey.IndexOf(':');
            if (separator >= 0) baseKey = baseKey.Substring(0, separator);
            return Loc.Format("obj.interaction." + baseKey + "." + comparison.ToString().ToLowerInvariant(), count);
        }

        protected override ObjectiveDefinition CreateDefinition() => new ObjectiveDefinition
        {
            type = ObjectiveDefinition.Interaction, target = counterKey, comparison = comparison, count = count
        };
    }

    /// <summary>Succeed within N attempts (GO presses) in the current level session.</summary>
    [Serializable]
    public sealed class AttemptsObjective : StarObjective
    {
        [Min(1)] public int maxAttempts = 3;

        public override bool Evaluate(LevelRunStats stats) => stats.Attempts <= maxAttempts;

        protected override string BuildDescription()
        {
            return maxAttempts <= 1 ? Loc.Get("obj.first_try") : Loc.Format("obj.attempts", maxAttempts);
        }

        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.Attempts, count = maxAttempts };
    }

    /// <summary>Trykli never stays (almost) still for more than N seconds.</summary>
    [Serializable]
    public sealed class NoStopObjective : StarObjective
    {
        [Min(0.1f)] public float maxIdleSeconds = 1f;

        public override bool Evaluate(LevelRunStats stats) => stats.LongestIdleTime <= maxIdleSeconds;
        protected override string BuildDescription() => Loc.Get("obj.no_stop");
        protected override ObjectiveDefinition CreateDefinition() =>
            new ObjectiveDefinition { type = ObjectiveDefinition.NoStop, value = maxIdleSeconds };
    }
}
