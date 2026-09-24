using Trykli.Data;
using UnityEngine;

namespace Trykli.Gameplay
{
    /// <summary>
    /// Orthographic portrait camera. Fits the level bounds inside the screen area left free by the HUD
    /// (top bar / inventory bar and safe areas). Large levels can follow Trykli, clamped to the bounds (CameraBounds).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraController : MonoBehaviour
    {
        private const float Margin = 1.04f;

        private Camera _camera;
        private Rect _bounds = new Rect(-5f, -5f, 10f, 14f);
        private CameraSettingsData _settings = new CameraSettingsData();
        private float _topInsetPixels;
        private float _bottomInsetPixels;
        private Transform _target;
        private int _lastWidth;
        private int _lastHeight;

        public Camera Camera => _camera;

        public static CameraController EnsureMainCamera(Color background)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                camera = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }

            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = background;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            var controller = camera.GetComponent<CameraController>();
            if (controller == null) controller = camera.gameObject.AddComponent<CameraController>();
            controller._camera = camera;
            return controller;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void Configure(Rect bounds, CameraSettingsData settings, Transform target)
        {
            _bounds = bounds;
            _settings = settings ?? new CameraSettingsData();
            _target = target;
            Fit();
        }

        /// <summary>Screen pixels covered by the HUD at the top and bottom of the screen.</summary>
        public void SetScreenInsets(float topPixels, float bottomPixels)
        {
            _topInsetPixels = Mathf.Max(0f, topPixels);
            _bottomInsetPixels = Mathf.Max(0f, bottomPixels);
            Fit();
        }

        public void Fit()
        {
            if (_camera == null) return;
            float screenHeight = Mathf.Max(1f, Screen.height);
            float screenWidth = Mathf.Max(1f, Screen.width);
            float usableFraction = Mathf.Clamp((screenHeight - _topInsetPixels - _bottomInsetPixels) / screenHeight, 0.3f, 1f);
            float aspect = screenWidth / screenHeight;

            float size;
            if (_settings.autoFit || _settings.orthographicSize <= 0f)
            {
                float sizeForHeight = _bounds.height / (2f * usableFraction);
                float sizeForWidth = _bounds.width / (2f * aspect);
                size = Mathf.Max(sizeForHeight, sizeForWidth) * Margin;
            }
            else
            {
                size = _settings.orthographicSize;
            }

            _camera.orthographicSize = size;
            transform.position = new Vector3(_bounds.center.x, _bounds.center.y - ScreenOffsetWorld(size), -10f);
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
        }

        /// <summary>World offset between the screen center and the center of the free area.</summary>
        private float ScreenOffsetWorld(float size)
        {
            float screenHeight = Mathf.Max(1f, Screen.height);
            float freeCenter = _bottomInsetPixels + (screenHeight - _topInsetPixels - _bottomInsetPixels) * 0.5f;
            return (freeCenter - screenHeight * 0.5f) / screenHeight * 2f * size;
        }

        private void LateUpdate()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight) Fit();
            if (!_settings.followTrykli || _target == null || _camera == null) return;

            float size = _camera.orthographicSize;
            float halfWidth = size * _camera.aspect;
            Vector3 position = transform.position;
            float offset = ScreenOffsetWorld(size);
            float targetX = Mathf.Clamp(_target.position.x, _bounds.xMin + halfWidth, _bounds.xMax - halfWidth);
            float targetY = Mathf.Clamp(_target.position.y - offset, _bounds.yMin + size - offset, _bounds.yMax - size - offset);
            if (_bounds.width < halfWidth * 2f) targetX = _bounds.center.x;
            if (_bounds.height < size * 2f) targetY = _bounds.center.y - offset;
            position.x = Mathf.Lerp(position.x, targetX, 1f - Mathf.Exp(-6f * Time.deltaTime));
            position.y = Mathf.Lerp(position.y, targetY, 1f - Mathf.Exp(-6f * Time.deltaTime));
            transform.position = position;
        }
    }
}
