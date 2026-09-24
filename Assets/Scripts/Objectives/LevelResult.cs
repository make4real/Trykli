using System;

namespace Trykli.Objectives
{
    /// <summary>Outcome of a successful run, saved into the progression.</summary>
    [Serializable]
    public sealed class LevelResult
    {
        public int levelId;
        public bool completed;
        public int stars;
        /// <summary>Bit i set when objective i was fulfilled.</summary>
        public int objectiveMask;
        public int crystals;
        public float time;
        public int attempts;
        public int itemsUsed;

        public bool IsObjectiveMet(int index) => (objectiveMask & (1 << index)) != 0;
    }
}
