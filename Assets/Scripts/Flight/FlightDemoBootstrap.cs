using UnityEngine;
using PaperWings.Folding;
using PaperWings.Flight;

namespace PaperWings.Demo
{
    /// <summary>
    /// Sets up the FlightDemo scene.
    /// Reads the selected plane from SelectedPlaneHolder (set by the folding screen).
    /// Creates a beautiful Grand Canyon-style test environment.
    /// </summary>
    public class FlightDemoBootstrap : MonoBehaviour
    {
        [Header("References")]
        public Transform spawnPoint;
        public Camera flightCamera;

        [Header("Environment")]
        public bool buildCanyonEnvironment = true;

        private void Start()
        {
            // Centralized Phase 6 performance settings
            PerformanceManager.EnsurePerformanceSettings();

            // Build nice environment first — now region-aware for distinct visual personality
            FlightEnvironment env = null;
            if (buildCanyonEnvironment)
            {
                env = gameObject.AddComponent<FlightEnvironment>();
            }

            // Use RegionManager if present in scene (preferred clean path)
            var regionManager = FindObjectOfType<RegionManager>();
            if (regionManager != null && region != null)
            {
                regionManager.ApplyRegion(region, flightCamera, null, env); // physics applied below
            }
            else if (env != null)
            {
                env.BuildEnvironment(region);
            }

            PaperPlaneDefinition planeDef = FlightSessionData.SelectedPlane;
            FlightRegion region = FlightSessionData.SelectedRegion;

            if (planeDef == null)
            {
                Debug.LogWarning("[FlightDemo] No plane selected via FlightSessionData. Spawning default test plane.");
            }

            if (region == null)
            {
                Debug.LogWarning("[FlightDemo] No region selected. Using default environment.");
            }

            // Spawn the plane at a good height
            GameObject planeGO = new GameObject(planeDef != null ? planeDef.displayName : "Paper Plane");
            planeGO.transform.SetParent(spawnPoint);
            float spawnHeight = region != null ? region.defaultSpawnHeight : 18f;
            planeGO.transform.localPosition = new Vector3(0, spawnHeight, 0) + (region?.defaultSpawnOffset ?? Vector3.zero);
            planeGO.transform.localRotation = Quaternion.Euler(-6f, 0, 0);

            // Add physics + controller
            var physics = planeGO.AddComponent<PaperAirplanePhysics>();
            var controller = planeGO.AddComponent<PaperAirplaneFlightController>();

            if (planeDef != null)
            {
                physics.InitializeFromDefinition(planeDef);
            }

            // Add flight audio (dynamic wind/whoosh)
            var flightAudio = gameObject.AddComponent<FlightAudio>();
            flightAudio.planeTransform = planeGO.transform;
            flightAudio.planeRigidbody = physics.GetComponent<Rigidbody>();

            // Add flight visual effects (paper flutter + region particles)
            var flightEffects = gameObject.AddComponent<FlightEffects>();
            flightEffects.planeTransform = planeGO.transform;
            flightEffects.planeRigidbody = physics.GetComponent<Rigidbody>();

            if (region != null)
            {
                flightEffects.SetRegion(region);
            }

            // Apply region-specific flight tuning via manager if available, otherwise direct
            if (region != null && physics != null)
            {
                var rm = FindObjectOfType<RegionManager>();
                if (rm != null)
                {
                    rm.ApplyRegion(region, flightCamera, physics, null); // visuals + physics in one place
                }
                else
                {
                    physics.windDirection = region.baseWindDirection;
                    physics.baseWindStrength = region.baseWindStrength;
                    physics.thermalMultiplier = region.thermalStrengthMultiplier;
                }
            }

            // Nice launch
            controller.LaunchFromFoldingScreen();

            // Strong initial whoosh on launch
            var flightAudio = GetComponent<FlightAudio>();
            if (flightAudio != null)
            {
                flightAudio.PlayLaunchWhoosh();
            }

            // Launch visual burst (paper particles)
            var flightEffects = GetComponent<FlightEffects>();
            if (flightEffects != null)
            {
                flightEffects.PlayLaunchBurst();
            }

            // Subtle screen shake on strong launch (satisfying feedback)
            var camFollower = flightCamera.GetComponent<FlightCameraFollower>();
            if (camFollower != null)
            {
                camFollower.Shake(0.7f, 0.4f);
            }

            // Apply rich region visuals (sky, fog, ambient) — this is what makes each place feel different
            if (region != null)
            {
                ApplyRegionVisuals(region);
            }

            // Following camera (right-half free look supported)
            if (flightCamera != null)
            {
                var follower = flightCamera.gameObject.AddComponent<FlightCameraFollower>();
                follower.target = planeGO.transform;
            }

            // Add simple UI for return button
            CreateReturnToFoldingUI();

            // Ensure scene transition exists for smooth returns
            if (FindObjectOfType<PaperWings.Core.SceneTransition>() == null)
            {
                new GameObject("SceneTransition").AddComponent<PaperWings.Core.SceneTransition>();
            }

            // Add flight performance stats
            gameObject.AddComponent<PaperWings.Flight.FlightStatsDisplay>();

            Debug.Log($"[FlightDemo] Spawned {(planeDef != null ? planeDef.displayName : "test plane")} into {(region != null ? region.displayName : "default area")}.");
        }

