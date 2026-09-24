using System.Collections.Generic;
using Trykli.Core;
using Trykli.Data;
using Trykli.Localization;
using Trykli.Save;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Animated Trykli drawn with UI images (menus, splash).</summary>
    public static class UITrykli
    {
        public static RectTransform Create(Transform parent, float size, Color bodyColor)
        {
            RectTransform root = UIFactory.CreateRect("Trykli", parent);
            root.sizeDelta = new Vector2(size, size);
            Image shadow = UIFactory.Image(root, "Shadow", ArtId.SoftGlow, new Color(0f, 0f, 0f, 0.25f));
            UIFactory.Anchor(shadow.rectTransform, new Vector2(0.5f, 0f), new Vector2(size * 1.1f, size * 0.35f), new Vector2(0f, -size * 0.08f));
            Image body = UIFactory.Image(root, "Body", ArtId.Circle, bodyColor);
            UIFactory.Stretch(body.rectTransform);
            Image highlight = UIFactory.Image(root, "Highlight", ArtId.Circle, new Color(1f, 1f, 1f, 0.35f));
            UIFactory.Anchor(highlight.rectTransform, new Vector2(0.32f, 0.72f), new Vector2(size * 0.22f, size * 0.15f), Vector2.zero);
            for (int i = 0; i < 2; i++)
            {
                float x = i == 0 ? 0.32f : 0.68f;
                Image eye = UIFactory.Image(root, i == 0 ? "EyeL" : "EyeR", ArtId.Circle, Color.white);
                UIFactory.Anchor(eye.rectTransform, new Vector2(x, 0.6f), new Vector2(size * 0.22f, size * 0.25f), Vector2.zero);
                Image pupil = UIFactory.Image(eye.transform, "Pupil", ArtId.Circle, new Color(0.1f, 0.1f, 0.16f));
                UIFactory.Anchor(pupil.rectTransform, new Vector2(0.55f, 0.45f), new Vector2(size * 0.11f, size * 0.12f), Vector2.zero);
            }

            return root;
        }
    }

    public static class ProgressBar
    {
        public static Image Create(Transform parent, float fraction, float height)
        {
            Image track = UIFactory.Image(parent, "ProgressTrack", ArtId.UIPanel, new Color(0f, 0f, 0f, 0.25f));
            UIFactory.SetPreferredSize(track.gameObject, -1f, height);
            Image fill = UIFactory.Image(track.transform, "Fill", ArtId.UIPanel, UITheme.Star);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            fill.gameObject.SetActive(fraction > 0.001f);
            return fill;
        }
    }
}
