namespace Trykli.Data
{
    /// <summary>Kinds of objects the player can place from the inventory.</summary>
    public enum ItemKind
    {
        Spring = 0,
        Ramp = 1,
        Fan = 2,
        Portal = 3,
        Magnet = 4,
        Bomb = 5,
        Bumper = 6,
        MiniPlatform = 7,
        GravitySwitch = 8
    }

    /// <summary>Every element a level layout can contain.</summary>
    public enum ElementType
    {
        Block = 0,
        Spikes = 1,
        Crystal = 2,
        Spring = 3,
        Ramp = 4,
        Fan = 5,
        Portal = 6,
        Magnet = 7,
        Bomb = 8,
        Bumper = 9,
        MovingPlatform = 10,
        RotatingPlatform = 11,
        Button = 12,
        Door = 13,
        Laser = 14,
        GravitySwitch = 15,
        StickyZone = 16,
        SlipperyZone = 17,
        Cannon = 18,
        Marker = 19,
        MiniPlatform = 20
    }

    /// <summary>Surface categories used by contact based objectives ("ne toucher aucun mur"...).</summary>
    public enum SurfaceKind
    {
        Ground = 0,
        Wall = 1,
        Ceiling = 2,
        Obstacle = 3,
        Platform = 4
    }

    /// <summary>How a mechanism reacts to switch signals (buttons).</summary>
    public enum ActivationMode
    {
        /// <summary>Always active, ignores signals.</summary>
        AlwaysActive = 0,
        /// <summary>Inactive until a signal is received on its channel.</summary>
        ActivatedBySignal = 1,
        /// <summary>Active until a signal is received on its channel.</summary>
        DeactivatedBySignal = 2,
        /// <summary>Each signal toggles the active state.</summary>
        ToggledBySignal = 3
    }

    public enum GravityMode
    {
        Toggle = 0,
        Invert = 1,
        Normal = 2
    }

    public enum ZoneShape
    {
        /// <summary>Exact slot: the item snaps to the zone center.</summary>
        Point = 0,
        /// <summary>Rectangular area: the item can be placed anywhere inside (snapped to a grid).</summary>
        Rect = 1,
        /// <summary>Segment: the item slides along a rail / surface.</summary>
        Rail = 2
    }

    public enum TutorialKind
    {
        None = 0,
        DragAndGo = 1,
        Rotate = 2,
        Restart = 3
    }

    public enum BounceSource
    {
        Any = 0,
        Spring = 1,
        Bumper = 2
    }

    public enum HazardCategory
    {
        Any = 0,
        Spikes = 1,
        Laser = 2,
        Explosion = 3
    }

    public enum Comparison
    {
        AtLeast = 0,
        AtMost = 1,
        Exactly = 2
    }

    public static class ComparisonExtensions
    {
        public static bool Evaluate(this Comparison comparison, int value, int target)
        {
            switch (comparison)
            {
                case Comparison.AtLeast: return value >= target;
                case Comparison.AtMost: return value <= target;
                default: return value == target;
            }
        }
    }
}
