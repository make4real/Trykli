using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Mechanics;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Placement
{
    /// <summary>An inventory item placed (or being dragged) in the level by the player.</summary>
    public sealed class PlacedItem : MonoBehaviour
    {
        public const float PickRadius = 0.7f;

        private SpriteRenderer[] _renderers;
        private Color[] _baseColors;
        private int[] _baseOrders;
        private Collider2D[] _colliders;
        private SpriteRenderer _selection;

        public ItemDefinition Item { get; private set; }
        public PlacementZone Zone { get; private set; }
        public float Rotation { get; private set; }
        public string Endpoint { get; private set; } = "";
        public bool Locked { get; set; }
        public Vector2 Position => transform.position;

        public void Initialize(ItemDefinition item, string endpoint)
        {
            Item = item;
            Endpoint = endpoint ?? string.Empty;
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _baseColors = new Color[_renderers.Length];
            _baseOrders = new int[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sortingOrder += SortingOrders.PlacedItems - SortingOrders.Mechanisms;
                _baseColors[i] = _renderers[i].color;
                _baseOrders[i] = _renderers[i].sortingOrder;
            }

            _colliders = GetComponentsInChildren<Collider2D>(true);
            _selection = VisualFactory.Sprite(transform, "Selection", ArtId.DashedRing, new Color(1f, 0.9f, 0.3f, 0.9f), SortingOrders.Hints,
                Vector2.zero, new Vector2(1.6f, 1.6f));
            _selection.gameObject.SetActive(false);
        }

        public void PlaceAt(PlacementZone zone, Vector2 position, float rotation)
        {
            Zone = zone;
            transform.position = position;
            SetRotation(rotation);
            ApplyZoneLink(zone != null ? zone.Data : null, GetComponent<MechanismBase>());
        }

        /// <summary>Items placed in a zone linked to a switch channel only work once that channel is signalled.</summary>
        public static void ApplyZoneLink(PlacementZoneData zone, MechanismBase mechanism)
        {
            if (mechanism == null || mechanism is Portal) return;
            if (zone != null && zone.linkedChannel >= 0) mechanism.OverrideActivation(ActivationMode.ActivatedBySignal, zone.linkedChannel);
            else mechanism.OverrideActivation(ActivationMode.AlwaysActive, -1);
        }

        public void Detach()
        {
            Zone = null;
        }

        public void SetRotation(float rotation)
        {
            Rotation = MathUtils.NormalizeAngle(rotation);
            transform.rotation = Quaternion.Euler(0f, 0f, Rotation);
            if (_selection != null) _selection.transform.rotation = Quaternion.identity;
        }

        public void SetEndpoint(string endpoint)
        {
            Endpoint = endpoint;
            var portal = GetComponent<Portal>();
            if (portal != null && Item != null) portal.SetEndpoint(Mathf.Max(0, Item.portalChannel), endpoint);
        }

        public void SetSelected(bool selected)
        {
            if (_selection != null) _selection.gameObject.SetActive(selected);
        }

        public void SetDragVisual(bool dragging, bool valid)
        {
            foreach (Collider2D collider in _colliders)
            {
                if (collider != null) collider.enabled = !dragging;
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;
                Color color = _baseColors[i];
                if (dragging)
                {
                    color = valid ? color : Color.Lerp(color, new Color(1f, 0.3f, 0.3f, color.a), 0.6f);
                    color.a *= 0.75f;
                }

                _renderers[i].color = color;
                _renderers[i].sortingOrder = _baseOrders[i] + (dragging ? SortingOrders.Dragged * 10 : 0);
            }
        }

        public bool ContainsPoint(Vector2 worldPoint)
        {
            if ((worldPoint - (Vector2)transform.position).magnitude <= PickRadius) return true;
            foreach (Collider2D collider in _colliders)
            {
                if (collider != null && collider.enabled && collider.OverlapPoint(worldPoint)) return true;
            }

            return false;
        }

        public PlacementRecord ToRecord()
        {
            return new PlacementRecord
            {
                itemId = Item != null ? Item.itemId : string.Empty,
                zoneIndex = Zone != null ? Zone.Index : -1,
                position = transform.position,
                rotation = Rotation,
                endpoint = Endpoint
            };
        }
    }
}
