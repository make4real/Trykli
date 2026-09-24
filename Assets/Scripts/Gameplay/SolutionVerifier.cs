using System.Collections.Generic;
using Trykli.Data;
using Trykli.Objectives;
using Trykli.Save;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Headless run of a level with its reference solution: builds the level, places the solution items,
    /// simulates with the exact runtime rules (same fixed step, same mechanisms) and reports the outcome.
    /// Used by "Tools > TRYKLI > Verify Level Solutions" and by the PlayMode tests to tune the 100 levels.
    /// </summary>
    public static class SolutionVerifier
    {
        public struct Result
        {
            public int LevelId;
            public bool Success;
            public FailureReason Failure;
            public float Time;
            public int Stars;
            public int Crystals;
            public string Message;

            public override string ToString()
            {
                return Success
                    ? $"Level {LevelId:000}: OK in {Time:0.00}s, {Stars} star(s), {Crystals} crystal(s)"
                    : $"Level {LevelId:000}: FAILED ({Failure}) after {Time:0.00}s {Message}";
            }
        }

        /// <summary>
        /// Runs the level synchronously. <paramref name="parent"/> must belong to the scene simulated by
        /// <paramref name="physicsScene"/>. Created objects are destroyed afterwards when <paramref name="cleanup"/> is set.
        /// </summary>
        public static Result Run(LevelData level, ItemCatalog catalog, Transform parent, PhysicsScene2D physicsScene, bool cleanup = true)
        {
            var result = new Result { LevelId = level.levelId };
            GameplayConfig config = GameplayConfig.Instance;
            WorldTheme theme = WorldTheme.Default;

            LevelContext context = LevelBuilder.Build(level, theme, parent, catalog);
            TrykliController trykli = TrykliController.Create(context.transform, context.SpawnPosition, config, SkinCatalog.Get(SkinCatalog.DefaultSkinId));
            context.Trykli = trykli;

            var units = new Dictionary<string, int>();
            var pieces = new Dictionary<string, int>();
            foreach (SolutionStep step in level.solution)
            {
                ItemDefinition item = catalog.Get(step.itemId);
                if (item == null || step.zoneIndex < 0 || step.zoneIndex >= context.Zones.Count)
                {
                    result.Message = $"invalid solution step ({step.itemId} in zone {step.zoneIndex})";
                    Cleanup(context, cleanup);
                    return result;
                }

                pieces.TryGetValue(item.itemId, out int placedPieces);
                string endpoint = item.kind == ItemKind.Portal ? (placedPieces % 2 == 0 ? "A" : "B") : string.Empty;
                pieces[item.itemId] = placedPieces + 1;
                units[item.itemId] = item.UnitsForPieces(placedPieces + 1);

                PlacementZoneData zone = context.Zones[step.zoneIndex].Data;
                Vector2 position = zone.shape == ZoneShape.Point ? zone.position : step.position;
                float rotation = Placement.PlacementRules.ClampRotation(zone, step.rotation, item.rotationStep, item.rotatable);
                ElementData data = ElementFactory.DataForItem(item, position, rotation, endpoint);
                ElementFactory.Create(data, context.PlacedRoot, theme);
            }

            bool finished = false;
            context.GoalReached += () =>
            {
                finished = true;
                result.Success = true;
                context.StopSimulation();
            };
            context.FailureRequested += reason =>
            {
                finished = true;
                result.Failure = reason;
                context.StopSimulation();
            };

            var runner = context.gameObject.AddComponent<SimulationRunner>();
            runner.Configure(config.physicsStep, config.maxStepsPerFrame);
            runner.SetPhysicsScene(physicsScene);
            context.Tracker.Begin(1, context.Crystals.Count, units);
            context.StartSimulation();
            runner.Begin(context);

            float limit = level.simulationTimeLimit > 0f ? level.simulationTimeLimit : config.defaultTimeLimit;
            int maxSteps = Mathf.CeilToInt(limit / runner.StepDuration) + 1;
            for (int i = 0; i < maxSteps && !finished; i++) runner.Step();
            if (!finished) result.Failure = FailureReason.Timeout;

            result.Time = context.SimulationTime;
            LevelRunStats stats = context.Tracker.Stats;
            stats.ReachedGoal = result.Success;
            stats.ElapsedTime = context.SimulationTime;
            result.Crystals = stats.CrystalCount;
            if (result.Success) result.Stars = StarEvaluator.Evaluate(level.levelId, level.starObjectives, stats).stars;
            Cleanup(context, cleanup);
            return result;
        }

        private static void Cleanup(LevelContext context, bool cleanup)
        {
            if (!cleanup || context == null) return;
            if (Application.isPlaying) Object.Destroy(context.gameObject);
            else Object.DestroyImmediate(context.gameObject);
        }
    }
}
