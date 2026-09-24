using System.Collections.Generic;
using Trykli.Localization;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>A world: 10 levels sharing a theme and a main mechanic.</summary>
    [CreateAssetMenu(menuName = "TRYKLI/World Data", fileName = "World_00")]
    public sealed class WorldData : ScriptableObject
    {
        public int worldId = 1;
        public string displayNameKey = "";
        public string themeKey = "";
        public string mechanicKey = "";
        [Header("Theme colors")]
        public Color backgroundTop = new Color(0.85f, 0.93f, 1f);
        public Color backgroundBottom = new Color(0.72f, 0.84f, 0.96f);
        public Color blockColor = new Color(0.3f, 0.37f, 0.48f);
        public Color accentColor = new Color(1f, 0.69f, 0.13f);
        public List<LevelData> levels = new List<LevelData>();

        public string DisplayName => string.IsNullOrEmpty(displayNameKey) ? name : Loc.Get(displayNameKey);
    }
}
