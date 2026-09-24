using System;
using System.Collections.Generic;
using Trykli.Audio;
using Trykli.Data;
using Trykli.Feedback;
using Trykli.Gameplay;
using UnityEngine;

namespace Trykli.Placement
{
    /// <summary>
    /// Object placement (GDD 49): drag &amp; drop from the inventory, selection, move, rotation (buttons or
    /// double tap), deletion (drag to the inventory bar or delete button), placement zones validation.
    /// Active only in the Placement state; placed items are locked during the simulation.
    /// </summary>
    public sealed class PlacementManager : MonoBehaviour
    {
        private const float DragThresholdInches = 0.06f;
        private const float DoubleTapTime = 0.35f;
        private const float TouchLift = 0.55f;

        private readonly List<PlacedItem> _placed = new List<PlacedItem>();
        private readonly Dictionary<string, ItemDefinition> _definitions = new Dictionary<string, ItemDefinition>();
        private readonly List<Vector2> _occupiedBuffer = new List<Vector2>();

        private LevelContext _context;
        private Camera _camera;
        private PlacementRules.Settings _settings = PlacementRules.Settings.Default;

        private PlacedItem _pressedItem;
        private Vector2 _pressScreenPosition;
        private PlacedItem _dragItem;
        private bool _dragIsNew;
        private PlacementZone _dragOriginZone;
        private Vector2 _dragOriginPosition;
        private float _dragOriginRotation;
        private PlacementZone _dragTargetZone;
        private Vector2 _dragTargetPosition;
        private bool _dragOverTrash;
        private float _lastTapTime = -10f;
        private PlacedItem _lastTapItem;

        public InventoryModel Inventory { get; } = new InventoryModel();
        public IReadOnlyList<PlacedItem> PlacedItems => _placed;
        public PlacedItem Selected { get; private set; }
        public bool IsDragging => _dragItem != null;
        public bool IsDraggingOverTrash => _dragItem != null && _dragOverTrash;
        public bool InputEnabled { get; private set; }

        /// <summary>Provided by the HUD: true when the screen position is over the inventory bar (trash).</summary>
        public Func<Vector2, bool> IsOverTrash { get; set; }

        public event Action<PlacedItem> SelectionChanged;
        public event Action InventoryChanged;
        public event Action<bool> DragStateChanged;
        public event Action<PlacedItem> ItemPlaced;

        private void Awake()
        {
            Inventory.Changed += () => InventoryChanged?.Invoke();
        }

        /// <summary>Prepares a freshly built level. Resets the inventory to the level's items.</summary>
        public void Setup(LevelContext context, IReadOnlyList<ItemStack> stacks, Camera camera)
        {
            CancelDrag();
            _context = context;
            _camera = camera;
            _placed.Clear();
            Selected = null;
            _definitions.Clear();

            GameplayConfig config = GameplayConfig.Instance;
            _settings.PointSnapDistance = config.pointZoneSnapDistance;
            _settings.GridStep = config.placementGridStep;
            _settings.MinSpacing = config.minItemSpacing;

            ResetInventory(stacks);
            SelectionChanged?.Invoke(null);
        }

        private void ResetInventory(IReadOnlyList<ItemStack> stacks)
        {
            Inventory.Clear();
            if (stacks == null) return;
            foreach (ItemStack stack in stacks)
            {
                if (stack?.item == null || stack.count <= 0) continue;
                _definitions[stack.item.itemId] = stack.item;
                Inventory.Add(stack.item.itemId, stack.count, stack.item.PiecesPerUnit);
            }
        }

        public ItemDefinition GetDefinition(string itemId)
        {
            return itemId != null && _definitions.TryGetValue(itemId, out ItemDefinition item) ? item : null;
        }

        public IEnumerable<ItemDefinition> Definitions => _definitions.Values;

        public void SetInputEnabled(bool enabled)
        {
            InputEnabled = enabled;
            if (!enabled)
            {
                CancelDrag();
                Select(null);
            }

            foreach (PlacedItem item in _placed) item.Locked = !enabled;
            RefreshZones();
        }

