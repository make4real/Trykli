using System.IO;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    public static class EditorAssetUtility
    {
        /// <summary>Creates every folder of <paramref name="path"/> (e.g. "Assets/Art/Generated").</summary>
        public static void EnsureFolder(string path)
        {
            path = path.Replace('\\', '/').TrimEnd('/');
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        /// <summary>Loads the asset at <paramref name="path"/> or creates it.</summary>
        public static T LoadOrCreate<T>(string path, out bool created) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            created = asset == null;
            if (asset != null) return asset;
            EnsureFolder(Path.GetDirectoryName(path));
            asset = ScriptableObject.CreateInstance<T>();
            asset.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            return LoadOrCreate<T>(path, out _);
        }
    }
}
