using System.IO;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>On the first opening of the project, offers to run "Tools > TRYKLI > Setup Project".</summary>
    [InitializeOnLoad]
    public static class FirstOpenWelcome
    {
        private static readonly string PrefKey = "TRYKLI_SetupOffered_" + Application.dataPath.GetHashCode();

        static FirstOpenWelcome()
        {
            if (EditorPrefs.GetBool(PrefKey, false) || Application.isBatchMode) return;
            EditorApplication.delayCall += Offer;
        }

        private static void Offer()
        {
            EditorPrefs.SetBool(PrefKey, true);
            if (File.Exists(EditorPaths.LevelDatabase)) return;
            bool run = EditorUtility.DisplayDialog("TRYKLI",
                "Welcome to TRYKLI!\n\nRun the one-click setup now? It bakes the placeholder art, creates the item / level / world " +
                "assets, the prefabs and configures the mobile settings.\n\nThe game is also playable without it " +
                "(levels are then loaded from JSON).", "Setup Project", "Later");
            if (run) ProjectSetup.Run(true);
        }
    }
}
