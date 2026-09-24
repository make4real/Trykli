namespace Trykli.Audio
{
    /// <summary>Every sound hook of the game. Map them to real clips in the AudioLibrary asset.</summary>
    public enum SfxId
    {
        UIClick = 0,
        Place = 1,
        Pickup = 2,
        Rotate = 3,
        Remove = 4,
        Go = 5,
        Spring = 6,
        Portal = 7,
        Explosion = 8,
        Collision = 9,
        Crystal = 10,
        Victory = 11,
        Failure = 12,
        Bumper = 13,
        Button = 14,
        Door = 15,
        Laser = 16,
        GravityFlip = 17,
        Star = 18,
        Cannon = 19,
        Unlock = 20,
        Magnet = 21,
        Fan = 22,
        Hint = 23
    }

    public enum MusicTrack
    {
        None = 0,
        Menu = 1,
        Gameplay = 2
    }
}
