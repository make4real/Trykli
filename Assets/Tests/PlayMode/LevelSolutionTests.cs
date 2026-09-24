using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Trykli.Data;
using Trykli.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Trykli.Tests
{
    /// <summary>
    /// Plays every level with its reference solution in an isolated 2D physics scene (the same code path as
    /// "Tools > TRYKLI > Verify Level Solutions"). This is the real Unity physics, unlike the Python
    /// authoring simulator: a failure here means the level needs tuning in the Level Editor.
    /// </summary>
    public class LevelSolutionTests
    {
        private static IEnumerable<int> WorldIds()
        {
            for (int world = 1; world <= 10; world++) yield return world;
        }

        [UnityTest]
        [Timeout(600000)]
        public IEnumerator ReferenceSolutionsReachTheExit([ValueSource(nameof(WorldIds))] int worldId)
        {
            LevelDatabase database = LevelDatabase.BuildFromDefinitions();
            ItemCatalog catalog = database.Items;
            Scene scene = SceneManager.CreateScene("TrykliSolutionTest_" + worldId, new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            var failures = new List<string>();
            try
            {
                var root = new GameObject("VerifierRoot");
                SceneManager.MoveGameObjectToScene(root, scene);
                PhysicsScene2D physics = scene.GetPhysicsScene2D();
                foreach (LevelData level in database.GetLevelsOfWorld(worldId))
                {
                    SolutionVerifier.Result result = SolutionVerifier.Run(level, catalog, root.transform, physics);
                    if (!result.Success) failures.Add(result.ToString());
                    yield return null;
                }
            }
            finally
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            Assert.IsEmpty(failures, string.Join("\n", failures));
        }
    }
}
