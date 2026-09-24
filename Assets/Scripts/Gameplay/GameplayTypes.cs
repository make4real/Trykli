using UnityEngine;

namespace Trykli.Gameplay
{
    public enum FailureReason
    {
        None = 0,
        Hazard = 1,
        OutOfBounds = 2,
        Stuck = 3,
        Timeout = 4,
        Explosion = 5
    }

    public static class FailureReasonExtensions
    {
        public static string LocalizationKey(this FailureReason reason)
        {
            switch (reason)
            {
                case FailureReason.Hazard: return "fail.reason.hazard";
                case FailureReason.OutOfBounds: return "fail.reason.out";
                case FailureReason.Stuck: return "fail.reason.stuck";
                case FailureReason.Timeout: return "fail.reason.timeout";
                case FailureReason.Explosion: return "fail.reason.explosion";
                default: return "fail.reason.unknown";
            }
        }
    }

    /// <summary>Implemented by every component of a level that needs the level context.</summary>
    public interface ILevelElement
    {
        void Bind(LevelContext context);
    }

    /// <summary>
    /// Implemented by elements that evolve during the simulation. Steps are driven by the
    /// SimulationRunner with a fixed time step, in hierarchy order, before each physics step:
    /// time-dependent mechanisms are therefore fully deterministic.
    /// </summary>
    public interface ISimulationElement
    {
        void OnSimulationStart();
        void OnSimulationStep(float deltaTime, float simulationTime);
    }

    /// <summary>
    /// Implemented by components created from layout data (<see cref="Data.ElementData"/>).
    /// Configure may be called on a freshly created object or on an instantiated prefab: it must create its
    /// visual only when missing and always (re)apply sizes, colors and parameters.
    /// </summary>
    public interface IConfigurableElement
    {
        void Configure(Data.ElementData data, WorldTheme theme);
    }

    /// <summary>Theme colors of the current world.</summary>
    public readonly struct WorldTheme
    {
        public readonly Color BackgroundTop;
        public readonly Color BackgroundBottom;
        public readonly Color Block;
        public readonly Color Accent;

        public WorldTheme(Color backgroundTop, Color backgroundBottom, Color block, Color accent)
        {
            BackgroundTop = backgroundTop;
            BackgroundBottom = backgroundBottom;
            Block = block;
            Accent = accent;
        }

        public static WorldTheme Default => new WorldTheme(
            new Color(0.86f, 0.93f, 1f), new Color(0.72f, 0.84f, 0.96f), new Color(0.3f, 0.37f, 0.48f), new Color(1f, 0.69f, 0.13f));
    }

    /// <summary>Sprite sorting orders (single sorting layer, no project setting required).</summary>
    public static class SortingOrders
    {
        public const int Background = -100;
        public const int BackgroundDecor = -90;
        public const int Zones = -20;
        public const int Effects = -5;
        public const int Blocks = 0;
        public const int Goal = 2;
        public const int Mechanisms = 5;
        public const int Hazards = 6;
        public const int Crystals = 8;
        public const int PlacedItems = 10;
        public const int Trykli = 20;
        public const int Dragged = 30;
        public const int Hints = 40;
        public const int Overlay = 50;
    }

}