        private void CreateReturnToFoldingUI()
        {
            GameObject uiGO = new GameObject("FlightUI");
            var uiDoc = uiGO.AddComponent<UnityEngine.UIElements.UIDocument>();

            // In a real project you would assign a proper UXML here.
            // For the demo we create a simple on-screen button via code.
            var canvas = new GameObject("ReturnCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var returnBtn = new GameObject("ReturnButton", typeof(UnityEngine.UI.Button), typeof(UnityEngine.UI.Image));
            returnBtn.transform.SetParent(canvas.transform);

            var rect = returnBtn.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.92f);
            rect.anchorMax = new Vector2(0.5f, 0.92f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(280, 52);

            var image = returnBtn.GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.15f, 0.25f, 0.35f, 0.85f);

            var btn = returnBtn.GetComponent<UnityEngine.UI.Button>();
            btn.onClick.AddListener(ShowPostFlightSummary);

            var textGO = new GameObject("Text", typeof(UnityEngine.UI.Text));
            textGO.transform.SetParent(returnBtn.transform);
            var txt = textGO.GetComponent<UnityEngine.UI.Text>();
            txt.text = "Return to Folding";
            txt.fontSize = 22;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            var txtRect = textGO.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
        }

        private void ShowPostFlightSummary()
        {
            // Record the flight (idempotent if already recorded)
            RecordFlightProgress();

            // Get final stats
            var stats = FindObjectOfType<FlightStatsDisplay>();
            var planeDef = FlightSessionData.SelectedPlane;
            var region = FlightSessionData.SelectedRegion;

            float dist = stats != null ? stats.CurrentDistance : 0f;
            float time = stats != null ? stats.CurrentFlightTime : 0f;
            float maxAlt = stats != null ? stats.MaxAltitude : 0f;

            // Check if this was a new personal best
            bool newBest = false;
            string celebration = "";
            if (planeDef != null && region != null)
            {
                var (prevDist, _) = PaperWings.Progression.FlightProgress.GetBest(planeDef.planeId, region.regionId);
                if (dist > prevDist + 1f) // small epsilon
                {
                    newBest = true;
                    var tier = PaperWings.Progression.FlightProgress.GetMasteryTier(planeDef.planeId, region.regionId);
                    string badge = PaperWings.Progression.FlightProgress.GetBadgeEmoji(tier);
                    celebration = newBest ? $"🎉 New Personal Best! {badge}" : "";

                    // Placeholder celebratory sound (kid-friendly chime)
                    AudioSource.PlayClipAtPoint(GenerateSimpleTone(1400, 0.35f), Vector3.zero, 0.9f);
                }
            }

            // Hide the return button UI
            GameObject flightUI = GameObject.Find("FlightUI");
            if (flightUI != null) flightUI.SetActive(false);

            // Create polished summary canvas (centered card)
            GameObject summaryGO = new GameObject("PostFlightSummary");
            var canvas = summaryGO.AddComponent<UnityEngine.UI.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = summaryGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

            // Background panel
            GameObject panelGO = new GameObject("Panel", typeof(UnityEngine.UI.Image));
            panelGO.transform.SetParent(summaryGO.transform);
            var panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(520, 380);

            var panelImg = panelGO.GetComponent<UnityEngine.UI.Image>();
            // Unified kid-friendly palette
            panelImg.color = new Color(0.97f, 0.96f, 0.94f, 0.97f); // CreamBg equivalent

            // Add rounded look via material or just color for demo

            // Title
            var titleGO = CreateText(panelGO.transform, "Flight Complete", new Vector2(0, 140), 32, true, new Color(0.12f, 0.28f, 0.48f)); // TitleColor

            // Stats
            string statsText = $"Distance: {dist:F0} m\nFlight Time: {time:F1} s\nMax Altitude: {maxAlt:F0} m";
            CreateText(panelGO.transform, statsText, new Vector2(0, 40), 20, false, new Color(0.35f, 0.42f, 0.52f)); // TextMuted

            // Celebration
            if (!string.IsNullOrEmpty(celebration))
            {
                CreateText(panelGO.transform, celebration, new Vector2(0, -30), 24, true, new Color(0.1f, 0.6f, 0.3f));
            }

            // Buttons container
            GameObject btnContainer = new GameObject("Buttons", typeof(UnityEngine.UI.HorizontalLayoutGroup));
            btnContainer.transform.SetParent(panelGO.transform);
            var btnRect = btnContainer.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = new Vector2(0, -110);
            btnRect.sizeDelta = new Vector2(480, 60);

            var hlg = btnContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            // Fly Again button - PrimaryBlue
            var flyAgainBtn = CreateButton(btnContainer.transform, "Fly Again in Same Region", new Color(0.23f, 0.51f, 0.82f));
            flyAgainBtn.onClick.AddListener(() =>
            {
                // Simple and reliable for demo: reload the scene (static session data persists)
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            });

            // Return to Folding button - WarmAccent
            var returnBtn = CreateButton(btnContainer.transform, "Return to Folding", new Color(0.96f, 0.62f, 0.15f));
            returnBtn.onClick.AddListener(() =>
            {
                // Final return
                SelectedPlaneHolder.Clear();
                FlightSessionData.Clear();

                var transition = FindObjectOfType<PaperWings.Core.SceneTransition>();
                if (transition != null)
                {
                    transition.LoadSceneWithFade("FoldingTutorialDemo");
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FoldingTutorialDemo");
                }
            });
        }

