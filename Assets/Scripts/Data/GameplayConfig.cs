using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Global gameplay tuning. Optional asset at Resources/TrykliGameplayConfig; defaults are used otherwise.
    /// </summary>
    [CreateAssetMenu(menuName = "TRYKLI/Gameplay Config", fileName = "TrykliGameplayConfig")]
    public sealed class GameplayConfig : ScriptableObject
    {
        public const string ResourcePath = "TrykliGameplayConfig";

        [Header("Physics")]
        [Tooltip("Fixed physics step in seconds (GDD: 0.02).")]
        public float physicsStep = 0.02f;
        public float gravity = 9.81f;
        [Tooltip("Maximum physics steps simulated per rendered frame.")]
        public int maxStepsPerFrame = 8;

        [Header("Trykli")]
        public float trykliRadius = 0.35f;
        public float trykliMass = 1f;
        public float trykliFriction = 0.4f;
        public float trykliBounciness = 0.15f;
        [Tooltip("Speed above which Trykli is clamped (safety against extreme forces).")]
        public float maxSpeed = 30f;

        [Header("Failure rules")]
        public float stuckSpeedThreshold = 0.15f;
        [Tooltip("GDD 63: speed below threshold for 3 seconds = stuck.")]
        public float stuckDuration = 3f;
        public float idleThresholdForObjectives = 0.5f;
        public float outOfBoundsMargin = 1.5f;
        public float defaultTimeLimit = 15f;

        [Header("Anti-frustration")]
        public int failuresBeforeTip = 3;
        public int failuresBeforeHint = 5;

        [Header("Placement")]
        public float placementGridStep = 0.25f;
        public float pointZoneSnapDistance = 1.1f;
        public float minItemSpacing = 0.7f;

        private static GameplayConfig _instance;

        public static GameplayConfig Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<GameplayConfig>(ResourcePath);
                if (_instance == null)
                {
                    _instance = CreateInstance<GameplayConfig>();
                    _instance.name = "GameplayConfig (defaults)";
                }

                return _instance;
            }
        }
    }
}