        public List<PlacementRecord> CaptureRecords()
        {
            var records = new List<PlacementRecord>();
            foreach (PlacedItem item in _placed) records.Add(item.ToRecord());
            return records;
        }

        /// <summary>Recreates placements (Restart / Modify) in a rebuilt level.</summary>
        public void Restore(IEnumerable<PlacementRecord> records)
        {
            if (records == null) return;
            foreach (PlacementRecord record in records)
            {
                ItemDefinition item = GetDefinition(record.itemId);
                if (item == null || !Inventory.TryTake(record.itemId)) continue;
                PlacementZone zone = FindZone(record.zoneIndex);
                PlacedItem placed = CreateItem(item, record.endpoint);
                placed.PlaceAt(zone, record.position, record.rotation);
                placed.SetDragVisual(false, true);
                _placed.Add(placed);
            }

            RefreshZones();
        }

        /// <summary>Places an item directly (reference solution, tests, hints preview).</summary>
        public PlacedItem PlaceDirect(string itemId, int zoneIndex, Vector2 position, float rotation)
        {
            ItemDefinition item = GetDefinition(itemId);
            PlacementZone zone = FindZone(zoneIndex);
            if (item == null || zone == null || !Inventory.TryTake(itemId)) return null;
            PlacedItem placed = CreateItem(item, NextEndpoint(item));
            Vector2 target = zone.Data.shape == ZoneShape.Point ? zone.Position : position;
            placed.PlaceAt(zone, target, PlacementRules.ClampRotation(zone.Data, rotation, item.rotationStep, item.rotatable));
            placed.SetDragVisual(false, true);
            _placed.Add(placed);
            return placed;
        }

        public void ClearAll()
        {
            CancelDrag();
            Select(null);
            foreach (PlacedItem item in _placed)
            {
                if (item != null) Destroy(item.gameObject);
            }

            _placed.Clear();
            Inventory.ReturnAll();
            RefreshZones();
        }

        public void BeginInventoryDrag(ItemDefinition item)
        {
            if (!InputEnabled || item == null || IsDragging) return;
            if (!Inventory.TryTake(item.itemId)) return;
            PlacedItem placed = CreateItem(item, NextEndpoint(item));
            placed.SetRotation(item.defaultRotation);
            placed.transform.position = PointerWorld(PointerInput.ScreenPosition);
            StartDrag(placed, isNew: true);
            AudioManager.PlaySfx(SfxId.Pickup);
            Haptics.Play(HapticType.Light);
        }

        public void RotateSelected(int direction)
        {
            if (Selected == null || Selected.Locked || Selected.Item == null) return;
            PlacementZoneData zone = Selected.Zone != null ? Selected.Zone.Data : null;
            float next = PlacementRules.Rotate(zone, Selected.Rotation, Selected.Item.rotationStep, direction, Selected.Item.rotatable);
            if (Mathf.Approximately(next, Selected.Rotation)) return;
            Selected.SetRotation(next);
            AudioManager.PlaySfx(SfxId.Rotate);
            Haptics.Play(HapticType.Light);
        }

        public bool CanRotateSelected()
        {
            if (Selected == null || Selected.Item == null || !Selected.Item.rotatable) return false;
            return Selected.Zone == null || Selected.Zone.Data.allowRotation;
        }

        public void DeleteSelected()
        {
            if (Selected == null || Selected.Locked) return;
            PlacedItem item = Selected;
            Select(null);
            RemoveItem(item);
        }

