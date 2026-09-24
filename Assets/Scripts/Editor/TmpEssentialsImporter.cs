using System.IO;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Imports the TextMeshPro Essential Resources automatically when they are missing (fonts, shaders, settings),
    /// so the code-built UI displays text on the very first Play.
    /// </summary>
    [InitializeOnLoad]
    public static class TmpEssentialsImporter
    {
        private static readonly string[] PackagePaths =
        {
            "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage",
            "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage"
        };

        private const string SessionKey = "TRYKLI_TmpImportChecked";

        static TmpEssentialsImporter()
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += () => ImportIfMissing(false);
        }

        public static bool AreEssentialsPresent()
        {
            return AssetDatabase.FindAssets("t:TMP_Settings").Length > 0;
        }

        public static bool ImportIfMissing(bool interactive)
        {
            if (AreEssentialsPresent()) return true;
            foreach (string path in PackagePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath)) continue;
                AssetDatabase.ImportPackage(path, interactive);
                Debug.Log("[TRYKLI] TextMeshPro Essential Resources imported.");
                return true;
            }

            Debug.LogWarning("[TRYKLI] TMP Essential Resources package not found. Use Window > TextMeshPro > Import TMP Essential Resources.");
            return false;
        }
    }
}
