using System.Collections.Generic;
using Trykli.Data;
using Trykli.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Generates one prefab per level element type (built with the same code as the runtime) and registers them
    /// in Resources/TrykliElementPrefabs. Customize a prefab (art, extra children) and the levels use it.
    /// </summary>
    public static class PrefabGenerator
    {
        public static ElementPrefabCatalog Generate(bool overwriteExisting)
        {
            var entries = new List<ElementPrefabCatalog.Entry>();
            foreach (ElementType type in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
            {
                string folder = FolderFor(type);
                EditorAssetUtility.EnsureFolder(folder);
                string path = $"{folder}/{type}.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null || overwriteExisting)
                {
                    GameObject instance = ElementFactory.Create(ElementDefaults.Create(type), null, WorldTheme.Default, allowPrefab: false);
                    instance.name = type.ToString();
                    instance.transform.position = Vector3.zero;
                    instance.transform.rotation = Quaternion.identity;
                    prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
                    Object.DestroyImmediate(instance);
                }

                entries.Add(new ElementPrefabCatalog.Entry { type = type, prefab = prefab });
            }

            var catalog = EditorAssetUtility.LoadOrCreate<ElementPrefabCatalog>(EditorPaths.ElementPrefabCatalog);
            catalog.SetEntries(entries);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            ElementPrefabCatalog.ClearCache();
            Debug.Log($"[TRYKLI] {entries.Count} element prefabs registered in {EditorPaths.ElementPrefabCatalog}.");
            return catalog;
        }

        private static string FolderFor(ElementType type)
        {
            switch (type)
            {
                case ElementType.Block:
                case ElementType.Crystal:
                case ElementType.Marker:
                    return EditorPaths.PrefabsGameplay;
                case ElementType.Spikes:
                case ElementType.Laser:
                case ElementType.Door:
                case ElementType.StickyZone:
                case ElementType.SlipperyZone:
                    return EditorPaths.PrefabsObstacles;
                default:
                    return EditorPaths.PrefabsMechanics;
            }
        }
    }
}
