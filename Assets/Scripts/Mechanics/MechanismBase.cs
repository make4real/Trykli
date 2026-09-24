using Trykli.Data;
using Trykli.Gameplay;
using UnityEngine;

namespace Trykli.Mechanics
{
    /// <summary>
    /// Base class of every mechanism: layout configuration, activation through switch channels
    /// (buttons) and deterministic simulation steps.
    /// </summary>
    public abstract class MechanismBase : MonoBehaviour, IConfigurableElement, ILevelElement, ISimulationElement
    {
        [Header("Activation")]
        [SerializeField] protected ActivationMode activation = ActivationMode.AlwaysActive;
        [Tooltip("Switch channel listened to (-1 = none).")]
        [SerializeField] protected int listenChannel = -1;
        [SerializeField] protected string elementId = "";

        protected LevelContext Context { get; private set; }
        protected TrykliController Trykli => Context != null ? Context.Trykli : null;
        protected WorldTheme Theme { get; private set; } = WorldTheme.Default;
        protected Transform VisualRoot { get; private set; }

        public bool IsActive { get; private set; } = true;
        public string ElementId => elementId;
        public ActivationMode Activation => activation;

        /// <summary>Channel this mechanism listens to. Portals use their channel as pair id instead.</summary>
        protected virtual int SignalChannel => listenChannel;

        /// <summary>
        /// Changes how the mechanism reacts to buttons (used when an item is placed in a zone linked to a channel).
        /// Takes effect when the simulation starts.
        /// </summary>
        public void OverrideActivation(ActivationMode mode, int channel)
        {
            activation = mode;
            listenChannel = channel;
            IsActive = mode != ActivationMode.ActivatedBySignal;
            RefreshActiveVisual();
        }

        protected virtual void Awake()
        {
            // Mechanisms authored directly in a level prefab are not configured from data: use their visual child.
            if (VisualRoot == null) VisualRoot = transform.Find(VisualFactory.VisualRootName);
        }

        public void Configure(ElementData data, WorldTheme theme)
        {
            Theme = theme;
            activation = data.activation;
            listenChannel = data.channel;
            elementId = data.id ?? string.Empty;
            ApplyData(data);
            VisualRoot = VisualFactory.GetOrCreateVisualRoot(transform, out bool created);
            if (created) BuildVisual(VisualRoot);
            ApplyLayout(VisualRoot);
            IsActive = activation != ActivationMode.ActivatedBySignal;
            RefreshActiveVisual();
        }

        /// <summary>Reads the parameters of the layout element (0 = keep the default value).</summary>
        protected abstract void ApplyData(ElementData data);

        /// <summary>Creates the placeholder visual (only when the object has no "Visual" child).</summary>
        protected abstract void BuildVisual(Transform root);

        /// <summary>Applies sizes / colliders / colors. Called after every Configure.</summary>
        protected virtual void ApplyLayout(Transform root) { }

        public virtual void Bind(LevelContext context)
        {
            Context = context;
            IsActive = activation != ActivationMode.ActivatedBySignal;
            if (activation != ActivationMode.AlwaysActive && SignalChannel >= 0) context.Switches.Subscribe(SignalChannel, OnSignal);
            RefreshActiveVisual();
        }

        private void OnSignal()
        {
            switch (activation)
            {
                case ActivationMode.ActivatedBySignal:
                    SetActive(true);
                    break;
                case ActivationMode.DeactivatedBySignal:
                    SetActive(false);
                    break;
                case ActivationMode.ToggledBySignal:
                    SetActive(!IsActive);
                    break;
            }
        }

        protected void SetActive(bool active)
        {
            if (IsActive == active) return;
            IsActive = active;
            OnActiveChanged(active);
            RefreshActiveVisual();
        }

        protected virtual void OnActiveChanged(bool active) { }

        protected virtual void RefreshActiveVisual() { }

        public virtual void OnSimulationStart() { }

        public virtual void OnSimulationStep(float deltaTime, float simulationTime) { }

        protected Transform FindVisual(string childName)
        {
            return VisualRoot != null ? VisualRoot.Find(childName) : null;
        }

        protected SpriteRenderer FindRenderer(string childName)
        {
            Transform child = FindVisual(childName);
            return child != null ? child.GetComponent<SpriteRenderer>() : null;
        }

        protected static float OrDefault(float value, float fallback) => value > 0f ? value : fallback;

        protected static Vector2 OrDefault(Vector2 value, Vector2 fallback) => value.x > 0f && value.y > 0f ? value : fallback;
    }
}
