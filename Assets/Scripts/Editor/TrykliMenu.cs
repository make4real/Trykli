using System.IO;
using Trykli.Core;
using Trykli.Save;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>Entry points of the TRYKLI editor tools (menu "Tools > TRYKLI").</summary>
    public static class TrykliMenu
    {
        private const string Root = "Tools/TRYKLI/";

        [MenuItem(Root + "Setup Project", priority = 0)]
        public static void SetupProject() => ProjectSetup.Run(true);

        [MenuItem(Root + "Generate Levels", priority = 1)]
        public static void GenerateLevels()
        {
            int choice = EditorUtility.DisplayDialogComplex("TRYKLI - Generate Levels",
                "Generate LevelData / WorldData assets from Resources/LevelDefinitions/*.json.\n\n" +
                "Overwrite existing LevelData assets? Manual edits made in the Level Editor would be replaced.",
                "Only missing", "Cancel", "Overwrite all");
            if (choice == 1) return;
            LevelAssetGenerator.Report report = LevelAssetGenerator.Generate(choice == 2 ? LevelAssetGenerator.Mode.OverwriteAll : LevelAssetGenerator.Mode.OnlyMissing);
            int errors = report.Issues.FindAll(i => i.Severity == Data.LevelValidator.Severity.Error).Count;
            EditorUtility.DisplayDialog("TRYKLI", $"{report.Created} created, {report.Updated} updated, {report.Skipped} kept.\n" +
                                                  $"{report.Worlds} worlds. Validation: {errors} error(s).", "OK");
        }

        [MenuItem(Root + "Level Editor", priority = 20)]
        public static void OpenLevelEditor() => LevelEditorWindow.Open(0);

        [MenuItem(Root + "Validate Levels", priority = 21)]
        public static void ValidateLevels() => LevelValidationWindow.ShowWindow();

        [MenuItem(Root + "Verify Level Solutions", priority = 22)]
        public static void VerifySolutions() => SolutionVerifierTool.VerifyAll(true);

        [MenuItem(Root + "Export Levels to JSON", priority = 23)]
        public static void ExportLevels()
        {
            if (!EditorUtility.DisplayDialog("TRYKLI", "Overwrite Resources/LevelDefinitions/*.json with the current LevelData assets?", "Export", "Cancel")) return;
            int count = LevelAssetGenerator.ExportAllToJson();
            EditorUtility.DisplayDialog("TRYKLI", $"{count} level(s) exported.", "OK");
        }

        [MenuItem(Root + "Refresh Level Database", priority = 24)]
        public static void RefreshDatabase()
        {
            LevelAssetGenerator.RefreshDatabase();
            Debug.Log("[TRYKLI] Level database refreshed.");
        }

        [MenuItem(Root + "Advanced/Regenerate Scenes", priority = 40)]
        public static void RegenerateScenes()
        {
            if (EditorUtility.DisplayDialog("TRYKLI", "Recreate the 5 scenes (Boot, MainMenu, WorldSelect, LevelSelect, Gameplay)?", "Regenerate", "Cancel"))
                SceneGenerator.Generate(force: true);
        }

        [MenuItem(Root + "Advanced/Rebake Placeholder Art", priority = 41)]
        public static void RebakeArt() => ArtBaker.Bake(overwriteExisting: true);

        [MenuItem(Root + "Advanced/Regenerate Element Prefabs", priority = 42)]
        public static void RegeneratePrefabs()
        {
            if (EditorUtility.DisplayDialog("TRYKLI", "Overwrite the element prefabs (custom changes will be lost)?", "Regenerate", "Cancel"))
                PrefabGenerator.Generate(overwriteExisting: true);
        }

        [MenuItem(Root + "Advanced/Import TMP Essentials", priority = 43)]
        public static void ImportTmp() => TmpEssentialsImporter.ImportIfMissing(true);

        [MenuItem(Root + "Play From Boot", priority = 60)]
        public static void PlayFromBoot()
        {
            if (EditorApplication.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(SceneGenerator.ScenePath(SceneNames.Boot));
            EditorApplication.isPlaying = true;
        }

        [MenuItem(Root + "Save/Open Save Folder", priority = 80)]
        public static void OpenSaveFolder() => EditorUtility.RevealInFinder(Application.persistentDataPath);

        [MenuItem(Root + "Save/Delete Save", priority = 81)]
        public static void DeleteSave()
        {
            if (!EditorUtility.DisplayDialog("TRYKLI", "Delete the local save (progression and settings)?", "Delete", "Cancel")) return;
            new FileSaveStorage().Delete();
            Debug.Log("[TRYKLI] Save deleted: " + Path.Combine(Application.persistentDataPath, FileSaveStorage.DefaultFileName));
        }

        [MenuItem(Root + "Save/Unlock All Levels", priority = 82)]
        public static void UnlockAll()
        {
            var manager = new SaveManager(new FileSaveStorage());
            manager.Load();
            manager.Data.highestUnlockedLevel = 1000;
            for (int world = 1; world <= 10; world++)
            {
                if (!manager.Data.unlockedWorlds.Contains(world)) manager.Data.unlockedWorlds.Add(world);
            }

            manager.Save();
            Debug.Log("[TRYKLI] Every level unlocked (debug).");
        }
    }
}
