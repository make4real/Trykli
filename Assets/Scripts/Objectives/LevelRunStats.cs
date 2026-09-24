using System.Collections.Generic;
using Trykli.Data;

namespace Trykli.Objectives
{
    /// <summary>
    /// Everything that happened during one simulation run. Pure data: filled by the gameplay RunTracker,
    /// read by objectives. Independent from Unity objects so it can be unit tested.
    /// </summary>
    public sealed class LevelRunStats
    {
        /// <summary>Counter keys shared between gameplay and objectives.</summary>
        public static class Counters
        {
            public const string ButtonPressed = "button";
            public const string PortalUsed = "portal";
            public const string Explosion = "explosion";
            public const string GravityFlip = "gravity_flip";
            public const string DoorOpened = "door_open";
            public const string LaserDisabled = "laser_off";
            public const string CannonShot = "cannon";

            public static string Button(string id) => "button:" + id;
            public static string Portal(int channel) => "portal:" + channel;
            public static string Zone(string id) => "zone:" + id;
        }

        private readonly Dictionary<string, int> _itemUnits = new Dictionary<string, int>();
        private readonly HashSet<string> _crystals = new HashSet<string>();
        private readonly Dictionary<BounceSource, int> _bounces = new Dictionary<BounceSource, int>();
        private readonly Dictionary<string, int> _counters = new Dictionary<string, int>();
        private readonly Dictionary<SurfaceKind, int> _contacts = new Dictionary<SurfaceKind, int>();
        private readonly Dictionary<HazardCategory, int> _hazards = new Dictionary<HazardCategory, int>();

        public bool ReachedGoal { get; set; }
        public float ElapsedTime { get; set; }
        /// <summary>Number of GO presses in this level session (including the current one).</summary>
        public int Attempts { get; set; } = 1;
        public int TotalCrystalsInLevel { get; set; }
        public float LongestIdleTime { get; set; }

        public IEnumerable<string> CollectedCrystals => _crystals;
        public int CrystalCount => _crystals.Count;

        public int TotalItemsUsed
        {
            get
            {
                int total = 0;
                foreach (int units in _itemUnits.Values) total += units;
                return total;
            }
        }

        public IReadOnlyDictionary<string, int> ItemsUsed => _itemUnits;

        public void SetItemUnits(string itemId, int units)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            if (units <= 0) _itemUnits.Remove(itemId);
            else _itemUnits[itemId] = units;
        }

        public int GetItemUnits(string itemId)
        {
            return _itemUnits.TryGetValue(itemId, out int units) ? units : 0;
        }

        public void AddCrystal(string crystalId)
        {
            _crystals.Add(crystalId ?? string.Empty);
        }

        public bool HasCrystal(string crystalId)
        {
            return _crystals.Contains(crystalId);
        }

        public void AddBounce(BounceSource source)
        {
            Add(_bounces, source, 1);
            if (source != BounceSource.Any) Add(_bounces, BounceSource.Any, 1);
        }

        public int GetBounces(BounceSource source)
        {
            return _bounces.TryGetValue(source, out int count) ? count : 0;
        }

        public void Increment(string counter, int amount = 1)
        {
            if (!string.IsNullOrEmpty(counter)) Add(_counters, counter, amount);
        }

        public int GetCounter(string counter)
        {
            return counter != null && _counters.TryGetValue(counter, out int value) ? value : 0;
        }

        public void AddContact(SurfaceKind surface)
        {
            Add(_contacts, surface, 1);
        }

        public int GetContacts(SurfaceKind surface)
        {
            return _contacts.TryGetValue(surface, out int count) ? count : 0;
        }

        public void AddHazardContact(HazardCategory category)
        {
            Add(_hazards, category, 1);
            if (category != HazardCategory.Any) Add(_hazards, HazardCategory.Any, 1);
        }

        public int GetHazardContacts(HazardCategory category)
        {
            return _hazards.TryGetValue(category, out int count) ? count : 0;
        }

        private static void Add<TKey>(Dictionary<TKey, int> dictionary, TKey key, int amount)
        {
            dictionary.TryGetValue(key, out int current);
            dictionary[key] = current + amount;
        }
    }
}
