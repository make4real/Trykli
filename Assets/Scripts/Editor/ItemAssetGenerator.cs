using System.Collections.Generic;
using Trykli.Data;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>Creates one ItemDefinition asset per built-in item and the ItemCatalog.</summary>
    public static class ItemAssetGenerator
    {
        public static ItemCatalog Generate()
        {
            EditorAssetUtility.EnsureFolder(EditorPaths.ItemsFolder);
            var items = new List<ItemDefinition>();
            foreach (string itemId in BuiltInItems.AllIds)
            {
                string path = $"{EditorPaths.ItemsFolder}/Item_{itemId}.asset";
                var asset = EditorAssetUtility.LoadOrCreate<ItemDefinition>(path, out bool created);
                if (created)
                {
                    ItemDefinition defaults = BuiltInItems.Create(itemId);
                    asset.CopyFrom(defaults);
                    Object.DestroyImmediate(defaults);
                    EditorUtility.SetDirty(asset);
                }

                items.Add(asset);
            }

            var catalog = EditorAssetUtility.LoadOrCreate<ItemCatalog>(EditorPaths.ItemCatalog);
            catalog.SetItems(items);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            return catalog;
        }

        public static ItemCatalog LoadCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ItemCatalog>(EditorPaths.ItemCatalog);
            return catalog != null ? catalog : Generate();
        }
    }
}
