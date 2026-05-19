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
            // Build nice environment first
            if (buildCanyonEnvironment)
            {
                var env = gameObject.AddComponent<FlightEnvironment>();
                env.BuildEnvironment();
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

            // Apply region-specific flight environment
            if (region != null && physics != null)
            {
                physics.windDirection = region.baseWindDirection;
                physics.baseWindStrength = region.baseWindStrength;
            }

            // Nice launch
            controller.LaunchFromFoldingScreen();

            // Following camera
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
        }

        private void ApplyRegionVisuals(FlightRegion region)
        {
            // Skybox
            if (region.skyboxMaterial != null && flightCamera != null)
            {
                RenderSettings.skybox = region.skyboxMaterial;
            }

            // Ambient light
            RenderSettings.ambientLight = region.ambientLightColor;
            RenderSettings.ambientIntensity = region.ambientIntensity;

            // Fog
            RenderSettings.fog = true;
            RenderSettings.fogColor = region.fogColor;
            RenderSettings.fogDensity = region.fogDensity;
            RenderSettings.fogMode = FogMode.Exponential;

            // Camera background as fallback
            if (flightCamera != null)
            {
                flightCamera.backgroundColor = region.fogColor;
            }
        }

            Debug.Log($"[FlightDemo] Spawned {(planeDef != null ? planeDef.displayName : "test plane")} successfully.");
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
            SelectedPlaneHolder.Clear();

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
    }

    /// <summary>
    /// High-quality camera follower for the flight demo.
    /// Features smooth following + optional free-look mode (two-finger drag).
    /// </summary>
    public class FlightCameraFollower : MonoBehaviour
    {
        public Transform target;

        [Header("Follow Settings")]
        public float followDistance = 4.8f;
        public float heightOffset = 1.6f;
        public float smoothSpeed = 4.0f;

        [Header("Free Look")]
        [Tooltip("Allow two-finger drag to look around while still following the plane")]
        public bool allowFreeLook = true;
        public float freeLookSensitivity = 0.4f;

        private Vector3 velocity;
        private float currentYaw;
        private float currentPitch;
        private bool isFreeLooking = false;
        private Vector2 lastLookPosition;

        private void LateUpdate()
        {
            if (target == null) return;

            HandleFreeLookInput();

            // Calculate base follow position
            Vector3 followDirection = Quaternion.Euler(currentPitch, currentYaw, 0) * Vector3.back;
            Vector3 desiredPosition = target.position + followDirection * followDistance + Vector3.up * heightOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);

            // Look at the plane with a slight lead
            Vector3 lookTarget = target.position + target.forward * 2.5f + Vector3.up * 0.6f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), Time.deltaTime * 6f);
        }

        private void HandleFreeLookInput()
        {
            if (!allowFreeLook) return;

            if (Input.touchCount == 2)
            {
                isFreeLooking = true;
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                Vector2 avgPos = (t0.position + t1.position) / 2f;

                if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                {
                    lastLookPosition = avgPos;
                }
                else if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
                {
                    Vector2 delta = avgPos - lastLookPosition;

                    currentYaw += delta.x * freeLookSensitivity;
                    currentPitch -= delta.y * freeLookSensitivity * 0.7f;
                    currentPitch = Mathf.Clamp(currentPitch, -35f, 45f);

                    lastLookPosition = avgPos;
                }
            }
            else
            {
                isFreeLooking = false;

                // Slowly return to following behind the plane
                currentYaw = Mathf.LerpAngle(currentYaw, 0f, Time.deltaTime * 2.5f);
                currentPitch = Mathf.Lerp(currentPitch, 8f, Time.deltaTime * 2.5f);
            }
        }
    }
}