        private UnityEngine.UI.Text CreateText(Transform parent, string text, Vector2 anchoredPos, int fontSize, bool bold, Color color)
        {
            GameObject go = new GameObject("Text", typeof(UnityEngine.UI.Text));
            go.transform.SetParent(parent);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(480, 80);

            var txt = go.GetComponent<UnityEngine.UI.Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = TextAnchor.MiddleCenter;
            if (bold) txt.fontStyle = UnityEngine.FontStyle.Bold;

            return txt;
        }

        private UnityEngine.UI.Button CreateButton(Transform parent, string label, Color bgColor)
        {
            GameObject go = new GameObject(label, typeof(UnityEngine.UI.Button), typeof(UnityEngine.UI.Image));
            go.transform.SetParent(parent);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(220, 52);

            var img = go.GetComponent<UnityEngine.UI.Image>();
            img.color = bgColor;

            var btn = go.GetComponent<UnityEngine.UI.Button>();

            var textGO = new GameObject("Text", typeof(UnityEngine.UI.Text));
            textGO.transform.SetParent(go.transform);
            var tRect = textGO.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;

            var txt = textGO.GetComponent<UnityEngine.UI.Text>();
            txt.text = label;
            txt.fontSize = 18;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            return btn;
        }

        // Simple placeholder tone generator for celebratory sounds (no asset required)
        private AudioClip GenerateSimpleTone(float frequency, float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.FloorToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Exp(-4f * t);
            }
            AudioClip clip = AudioClip.Create("CelebrationTone", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private void RecordFlightProgress()
        {
            var stats = FindObjectOfType<FlightStatsDisplay>();
            var planeDef = FlightSessionData.SelectedPlane;
            var region = FlightSessionData.SelectedRegion;

            if (stats != null && planeDef != null && region != null)
            {
                float dist = stats.CurrentDistance;
                float time = stats.CurrentFlightTime;

                if (dist > 5f || time > 2f) // ignore trivial "flights"
                {
                    PaperWings.Progression.FlightProgress.RecordFlight(planeDef.planeId, region.regionId, dist, time);
                }
            }
        }
    }
}