using System;
using System.Collections.Generic;
using Trykli.Objectives;

namespace Trykli.Data
{
    /// <summary>
    /// JSON representation of a level (Resources/LevelDefinitions/level_XXX.json).
    /// It is the portable source format: "Tools > TRYKLI > Generate Levels" converts it into LevelData
    /// assets, and the runtime uses it directly when no generated asset exists.
    /// </summary>
    [Serializable]
    public class LevelDefinition
    {
        public int levelId;
        public int worldId;
        public int levelNumber;
        public string nameKey = "";
        public int difficulty = 1;
        public bool isBoss;
        public float timeLimit = 15f;
        public TutorialKind tutorial;
        public LevelLayout layout = new LevelLayout();
        public List<ItemCount> inventory = new List<ItemCount>();
        public List<ObjectiveDefinition> objectives = new List<ObjectiveDefinition>();
        public List<SolutionStep> solution = new List<SolutionStep>();
        public List<HintData> hints = new List<HintData>();
        public string tipKey = "";
        public CameraSettingsData camera = new CameraSettingsData();
        public string notes = "";
    }

    [Serializable]
    public class WorldDefinition
    {
        public int worldId;
        public string nameKey = "";
        public string themeKey = "";
        public string mechanicKey = "";
        public string backgroundTop = "#DCEEFF";
        public string backgroundBottom = "#B8D6F5";
        public string blockColor = "#4C5E7A";
        public string accentColor = "#FFB020";
    }

    [Serializable]
    public class WorldCollectionDefinition
    {
        public List<WorldDefinition> worlds = new List<WorldDefinition>();
    }
}
