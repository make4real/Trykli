using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Placement
{
    public enum ZoneVisualState
    {
        Idle,
        Available,
        Target,
        Hidden
    }

    /// <summary>Area where the player may drop items (point slot, rectangle or rail).</summary>
    public sealed class PlacementZone : MonoBehaviour
    {
        [SerializeField] private PlacementZoneData data = new PlacementZoneData();
        [SerializeField] private int index;

        private static bool _creatingFromCode;

        private SpriteRenderer _outline;
        private SpriteRenderer _icon;
        private ZoneVisualState _state = ZoneVisualState.Idle;
        private bool _hinted;

        public PlacementZoneData Data => data;
        public int Index => index;
        public Vector2 Position => data.position;
        public bool IsHinted => _hinted;

        public static PlacementZone Create(Transform parent, PlacementZoneData data, int index, ItemCatalog catalog)
        {
            var go = new GameObject($"Zone_{index}_{data.shape}");
            go.transform.SetParent(parent, false);
            go.transform.position = data.position;
            go.transform.rotation = Quaternion.Euler(0f, 0f, data.shape == ZoneShape.Point ? 0f : data.rotation);
            _creatingFromCode = true;
            var zone = go.AddComponent<PlacementZone>();
            _creatingFromCode = false;
            zone.data = data.Clone();
            zone.index = index;
            zone.BuildVisual(catalog);
            return zone;
        }

        private void Awake()
        {
            // Zones authored inside a level prefab take their position from the transform.
            if (_creatingFromCode) return;
            if (_outline == null && transform.Find(VisualFactory.VisualRootName) == null)
            {
                data.position = transform.position;
                if (data.shape != ZoneShape.Point) data.rotation = transform.eulerAngles.z;
                BuildVisual(null);
            }
        }

        private void BuildVisual(ItemCatalog catalog)
        {
            Transform root = VisualFactory.GetOrCreateVisualRoot(transform, out _);
            Color color = Color.white.WithAlpha(0.7f);
            switch (data.shape)
            {
                case ZoneShape.Point:
                    _outline = VisualFactory.Sprite(root, "Outline", ArtId.DashedRing, color, SortingOrders.Zones, Vector2.zero, new Vector2(1.3f, 1.3f));
                    break;
                case ZoneShape.Rect:
                    _outline = VisualFactory.Sliced(root, "Outline", ArtId.ZoneOutline, color, SortingOrders.Zones,
                        new Vector2(Mathf.Max(0.8f, data.size.x + 0.6f), Mathf.Max(0.8f, data.size.y + 0.6f)));
                    break;
                default:
                    _outline = VisualFactory.Sliced(root, "Outline", ArtId.ZoneOutline, color, SortingOrders.Zones,
                        new Vector2(Mathf.Max(0.8f, data.size.x + 0.6f), 0.6f));
                    break;
            }

            // A single accepted item is shown as a faint icon: the player understands the slot at a glance.
            if (data.allowedItems != null && data.allowedItems.Count == 1)
            {
                ItemDefinition item = catalog != null ? catalog.Get(data.allowedItems[0]) : null;
                Sprite sprite = item != null ? item.GetIcon() : null;
                if (sprite != null)
                {
                    _icon = VisualFactory.Sprite(root, "Icon", ArtId.Circle, Color.white.WithAlpha(0.28f), SortingOrders.Zones + 1, Vector2.zero, new Vector2(0.55f, 0.55f));
                    _icon.sprite = sprite;
                }
            }

            ApplyState();
        }

        public void SetState(ZoneVisualState state)
        {
            if (_state == state) return;
            _state = state;
            ApplyState();
        }

        public void SetHinted(bool hinted)
        {
            _hinted = hinted;
            ApplyState();
        }

        private void ApplyState()
        {
            if (_outline == null) return;
            bool visible = _state != ZoneVisualState.Hidden || _hinted;
            _outline.gameObject.SetActive(visible);
            if (_icon != null) _icon.gameObject.SetActive(_state != ZoneVisualState.Hidden);
            switch (_state)
            {
                case ZoneVisualState.Available:
                    _outline.color = new Color(0.55f, 1f, 0.6f, 0.85f);
                    break;
                case ZoneVisualState.Target:
                    _outline.color = new Color(0.3f, 1f, 0.45f, 1f);
                    break;
                default:
                    _outline.color = Color.white.WithAlpha(0.6f);
                    break;
            }

            if (_hinted) _outline.color = new Color(1f, 0.85f, 0.2f, 1f);
        }

        private void Update()
        {
            if (_outline == null || (!_hinted && _state != ZoneVisualState.Target)) return;
            float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.06f;
            _outline.transform.localScale = data.shape == ZoneShape.Point ? new Vector3(1.3f * pulse, 1.3f * pulse, 1f) : new Vector3(pulse, pulse, 1f);
        }
    }
}
