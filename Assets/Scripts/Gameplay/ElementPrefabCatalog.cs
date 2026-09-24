using System;
using System.Collections.Generic;
using Trykli.Data;
using Trykli.Mechanics;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Optional registry (Resources/TrykliElementPrefabs) of prefabs used instead of procedural objects.
    /// Each prefab must contain the component matching its element type.
    /// </summary>
    [CreateAssetMenu(menuName = "TRYKLI/Element Prefab Catalog", fileName = "TrykliElementPrefabs")]
    public sealed class ElementPrefabCatalog : ScriptableObject
    {
        public const string ResourcePath = "TrykliElementPrefabs";

        [Serializable]
        public struct Entry
        {
            public ElementType type;
            public GameObject prefab;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();
        [Tooltip("Disable to always build elements procedurally.")]
        [SerializeField] private bool usePrefabs = true;

        private static ElementPrefabCatalog _instance;
        private static bool _checked;

        public IReadOnlyList<Entry> Entries => entries;

        public void SetEntries(List<Entry> newEntries)
        {
            entries = newEntries ?? new List<Entry>();
        }

        public static GameObject GetPrefab(ElementType type)
        {
            if (!_checked)
            {
                _checked = true;
                _instance = Resources.Load<ElementPrefabCatalog>(ResourcePath);
            }

            if (_instance == null || !_instance.usePrefabs) return null;
            foreach (Entry entry in _instance.entries)
            {
                if (entry.type == type && entry.prefab != null) return entry.prefab;
            }

            return null;
        }

        public static void ClearCache()
        {
            _instance = null;
            _checked = false;
        }
    }
}
