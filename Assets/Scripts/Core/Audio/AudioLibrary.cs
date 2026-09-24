using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Audio
{
    /// <summary>
    /// Optional asset (Resources/TrykliAudioLibrary) mapping sound hooks to real clips.
    /// Any missing clip is replaced by a procedural placeholder sound, so the game never errors without audio assets.
    /// </summary>
    [CreateAssetMenu(menuName = "TRYKLI/Audio Library", fileName = "TrykliAudioLibrary")]
    public sealed class AudioLibrary : ScriptableObject
    {
        public const string ResourcePath = "TrykliAudioLibrary";

        [Serializable]
        public class SfxEntry
        {
            public SfxId id;
            [Tooltip("A random clip is picked each time.")]
            public AudioClip[] clips = Array.Empty<AudioClip>();
            [Range(0f, 1f)] public float volume = 1f;
        }

        public List<SfxEntry> sfx = new List<SfxEntry>();
        public AudioClip menuMusic;
        public AudioClip gameplayMusic;
        [Range(0f, 1f)] public float musicVolume = 0.45f;

        public SfxEntry Find(SfxId id)
        {
            foreach (SfxEntry entry in sfx)
            {
                if (entry.id == id && entry.clips != null && entry.clips.Length > 0) return entry;
            }

            return null;
        }
    }
}