        private void Update()
        {
            if (!InputEnabled || _context == null || _camera == null) return;
            PointerPhase phase = PointerInput.Phase;
            Vector2 screen = PointerInput.ScreenPosition;

            if (IsDragging)
            {
                if (phase == PointerPhase.Up || phase == PointerPhase.None) EndDrag();
                else UpdateDrag(screen);
                return;
            }

            switch (phase)
            {
                case PointerPhase.Down:
                    if (PointerInput.IsOverUI(screen))
                    {
                        _pressedItem = null;
                        return;
                    }

                    _pressedItem = FindItemAt(PointerWorld(screen));
                    _pressScreenPosition = screen;
                    if (_pressedItem == null) Select(null);
                    break;
                case PointerPhase.Held:
                    if (_pressedItem != null && (screen - _pressScreenPosition).magnitude > DragThresholdPixels())
                    {
                        StartDrag(_pressedItem, isNew: false);
                        _pressedItem = null;
                    }

                    break;
                case PointerPhase.Up:
                    if (_pressedItem != null) HandleTap(_pressedItem);
                    _pressedItem = null;
                    break;
            }
        }

        private void HandleTap(PlacedItem item)
        {
            bool doubleTap = item == _lastTapItem && Time.unscaledTime - _lastTapTime < DoubleTapTime && Selected == item;
            _lastTapItem = item;
            _lastTapTime = Time.unscaledTime;
            Select(item);
            if (doubleTap)
            {
                RotateSelected(1);
                _lastTapItem = null;
            }
        }

        private void StartDrag(PlacedItem item, bool isNew)
        {
            _dragItem = item;
            _dragIsNew = isNew;
            _dragOriginZone = item.Zone;
            _dragOriginPosition = item.transform.position;
            _dragOriginRotation = item.Rotation;
            _dragTargetZone = null;
            item.Detach();
            item.SetDragVisual(true, false);
            Select(item);
            DragStateChanged?.Invoke(true);
            UpdateDrag(PointerInput.ScreenPosition);
        }

        private void UpdateDrag(Vector2 screen)
        {
            Vector2 world = PointerWorld(screen);
            if (Input.touchCount > 0) world += Vector2.up * TouchLift;

            _dragOverTrash = IsOverTrash != null && IsOverTrash(screen);
            bool valid = !_dragOverTrash && TryFindTarget(_dragItem.Item, world, out _dragTargetZone, out _dragTargetPosition);
            if (!valid) _dragTargetZone = null;

            _dragItem.transform.position = valid ? _dragTargetPosition : world;
            if (valid)
            {
                float rotation = _dragIsNew ? _dragTargetZone.Data.defaultRotation : _dragOriginRotation;
                _dragItem.SetRotation(PlacementRules.ClampRotation(_dragTargetZone.Data, rotation, _dragItem.Item.rotationStep, _dragItem.Item.rotatable));
            }

            _dragItem.SetDragVisual(true, valid);
            RefreshZones();
        }

        private void EndDrag()
        {
            PlacedItem item = _dragItem;
            _dragItem = null;
            if (item == null) return;

            if (_dragOverTrash)
            {
                RemoveItem(item);
                Select(null);
            }
            else if (_dragTargetZone != null)
            {
                item.PlaceAt(_dragTargetZone, _dragTargetPosition, item.Rotation);
                item.SetDragVisual(false, true);
                if (!_placed.Contains(item)) _placed.Add(item);
                Select(item);
                AudioManager.PlaySfx(SfxId.Place);
                Haptics.Play(HapticType.Light);
                ItemPlaced?.Invoke(item);
            }
            else if (_dragIsNew)
            {
                RemoveItem(item);
                Select(null);
            }
            else
            {
                item.PlaceAt(_dragOriginZone, _dragOriginPosition, _dragOriginRotation);
                item.SetDragVisual(false, true);
            }

            _dragOverTrash = false;
            _dragTargetZone = null;
            DragStateChanged?.Invoke(false);
            RefreshZones();
        }

        private void CancelDrag()
        {
            if (_dragItem == null) return;
            PlacedItem item = _dragItem;
            _dragItem = null;
            if (_dragIsNew) RemoveItem(item);
            else
            {
                item.PlaceAt(_dragOriginZone, _dragOriginPosition, _dragOriginRotation);
                item.SetDragVisual(false, true);
            }

            DragStateChanged?.Invoke(false);
        }

