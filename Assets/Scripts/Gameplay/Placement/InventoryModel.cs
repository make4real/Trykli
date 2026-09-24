using System;
using System.Collections.Generic;

namespace Trykli.Placement
{
    /// <summary>
    /// Pure inventory logic. Counts pieces: a portal pair A/B is one inventory unit made of two pieces.
    /// "Objects used" objectives count units (ceil(pieces / piecesPerUnit)).
    /// </summary>
    public sealed class InventoryModel
    {
        private sealed class Entry
        {
            public string ItemId;
            public int Units;
            public int PiecesPerUnit;
            public int PlacedPieces;
            public int TotalPieces => Units * PiecesPerUnit;
        }

        private readonly List<Entry> _entries = new List<Entry>();

        public event Action Changed;

        public IEnumerable<string> ItemIds
        {
            get
            {
                foreach (Entry entry in _entries) yield return entry.ItemId;
            }
        }

        public void Add(string itemId, int units, int piecesPerUnit = 1)
        {
            if (string.IsNullOrEmpty(itemId) || units <= 0) return;
            Entry entry = Find(itemId);
            if (entry == null)
            {
                entry = new Entry { ItemId = itemId, PiecesPerUnit = Math.Max(1, piecesPerUnit) };
                _entries.Add(entry);
            }

            entry.Units += units;
            Changed?.Invoke();
        }

        public void Clear()
        {
            _entries.Clear();
            Changed?.Invoke();
        }

        public int TotalPieces(string itemId) => Find(itemId)?.TotalPieces ?? 0;
        public int PlacedPieces(string itemId) => Find(itemId)?.PlacedPieces ?? 0;
        public int RemainingPieces(string itemId) => Math.Max(0, TotalPieces(itemId) - PlacedPieces(itemId));
        public bool HasRemaining(string itemId) => RemainingPieces(itemId) > 0;

        public bool TryTake(string itemId)
        {
            Entry entry = Find(itemId);
            if (entry == null || entry.PlacedPieces >= entry.TotalPieces) return false;
            entry.PlacedPieces++;
            Changed?.Invoke();
            return true;
        }

        public void Return(string itemId)
        {
            Entry entry = Find(itemId);
            if (entry == null || entry.PlacedPieces <= 0) return;
            entry.PlacedPieces--;
            Changed?.Invoke();
        }

        public void ReturnAll()
        {
            foreach (Entry entry in _entries) entry.PlacedPieces = 0;
            Changed?.Invoke();
        }

        public int UnitsUsed(string itemId)
        {
            Entry entry = Find(itemId);
            if (entry == null || entry.PlacedPieces <= 0) return 0;
            return (entry.PlacedPieces + entry.PiecesPerUnit - 1) / entry.PiecesPerUnit;
        }

        public int TotalUnitsUsed
        {
            get
            {
                int total = 0;
                foreach (Entry entry in _entries) total += UnitsUsed(entry.ItemId);
                return total;
            }
        }

        public Dictionary<string, int> GetUnitsUsed()
        {
            var result = new Dictionary<string, int>();
            foreach (Entry entry in _entries)
            {
                int units = UnitsUsed(entry.ItemId);
                if (units > 0) result[entry.ItemId] = units;
            }

            return result;
        }

        private Entry Find(string itemId)
        {
            foreach (Entry entry in _entries)
            {
                if (entry.ItemId == itemId) return entry;
            }

            return null;
        }
    }

    /// <summary>Serializable description of one placed item (used to restore placements on Restart / Modify).</summary>
    [Serializable]
    public struct PlacementRecord
    {
        public string itemId;
        public int zoneIndex;
        public UnityEngine.Vector2 position;
        public float rotation;
        public string endpoint;
    }
}
