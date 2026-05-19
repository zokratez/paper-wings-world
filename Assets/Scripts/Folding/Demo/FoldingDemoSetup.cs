using UnityEngine;
using PaperWings.Folding;

namespace PaperWings.Demo
{
    /// <summary>
    /// Quick helper to test the Folding Tutorial System in the demo scene.
    /// Attach to an empty GameObject in the FoldingTutorialDemo scene.
    /// </summary>
    public class FoldingDemoSetup : MonoBehaviour
    {
        [Header("References")]
        public PaperPlaneLibrary planeLibrary;
        public FoldingTutorialManager tutorialManager;

        [Header("3D Setup")]
        public Transform paperModelParent;
        public Camera foldingCamera;
        public PaperModelOrbitController orbitController;

        [Header("Debug")]
        public bool loadFirstPlaneOnStart = true;

        private void Start()
        {
            if (planeLibrary == null || tutorialManager == null)
            {
                Debug.LogError("Please assign the PaperPlaneLibrary and FoldingTutorialManager in the inspector.");
                return;
            }

            if (loadFirstPlaneOnStart && planeLibrary.allPlanes.Count > 0)
            {
                // For early testing - you can later trigger this from the UI
                Debug.Log("Demo: Loading first plane for testing purposes.");
            }
        }

        // You can call this from a button during early development
        public void LoadPlaneByIndex(int index)
        {
            if (planeLibrary != null && index >= 0 && index < planeLibrary.allPlanes.Count)
            {
                var plane = planeLibrary.allPlanes[index];
                tutorialManager.StartFoldingTutorial(plane);   // Will be exposed later if needed
            }
        }
    }
}