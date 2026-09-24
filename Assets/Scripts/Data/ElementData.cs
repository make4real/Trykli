using System;
using UnityEngine;

namespace Trykli.Data
{
    /// <summary>
    /// Serializable description of one element of a level layout (block, hazard, mechanism, crystal...).
    /// A single flat structure is used for every element type so that layouts stay easy to author in JSON
    /// and in the Inspector. Parameters left at 0 use the mechanism's default tuning.
    /// </summary>
    [Serializable]
    public class ElementData
    {
        public ElementType type;
        [Tooltip("Optional identifier (crystal id, marker id, button id...). Used by objectives.")]
        public string id = "";
        public Vector2 position;
        [Tooltip("Rotation in degrees around Z.")]
        public float rotation;
        [Tooltip("Width / height for blocks, zones, platforms, doors. (0,0) = default size.")]
        public Vector2 size;
        public SurfaceKind surface = SurfaceKind.Ground;
        [Tooltip("Portal pair id, or the switch channel this mechanism listens to (-1 = none).")]
        public int channel = -1;
        [Tooltip("Switch channel emitted by buttons (-1 = none).")]
        public int emitChannel = -1;
        public ActivationMode activation = ActivationMode.AlwaysActive;
        [Tooltip("Main strength parameter (launch speed, force, impulse...). 0 = default.")]
        public float power;
        [Tooltip("Effect radius (magnet, bomb, goal...). 0 = default.")]
        public float radius;
        [Tooltip("Effect length (fan stream, laser beam, rail). 0 = default.")]
        public float length;
        [Tooltip("Moving platform: displacement from the start position to the end position.")]
        public Vector2 travel;
        [Tooltip("Duration of a full cycle (moving platform, laser).")]
        public float period;
        public float onDuration;
        public float offDuration;
        [Tooltip("Cycle offset in seconds.")]
        public float phase;
        [Tooltip("Delay before the mechanism starts / fires.")]
        public float delay;
        [Tooltip("Rotation speed (deg/s) for rotating platforms.")]
        public float speed;
        [Tooltip("Hazards (spikes, lasers): contact kills Trykli; when false it only pushes Trykli back.\nBombs: the blast kills Trykli when close.")]
        public bool lethal = true;
        [Tooltip("Moving platforms: loop back and forth (false = move once from start to end).")]
        public bool loop = true;
        [Tooltip("Buttons / gravity switches trigger only once.")]
        public bool oneShot = true;
        public GravityMode gravityMode = GravityMode.Toggle;
        [Tooltip("Portal endpoint: \"A\" or \"B\".")]
        public string endpoint = "";

        public ElementData Clone()
        {
            return (ElementData)MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{type} '{id}' @ {position}";
        }
    }
}
