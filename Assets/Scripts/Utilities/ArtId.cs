namespace Trykli.Utilities
{
    /// <summary>
    /// Identifiers of every placeholder sprite. Each id can be overridden by a real sprite through
    /// the <see cref="SpriteLibrary"/> asset (Resources/TrykliSpriteLibrary) without touching code.
    /// </summary>
    public enum ArtId
    {
        Square = 0,
        Circle = 1,
        Ring = 2,
        SoftGlow = 3,
        RoundedBox = 4,
        ZoneOutline = 5,
        UIPanel = 6,
        UIPanelOutline = 7,
        Spike = 8,
        Crystal = 9,
        Star = 10,
        StarEmpty = 11,
        Arrow = 12,
        DashedRing = 13,

        IconPause = 30,
        IconPlay = 31,
        IconRestart = 32,
        IconReset = 33,
        IconSettings = 34,
        IconBack = 35,
        IconLock = 36,
        IconCheck = 37,
        IconCross = 38,
        IconHint = 39,
        IconRotateLeft = 40,
        IconRotateRight = 41,
        IconTrash = 42,
        IconSpeed = 43,
        IconHome = 44,
        IconLevels = 45,
        IconMusic = 46,
        IconSound = 47,
        IconVibration = 48,
        IconLanguage = 49,
        IconSkins = 50,
        IconFinger = 51,

        ItemSpring = 70,
        ItemRamp = 71,
        ItemFan = 72,
        ItemPortal = 73,
        ItemMagnet = 74,
        ItemBomb = 75,
        ItemBumper = 76,
        ItemPlatform = 77,
        ItemGravity = 78
    }
}
