using UnityEngine;

namespace Trykli.Feedback
{
    public enum HapticType
    {
        /// <summary>Object placed / picked.</summary>
        Light = 0,
        /// <summary>GO pressed.</summary>
        Medium = 1,
        /// <summary>Strong impact (explosion, bumper...).</summary>
        Heavy = 2,
        /// <summary>Victory.</summary>
        Success = 3,
        Failure = 4
    }

    /// <summary>
    /// Mobile vibration abstraction. Android uses the Vibrator service with short durations / amplitudes;
    /// iOS only exposes Handheld.Vibrate (used for strong feedback). Disabled from the settings.
    /// </summary>
    public static class Haptics
    {
        private const float MinInterval = 0.06f;
        private static float _lastTime = -10f;

        public static bool Enabled { get; set; } = true;

        public static void Play(HapticType type)
        {
            if (!Enabled) return;
            float now = Time.unscaledTime;
            if (now - _lastTime < MinInterval) return;
            _lastTime = now;
            Backend.Vibrate(type);
        }

        private static class Backend
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaObject _vibrator;
            private static int _sdkInt = -1;

            public static void Vibrate(HapticType type)
            {
                GetDuration(type, out long milliseconds, out int amplitude);
                try
                {
                    if (_vibrator == null)
                    {
                        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                        }

                        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                        {
                            _sdkInt = version.GetStatic<int>("SDK_INT");
                        }
                    }

                    if (_vibrator == null) return;
                    if (_sdkInt >= 26)
                    {
                        using (var effectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                        using (AndroidJavaObject effect = effectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude))
                        {
                            _vibrator.Call("vibrate", effect);
                        }
                    }
                    else
                    {
                        _vibrator.Call("vibrate", milliseconds);
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning("[TRYKLI] Vibration unavailable: " + exception.Message);
                    Enabled = false;
                }
            }
#elif UNITY_IOS && !UNITY_EDITOR
            public static void Vibrate(HapticType type)
            {
                // iOS has no fine-grained public API without a native plugin: only strong feedback vibrates.
                if (type == HapticType.Heavy || type == HapticType.Success) Handheld.Vibrate();
            }
#else
            public static void Vibrate(HapticType type)
            {
                // Editor / desktop: no vibration hardware.
            }
#endif

            // Used by the Android backend only.
            // ReSharper disable once UnusedMember.Local
            private static void GetDuration(HapticType type, out long milliseconds, out int amplitude)
            {
                switch (type)
                {
                    case HapticType.Light:
                        milliseconds = 12;
                        amplitude = 60;
                        break;
                    case HapticType.Medium:
                        milliseconds = 25;
                        amplitude = 120;
                        break;
                    case HapticType.Heavy:
                        milliseconds = 45;
                        amplitude = 255;
                        break;
                    case HapticType.Success:
                        milliseconds = 60;
                        amplitude = 180;
                        break;
                    default:
                        milliseconds = 35;
                        amplitude = 140;
                        break;
                }
            }
        }
    }
}
