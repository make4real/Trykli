using Trykli.Objectives;

namespace Trykli.Save
{
    /// <summary>
    /// Pure progression rules: unlocking (finishing a level unlocks the next one, finishing the last level of a
    /// world unlocks the next world), best results, stars and crystal totals.
    /// </summary>
    public sealed class ProgressionService
    {
        public readonly struct RecordOutcome
        {
            public readonly bool FirstCompletion;
            public readonly bool NewBestStars;
            public readonly int UnlockedLevelId;
            public readonly int UnlockedWorldId;
            public readonly bool GameCompleted;

            public RecordOutcome(bool firstCompletion, bool newBestStars, int unlockedLevelId, int unlockedWorldId, bool gameCompleted)
            {
                FirstCompletion = firstCompletion;
                NewBestStars = newBestStars;
                UnlockedLevelId = unlockedLevelId;
                UnlockedWorldId = unlockedWorldId;
                GameCompleted = gameCompleted;
            }
        }

        private readonly SaveData _data;

        public ProgressionService(SaveData data, int totalLevels, int levelsPerWorld)
        {
            _data = data;
            TotalLevels = totalLevels > 0 ? totalLevels : 100;
            LevelsPerWorld = levelsPerWorld > 0 ? levelsPerWorld : 10;
        }

        public int TotalLevels { get; }
        public int LevelsPerWorld { get; }
        public int WorldCount => (TotalLevels + LevelsPerWorld - 1) / LevelsPerWorld;
        public int HighestUnlockedLevel => _data.highestUnlockedLevel;

        public int WorldOf(int levelId) => (levelId - 1) / LevelsPerWorld + 1;
        public int FirstLevelOf(int worldId) => (worldId - 1) * LevelsPerWorld + 1;
        public int LevelNumberInWorld(int levelId) => (levelId - 1) % LevelsPerWorld + 1;

        public bool IsLevelUnlocked(int levelId) => levelId >= 1 && levelId <= TotalLevels && levelId <= _data.highestUnlockedLevel;

        public bool IsLevelCompleted(int levelId)
        {
            LevelProgress progress = _data.FindLevel(levelId);
            return progress != null && progress.completed;
        }

        public bool IsWorldUnlocked(int worldId)
        {
            return worldId >= 1 && worldId <= WorldCount && (_data.unlockedWorlds.Contains(worldId) || IsLevelUnlocked(FirstLevelOf(worldId)));
        }

        public int GetStars(int levelId)
        {
            LevelProgress progress = _data.FindLevel(levelId);
            return progress != null ? progress.bestStars : 0;
        }

        public int GetCrystals(int levelId)
        {
            LevelProgress progress = _data.FindLevel(levelId);
            return progress != null ? progress.bestCrystals : 0;
        }

        public int GetWorldStars(int worldId)
        {
            int total = 0;
            int first = FirstLevelOf(worldId);
            for (int id = first; id < first + LevelsPerWorld; id++) total += GetStars(id);
            return total;
        }

        public int GetWorldCompletedCount(int worldId)
        {
            int total = 0;
            int first = FirstLevelOf(worldId);
            for (int id = first; id < first + LevelsPerWorld; id++)
            {
                if (IsLevelCompleted(id)) total++;
            }

            return total;
        }

        public int TotalStars
        {
            get
            {
                int total = 0;
                foreach (LevelProgress level in _data.levels) total += level.bestStars;
                return total;
            }
        }

        public int TotalCrystals
        {
            get
            {
                int total = 0;
                foreach (LevelProgress level in _data.levels) total += level.bestCrystals;
                return total;
            }
        }

        public int CompletedLevels
        {
            get
            {
                int total = 0;
                foreach (LevelProgress level in _data.levels)
                {
                    if (level.completed) total++;
                }

                return total;
            }
        }

        /// <summary>Level started by the "JOUER" button: first unlocked level not yet completed.</summary>
        public int GetContinueLevelId()
        {
            for (int id = 1; id <= _data.highestUnlockedLevel && id <= TotalLevels; id++)
            {
                if (!IsLevelCompleted(id)) return id;
            }

            return System.Math.Min(_data.highestUnlockedLevel, TotalLevels);
        }

        public void RecordAttempt(int levelId)
        {
            _data.GetOrCreateLevel(levelId).attempts++;
        }

        public RecordOutcome RecordResult(LevelResult result)
        {
            if (result == null || !result.completed) return default;

            LevelProgress progress = _data.GetOrCreateLevel(result.levelId);
            bool firstCompletion = !progress.completed;
            bool newBestStars = result.stars > progress.bestStars;

            progress.completed = true;
            progress.victories++;
            progress.objectivesMask |= result.objectiveMask;
            if (newBestStars) progress.bestStars = result.stars;
            if (result.crystals > progress.bestCrystals) progress.bestCrystals = result.crystals;
            if (progress.bestTime < 0f || result.time < progress.bestTime) progress.bestTime = result.time;
            if (progress.bestItemsUsed < 0 || result.itemsUsed < progress.bestItemsUsed) progress.bestItemsUsed = result.itemsUsed;

            int unlockedLevel = -1;
            int unlockedWorld = -1;
            int next = result.levelId + 1;
            if (next <= TotalLevels && next > _data.highestUnlockedLevel)
            {
                _data.highestUnlockedLevel = next;
                unlockedLevel = next;
                int nextWorld = WorldOf(next);
                if (nextWorld != WorldOf(result.levelId) && !_data.unlockedWorlds.Contains(nextWorld))
                {
                    _data.unlockedWorlds.Add(nextWorld);
                    unlockedWorld = nextWorld;
                }
            }

            bool gameCompleted = result.levelId >= TotalLevels && !_data.masterAchieved;
            if (gameCompleted) _data.masterAchieved = true;

            return new RecordOutcome(firstCompletion, newBestStars, unlockedLevel, unlockedWorld, gameCompleted);
        }
    }
}
