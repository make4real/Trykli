using System;
using Trykli.Data;

namespace Trykli.Objectives
{
    /// <summary>
    /// Serializable (JSON) description of a star objective. Converted to a <see cref="StarObjective"/>
    /// instance by <see cref="ObjectiveFactory"/>.
    /// </summary>
    [Serializable]
    public class ObjectiveDefinition
    {
        public const string Complete = "Complete";
        public const string MaxObjects = "MaxObjects";
        public const string ItemUsage = "ItemUsage";
        public const string CollectCrystals = "CollectCrystals";
        public const string Time = "Time";
        public const string BounceCount = "BounceCount";
        public const string NoHazard = "NoHazard";
        public const string AvoidContact = "AvoidContact";
        public const string Interaction = "Interaction";
        public const string Attempts = "Attempts";
        public const string NoStop = "NoStop";

        public string type = Complete;
        public int count;
        public float value;
        public Comparison comparison = Comparison.AtLeast;
        [UnityEngine.Tooltip("Item id, counter key, crystal id, surface / hazard / bounce category name...")]
        public string target = "";
        public bool invert;
        [UnityEngine.Tooltip("Optional localization key replacing the generated description.")]
        public string descriptionKey = "";
    }
}
