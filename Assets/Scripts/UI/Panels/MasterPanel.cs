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
    /// <summary>End of the game (after level 100): TRYKLI MASTER and statistics.</summary>
    public sealed class MasterPanel : ModalPanel
    {
        protected override Color CardColor => ColorUtils.Hex("#1B2238");

        protected override void Build()
        {
            GameManager game = GameManager.Instance;
            ProgressionService progression = game.Progression;
            int levels = game.Database.LevelCount;
            TextMeshProUGUI title = UIFactory.LocText(Content, "Title", "master.title", UITheme.TitleSize * 0.8f, UITheme.Star, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.SetPreferredSize(title.gameObject, -1f, 150f);
            UITween.Pulse(title, 0.04f);
            UIFactory.Text(Content, "Levels", Loc.Format("master.levels", progression.CompletedLevels, levels), UITheme.SubheadingSize, UITheme.TextLight);
            UIFactory.Text(Content, "Stars", Loc.Format("master.stars", progression.TotalStars, levels * 3), UITheme.SubheadingSize, UITheme.TextLight);
            UIFactory.Text(Content, "Crystals", Loc.Format("master.crystals", progression.TotalCrystals, CountCrystals(game)), UITheme.SubheadingSize, UITheme.TextLight);
            AddButton("master.replay", UITheme.Primary, () =>
            {
                Close();
                game.GoToWorldSelect();
            }, ArtId.IconLevels);
            AddButton("master.complete_stars", UITheme.Secondary, () =>
            {
                Close();
                game.PlayLevel(FirstLevelMissingStars(game));
            }, ArtId.Star);
        }

        private static int CountCrystals(GameManager game)
        {
            int total = 0;
            foreach (Data.LevelData level in game.Database.Levels) total += level.CountCrystals();
            return total;
        }

        private static int FirstLevelMissingStars(GameManager game)
        {
            foreach (Data.LevelData level in game.Database.Levels)
            {
                if (game.Progression.GetStars(level.levelId) < 3) return level.levelId;
            }

            return 1;
        }
    }
}
