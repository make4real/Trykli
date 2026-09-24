using System;
using System.Collections.Generic;
using System.IO;
using Trykli.Utilities;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Bakes every procedural placeholder sprite to a PNG (Assets/Art/Generated) and registers them in the
    /// SpriteLibrary asset. Artists replace the PNGs (or the library entries) to swap the art.
    /// </summary>
    public static class ArtBaker
    {
        public static SpriteLibrary Bake(bool overwriteExisting)
        {
            EditorAssetUtility.EnsureFolder(EditorPaths.GeneratedArt);
            var ids = (ArtId[])Enum.GetValues(typeof(ArtId));
            var written = new List<string>();
            foreach (ArtId id in ids)
            {
                string path = $"{EditorPaths.GeneratedArt}/{id}.png";
                if (!overwriteExisting && File.Exists(path)) continue;
                Texture2D texture = PlaceholderArt.BuildTexture(id);
                File.WriteAllBytes(path, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                written.Add(path);
            }

            AssetDatabase.Refresh();
            foreach (ArtId id in ids) ConfigureImporter(id, $"{EditorPaths.GeneratedArt}/{id}.png");

            var library = EditorAssetUtility.LoadOrCreate<SpriteLibrary>(EditorPaths.SpriteLibrary);
            var entries = new List<SpriteLibrary.Entry>();
            foreach (ArtId id in ids)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EditorPaths.GeneratedArt}/{id}.png");
                if (sprite != null) entries.Add(new SpriteLibrary.Entry { id = id, sprite = sprite });
            }

            library.SetEntries(entries);
            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
            PlaceholderArt.ClearCache();
            Debug.Log($"[TRYKLI] Placeholder art baked: {written.Count} PNG written, {entries.Count} sprites registered.");
            return library;
        }

        private static void ConfigureImporter(ArtId id, string path)
        {
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) return;
            PlaceholderArt.SpriteSpec spec = PlaceholderArt.GetSpec(id);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.textureType = TextureImporterType.Sprite;
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spritePixelsPerUnit = spec.PixelsPerUnit;
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteBorder = spec.Border;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.alphaIsTransparency = true;
            settings.mipmapEnabled = false;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.filterMode = FilterMode.Bilinear;
            importer.SetTextureSettings(settings);
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
