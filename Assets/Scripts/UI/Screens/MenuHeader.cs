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
    /// <summary>Back button + title + subtitle used by the menu screens.</summary>
    public sealed class MenuHeader : MonoBehaviour
    {
        public TextMeshProUGUI Title { get; private set; }

        public static MenuHeader Create(RectTransform safe, string titleKey, System.Action onBack, string subtitle)
        {
            RectTransform bar = UIFactory.CreateRect("Header", safe);
            UIFactory.Band(bar, true, 200f);
            var header = bar.gameObject.AddComponent<MenuHeader>();
            UIButton back = UIFactory.IconButton(bar, "Back", ArtId.IconBack, UITheme.Neutral, onBack, 130f);
            UIFactory.Anchor((RectTransform)back.transform, new Vector2(0f, 0.5f), new Vector2(130f, 130f), new Vector2(40f, 0f), new Vector2(0f, 0.5f));
            header.Title = string.IsNullOrEmpty(titleKey)
                ? UIFactory.Text(bar, "Title", string.Empty, UITheme.HeadingSize, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold)
                : UIFactory.LocText(bar, "Title", titleKey, UITheme.HeadingSize, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Anchor(header.Title.rectTransform, new Vector2(0.5f, 0.62f), new Vector2(700f, 100f), Vector2.zero);
            UIFactory.AutoSize(header.Title, 36f, UITheme.HeadingSize);
            TextMeshProUGUI sub = UIFactory.Text(bar, "Subtitle", subtitle, UITheme.BodySize, UITheme.Star);
            UIFactory.Anchor(sub.rectTransform, new Vector2(0.5f, 0.18f), new Vector2(700f, 60f), Vector2.zero);
            return header;
        }
    }
}
