namespace Trykli.EditorTools
{
    /// <summary>Asset locations used by the TRYKLI editor tools.</summary>
    public static class EditorPaths
    {
        public const string Resources = "Assets/Resources";
        public const string LevelDefinitions = "Assets/Resources/LevelDefinitions";
        public const string WorldsJson = "Assets/Resources/LevelDefinitions/worlds.json";
        public const string LevelDatabase = "Assets/Resources/TrykliLevelDatabase.asset";
        public const string SpriteLibrary = "Assets/Resources/TrykliSpriteLibrary.asset";
        public const string ElementPrefabCatalog = "Assets/Resources/TrykliElementPrefabs.asset";
        public const string GameplayConfig = "Assets/Resources/TrykliGameplayConfig.asset";
        public const string LevelsFolder = "Assets/ScriptableObjects/Levels";
        public const string WorldsFolder = "Assets/ScriptableObjects/Worlds";
        public const string ItemsFolder = "Assets/ScriptableObjects/Items";
        public const string ItemCatalog = "Assets/ScriptableObjects/Items/ItemCatalog.asset";
        public const string GeneratedArt = "Assets/Art/Generated";
        public const string PrefabsGameplay = "Assets/Prefabs/Gameplay";
        public const string PrefabsObstacles = "Assets/Prefabs/Obstacles";
        public const string PrefabsMechanics = "Assets/Prefabs/Mechanics";
        public const string Scenes = "Assets/Scenes";

        public static string LevelAsset(int levelId) => $"{LevelsFolder}/Level_{levelId:000}.asset";
        public static string WorldAsset(int worldId) => $"{WorldsFolder}/World_{worldId:00}.asset";
        public static string LevelJson(int levelId) => $"{LevelDefinitions}/level_{levelId:000}.json";
    }
}
