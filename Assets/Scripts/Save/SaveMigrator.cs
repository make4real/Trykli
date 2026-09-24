using System.Collections.Generic;

namespace Trykli.Save
{
    /// <summary>
    /// Upgrades old save files to the current format and repairs invalid values.
    /// Add a "case" per version when the format evolves.
    /// </summary>
    public static class SaveMigrator
    {
        public static SaveData Migrate(SaveData data)
        {
            // Version 0 = files written before versioning existed: same fields, just stamp the version.
            if (data.version < 1) data.version = 1;

            Repair(data);
            return data;
        }

        private static void Repair(SaveData data)
        {
            if (data.levels == null) data.levels = new List<LevelProgress>();
            if (data.unlockedWorlds == null) data.unlockedWorlds = new List<int>();
            if (!data.unlockedWorlds.Contains(1)) data.unlockedWorlds.Add(1);
            if (data.settings == null) data.settings = new SettingsData();
            if (string.IsNullOrEmpty(data.selectedSkin)) data.selectedSkin = SkinCatalog.DefaultSkinId;
            if (data.highestUnlockedLevel < 1) data.highestUnlockedLevel = 1;

            data.levels.RemoveAll(level => level == null || level.levelId <= 0);
            foreach (LevelProgress level in data.levels)
            {
                if (level.bestStars < 0) level.bestStars = 0;
                if (level.bestStars > 3) level.bestStars = 3;
                if (level.bestCrystals < 0) level.bestCrystals = 0;
                if (level.bestStars > 0) level.completed = true;
            }
        }
    }
}
