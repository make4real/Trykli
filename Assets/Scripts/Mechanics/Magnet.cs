using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Utilities;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Attracts Trykli inside a defined radius. The acceleration grows linearly as Trykli gets closer
    /// (40 % at the edge, 100 % at the center). Can be switched on / off by buttons.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class Magnet : MechanismBase
    {
        [Header("Magnet")]
        [SerializeField] private float fieldRadius = 3f;
        [Tooltip("Maximum attraction acceleration (m/s^2).")]
        [SerializeField] private float strength = 20f;
        [SerializeField] private float bodyRadius = 0.35f;

        private Transform _wave;
        private float _waveTime;

        public float FieldRadius => fieldRadius;

        protected override void ApplyData(ElementData data)
        {
            fieldRadius = OrDefault(data.radius, fieldRadius);
            strength = OrDefault(data.power, strength);
        }

        protected override void BuildVisual(Transform root)
        {
            Color red = ColorUtils.Hex("#E74C3C");
            VisualFactory.Sprite(root, "Field", ArtId.Circle, red.WithAlpha(0.07f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "FieldEdge", ArtId.DashedRing, red.WithAlpha(0.35f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Wave", ArtId.Ring, red.WithAlpha(0.3f), SortingOrders.Effects, Vector2.zero, Vector2.one);
            VisualFactory.Sprite(root, "Body", ArtId.ItemMagnet, Color.white, SortingOrders.Mechanisms, Vector2.zero, new Vector2(0.9f, 0.9f));
        }

        protected override void ApplyLayout(Transform root)
        {
            GetComponent<CircleCollider2D>().radius = bodyRadius;
            float d = fieldRadius * 2f;
            Transform field = FindVisual("Field");
            if (field != null) field.localScale = new Vector3(d, d, 1f);
            Transform edge = FindVisual("FieldEdge");
            if (edge != null) edge.localScale = new Vector3(d, d, 1f);
            _wave = FindVisual("Wave");
        }

        protected override void RefreshActiveVisual()
        {
            Transform field = FindVisual("Field");
            if (field != null) field.gameObject.SetActive(IsActive);
            if (_wave != null) _wave.gameObject.SetActive(IsActive);
            SpriteRenderer body = FindRenderer("Body");
            if (body != null) body.color = IsActive ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        public override void OnSimulationStep(float deltaTime, float simulationTime)
        {
            TrykliController trykli = Trykli;
            if (!IsActive || trykli == null || !trykli.IsAlive) return;
            Vector2 toMagnet = (Vector2)transform.position - trykli.Position;
            float distance = toMagnet.magnitude;
            if (distance > fieldRadius || distance < 0.05f) return;
            float factor = 0.4f + 0.6f * (1f - distance / fieldRadius);
            trykli.ApplyAcceleration(toMagnet / distance * strength * factor);
        }

        private void Update()
        {
            if (_wave == null || !IsActive) return;
            _waveTime = Mathf.Repeat(_waveTime + Time.deltaTime * 0.8f, 1f);
            float d = fieldRadius * 2f * (1f - _waveTime);
            _wave.localScale = new Vector3(d, d, 1f);
            _wave.GetComponent<SpriteRenderer>().color = ColorUtils.Hex("#E74C3C").WithAlpha(0.35f * _waveTime);
        }
    }
}
