using Trykli.Data;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// "Tools > TRYKLI > Setup Project": one click configuration of the whole project. Idempotent and safe to
    /// run again (existing assets are kept, only missing ones are created).
    /// </summary>
    public static class ProjectSetup
    {
        public static void Run(bool interactive)
        {
            try
            {
                Step(0.05f, "TextMeshPro essentials");
                TmpEssentialsImporter.ImportIfMissing(false);

                Step(0.15f, "Player settings");
                bool restartRequired = PlayerSettingsConfigurator.Apply();

                Step(0.25f, "Folders & config");
                EnsureFolders();
                EditorAssetUtility.LoadOrCreate<GameplayConfig>(EditorPaths.GameplayConfig);

                Step(0.35f, "Placeholder art");
                ArtBaker.Bake(overwriteExisting: false);

                Step(0.5f, "Items");
                ItemAssetGenerator.Generate();

                Step(0.6f, "Prefabs");
                PrefabGenerator.Generate(overwriteExisting: false);

                Step(0.75f, "Levels");
                LevelAssetGenerator.Report report = LevelAssetGenerator.Generate(LevelAssetGenerator.Mode.OnlyMissing);

                EditorUtility.ClearProgressBar();
                SceneGenerator.Generate(force: false);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                int errors = report.Issues.FindAll(i => i.Severity == LevelValidator.Severity.Error).Count;
                string message = "TRYKLI project ready.\n\n" +
                                 $"Levels: {report.Created} created, {report.Skipped} kept ({report.Worlds} worlds).\n" +
                                 $"Validation: {errors} error(s), {report.Issues.Count - errors} warning(s).\n\n" +
                                 "Open Assets/Scenes/Boot.unity and press Play." +
                                 (restartRequired ? "\n\nRestart Unity to apply the input settings." : string.Empty);
                Debug.Log("[TRYKLI] " + message);
                if (interactive) EditorUtility.DisplayDialog("TRYKLI", message, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void Step(float progress, string label)
        {
            EditorUtility.DisplayProgressBar("TRYKLI Setup", label, progress);
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Assets/Art/Generated", "Assets/Audio", "Assets/Animations", "Assets/Materials", EditorPaths.PrefabsGameplay,
                EditorPaths.PrefabsObstacles, EditorPaths.PrefabsMechanics, "Assets/Prefabs/UI", EditorPaths.LevelsFolder,
                EditorPaths.WorldsFolder, EditorPaths.ItemsFolder, EditorPaths.LevelDefinitions, "Assets/Resources/Localization",
                "Assets/VFX", "Assets/Fonts", EditorPaths.Scenes
            };
            foreach (string folder in folders) EditorAssetUtility.EnsureFolder(folder);
        }
    }
}
