using UnityEngine;
using UnityEngine.UIElements;
using PaperWings.Folding;

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

            Debug.Log("Folding Demo ready. Select a plane from the grid.");
        }
    }
}