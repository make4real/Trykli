using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Save
{
    public enum SkinAccessory
    {
        None,
        Headband,
        Antenna,
        Helmet,
        EyePatch,
        Drip
    }

    /// <summary>Purely cosmetic skins unlocked with collected crystals (GDD 42-43, no pay-to-win).</summary>
    public readonly struct SkinDefinition
    {
        public readonly string Id;
        public readonly string NameKey;
        public readonly Color BodyColor;
        public readonly SkinAccessory Accessory;
        public readonly int CrystalCost;

        public SkinDefinition(string id, Color bodyColor, SkinAccessory accessory, int crystalCost)
        {
            Id = id;
            NameKey = "skin." + id;
            BodyColor = bodyColor;
            Accessory = accessory;
            CrystalCost = crystalCost;
        }
    }

    public static class SkinCatalog
    {
        public const string DefaultSkinId = "classic";

        public static readonly IReadOnlyList<SkinDefinition> All = new List<SkinDefinition>
        {
            new SkinDefinition("classic", new Color(1f, 0.62f, 0.2f), SkinAccessory.None, 0),
            new SkinDefinition("slime", new Color(0.4f, 0.9f, 0.45f), SkinAccessory.Drip, 15),
            new SkinDefinition("ninja", new Color(0.22f, 0.24f, 0.3f), SkinAccessory.Headband, 40),
            new SkinDefinition("robot", new Color(0.65f, 0.72f, 0.8f), SkinAccessory.Antenna, 80),
            new SkinDefinition("astronaut", new Color(0.95f, 0.96f, 1f), SkinAccessory.Helmet, 130),
            new SkinDefinition("pirate", new Color(0.85f, 0.3f, 0.3f), SkinAccessory.EyePatch, 190)
        };

        public static SkinDefinition Get(string id)
        {
            foreach (SkinDefinition skin in All)
            {
                if (skin.Id == id) return skin;
            }

            return All[0];
        }

        public static bool IsUnlocked(string id, int totalCrystals)
        {
            return totalCrystals >= Get(id).CrystalCost;
        }
    }
}