        private bool TryFindTarget(ItemDefinition item, Vector2 world, out PlacementZone bestZone, out Vector2 bestPosition)
        {
            bestZone = null;
            bestPosition = world;
            float bestDistance = float.MaxValue;
            foreach (PlacementZone zone in _context.Zones)
            {
                if (zone == null) continue;
                _occupiedBuffer.Clear();
                foreach (PlacedItem other in _placed)
                {
                    if (other != _dragItem && other.Zone == zone) _occupiedBuffer.Add(other.Position);
                }

                if (!PlacementRules.TryResolve(zone.Data, item.itemId, world, _occupiedBuffer, _settings, out Vector2 position, out float distance)) continue;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestZone = zone;
                    bestPosition = position;
                }
            }

            return bestZone != null;
        }

        private void RefreshZones()
        {
            if (_context == null) return;
            foreach (PlacementZone zone in _context.Zones)
            {
                if (zone == null) continue;
                if (!InputEnabled) zone.SetState(ZoneVisualState.Hidden);
                else if (_dragItem != null && zone == _dragTargetZone) zone.SetState(ZoneVisualState.Target);
                else if (_dragItem != null && zone.Data.Accepts(_dragItem.Item.itemId)) zone.SetState(ZoneVisualState.Available);
                else zone.SetState(ZoneVisualState.Idle);
            }
        }

        private void RemoveItem(PlacedItem item)
        {
            if (item == null) return;
            _placed.Remove(item);
            if (item.Item != null) Inventory.Return(item.Item.itemId);
            Destroy(item.gameObject);
            AudioManager.PlaySfx(SfxId.Remove);
            ReassignPortalEndpoints();
        }

        private PlacedItem CreateItem(ItemDefinition item, string endpoint)
        {
            ElementData data = ElementFactory.DataForItem(item, Vector2.zero, item.defaultRotation, endpoint);
            GameObject go = ElementFactory.Create(data, _context.PlacedRoot, _context.Theme);
            go.name = "Placed_" + item.itemId;
            var placed = go.AddComponent<PlacedItem>();
            placed.Initialize(item, endpoint);
            return placed;
        }

        private string NextEndpoint(ItemDefinition item)
        {
            if (item.kind != ItemKind.Portal) return string.Empty;
            bool hasA = false;
            foreach (PlacedItem placed in _placed)
            {
                if (placed != null && placed.Item == item && placed.Endpoint == "A") hasA = true;
            }

            if (_dragItem != null && _dragItem.Item == item && _dragItem.Endpoint == "A") hasA = true;
            return hasA ? "B" : "A";
        }

        private void ReassignPortalEndpoints()
        {
            // Keeps "A" assigned when "A" is deleted and "B" remains, so the letters stay consistent.
            var seen = new HashSet<ItemDefinition>();
            foreach (PlacedItem placed in _placed)
            {
                if (placed == null || placed.Item == null || placed.Item.kind != ItemKind.Portal) continue;
                placed.SetEndpoint(seen.Add(placed.Item) ? "A" : "B");
            }
        }

        private PlacedItem FindItemAt(Vector2 world)
        {
            PlacedItem best = null;
            float bestDistance = float.MaxValue;
            foreach (PlacedItem item in _placed)
            {
                if (item == null || item.Locked || !item.ContainsPoint(world)) continue;
                float distance = (item.Position - world).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = item;
                }
            }

            return best;
        }

        private PlacementZone FindZone(int index)
        {
            if (_context == null) return null;
            foreach (PlacementZone zone in _context.Zones)
            {
                if (zone != null && zone.Index == index) return zone;
            }

            return null;
        }

        private void Select(PlacedItem item)
        {
            if (Selected == item) return;
            if (Selected != null) Selected.SetSelected(false);
            Selected = item;
            if (Selected != null) Selected.SetSelected(true);
            SelectionChanged?.Invoke(Selected);
        }

        private Vector2 PointerWorld(Vector2 screen) => PointerInput.ToWorld(_camera, screen);

        private static float DragThresholdPixels()
        {
            float dpi = Screen.dpi > 0f ? Screen.dpi : 160f;
            return Mathf.Max(8f, dpi * DragThresholdInches);
        }
    }
}
