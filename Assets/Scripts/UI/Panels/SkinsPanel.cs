using Trykli.Core;
using Trykli.Localization;
using Trykli.Save;
using Trykli.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Cosmetic skins unlocked with crystals (no pay-to-win).</summary>
    public sealed class SkinsPanel : ModalPanel
    {
        private RectTransform _grid;

        protected override bool CloseOnDimmerTap => true;

        protected override void Build()
        {
            AddTitle("skins.title");
            AddText("skins.crystals", UITheme.BodySize, UITheme.TextMuted, GameManager.Instance.Progression.TotalCrystals);
            _grid = UIFactory.CreateRect("Grid", Content);
            var grid = _grid.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(260f, 300f);
            grid.spacing = new Vector2(24f, 24f);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            UIFactory.SetPreferredSize(_grid.gameObject, -1f, 2 * 300f + 24f);
            BuildSkins();
            AddButton("common.close", UITheme.Primary, Close);
        }

        private void BuildSkins()
        {
            for (int i = _grid.childCount - 1; i >= 0; i--) Destroy(_grid.GetChild(i).gameObject);
            GameManager game = GameManager.Instance;
            foreach (SkinDefinition skin in SkinCatalog.All)
            {
                bool unlocked = game.IsSkinUnlocked(skin.Id);
                bool selected = game.SaveSystem.Data.selectedSkin == skin.Id;
                string id = skin.Id;
                UIButton tile = UIFactory.Button(_grid, "Skin_" + skin.Id, null, selected ? UITheme.Primary : UITheme.Neutral, () =>
                {
                    if (!game.IsSkinUnlocked(id)) return;
                    game.SelectSkin(id);
                    BuildSkins();
                });
                Transform content = tile.transform.Find("Content");
                Image body = UIFactory.Image(content, "Preview", ArtId.Circle, skin.BodyColor);
                UIFactory.SetPreferredSize(body.gameObject, 120f, 120f);
                content.GetComponent<HorizontalLayoutGroup>().enabled = false;
                var vertical = content.gameObject.AddComponent<VerticalLayoutGroup>();
                vertical.childAlignment = TextAnchor.MiddleCenter;
                vertical.childControlHeight = true;
                vertical.childControlWidth = true;
                vertical.childForceExpandWidth = false;
                vertical.spacing = 8f;
                TextMeshProUGUI name = UIFactory.LocText(content, "Name", skin.NameKey, UITheme.SmallSize, UITheme.TextLight, TextAlignmentOptions.Center, FontStyles.Bold);
                UIFactory.SetPreferredSize(name.gameObject, 220f, 50f);
                string status = unlocked ? (selected ? Loc.Get("skins.selected") : Loc.Get("skins.available")) : Loc.Format("skins.cost", skin.CrystalCost);
                TextMeshProUGUI state = UIFactory.Text(content, "State", status, 28f, UITheme.TextLight);
                UIFactory.SetPreferredSize(state.gameObject, 220f, 40f);
                if (!unlocked)
                {
                    Image lockIcon = UIFactory.Image(body.transform, "Lock", ArtId.IconLock, UITheme.TextDark.WithAlpha(0.8f));
                    UIFactory.Stretch(lockIcon.rectTransform, 30f, 30f, 30f, 30f);
                }
            }
        }
    }
}
