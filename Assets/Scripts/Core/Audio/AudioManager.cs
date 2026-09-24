using System.Collections.Generic;
using UnityEngine;

namespace Trykli.Audio
{
    /// <summary>
    /// Persistent audio service with separate Music and SFX channels.
    /// Uses clips from <see cref="AudioLibrary"/> when present, procedural placeholders otherwise.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        private const int SfxVoices = 8;
        private const float MinRepeatInterval = 0.05f;

        private readonly Dictionary<SfxId, AudioClip> _generated = new Dictionary<SfxId, AudioClip>();
        private readonly Dictionary<SfxId, float> _lastPlayed = new Dictionary<SfxId, float>();
        private readonly List<AudioSource> _voices = new List<AudioSource>();

        private AudioLibrary _library;
        private AudioSource _music;
        private MusicTrack _currentTrack = MusicTrack.None;
        private AudioClip _menuLoop;
        private AudioClip _gameplayLoop;
        private int _nextVoice;

        public static AudioManager Instance { get; private set; }

        public bool MusicEnabled { get; private set; } = true;
        public bool SfxEnabled { get; private set; } = true;

        /// <summary>Null-safe shortcut used by gameplay code.</summary>
        public static void PlaySfx(SfxId id, float volume = 1f, float pitch = 1f)
        {
            if (Instance != null) Instance.Play(id, volume, pitch);
        }

        public void Initialize(bool musicEnabled, bool sfxEnabled)
        {
            Instance = this;
            _library = Resources.Load<AudioLibrary>(AudioLibrary.ResourcePath);

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;
            _music.volume = _library != null ? _library.musicVolume : 0.35f;

            for (int i = 0; i < SfxVoices; i++)
            {
                AudioSource voice = gameObject.AddComponent<AudioSource>();
                voice.playOnAwake = false;
                _voices.Add(voice);
            }

            SetSfxEnabled(sfxEnabled);
            SetMusicEnabled(musicEnabled);
        }

        public void Play(SfxId id, float volume = 1f, float pitch = 1f)
        {
            if (!SfxEnabled || _voices.Count == 0) return;
            float now = Time.unscaledTime;
            if (_lastPlayed.TryGetValue(id, out float last) && now - last < MinRepeatInterval) return;
            _lastPlayed[id] = now;

            AudioClip clip = ResolveClip(id, out float entryVolume);
            if (clip == null) return;

            AudioSource voice = _voices[_nextVoice];
            _nextVoice = (_nextVoice + 1) % _voices.Count;
            voice.pitch = pitch;
            voice.PlayOneShot(clip, Mathf.Clamp01(volume * entryVolume));
        }

        public void PlayMusic(MusicTrack track)
        {
            if (track == _currentTrack && (_music.isPlaying || !MusicEnabled)) return;
            _currentTrack = track;
            _music.Stop();
            _music.clip = ResolveMusic(track);
            if (MusicEnabled && _music.clip != null) _music.Play();
        }

        public void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
            if (_music == null) return;
            if (!enabled) _music.Stop();
            else if (_currentTrack != MusicTrack.None)
            {
                if (_music.clip == null) _music.clip = ResolveMusic(_currentTrack);
                if (_music.clip != null && !_music.isPlaying) _music.Play();
            }
        }

        public void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            if (enabled) return;
            foreach (AudioSource voice in _voices) voice.Stop();
        }

        private AudioClip ResolveClip(SfxId id, out float volume)
        {
            volume = 1f;
            AudioLibrary.SfxEntry entry = _library != null ? _library.Find(id) : null;
            if (entry != null)
            {
                volume = entry.volume;
                return entry.clips[Random.Range(0, entry.clips.Length)];
            }

            if (!_generated.TryGetValue(id, out AudioClip clip) || clip == null)
            {
                clip = ProceduralAudio.CreateSfx(id);
                _generated[id] = clip;
            }

            return clip;
        }

        private AudioClip ResolveMusic(MusicTrack track)
        {
            switch (track)
            {
                case MusicTrack.Menu:
                    if (_library != null && _library.menuMusic != null) return _library.menuMusic;
                    if (_menuLoop == null) _menuLoop = ProceduralAudio.CreateMusicLoop("MenuLoop", 392f, 84f);
                    return _menuLoop;
                case MusicTrack.Gameplay:
                    if (_library != null && _library.gameplayMusic != null) return _library.gameplayMusic;
                    if (_gameplayLoop == null) _gameplayLoop = ProceduralAudio.CreateMusicLoop("GameplayLoop", 330f, 96f);
                    return _gameplayLoop;
                default:
                    return null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
