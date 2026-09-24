using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Trykli.Placement
{
    public enum PointerPhase
    {
        None,
        Down,
        Held,
        Up
    }

    /// <summary>
    /// Single pointer abstraction: first touch on mobile, mouse in the editor / desktop.
    /// Uses the legacy Input Manager (configured by Tools > TRYKLI > Setup Project).
    /// </summary>
    public static class PointerInput
    {
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();
        private static int _cachedFrame = -1;
        private static PointerPhase _phase;
        private static Vector2 _position;

        public static PointerPhase Phase
        {
            get
            {
                Refresh();
                return _phase;
            }
        }

        public static Vector2 ScreenPosition
        {
            get
            {
                Refresh();
                return _position;
            }
        }

        public static bool IsOverUI(Vector2 screenPosition)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return false;
            var data = new PointerEventData(eventSystem) { position = screenPosition };
            RaycastResults.Clear();
            eventSystem.RaycastAll(data, RaycastResults);
            return RaycastResults.Count > 0;
        }

        public static Vector2 ToWorld(Camera camera, Vector2 screenPosition)
        {
            if (camera == null) return screenPosition;
            Vector3 world = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -camera.transform.position.z));
            return world;
        }

        private static void Refresh()
        {
            if (_cachedFrame == Time.frameCount) return;
            _cachedFrame = Time.frameCount;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                _position = touch.position;
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _phase = PointerPhase.Down;
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        _phase = PointerPhase.Up;
                        break;
                    default:
                        _phase = PointerPhase.Held;
                        break;
                }

                return;
            }

            _position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0)) _phase = PointerPhase.Down;
            else if (Input.GetMouseButtonUp(0)) _phase = PointerPhase.Up;
            else if (Input.GetMouseButton(0)) _phase = PointerPhase.Held;
            else _phase = PointerPhase.None;
        }
    }
}
