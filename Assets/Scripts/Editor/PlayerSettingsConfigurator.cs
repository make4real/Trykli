using UnityEditor;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Build;
#endif
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>Mobile project settings: portrait, identifiers, IL2CPP / ARM64, legacy input, script-driven 2D physics.</summary>
    public static class PlayerSettingsConfigurator
    {
        public const string CompanyName = "make4real";
        public const string ProductName = "TRYKLI";
        public const string ApplicationId = "com.make4real.trykli";

        /// <summary>Returns true when a change requires an editor restart (input handling).</summary>
        public static bool Apply()
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            if (string.IsNullOrEmpty(PlayerSettings.bundleVersion) || PlayerSettings.bundleVersion == "0.1") PlayerSettings.bundleVersion = "0.1.0";

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.runInBackground = false;

#if UNITY_2021_2_OR_NEWER
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, ApplicationId);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
#else
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, ApplicationId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, ApplicationId);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
#endif
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Physics is stepped by the SimulationRunner (deterministic fixed step).
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.gravity = new Vector2(0f, -9.81f);

            return EnsureLegacyInput();
        }

        /// <summary>
        /// The game reads touches / mouse through the legacy Input Manager. "Both" is accepted; "Input System only"
        /// is switched to "Both". Returns true when the setting changed (editor restart required).
        /// </summary>
        public static bool EnsureLegacyInput()
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0) return false;
            var serialized = new SerializedObject(assets[0]);
            SerializedProperty property = serialized.FindProperty("activeInputHandler");
            if (property == null || property.intValue != 1) return false;
            property.intValue = 2;
            serialized.ApplyModifiedProperties();
            Debug.LogWarning("[TRYKLI] Active Input Handling set to 'Both' (the game uses the legacy Input Manager). Restart the editor.");
            return true;
        }
    }
}
