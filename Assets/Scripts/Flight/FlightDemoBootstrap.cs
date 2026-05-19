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
            btn.onClick.AddListener(ReturnToFolding);

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

        private void ReturnToFolding()
        {
            // Record progress before leaving the flight scene
            RecordFlightProgress();

            // Clear session (legacy + new)
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