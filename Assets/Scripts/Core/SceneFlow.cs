using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Trykli.Core
{
    public static class SceneNames
    {
        public const string Boot = "Boot";
        public const string MainMenu = "MainMenu";
        public const string WorldSelect = "WorldSelect";
        public const string LevelSelect = "LevelSelect";
        public const string Gameplay = "Gameplay";

        public static readonly string[] All = { Boot, MainMenu, WorldSelect, LevelSelect, Gameplay };
    }

    /// <summary>Loads scenes behind a short fade (persistent overlay canvas).</summary>
    public sealed class SceneFlow : MonoBehaviour
    {
        private const float FadeDuration = 0.15f;

        private CanvasGroup _fader;
        private bool _loading;

        public bool IsLoading => _loading;

        public void Initialize()
        {
            var canvasGo = new GameObject("SceneFader", typeof(Canvas), typeof(CanvasGroup));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            _fader = canvasGo.GetComponent<CanvasGroup>();
            _fader.alpha = 0f;
            _fader.blocksRaycasts = false;

            var imageGo = new GameObject("Fade", typeof(RectTransform), typeof(Image));
            imageGo.transform.SetParent(canvasGo.transform, false);
            var rect = (RectTransform)imageGo.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            imageGo.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.14f, 1f);
        }

        public void Load(string sceneName)
        {
            if (_loading) return;
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[TRYKLI] Scene '{sceneName}' is not in the build settings. Run Tools > TRYKLI > Setup Project.");
                return;
            }

            StartCoroutine(LoadRoutine(sceneName));
        }

        private IEnumerator LoadRoutine(string sceneName)
        {
            _loading = true;
            _fader.blocksRaycasts = true;
            yield return Fade(0f, 1f);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (operation != null && !operation.isDone) yield return null;
            yield return null;
            yield return Fade(1f, 0f);
            _fader.blocksRaycasts = false;
            _loading = false;
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < FadeDuration)
            {
                t += Time.unscaledDeltaTime;
                _fader.alpha = Mathf.Lerp(from, to, t / FadeDuration);
                yield return null;
            }

            _fader.alpha = to;
        }
    }
}
