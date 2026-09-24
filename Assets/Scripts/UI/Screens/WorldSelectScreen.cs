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
    /// <summary>World selection: number, name, progression, stars, locked state.</summary>
    public sealed class WorldSelectScreen : MonoBehaviour
    {
        public static WorldSelectScreen Create()
        {
            Canvas canvas = UIFactory.CreateCanvas("WorldSelectCanvas", 10);
            UIFactory.Background(canvas.transform, UITheme.MenuBottom, UITheme.MenuTop);
            var screen = canvas.gameObject.AddComponent<WorldSelectScreen>();
            screen.Build(canvas);
            return screen;
        }

        private void Build(Canvas canvas)
        {
            GameManager game = GameManager.Instance;
            RectTransform safe = UIFactory.CreateSafeArea(canvas.transform);
            MenuHeader.Create(safe, "worlds.title", game.GoToMainMenu,
                Loc.Format("worlds.total_stars", game.Progression.TotalStars, game.Progression.TotalLevels * 3));

            RectTransform content = UIFactory.ScrollView(safe, "Worlds", 26f, new RectOffset(40, 40, 20, 60));
            UIFactory.Stretch((RectTransform)content.parent.parent, 0f, 200f, 0f, 0f);
            foreach (WorldData world in game.Database.Worlds) CreateCard(content, world, game);
        }

        private static void CreateCard(Transform parent, WorldData world, GameManager game)
        {
            ProgressionService progression = game.Progression;
            bool unlocked = progression.IsWorldUnlocked(world.worldId);
            int stars = progression.GetWorldStars(world.worldId);
            int completed = progression.GetWorldCompletedCount(world.worldId);
            int levelCount = Mathf.Max(1, world.levels.Count);
            int worldId = world.worldId;

            UIButton card = UIFactory.Button(parent, "World_" + worldId, null, unlocked ? world.accentColor.Darken(0.1f) : UITheme.Neutral.Darken(0.3f), null);
            UIFactory.SetPreferredSize(card.gameObject, -1f, 230f);
            card.OnClick(() =>
            {
                if (unlocked) game.GoToLevelSelect(worldId);
                else
                {
                    UITween.Shake(card);
                    Toast.Show(card.transform.root.GetComponentInChildren<SafeArea>().transform, Loc.Get("worlds.locked_message"));
                }
            });

            Transform content = card.transform.Find("Content");
            content.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;

            Image badge = UIFactory.Image(content, "Badge", ArtId.Circle, world.backgroundTop);
            UIFactory.SetPreferredSize(badge.gameObject, 150f, 150f);
            TextMeshProUGUI number = UIFactory.Text(badge.transform, "Number", worldId.ToString(), 80f, UITheme.TextDark, TextAlignmentOptions.Center, FontStyles.Bold);
            UIFactory.Stretch(number.rectTransform);

            RectTransform info = UIFactory.CreateRect("Info", content);
            UIFactory.Vertical(info.gameObject, 6f, null, TextAnchor.MiddleLeft);
            UIFactory.SetPreferredSize(info.gameObject, -1f, 190f, 1f);
            TextMeshProUGUI name = UIFactory.Text(info, "Name", Loc.Format("worlds.card_title", worldId, world.DisplayName), UITheme.SubheadingSize * 0.9f,
                UITheme.TextLight, TextAlignmentOptions.Left, FontStyles.Bold);
            UIFactory.AutoSize(name, 30f, UITheme.SubheadingSize * 0.9f);
            UIFactory.SetPreferredSize(name.gameObject, -1f, 70f);

            RectTransform starsRow = UIFactory.CreateRect("StarsRow", info);
            UIFactory.Horizontal(starsRow.gameObject, 10f, null, TextAnchor.MiddleLeft);
            UIFactory.SetPreferredSize(starsRow.gameObject, -1f, 56f);
            Image star = UIFactory.Image(starsRow, "Star", ArtId.Star, UITheme.Star);
            UIFactory.SetPreferredSize(star.gameObject, 50f, 50f);
            TextMeshProUGUI starText = UIFactory.Text(starsRow, "Count", $"{stars} / {levelCount * 3}", UITheme.BodySize, UITheme.TextLight, TextAlignmentOptions.Left);
            UIFactory.SetPreferredSize(starText.gameObject, 220f, 56f);
            TextMeshProUGUI levelsText = UIFactory.Text(starsRow, "Levels", Loc.Format("worlds.levels_done", completed, levelCount), UITheme.SmallSize,
                UITheme.TextLight.WithAlpha(0.85f), TextAlignmentOptions.Left);
            UIFactory.SetPreferredSize(levelsText.gameObject, 260f, 56f);

            ProgressBar.Create(info, completed / (float)levelCount, 26f);

            if (!unlocked)
            {
                Image lockIcon = UIFactory.Image(content, "Lock", ArtId.IconLock, UITheme.TextLight);
                UIFactory.SetPreferredSize(lockIcon.gameObject, 90f, 90f);
            }
        }
    }
}
