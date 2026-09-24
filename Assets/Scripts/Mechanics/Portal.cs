using Trykli.Audio;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Objectives;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Portal endpoint. Two portals sharing a channel form a pair: entering one moves Trykli to the other,
    /// keeping its speed (multiplied by <see cref="speedRetention"/>) and redirecting it along the exit
    /// portal's up direction (directional teleporter). The exit portal ignores Trykli until it leaves it,
    /// which prevents immediate infinite teleport loops.
    /// </summary>
    public sealed class Portal : MechanismBase
    {
        private static readonly Color[] ChannelColors =
        {
            ColorUtils.Hex("#9B59FF"), ColorUtils.Hex("#FF8C42"), ColorUtils.Hex("#1ABC9C"), ColorUtils.Hex("#E84393")
        };

        [Header("Portal")]
        [SerializeField] private int channel;
        [SerializeField] private string endpoint = "A";
        [SerializeField] private float radius = 0.45f;
        [Range(0f, 1.5f)]
        [SerializeField] private float speedRetention = 0.95f;
        [Tooltip("Minimum exit speed so Trykli always leaves the exit portal.")]
        [SerializeField] private float minimumExitSpeed = 3f;
        [SerializeField] private float exitOffset = 0.7f;
        [SerializeField] private float globalCooldown = 0.1f;

        private bool _ignoreUntilExit;
        private Transform _swirl;

        public int Channel => channel;
        public string Endpoint => endpoint;
        public float Radius => radius;
        protected override int SignalChannel => -1;

        public static Color ColorForChannel(int channel)
        {
            return ChannelColors[Mathf.Abs(channel) % ChannelColors.Length];
        }

        protected override void ApplyData(ElementData data)
        {
            channel = Mathf.Max(0, data.channel);
            endpoint = string.IsNullOrEmpty(data.endpoint) ? "A" : data.endpoint;
            radius = OrDefault(data.radius, radius);
            if (data.power > 0f) speedRetention = data.power;
        }

        /// <summary>Changes the endpoint letter (used when the player places a portal pair).</summary>
        public void SetEndpoint(int newChannel, string newEndpoint)
        {
            channel = newChannel;
            endpoint = newEndpoint;
            ApplyLayout(VisualRoot);
        }

        protected override void BuildVisual(Transform root)
        {
            VisualFactory.Sprite(root, "Glow", ArtId.SoftGlow, Color.white, SortingOrders.Mechanisms - 1, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Core", ArtId.Circle, new Color(0.1f, 0.08f, 0.2f, 0.9f), SortingOrders.Mechanisms, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Ring", ArtId.Ring, Color.white, SortingOrders.Mechanisms + 1, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Swirl", ArtId.DashedRing, Color.white, SortingOrders.Mechanisms + 1, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Exit", ArtId.Arrow, Color.white, SortingOrders.Mechanisms + 2, Vector2.zero, new Vector2(0.35f, 0.35f));
            VisualFactory.Sprite(root, "Dot1", ArtId.Circle, Color.white, SortingOrders.Mechanisms + 2, Vector2.zero, new Vector2(0.12f, 0.12f));
            VisualFactory.Sprite(root, "Dot2", ArtId.Circle, Color.white, SortingOrders.Mechanisms + 2, Vector2.zero, new Vector2(0.12f, 0.12f));
        }

        protected override void ApplyLayout(Transform root)
        {
            if (root == null) return;
            Color color = ColorForChannel(channel);
            float d = radius * 2f;
            SetSprite("Glow", color.WithAlpha(0.45f), d * 2f);
            SetSprite("Core", new Color(0.1f, 0.08f, 0.2f, 0.9f), d * 0.9f);
            SetSprite("Ring", color, d);
            SetSprite("Swirl", color.Lighten(0.5f), d * 0.7f);
            SpriteRenderer exit = FindRenderer("Exit");
            if (exit != null)
            {
                exit.color = color.Lighten(0.3f);
                exit.transform.localPosition = new Vector3(0f, radius + 0.25f, 0f);
            }

            // Endpoint marker: one dot for A, two dots for B (readable without text).
            bool isB = endpoint == "B";
            Transform dot1 = FindVisual("Dot1");
            Transform dot2 = FindVisual("Dot2");
            if (dot1 != null) dot1.localPosition = new Vector3(isB ? -0.09f : 0f, 0f, 0f);
            if (dot2 != null)
            {
                dot2.gameObject.SetActive(isB);
                dot2.localPosition = new Vector3(0.09f, 0f, 0f);
            }

            _swirl = FindVisual("Swirl");
        }

        private void SetSprite(string childName, Color color, float size)
        {
            SpriteRenderer renderer = FindRenderer(childName);
            if (renderer == null) return;
            renderer.color = color;
            renderer.transform.localScale = new Vector3(size, size, 1f);
        }

        public override void OnSimulationStart()
        {
            _ignoreUntilExit = false;
        }

        public void MarkArrival()
        {
            _ignoreUntilExit = true;
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (trykli == null || !trykli.IsAlive || trykli.IsCaptured) return;
            float distance = (trykli.Position - (Vector2)transform.position).magnitude;

            if (_ignoreUntilExit)
            {
                if (distance > radius + trykli.Radius + 0.1f) _ignoreUntilExit = false;
                return;
            }

            if (simulationTime < Context.NextTeleportTime) return;
            bool entered = ElementGeometry.SweptCircleOverlapsCircle(trykli.PreviousPosition, trykli.Position, trykli.Radius * 0.5f, transform.position, radius);
            if (!entered) return;

            Portal partner = Context.FindPartner(this);
            if (partner == null) return;

            Vector2 exitDirection = partner.transform.up;
            float speed = Mathf.Max(trykli.Speed * speedRetention, minimumExitSpeed);
            partner.MarkArrival();
            Context.NextTeleportTime = simulationTime + globalCooldown;
            trykli.TeleportTo((Vector2)partner.transform.position + exitDirection * exitOffset, exitDirection * speed);
            Context.Tracker.RecordCounter(LevelRunStats.Counters.PortalUsed);
            Context.Tracker.RecordCounter(LevelRunStats.Counters.Portal(channel));
            AudioManager.PlaySfx(SfxId.Portal);
        }

        private void Update()
        {
            if (_swirl != null) _swirl.Rotate(0f, 0f, 200f * Time.deltaTime);
        }
    }
}
