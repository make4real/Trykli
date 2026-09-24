using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>Registry of every <see cref="ItemDefinition"/>, looked up by id.</summary>
    [CreateAssetMenu(menuName = "TRYKLI/Item Catalog", fileName = "ItemCatalog")]
    public sealed class ItemCatalog : ScriptableObject
    {
        [SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();

        private Dictionary<string, ItemDefinition> _lookup;

        public IReadOnlyList<ItemDefinition> Items => items;

        public ItemDefinition Get(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            EnsureLookup();
            if (_lookup.TryGetValue(itemId, out ItemDefinition item)) return item;

            // Unknown in the catalog: fall back to built-in defaults so data never breaks the game.
            if (System.Array.IndexOf(BuiltInItems.AllIds, itemId) >= 0)
            {
                item = BuiltInItems.Create(itemId);
                items.Add(item);
                _lookup[itemId] = item;
                return item;
            }

            return null;
        }

        public bool Contains(string itemId)
        {
            EnsureLookup();
            return !string.IsNullOrEmpty(itemId) && (_lookup.ContainsKey(itemId) || System.Array.IndexOf(BuiltInItems.AllIds, itemId) >= 0);
        }

        public void SetItems(List<ItemDefinition> newItems)
        {
            items = newItems ?? new List<ItemDefinition>();
            _lookup = null;
        }

        public static ItemCatalog CreateDefault()
        {
            var catalog = CreateInstance<ItemCatalog>();
            catalog.name = "ItemCatalog (runtime)";
            catalog.SetItems(BuiltInItems.CreateAll());
            return catalog;
        }

        private void EnsureLookup()
        {
            if (_lookup != null) return;
            _lookup = new Dictionary<string, ItemDefinition>();
            foreach (ItemDefinition item in items)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemId)) _lookup[item.itemId] = item;
            }
        }
    }
}
