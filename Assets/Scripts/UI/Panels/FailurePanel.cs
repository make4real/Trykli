using System.Collections.Generic;
using Trykli.Core;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Objectives;
using Trykli.Save;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Short failure message, RÉESSAYER / MODIFIER, tip and hint after several failures.</summary>
    public sealed class FailurePanel : ModalPanel
    {
        private static readonly string[] Messages = { "fail.msg.1", "fail.msg.2", "fail.msg.3", "fail.msg.4" };

        public LevelManager Level;
        public System.Action HintRequested;

        protected override bool IsBottomSheet => true;

        protected override void Build() { }

        public void Populate()
        {
            // Deterministic variety: the message depends on the number of failures.
            string message = Messages[(Level.Session.Failures - 1 + Messages.Length) % Messages.Length];
            AddTitle(message, UITheme.HeadingSize);
            AddText(Level.LastFailure.LocalizationKey(), UITheme.BodySize, UITheme.TextMuted);

            if (Level.Hints.CanShowTip(Level.Session.Failures))
            {
                TextMeshProUGUI tip = AddRawText(Level.Hints.GetTipText(), UITheme.SmallSize, UITheme.Neutral);
                tip.fontStyle = FontStyles.Italic;
            }

            if (Level.Hints.CanRequestHint(Level.Session.Failures))
            {
                AddButton("hint.button", UITheme.Star, () =>
                {
                    Close();
                    HintRequested?.Invoke();
                }, ArtId.IconHint);
            }

            RectTransform row = AddRow(UITheme.ButtonHeight);
            AddButton("fail.modify", UITheme.Neutral, () =>
            {
                Close();
                Level.Modify();
            }, ArtId.IconBack, row);
            AddButton("fail.retry", UITheme.Primary, () =>
            {
                Close();
                Level.Restart();
            }, ArtId.IconRestart, row);
        }
    }
}
