using UnityEngine;
using UnityEngine.UIElements;
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
    }
}