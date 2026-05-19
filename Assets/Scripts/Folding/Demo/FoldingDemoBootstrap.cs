using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using PaperWings.Folding;
using PaperWings.Backend;

namespace PaperWings.Demo
{
    /// <summary>
    /// HIGH INTENSITY: One-click playable demo.
    /// Attach this to a GameObject in your scene. It builds the entire system at runtime.
    /// </summary>
    public class FoldingDemoBootstrap : MonoBehaviour
    {
        [Header("Data")]
        public PaperPlaneLibrary library;

        [Header("UI Documents (assign the uxmls)")]
        public UIDocument selectionUI;
        public UIDocument foldingUI;

        [Header("3D")]
        public Transform modelParent;
        public Camera mainCamera;

        [Header("Phase 5 Backend (Supabase + Monetization)")]
        public SupabaseConfig supabaseConfig;

        private void Start()
        {
            if (library == null)
            {
                Debug.LogError("Assign the PaperPlaneLibrary asset first!");
                return;
            }

            // Phase 6 basic performance target (tablet-first 60 FPS)
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            // Phase 6 simple splash/loading screen for initial app launch
            StartCoroutine(ShowInitialSplash());

            // Setup Orbit Controller on camera
            var orbit = mainCamera.gameObject.AddComponent<PaperModelOrbitController>();
            orbit.enabled = false; // Will be enabled when a plane loads

            // Setup Manager
            var manager = gameObject.AddComponent<FoldingTutorialManager>();
            manager.planeLibrary = library;
            manager.selectionDocument = selectionUI;
            manager.foldingDocument = foldingUI;
            manager.modelParent = modelParent;
            manager.foldingCamera = mainCamera;
            manager.orbitController = orbit;

            // ============================================================
            // Phase 5: Wire Supabase Auth + Progress Sync + Unlock Manager
            // ============================================================
            SetupBackendManagers();

            Debug.Log("Folding Demo ready. Select a plane from the grid.");
        }

        private void SetupBackendManagers()
        {
            // Try to auto-load a default config if none assigned (put SupabaseConfig.asset inside a Resources folder for auto-load)
            if (supabaseConfig == null)
            {
                supabaseConfig = Resources.Load<SupabaseConfig>("SupabaseConfig");
            }

            if (supabaseConfig == null || string.IsNullOrEmpty(supabaseConfig.supabaseUrl) || supabaseConfig.supabaseUrl.Contains("YOUR-PROJECT"))
            {
                Debug.LogWarning("[Phase 5] No valid SupabaseConfig assigned or filled. Backend will be inactive until configured.\n" +
                                 "Run 'Paper Wings / Phase 5 - Create Supabase Config Asset' and fill the keys.");
                return;
            }

            // Create persistent backend container (survives scene loads to FlightDemo)
            if (SupabaseAuth.Instance == null)
            {
                var backendGO = new GameObject("SupabaseBackend (Phase 5)");
                DontDestroyOnLoad(backendGO);

                var auth = backendGO.AddComponent<SupabaseAuth>();
                auth.config = supabaseConfig;

                var progress = backendGO.AddComponent<SupabaseProgressService>();
                progress.config = supabaseConfig;

                Debug.Log("[Phase 5] SupabaseAuth + SupabaseProgressService created and wired (persistent).");
            }
        }

        private IEnumerator ShowInitialSplash()
        {
            // Create splash overlay (simple, no assets needed)
            GameObject splashGO = new GameObject("InitialSplash");
            Canvas canvas = splashGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            // Dark background
            GameObject bgGO = new GameObject("SplashBG", typeof(Image));
            bgGO.transform.SetParent(splashGO.transform, false);
            Image bg = bgGO.GetComponent<Image>();
            bg.color = new Color(0.08f, 0.12f, 0.18f, 1f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Title
            GameObject titleGO = new GameObject("SplashTitle", typeof(Text));
            titleGO.transform.SetParent(splashGO.transform, false);
            Text title = titleGO.GetComponent<Text>();
            title.text = "Paper Wings World";
            title.fontSize = 52;
            title.color = Color.white;
            title.alignment = TextAnchor.MiddleCenter;
            title.fontStyle = FontStyle.Bold;
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.55f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.offsetMin = new Vector2(-320, -50);
            titleRect.offsetMax = new Vector2(320, 50);

            // Loading text
            GameObject loadGO = new GameObject("SplashLoading", typeof(Text));
            loadGO.transform.SetParent(splashGO.transform, false);
            Text load = loadGO.GetComponent<Text>();
            load.text = "Loading...";
            load.fontSize = 28;
            load.color = new Color(0.7f, 0.75f, 0.8f);
            load.alignment = TextAnchor.MiddleCenter;
            RectTransform loadRect = load.GetComponent<RectTransform>();
            loadRect.anchorMin = new Vector2(0.5f, 0.35f);
            loadRect.anchorMax = new Vector2(0.5f, 0.45f);
            loadRect.offsetMin = new Vector2(-150, -25);
            loadRect.offsetMax = new Vector2(150, 25);

            yield return new WaitForSeconds(1.5f);

            Destroy(splashGO);
        }
    }
}