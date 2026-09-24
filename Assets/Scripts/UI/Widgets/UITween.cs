using Trykli.Utilities;
using UnityEngine;

namespace Trykli.UI
{
    /// <summary>Tiny unscaled-time animation helpers (pop-in, pulse, float) for UI feedback.</summary>
    public sealed class UITween : MonoBehaviour
    {
        public enum Mode
        {
            PopIn,
            Pulse,
            Float,
            Shake
        }

        private Mode _mode;
        private float _delay;
        private float _duration = 0.35f;
        private float _time;
        private Vector3 _baseScale;
        private Vector3 _basePosition;
        private float _amplitude;

        public static UITween PopIn(Component target, float delay = 0f, float duration = 0.35f)
        {
            UITween tween = Add(target, Mode.PopIn);
            tween._delay = delay;
            tween._duration = duration;
            target.transform.localScale = Vector3.zero;
            return tween;
        }

        public static UITween Pulse(Component target, float amplitude = 0.05f)
        {
            UITween tween = Add(target, Mode.Pulse);
            tween._amplitude = amplitude;
            return tween;
        }

        public static UITween Float(Component target, float amplitude = 12f)
        {
            UITween tween = Add(target, Mode.Float);
            tween._amplitude = amplitude;
            return tween;
        }

        public static UITween Shake(Component target, float amplitude = 18f)
        {
            UITween tween = Add(target, Mode.Shake);
            tween._amplitude = amplitude;
            tween._duration = 0.35f;
            return tween;
        }

        private static UITween Add(Component target, Mode mode)
        {
            UITween existing = target.GetComponent<UITween>();
            if (existing != null) existing.Stop();
            var tween = target.gameObject.AddComponent<UITween>();
            tween._mode = mode;
            tween._baseScale = mode == Mode.PopIn ? Vector3.one : target.transform.localScale;
            tween._basePosition = target.transform.localPosition;
            return tween;
        }

        public void Stop()
        {
            if (_mode == Mode.Shake || _mode == Mode.Float) transform.localPosition = _basePosition;
            if (_mode != Mode.PopIn) transform.localScale = _baseScale;
            else transform.localScale = Vector3.one;
            Destroy(this);
        }

        private void Update()
        {
            _time += Time.unscaledDeltaTime;
            switch (_mode)
            {
                case Mode.PopIn:
                {
                    float t = (_time - _delay) / _duration;
                    if (t < 0f)
                    {
                        transform.localScale = Vector3.zero;
                        return;
                    }

                    if (t >= 1f)
                    {
                        transform.localScale = _baseScale;
                        Destroy(this);
                        return;
                    }

                    transform.localScale = _baseScale * MathUtils.EaseOutBack(t);
                    break;
                }
                case Mode.Pulse:
                    transform.localScale = _baseScale * (1f + Mathf.Sin(_time * 5f) * _amplitude);
                    break;
                case Mode.Float:
                    transform.localPosition = _basePosition + new Vector3(0f, Mathf.Sin(_time * 2f) * _amplitude, 0f);
                    break;
                case Mode.Shake:
                {
                    float t = _time / _duration;
                    if (t >= 1f)
                    {
                        Stop();
                        return;
                    }

                    transform.localPosition = _basePosition + new Vector3(Mathf.Sin(_time * 60f) * _amplitude * (1f - t), 0f, 0f);
                    break;
                }
            }
        }
    }
}
