using System;
using Trykli.Localization;

namespace Trykli.Objectives
{
    /// <summary>
    /// Base class of the extensible star objective system. LevelData stores three objectives
    /// with [SerializeReference] so any subclass can be configured in the Inspector / Level Editor.
    /// To add a new objective: subclass, implement Evaluate + BuildDescription + ToDefinition, and
    /// register the type name in <see cref="ObjectiveFactory"/>.
    /// </summary>
    [Serializable]
    public abstract class StarObjective
    {
        [UnityEngine.Tooltip("Optional localization key replacing the generated description.")]
        public string descriptionKey = "";

        /// <summary>True when the objective is fulfilled by the run. Called only for successful runs.</summary>
        public abstract bool Evaluate(LevelRunStats stats);

        public string GetDescription()
        {
            return !string.IsNullOrEmpty(descriptionKey) ? Loc.Get(descriptionKey) : BuildDescription();
        }

        protected abstract string BuildDescription();

        public ObjectiveDefinition ToDefinition()
        {
            ObjectiveDefinition definition = CreateDefinition();
            definition.descriptionKey = descriptionKey ?? string.Empty;
            return definition;
        }

        protected abstract ObjectiveDefinition CreateDefinition();
    }
}
