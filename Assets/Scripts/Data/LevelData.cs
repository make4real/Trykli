using System;
using System.Collections.Generic;
using Trykli.Localization;
using Trykli.Objectives;
using UnityEngine;

namespace Trykli.Data
{
    [Serializable]
    public class ItemStack
    {
        public ItemDefinition item;
        [Min(0)] public int count = 1;
    }

    /// <summary>
    /// Data-driven definition of one level. The Gameplay scene builds the level from this asset:
    /// either from <see cref="layout"/> (default) or from <see cref="levelPrefab"/> when assigned.
    /// </summary>
    [CreateAssetMenu(menuName = "TRYKLI/Level Data", fileName = "Level_000")]
    public sealed class LevelData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Global level number, 1..100.")]
        public int levelId = 1;
        [Tooltip("World number, 1..10.")]
        public int worldId = 1;
        [Tooltip("Level number inside the world, 1..10.")]
        public int levelNumber = 1;
        public string displayNameKey = "";
        [Range(1, 10)] public int difficulty = 1;
        public bool isBoss;
        public TutorialKind tutorial = TutorialKind.None;

        [Header("Simulation")]
        [Tooltip("Maximum simulation time in seconds before failure.")]
        [Min(1f)] public float simulationTimeLimit = 15f;

        [Header("Content")]
        [Tooltip("Optional prefab used instead of the layout. It must contain a spawn point and a goal.")]
        public GameObject levelPrefab;
        public LevelLayout layout = new LevelLayout();
        public List<ItemStack> availableItems = new List<ItemStack>();

        [Header("Stars (exactly 3)")]
        [SerializeReference] public List<StarObjective> starObjectives = new List<StarObjective>();

        [Header("Presentation")]
        public CameraSettingsData cameraSettings = new CameraSettingsData();
        [Tooltip("Progressive hints shown after several failures.")]
        public List<HintData> hintData = new List<HintData>();
        [Tooltip("Localization key of the short tip displayed after 3 failures.")]
        public string tipKey = "";

        [Header("Design")]
        [Tooltip("Reference solution used by hints and by Tools > TRYKLI > Verify Level Solutions.")]
        public List<SolutionStep> solution = new List<SolutionStep>();
        [TextArea(2, 6)] public string designNotes = "";

        public string DisplayName => string.IsNullOrEmpty(displayNameKey) ? name : Loc.Get(displayNameKey);

        /// <summary>Short code such as "3-7".</summary>
        public string Code => $"{worldId}-{levelNumber}";

        public int CountCrystals() => layout != null ? layout.CountElements(ElementType.Crystal) : 0;

        public int GetItemCount(string itemId)
        {
            int total = 0;
            foreach (ItemStack stack in availableItems)
            {
                if (stack.item != null && stack.item.itemId == itemId) total += stack.count;
            }

            return total;
        }
    }
}
