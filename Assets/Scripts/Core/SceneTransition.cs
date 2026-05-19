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

        public static bool IsLowEndDevice { get; private set; }

        [Header("Fade Settings")]
        public float fadeDuration = 0.35f;
        public Color fadeColor = Color.black;

        private Canvas fadeCanvas;
        private UnityEngine.UI.Image fadeImage;
        private UnityEngine.UI.Text loadingText; // Phase 6 simple loading indicator

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Phase 6 mobile optimization - device tier aware
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            IsLowEndDevice = SystemInfo.systemMemorySize < 3000 || SystemInfo.processorCount <= 4 || SystemInfo.graphicsMemorySize < 512;

            if (IsLowEndDevice)
            {
                // Lower shadow quality and overall for low-end devices to maintain 60 FPS
                QualitySettings.SetQualityLevel(1, true);
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowDistance = 20f;
                QualitySettings.shadowCascades = 1;
                // Note: Post-processing (if any URP Volume features like Bloom/AA) would be reduced by lower quality level
            }
            else if (QualitySettings.GetQualityLevel() > 3)
            {
                // Balanced for tablets
                QualitySettings.SetQualityLevel(3, true);
            }

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

            // Phase 6 simple loading indicator (visible during black fade / load)
            var loadingGO = new GameObject("LoadingText", typeof(UnityEngine.UI.Text));
            loadingGO.transform.SetParent(fadeCanvas.transform, false);
            loadingText = loadingGO.GetComponent<UnityEngine.UI.Text>();
            loadingText.text = "Loading...";
            loadingText.fontSize = 36;
            loadingText.color = Color.white;
            loadingText.alignment = TextAnchor.MiddleCenter;
            loadingText.enabled = false; // hidden by default

            var loadingRect = loadingText.GetComponent<RectTransform>();
            loadingRect.anchorMin = new Vector2(0.5f, 0.45f);
            loadingRect.anchorMax = new Vector2(0.5f, 0.55f);
            loadingRect.offsetMin = new Vector2(-200, -30);
            loadingRect.offsetMax = new Vector2(200, 30);
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

            // Show simple loading indicator while the new scene loads and initializes
            if (loadingText != null)
            {
                loadingText.enabled = true;
            }

            // Load (synchronous — the loading text gives visual feedback during hitch)
            SceneManager.LoadScene(sceneName);

            // Small delay so new scene can initialize (loading text remains visible)
            yield return new WaitForSeconds(0.25f);

            // Hide loading text before fade in
            if (loadingText != null)
            {
                loadingText.enabled = false;
            }

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