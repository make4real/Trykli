using UnityEngine;

namespace Trykli.Core
{
    public static class AppInfo
    {
        public static string VersionLabel => "v" + Application.version;

        /// <summary>Key used by the Level Editor to launch a specific level from the editor.</summary>
        public const string EditorTestLevelKey = "TRYKLI_TestLevelId";

        /// <summary>Level to launch directly when entering Play Mode from the Level Editor (editor only).</summary>
        public static int ConsumeEditorTestLevel()
        {
#if UNITY_EDITOR
            int levelId = UnityEditor.SessionState.GetInt(EditorTestLevelKey, 0);
            UnityEditor.SessionState.EraseInt(EditorTestLevelKey);
            return levelId;
#else
            return 0;
#endif
        }
    }
}
