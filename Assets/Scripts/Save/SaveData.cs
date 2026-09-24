using System;
using System.Collections.Generic;

namespace Trykli.Save
{
    /// <summary>
    /// Root of the persistent save file (JSON). Bump <see cref="SaveManager.CurrentVersion"/> and extend
    /// <see cref="SaveMigrator"/> whenever the format changes.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int version = SaveManager.CurrentVersion;
        public int highestUnlockedLevel = 1;
        public List<int> unlockedWorlds = new List<int> { 1 };
        public List<LevelProgress> levels = new List<LevelProgress>();
        public SettingsData settings = new SettingsData();
        public string selectedSkin = SkinCatalog.DefaultSkinId;
        public bool masterAchieved;
        public bool tutorialSeen;
        public long lastSaveUnixTime;

        public LevelProgress FindLevel(int levelId)
        {
            foreach (LevelProgress progress in levels)
            {
                if (progress.levelId == levelId) return progress;
            }

            return null;
        }

        public LevelProgress GetOrCreateLevel(int levelId)
        {
            LevelProgress progress = FindLevel(levelId);
            if (progress != null) return progress;
            progress = new LevelProgress { levelId = levelId };
            levels.Add(progress);
            levels.Sort((a, b) => a.levelId.CompareTo(b.levelId));
            return progress;
        }
    }

    [Serializable]
    public class LevelProgress
    {
        public int levelId;
        public bool completed;
        public int bestStars;
        /// <summary>Union of every objective ever fulfilled (bit per objective).</summary>
        public int objectivesMask;
        public int bestCrystals;
        /// <summary>Best simulation time in seconds (-1 = none).</summary>
        public float bestTime = -1f;
        public int bestItemsUsed = -1;
        public int attempts;
        public int victories;
    }

    [Serializable]
    public class SettingsData
    {
        public bool musicOn = true;
        public bool sfxOn = true;
        public bool vibrationOn = true;
        /// <summary>Language code ("fr", "en"...). Empty = detect from the device.</summary>
        public string language = "";
        public bool fastSimulation;
    }
}
