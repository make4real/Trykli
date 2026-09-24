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
    /// <summary>Level selection of a world: number, stars, locked / completed state, boss level.</summary>
    public sealed class LevelSelectScreen : MonoBehaviour
    {
        public static LevelSelectScreen Create(int worldId)
        {
            Canvas canvas = UIFactory.CreateCanvas("LevelSelectCanvas", 10);
            GameManager game = GameManager.Instance;
            WorldData world = game.Database.GetWorld(worldId);
            Color top = world != null ? world.accentColor.Darken(0.25f) : UITheme.MenuTop;
            UIFactory.Background(canvas.transform, UITheme.MenuBottom, top);
            var screen = canvas.gameObject.AddComponent<LevelSelectScreen>();
            screen.Build(canvas, worldId);
            return screen;
        }

        private void Build(Canvas canvas, int worldId)
        {
            GameManager game = GameManager.Instance;
            WorldData world = game.Database.GetWorld(worldId);
            RectTransform safe = UIFactory.CreateSafeArea(canvas.transform);
            string title = world != null ? Loc.Format("worlds.card_title", worldId, world.DisplayName) : worldId.ToString();
            MenuHeader header = MenuHeader.Create(safe, null, game.GoToWorldSelect,
                Loc.Format("worlds.total_stars", game.Progression.GetWorldStars(worldId), Mathf.Max(1, world != null ? world.levels.Count : 10) * 3));
            header.Title.text = title;

            if (world != null && !string.IsNullOrEmpty(world.mechanicKey))
            {
                TextMeshProUGUI mechanic = UIFactory.LocText(safe, "Mechanic", world.mechanicKey, UITheme.BodySize, UITheme.TextLight);
                UIFactory.Anchor(mechanic.rectTransform, new Vector2(0.5f, 1f), new Vector2(960f, 80f), new Vector2(0f, -230f), new Vector2(0.5f, 1f));
            }

            RectTransform gridRect = UIFactory.CreateRect("Grid", safe);
            UIFactory.Anchor(gridRect, new Vector2(0.5f, 1f), new Vector2(900f, 1200f), new Vector2(0f, -320f), new Vector2(0.5f, 1f));
            var grid = gridRect.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(270f, 270f);
            grid.spacing = new Vector2(45f, 40f);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;

            List<LevelData> levels = game.Database.GetLevelsOfWorld(worldId);
            for (int i = 0; i < levels.Count; i++) CreateTile(gridRect, levels[i], game, i);
        }

        private static void CreateTile(Transform parent, LevelData level, GameManager game, int index)
        {
            ProgressionService progression = game.Progression;
            bool unlocked = progression.IsLevelUnlocked(level.levelId);
            bool completed = progression.IsLevelCompleted(level.levelId);
            int stars = progression.GetStars(level.levelId);
            Color color = !unlocked ? UITheme.Neutral.Darken(0.3f) : level.isBoss ? UITheme.Danger : completed ? UITheme.Secondary : UITheme.Primary;
            int levelId = level.levelId;

            UIButton tile = UIFactory.Button(parent, "Level_" + levelId, null, color, null);
            tile.OnClick(() =>
            {
                if (unlocked) game.PlayLevel(levelId);
                else UITween.Shake(tile);
            });
            UITween.PopIn(tile, index * 0.03f, 0.25f);

            Transform content = tile.transform.Find("Content");
            content.GetComponent<HorizontalLayoutGroup>().enabled = false;
            if (unlocked)
            {
                TextMeshProUGUI number = UIFactory.Text(content, "Number", level.levelNumber.ToString(), 110f, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
                UIFactory.Anchor(number.rectTransform, new Vector2(0.5f, 0.62f), new Vector2(220f, 140f), Vector2.zero);
                Image[] starImages = UIFactory.Stars(content, "Stars", stars, 3, 58f);
                RectTransform starsRect = (RectTransform)starImages[0].transform.parent;
                UIFactory.Anchor(starsRect, new Vector2(0.5f, 0.2f), new Vector2(200f, 60f), Vector2.zero);
                if (level.isBoss)
                {
                    TextMeshProUGUI boss = UIFactory.LocText(content, "Boss", "levels.boss", 26f, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
                    UIFactory.Anchor(boss.rectTransform, new Vector2(0.5f, 0.93f), new Vector2(220f, 40f), Vector2.zero);
                }

                if (completed)
                {
                    Image check = UIFactory.Image(content, "Check", ArtId.IconCheck, UITheme.TextLight);
                    UIFactory.Anchor(check.rectTransform, new Vector2(0.88f, 0.86f), new Vector2(50f, 50f), Vector2.zero);
                }
            }
            else
            {
                Image lockIcon = UIFactory.Image(content, "Lock", ArtId.IconLock, UITheme.TextLight.WithAlpha(0.8f));
                UIFactory.Anchor(lockIcon.rectTransform, new Vector2(0.5f, 0.55f), new Vector2(110f, 110f), Vector2.zero);
                TextMeshProUGUI number = UIFactory.Text(content, "Number", level.levelNumber.ToString(), 40f, UITheme.TextLight.WithAlpha(0.7f));
                UIFactory.Anchor(number.rectTransform, new Vector2(0.5f, 0.15f), new Vector2(200f, 60f), Vector2.zero);
            }
        }
    }
}
