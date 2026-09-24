using UnityEngine;

namespace Trykli.Audio
{
    /// <summary>Generates simple placeholder sounds and a soft music loop at runtime (no audio asset needed).</summary>
    public static class ProceduralAudio
    {
        private const int SampleRate = 22050;

        private enum Wave
        {
            Sine,
            Square,
            Triangle,
            Noise
        }

        public static AudioClip CreateSfx(SfxId id)
        {
            switch (id)
            {
                case SfxId.UIClick: return Tone(id, 900f, 700f, 0.05f, Wave.Sine, 0.35f);
                case SfxId.Place: return Tone(id, 380f, 260f, 0.09f, Wave.Triangle, 0.5f);
                case SfxId.Pickup: return Tone(id, 300f, 480f, 0.07f, Wave.Triangle, 0.4f);
                case SfxId.Rotate: return Tone(id, 600f, 640f, 0.05f, Wave.Triangle, 0.35f);
                case SfxId.Remove: return Tone(id, 420f, 200f, 0.1f, Wave.Triangle, 0.4f);
                case SfxId.Go: return Chord(id, new[] { 523f, 659f, 784f }, 0.25f, 0.4f);
                case SfxId.Spring: return Tone(id, 220f, 880f, 0.18f, Wave.Square, 0.25f);
                case SfxId.Portal: return Tone(id, 1200f, 300f, 0.25f, Wave.Sine, 0.45f);
                case SfxId.Explosion: return Tone(id, 120f, 40f, 0.45f, Wave.Noise, 0.6f);
                case SfxId.Collision: return Tone(id, 160f, 110f, 0.06f, Wave.Triangle, 0.35f);
                case SfxId.Crystal: return Chord(id, new[] { 1319f, 1760f }, 0.2f, 0.35f);
                case SfxId.Victory: return Arpeggio(id, new[] { 523f, 659f, 784f, 1047f }, 0.11f, 0.45f);
                case SfxId.Failure: return Arpeggio(id, new[] { 392f, 330f, 262f }, 0.13f, 0.4f);
                case SfxId.Bumper: return Tone(id, 700f, 1100f, 0.1f, Wave.Square, 0.25f);
                case SfxId.Button: return Tone(id, 500f, 500f, 0.08f, Wave.Square, 0.25f);
                case SfxId.Door: return Tone(id, 200f, 320f, 0.3f, Wave.Triangle, 0.35f);
                case SfxId.Laser: return Tone(id, 1500f, 900f, 0.12f, Wave.Square, 0.2f);
                case SfxId.GravityFlip: return Tone(id, 300f, 900f, 0.3f, Wave.Sine, 0.4f);
                case SfxId.Star: return Tone(id, 1047f, 1568f, 0.15f, Wave.Sine, 0.4f);
                case SfxId.Cannon: return Tone(id, 180f, 60f, 0.3f, Wave.Noise, 0.5f);
                case SfxId.Unlock: return Arpeggio(id, new[] { 659f, 988f }, 0.1f, 0.4f);
                case SfxId.Magnet: return Tone(id, 150f, 170f, 0.2f, Wave.Sine, 0.3f);
                case SfxId.Fan: return Tone(id, 200f, 200f, 0.2f, Wave.Noise, 0.12f);
                case SfxId.Hint: return Chord(id, new[] { 880f, 1320f }, 0.3f, 0.3f);
                default: return Tone(id, 440f, 440f, 0.1f, Wave.Sine, 0.3f);
            }
        }

        /// <summary>Gentle pentatonic loop used as placeholder music.</summary>
        public static AudioClip CreateMusicLoop(string name, float rootFrequency, float bpm)
        {
            float beat = 60f / bpm;
            int[] melody = { 0, 4, 7, 9, 7, 4, 2, 4, 0, 2, 4, 7, 12, 9, 7, 4 };
            int[] bass = { 0, 0, -5, -5, -3, -3, -5, -5 };
            int beatCount = melody.Length * 2;
            int length = Mathf.CeilToInt(beatCount * beat * 0.5f * SampleRate);
            var data = new float[length];
            int noteSamples = Mathf.CeilToInt(beat * 0.5f * SampleRate);

            for (int n = 0; n < beatCount; n++)
            {
                float freq = rootFrequency * Mathf.Pow(2f, melody[n % melody.Length] / 12f);
                AddNote(data, n * noteSamples, noteSamples * 2, freq, 0.11f);
            }

            int bassSamples = noteSamples * 4;
            for (int n = 0; n * bassSamples < length; n++)
            {
                float freq = rootFrequency * 0.5f * Mathf.Pow(2f, bass[n % bass.Length] / 12f);
                AddNote(data, n * bassSamples, bassSamples, freq, 0.09f);
            }

            var clip = AudioClip.Create(name, length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static void AddNote(float[] data, int start, int length, float frequency, float volume)
        {
            for (int i = 0; i < length && start + i < data.Length; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = Mathf.Min(1f, i / (SampleRate * 0.02f)) * Mathf.Exp(-3f * i / (float)length);
                data[start + i] += Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
            }
        }

        private static AudioClip Tone(SfxId id, float startFrequency, float endFrequency, float duration, Wave wave, float volume)
        {
            int length = Mathf.Max(1, Mathf.CeilToInt(duration * SampleRate));
            var data = new float[length];
            float phase = 0f;
            var random = new System.Random((int)id * 7919 + 17);
            for (int i = 0; i < length; i++)
            {
                float progress = i / (float)length;
                float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
                phase += frequency / SampleRate;
                float envelope = Mathf.Min(1f, i / (SampleRate * 0.005f)) * (1f - progress) * (1f - progress);
                data[i] = Sample(wave, phase, random) * envelope * volume;
            }

            return Create(id.ToString(), data);
        }

        private static AudioClip Chord(SfxId id, float[] frequencies, float duration, float volume)
        {
            int length = Mathf.CeilToInt(duration * SampleRate);
            var data = new float[length];
            for (int i = 0; i < length; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = Mathf.Min(1f, i / (SampleRate * 0.005f)) * Mathf.Exp(-5f * i / (float)length);
                float sum = 0f;
                foreach (float f in frequencies) sum += Mathf.Sin(2f * Mathf.PI * f * t);
                data[i] = sum / frequencies.Length * envelope * volume;
            }

            return Create(id.ToString(), data);
        }

        private static AudioClip Arpeggio(SfxId id, float[] frequencies, float noteDuration, float volume)
        {
            int noteLength = Mathf.CeilToInt(noteDuration * SampleRate);
            var data = new float[noteLength * (frequencies.Length + 2)];
            for (int n = 0; n < frequencies.Length; n++)
            {
                int length = n == frequencies.Length - 1 ? noteLength * 3 : noteLength;
                AddNote(data, n * noteLength, length, frequencies[n], volume);
            }

            return Create(id.ToString(), data);
        }

        private static float Sample(Wave wave, float phase, System.Random random)
        {
            float p = phase - Mathf.Floor(phase);
            switch (wave)
            {
                case Wave.Square: return p < 0.5f ? 1f : -1f;
                case Wave.Triangle: return 4f * Mathf.Abs(p - 0.5f) - 1f;
                case Wave.Noise: return (float)(random.NextDouble() * 2.0 - 1.0);
                default: return Mathf.Sin(2f * Mathf.PI * p);
            }
        }

        private static AudioClip Create(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
