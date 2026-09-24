using System.Collections.Generic;
using System.IO;
using System.Text;
using Trykli.Data;
using Trykli.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Runs the reference solution of every level in an isolated preview scene with the runtime physics rules
    /// and reports which levels reach the exit. The report is written to Logs/TrykliSolutionReport.txt.
    /// </summary>
    public static class SolutionVerifierTool
    {
        public const string ReportPath = "Logs/TrykliSolutionReport.txt";

        public static List<SolutionVerifier.Result> VerifyAll(bool showDialog)
        {
            return Verify(LevelAssetGenerator.LoadLevelAssets(), showDialog);
        }

        public static List<SolutionVerifier.Result> Verify(IList<LevelData> levels, bool showDialog)
        {
            var results = new List<SolutionVerifier.Result>();
            if (levels.Count == 0)
            {
                if (showDialog) EditorUtility.DisplayDialog("TRYKLI", "No LevelData asset. Run Tools > TRYKLI > Generate Levels first.", "OK");
                return results;
            }

            ItemCatalog catalog = ItemAssetGenerator.LoadCatalog();
            SimulationMode2D previousMode = Physics2D.simulationMode;
            Vector2 previousGravity = Physics2D.gravity;
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.gravity = new Vector2(0f, -GameplayConfig.Instance.gravity);
            Scene preview = EditorSceneManager.NewPreviewScene();
            try
            {
                var root = new GameObject("SolutionVerifier");
                SceneManager.MoveGameObjectToScene(root, preview);
                PhysicsScene2D physics = preview.GetPhysicsScene2D();
                for (int i = 0; i < levels.Count; i++)
                {
                    LevelData level = levels[i];
                    if (EditorUtility.DisplayCancelableProgressBar("TRYKLI", $"Verifying level {level.levelId:000}", i / (float)levels.Count)) break;
                    results.Add(level.solution.Count == 0 && level.availableItems.Count > 0
                        ? new SolutionVerifier.Result { LevelId = level.levelId, Message = "no reference solution" }
                        : SolutionVerifier.Run(level, catalog, root.transform, physics));
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                EditorSceneManager.ClosePreviewScene(preview);
                Physics2D.simulationMode = previousMode;
                Physics2D.gravity = previousGravity;
            }

            WriteReport(results);
            int passed = results.FindAll(r => r.Success).Count;
            string summary = $"{passed} / {results.Count} reference solutions reach the exit.\nReport: {ReportPath}";
            Debug.Log("[TRYKLI] Solution verification: " + summary);
            foreach (SolutionVerifier.Result result in results)
            {
                if (!result.Success) Debug.LogWarning("[TRYKLI] " + result);
            }

            if (showDialog) EditorUtility.DisplayDialog("TRYKLI - Level solutions", summary, "OK");
            return results;
        }

        private static void WriteReport(List<SolutionVerifier.Result> results)
        {
            var builder = new StringBuilder();
            builder.AppendLine("TRYKLI - reference solution verification");
            builder.AppendLine(System.DateTime.Now.ToString("u"));
            builder.AppendLine();
            foreach (SolutionVerifier.Result result in results) builder.AppendLine(result.ToString());
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath) ?? "Logs");
            File.WriteAllText(ReportPath, builder.ToString());
        }
    }
}
