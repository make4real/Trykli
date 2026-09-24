using System;
using Trykli.Audio;
using Trykli.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Trykli.UI
{
    /// <summary>Large rounded button with press animation, click sound and light haptic feedback.</summary>
    [RequireComponent(typeof(Image), typeof(Button))]
    public sealed class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private Vector3 _baseScale = Vector3.one;
        private float _targetScale = 1f;
        private Color _color;

        public Button Button { get; private set; }
        public Image Background { get; private set; }
        public TextMeshProUGUI Label { get; set; }
        public LocalizedText LocalizedLabel { get; set; }
        public Image Icon { get; set; }
        public bool PlayClickSound { get; set; } = true;

        public event Action PointerDown;

        public bool Interactable
        {
            get => Button.interactable;
            set
            {
                Button.interactable = value;
                Background.color = value ? _color : Color.Lerp(_color, Color.gray, 0.6f);
            }
        }

        private void Awake()
        {
            Button = GetComponent<Button>();
            Background = GetComponent<Image>();
            _color = Background.color;
            Button.transition = Selectable.Transition.None;
            Button.onClick.AddListener(OnClicked);
        }

        public void SetColor(Color color)
        {
            _color = color;
            if (Background != null) Background.color = Button != null && !Button.interactable ? Color.Lerp(color, Color.gray, 0.6f) : color;
        }

        public void SetLabelKey(string key, params object[] args)
        {
            if (LocalizedLabel != null) LocalizedLabel.SetKey(key, args);
        }

        public void SetText(string text)
        {
            if (LocalizedLabel != null) LocalizedLabel.enabled = false;
            if (Label != null) Label.text = text;
        }

        public void OnClick(Action action)
        {
            if (action != null) Button.onClick.AddListener(() => action());
        }

        private void OnClicked()
        {
            if (PlayClickSound) AudioManager.PlaySfx(SfxId.UIClick);
            Haptics.Play(HapticType.Light);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Button.interactable) return;
            _targetScale = 0.93f;
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData) => _targetScale = 1f;

        public void OnPointerExit(PointerEventData eventData) => _targetScale = 1f;

        private void OnEnable()
        {
            _baseScale = Vector3.one;
            transform.localScale = _baseScale;
            _targetScale = 1f;
        }

        private void Update()
        {
            float current = transform.localScale.x / Mathf.Max(0.001f, _baseScale.x);
            if (Mathf.Abs(current - _targetScale) < 0.001f) return;
            float next = Mathf.MoveTowards(current, _targetScale, Time.unscaledDeltaTime * 2.5f);
            transform.localScale = _baseScale * next;
        }
    }
}
