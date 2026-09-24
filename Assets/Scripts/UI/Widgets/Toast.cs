using System;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Short message sliding at the top of the screen.</summary>
    public sealed class Toast : MonoBehaviour
    {
        private float _time;
        private float _duration;
        private CanvasGroup _group;

        public static Toast Show(Transform parent, string text, float duration = 2f)
        {
            Image background = UIFactory.Panel(parent, "Toast", UITheme.PanelDark, false);
            RectTransform rect = background.rectTransform;
            UIFactory.Anchor(rect, new Vector2(0.5f, 0.78f), new Vector2(900f, 140f), Vector2.zero, new Vector2(0.5f, 0.5f));
            TextMeshProUGUI label = UIFactory.Text(rect, "Text", text, UITheme.BodySize, UITheme.TextLight);
            UIFactory.Stretch(label.rectTransform, 30f, 10f, 30f, 10f);
            UIFactory.AutoSize(label, 28f, UITheme.BodySize);
            var toast = background.gameObject.AddComponent<Toast>();
            toast._duration = duration;
            toast._group = background.gameObject.AddComponent<CanvasGroup>();
            toast._group.blocksRaycasts = false;
            UITween.PopIn(background, 0f, 0.2f);
            return toast;
        }

        private void Update()
        {
            _time += Time.unscaledDeltaTime;
            if (_time > _duration) _group.alpha = Mathf.Clamp01(1f - (_time - _duration) / 0.3f);
            if (_time > _duration + 0.3f) Destroy(gameObject);
        }
    }
}
