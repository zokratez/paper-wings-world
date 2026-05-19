using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PaperWings.Core
{
    /// <summary>
    /// Simple, high-quality scene transition with fade.
    /// Used for smooth folding <-> flight transitions.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [Header("Fade Settings")]
        public float fadeDuration = 0.35f;
        public Color fadeColor = Color.black;

        private Canvas fadeCanvas;
        private UnityEngine.UI.Image fadeImage;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateFadeOverlay();
        }

        private void CreateFadeOverlay()
        {
            fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 999;

            var scaler = fadeCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

            fadeImage = new GameObject("FadeImage", typeof(UnityEngine.UI.Image)).GetComponent<UnityEngine.UI.Image>();
            fadeImage.transform.SetParent(fadeCanvas.transform, false);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

            var rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public void LoadSceneWithFade(string sceneName)
        {
            StartCoroutine(FadeAndLoad(sceneName));
        }

        private IEnumerator FadeAndLoad(string sceneName)
        {
            // Fade out
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }

            // Load
            SceneManager.LoadScene(sceneName);

            // Small delay so new scene can initialize
            yield return new WaitForSeconds(0.1f);

            // Fade in
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }

            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }
}