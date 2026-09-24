using System.Collections.Generic;

namespace Trykli.Objectives
{
    /// <summary>Computes the stars of a run: one star per fulfilled objective (3 objectives per level).</summary>
    public static class StarEvaluator
    {
        public const int StarsPerLevel = 3;

        public static LevelResult Evaluate(int levelId, IReadOnlyList<StarObjective> objectives, LevelRunStats stats)
        {
            var result = new LevelResult
            {
                levelId = levelId,
                completed = stats.ReachedGoal,
                crystals = stats.CrystalCount,
                time = stats.ElapsedTime,
                attempts = stats.Attempts,
                itemsUsed = stats.TotalItemsUsed
            };

            if (!stats.ReachedGoal) return result;

            int count = objectives != null ? objectives.Count : 0;
            for (int i = 0; i < count && i < StarsPerLevel; i++)
            {
                StarObjective objective = objectives[i];
                if (objective != null && objective.Evaluate(stats))
                {
                    result.objectiveMask |= 1 << i;
                    result.stars++;
                }
            }

            // Reaching the exit always grants at least one star, even with misconfigured data.
            if (result.stars == 0) result.stars = 1;
            return result;
        }
    }
}
