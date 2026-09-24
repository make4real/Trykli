using System.Collections.Generic;
using System.IO;
using Trykli.Data;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Deterministic generation of WorldData / LevelData assets and of the LevelDatabase from the JSON level
    /// definitions (Resources/LevelDefinitions). Also exports LevelData back to JSON.
    /// </summary>
    public static class LevelAssetGenerator
    {
        public enum Mode
        {
            OnlyMissing,
            OverwriteAll
        }

        public struct Report
        {
            public int Created;
            public int Updated;
            public int Skipped;
            public int Worlds;
            public List<LevelValidator.Issue> Issues;
        }

        public static List<LevelDefinition> LoadDefinitions()
        {
            var definitions = new List<LevelDefinition>();
            if (!Directory.Exists(EditorPaths.LevelDefinitions)) return definitions;
            foreach (string file in Directory.GetFiles(EditorPaths.LevelDefinitions, "level_*.json"))
            {
                LevelDefinition definition = LevelDefinitionConverter.ParseLevel(File.ReadAllText(file));
                if (definition != null) definitions.Add(definition);
                else Debug.LogError("[TRYKLI] Could not parse " + file);
            }

            definitions.Sort((a, b) => a.levelId.CompareTo(b.levelId));
            return definitions;
        }

        public static List<WorldDefinition> LoadWorldDefinitions()
        {
            if (!File.Exists(EditorPaths.WorldsJson)) return new List<WorldDefinition>();
            List<WorldDefinition> worlds = LevelDefinitionConverter.ParseWorlds(File.ReadAllText(EditorPaths.WorldsJson)).worlds;
            worlds.Sort((a, b) => a.worldId.CompareTo(b.worldId));
            return worlds;
        }

        public static Report Generate(Mode mode)
        {
            var report = new Report { Issues = new List<LevelValidator.Issue>() };
            ItemCatalog catalog = ItemAssetGenerator.LoadCatalog();
            List<LevelDefinition> definitions = LoadDefinitions();
            List<WorldDefinition> worldDefinitions = LoadWorldDefinitions();
            EditorAssetUtility.EnsureFolder(EditorPaths.LevelsFolder);
            EditorAssetUtility.EnsureFolder(EditorPaths.WorldsFolder);

            try
            {
                AssetDatabase.StartAssetEditing();
                var levels = new List<LevelData>();
                for (int i = 0; i < definitions.Count; i++)
                {
                    LevelDefinition definition = definitions[i];
                    EditorUtility.DisplayProgressBar("TRYKLI", $"Level {definition.levelId:000}", i / (float)Mathf.Max(1, definitions.Count));
                    string path = EditorPaths.LevelAsset(definition.levelId);
                    var level = EditorAssetUtility.LoadOrCreate<LevelData>(path, out bool created);
                    if (created || mode == Mode.OverwriteAll)
                    {
                        LevelDefinitionConverter.Apply(definition, level, catalog);
                        EditorUtility.SetDirty(level);
                        if (created) report.Created++;
                        else report.Updated++;
                    }
                    else
                    {
                        report.Skipped++;
                    }

                    levels.Add(level);
                }

                var worlds = new List<WorldData>();
                foreach (WorldDefinition definition in worldDefinitions)
                {
                    var world = EditorAssetUtility.LoadOrCreate<WorldData>(EditorPaths.WorldAsset(definition.worldId), out bool created);
                    if (created || mode == Mode.OverwriteAll) LevelDefinitionConverter.Apply(definition, world);
                    world.levels = levels.FindAll(level => level.worldId == definition.worldId);
                    world.levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));
                    EditorUtility.SetDirty(world);
                    worlds.Add(world);
                }

                report.Worlds = worlds.Count;
                var database = EditorAssetUtility.LoadOrCreate<LevelDatabase>(EditorPaths.LevelDatabase);
                database.SetContent(catalog, worlds, levels);
                EditorUtility.SetDirty(database);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            report.Issues = LevelValidator.ValidateAll(definitions, Mathf.Max(1, worldDefinitions.Count), 10, catalog);
            Debug.Log($"[TRYKLI] Levels generated: {report.Created} created, {report.Updated} updated, {report.Skipped} kept, {report.Worlds} worlds.");
            return report;
        }

        /// <summary>Adds the LevelData assets that are not referenced yet (e.g. a new level 101) to the database.</summary>
        public static LevelDatabase RefreshDatabase()
        {
            ItemCatalog catalog = ItemAssetGenerator.LoadCatalog();
            var levels = new List<LevelData>();
            foreach (string guid in AssetDatabase.FindAssets("t:LevelData"))
            {
                var level = AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid));
                if (level != null) levels.Add(level);
            }

            var worlds = new List<WorldData>();
            foreach (string guid in AssetDatabase.FindAssets("t:WorldData"))
            {
                var world = AssetDatabase.LoadAssetAtPath<WorldData>(AssetDatabase.GUIDToAssetPath(guid));
                if (world == null) continue;
                world.levels = levels.FindAll(level => level.worldId == world.worldId);
                world.levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));
                EditorUtility.SetDirty(world);
                worlds.Add(world);
            }

            var database = EditorAssetUtility.LoadOrCreate<LevelDatabase>(EditorPaths.LevelDatabase);
            database.SetContent(catalog, worlds, levels);
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            return database;
        }

        public static void ExportToJson(LevelData level)
        {
            EditorAssetUtility.EnsureFolder(EditorPaths.LevelDefinitions);
            LevelDefinition definition = LevelDefinitionConverter.ToDefinition(level);
            File.WriteAllText(EditorPaths.LevelJson(level.levelId), LevelDefinitionConverter.ToJson(definition));
        }

        public static int ExportAllToJson()
        {
            int count = 0;
            foreach (string guid in AssetDatabase.FindAssets("t:LevelData"))
            {
                var level = AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid));
                if (level == null) continue;
                ExportToJson(level);
                count++;
            }

            AssetDatabase.Refresh();
            return count;
        }

        public static List<LevelData> LoadLevelAssets()
        {
            var levels = new List<LevelData>();
            var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(EditorPaths.LevelDatabase);
            if (database != null)
            {
                foreach (LevelData level in database.Levels)
                {
                    if (level != null) levels.Add(level);
                }
            }

            if (levels.Count == 0)
            {
                foreach (string guid in AssetDatabase.FindAssets("t:LevelData"))
                {
                    var level = AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid));
                    if (level != null) levels.Add(level);
                }
            }

            levels.Sort((a, b) => a.levelId.CompareTo(b.levelId));
            return levels;
        }
    }
}
