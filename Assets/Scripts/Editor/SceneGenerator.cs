using System;
using System.Collections.Generic;
using Trykli.Core;
using Trykli.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Creates the five scenes (each contains only its scene controller; everything else is built at runtime)
    /// and registers them in the build settings.
    /// </summary>
    public static class SceneGenerator
    {
        private static readonly (string scene, Type controller)[] Scenes =
        {
            (SceneNames.Boot, typeof(BootController)),
            (SceneNames.MainMenu, typeof(MainMenuController)),
            (SceneNames.WorldSelect, typeof(WorldSelectController)),
            (SceneNames.LevelSelect, typeof(LevelSelectController)),
            (SceneNames.Gameplay, typeof(GameplayController))
        };

        public static string ScenePath(string sceneName) => $"{EditorPaths.Scenes}/{sceneName}.unity";

        /// <summary>Regenerates missing or invalid scenes (all of them when <paramref name="force"/> is set).</summary>
        public static void Generate(bool force)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorAssetUtility.EnsureFolder(EditorPaths.Scenes);
            string previous = SceneManager.GetActiveScene().path;
            foreach ((string sceneName, Type controller) in Scenes)
            {
                string path = ScenePath(sceneName);
                if (!force && IsValid(path, controller)) continue;
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var root = new GameObject(sceneName + "Controller");
                root.AddComponent(controller);
                EditorSceneManager.SaveScene(scene, path);
                Debug.Log("[TRYKLI] Scene generated: " + path);
            }

            UpdateBuildSettings();
            if (!string.IsNullOrEmpty(previous) && System.IO.File.Exists(previous)) EditorSceneManager.OpenScene(previous);
        }

        public static void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();
            foreach ((string sceneName, Type _) in Scenes) scenes.Add(new EditorBuildSettingsScene(ScenePath(sceneName), true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static bool IsValid(string path, Type controller)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) return false;
            // A scene is considered valid when its controller script is referenced (checked in the YAML).
            MonoScript script = FindScript(controller);
            if (script == null) return false;
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script));
            string text = System.IO.File.ReadAllText(path);
            return text.Contains(guid);
        }

        private static MonoScript FindScript(Type type)
        {
            foreach (string guid in AssetDatabase.FindAssets(type.Name + " t:MonoScript"))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                if (script != null && script.GetClass() == type) return script;
            }

            return null;
        }
    }
}
