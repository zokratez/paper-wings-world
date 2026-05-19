using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using PaperWings.Folding;
using PaperWings.Demo;

namespace PaperWings.Folding
{
    /// <summary>
    /// HIGH INTENSITY VERSION - Fully playable Folding Tutorial System.
    /// Works with the 8 locked planes. Classic Dart has real demo steps + animation.
    /// </summary>
    public class FoldingTutorialManager : MonoBehaviour
    {
        [Header("Data")]
        public PaperPlaneLibrary planeLibrary;

        [Header("UI (UI Toolkit)")]
        public UIDocument selectionDocument;
        public UIDocument foldingDocument;

        [Header("3D View")]
        public Transform modelParent;
        public Camera foldingCamera;
        public PaperModelOrbitController orbitController;

        // Runtime
        private PaperPlaneDefinition currentPlane;
        private int currentStep = 0;
        private ProceduralPaperPlane currentPaper;
        private VisualElement selectionRoot;
        private VisualElement foldingRoot;

        // UI References
        private Label planeNameLabel;
        private Label difficultyLabel;
        private Label descriptionLabel;
        private Label stepLabel;
        private Label instructionLabel;
        private ProgressBar progressBar;
        private Button nextBtn;
        private Button prevBtn;
        private Button printBtn;
        private Button launchToFlightBtn;

        // Success screen
        private VisualElement successPanel;
        private Label successTitle;

        private FoldingAudio audioPlayer;

        private void Start()
        {
            if (planeLibrary == null || planeLibrary.allPlanes.Count == 0)
            {
                Debug.LogError("PaperPlaneLibrary not assigned or empty. Run the generator first!");
                return;
            }

            ShowSelectionScreen();
        }

        #region Selection Screen

        private void ShowSelectionScreen()
        {
            if (foldingDocument) foldingDocument.rootVisualElement.style.display = DisplayStyle.None;
            if (selectionDocument) selectionDocument.rootVisualElement.style.display = DisplayStyle.Flex;

            selectionRoot = selectionDocument.rootVisualElement;
            var grid = selectionRoot.Q<VisualElement>("plane-grid");
            grid.Clear();

            foreach (var plane in planeLibrary.allPlanes)
            {
                var card = CreatePlaneCard(plane);
                grid.Add(card);
            }
        }

        private VisualElement CreatePlaneCard(PaperPlaneDefinition plane)
        {
            var card = new VisualElement();
            card.AddToClassList("plane-card");

            var nameLabel = new Label(plane.displayName);
            nameLabel.style.fontSize = 18;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

            var meta = new Label($"{plane.difficulty} • {plane.primaryCategory}");
            meta.style.fontSize = 12;

            card.Add(nameLabel);
            card.Add(meta);

            card.RegisterCallback<ClickEvent>(e => StartTutorial(plane));
            return card;
        }

        #endregion

        public void StartTutorial(PaperPlaneDefinition plane)
        {
            currentPlane = plane;
            currentStep = 0;

            selectionDocument.rootVisualElement.style.display = DisplayStyle.None;
            foldingDocument.rootVisualElement.style.display = DisplayStyle.Flex;

            foldingRoot = foldingDocument.rootVisualElement;

            // Plane info
            planeNameLabel = foldingRoot.Q<Label>("plane-name");
            difficultyLabel = foldingRoot.Q<Label>("plane-difficulty");
            descriptionLabel = foldingRoot.Q<Label>("plane-description");

            // Step UI
            stepLabel = foldingRoot.Q<Label>("step-number");
            instructionLabel = foldingRoot.Q<Label>("instruction-text");
            progressBar = foldingRoot.Q<ProgressBar>("progress-bar");
            nextBtn = foldingRoot.Q<Button>("next-button");
            prevBtn = foldingRoot.Q<Button>("prev-button");
            printBtn = foldingRoot.Q<Button>("print-button");

            // Wire buttons
            nextBtn.clicked += NextStep;
            prevBtn.clicked += PreviousStep;
            printBtn.clicked += () => Debug.Log("Printable template requested for " + plane.displayName);

            var back = foldingRoot.Q<Button>("back-button");
            if (back != null) back.clicked += ShowSelectionScreen;

            // Launch button (for Phase 2 transition)
            launchToFlightBtn = foldingRoot.Q<Button>("launch-flight-button");
            if (launchToFlightBtn != null)
            {
                launchToFlightBtn.clicked += LaunchToFlight;
                launchToFlightBtn.style.display = DisplayStyle.None; // Hidden until complete
            }

            // Success panel
            successPanel = foldingRoot.Q<VisualElement>("success-panel");
            successTitle = foldingRoot.Q<Label>("success-title");
            if (successPanel != null) successPanel.style.display = DisplayStyle.None;

            // Audio (optional - safe if missing)
            audioPlayer = GetComponent<FoldingAudio>();

            LoadPaperModel();
            PopulatePlaneInfo();
            UpdateUI();
        }

        private void PopulatePlaneInfo()
        {
            if (planeNameLabel != null) planeNameLabel.text = currentPlane.displayName;
            if (difficultyLabel != null) difficultyLabel.text = currentPlane.difficulty.ToString();
            if (descriptionLabel != null) descriptionLabel.text = currentPlane.shortDescription;
        }

        private void LoadPaperModel()
        {
            // Clean up previous model
            foreach (Transform child in modelParent)
            {
                if (child.name.Contains("Model")) Destroy(child.gameObject);
            }
            currentPaper = null;
            currentRealAnimator = null;
            currentProcedural = null;

            GameObject modelInstance;

            if (currentPlane.paperModelPrefab != null)
            {
                modelInstance = Instantiate(currentPlane.paperModelPrefab, modelParent);
                modelInstance.name = currentPlane.displayName + " Model";

                currentRealAnimator = modelInstance.GetComponent<PaperPlaneAnimator>();
                if (currentRealAnimator == null)
                    currentRealAnimator = modelInstance.AddComponent<PaperPlaneAnimator>();

                currentRealAnimator.Initialize();
            }
            else
            {
                modelInstance = new GameObject(currentPlane.displayName + " Model");
                modelInstance.transform.SetParent(modelParent);
                modelInstance.transform.localPosition = Vector3.zero;

                currentProcedural = modelInstance.AddComponent<ProceduralPaperPlane>();
                currentProcedural.Initialize(currentPlane.planeId);
            }

            if (orbitController != null)
            {
                orbitController.target = modelInstance.transform;
                orbitController.ResetView(GetPreferredCameraDistance(currentPlane.planeId));
            }
        }

        private float GetPreferredCameraDistance(string planeId)
        {
            // Give each plane type a sensible starting zoom
            return planeId switch
            {
                "the_ring" => 2.2f,
                "nakamichi_glider" or "stealth_glider" or "the_bird" => 3.4f,
                _ => 2.8f
            };
        }

        private void UpdateUI()
        {
            if (currentPlane == null || currentPlane.steps.Count == 0) return;

            var step = currentPlane.steps[currentStep];

            stepLabel.text = $"Step {step.stepNumber} / {currentPlane.steps.Count}";
            instructionLabel.text = step.instructionText;
            progressBar.value = ((float)(currentStep + 1) / currentPlane.steps.Count) * 100f;

            // Animate the procedural model
            if (currentPaper != null)
            {
                StartCoroutine(AnimateStep(step.animationClipName, step.animationDuration));
            }
        }

        private PaperPlaneAnimator currentRealAnimator;
        private ProceduralPaperPlane currentProcedural;

        private IEnumerator AnimateStep(string foldName, float duration)
        {
            if (audioPlayer != null) audioPlayer.PlayFoldSound();

            // Find the right animator (real or procedural)
            if (currentRealAnimator == null && currentPaper != null)
            {
                // Try to get from the spawned model
                currentRealAnimator = modelParent.GetComponentInChildren<PaperPlaneAnimator>();
                currentProcedural = modelParent.GetComponentInChildren<ProceduralPaperPlane>();
            }

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                float eased = Mathf.Clamp01(t);

                if (currentRealAnimator != null)
                    currentRealAnimator.AnimateFold(foldName, eased);
                else if (currentProcedural != null)
                    currentProcedural.AnimateFold(foldName, eased);

                yield return null;
            }

            // Final pose
            if (currentRealAnimator != null)
                currentRealAnimator.AnimateFold(foldName, 1f);
            else if (currentProcedural != null)
                currentProcedural.AnimateFold(foldName, 1f);

            if (audioPlayer != null) audioPlayer.PlaySuccessSound();
        }

        private void NextStep()
        {
            if (currentStep < currentPlane.steps.Count - 1)
            {
                currentStep++;
                StartCoroutine(SmoothStepTransition());
            }
            else
            {
                // Plane fully folded — show success reward
                ShowSuccessScreen();
            }
        }

        private FlightRegion selectedRegionForLaunch;

        private void ShowSuccessScreen()
        {
            if (successPanel != null)
            {
                successPanel.style.display = DisplayStyle.Flex;
                if (successTitle != null)
                    successTitle.text = $"Great job!\n{currentPlane.displayName} complete!";
            }

            // Hide launch button until region is chosen
            if (launchToFlightBtn != null)
                launchToFlightBtn.style.display = DisplayStyle.None;

            // Show region selection (simple for foundation)
            ShowRegionSelection();

            if (audioPlayer != null)
                audioPlayer.PlaySuccessSound();

            Debug.Log($"[Folding] {currentPlane.displayName} folding tutorial completed successfully.");
        }

        private void ShowRegionSelection()
        {
            // For Phase 3 foundation, we create 3 simple buttons dynamically if they don't exist.
            // In a polished build this would be a proper carousel or list in UI Builder.

            var regionContainer = foldingRoot.Q<VisualElement>("region-selection");
            if (regionContainer == null)
            {
                regionContainer = new VisualElement { name = "region-selection" };
                regionContainer.style.flexDirection = FlexDirection.Row;
                regionContainer.style.justifyContent = Justify.Center;
                regionContainer.style.marginTop = 12;
                foldingRoot.Add(regionContainer);
            }

            regionContainer.Clear();

            // Hardcoded 3 regions for foundation (will come from library later)
            CreateRegionButton(regionContainer, "Grand Canyon", "grand_canyon");
            CreateRegionButton(regionContainer, "Fuji Foothills", "fuji_foothills");
            CreateRegionButton(regionContainer, "Norwegian Coast", "norwegian_coast");
        }

        private void CreateRegionButton(VisualElement container, string displayName, string regionId)
        {
            var btn = new Button { text = displayName };
            btn.style.marginRight = 8;
            btn.style.paddingLeft = 12;
            btn.style.paddingRight = 12;
            btn.style.paddingTop = 8;
            btn.style.paddingBottom = 8;

            btn.clicked += () =>
            {
                // Load the region asset (in real build this would come from FlightRegionLibrary)
                selectedRegionForLaunch = LoadRegionById(regionId);

                // Enable launch button
                if (launchToFlightBtn != null)
                    launchToFlightBtn.style.display = DisplayStyle.Flex;

                Debug.Log($"Region selected: {displayName}");
            };

            container.Add(btn);
        }

        private FlightRegion LoadRegionById(string id)
        {
            // In production we load from a FlightRegionLibrary asset.
            // For the current demo foundation, we try to find it via Resources (user can move the 3 region assets into a Resources/FlightRegions folder).
            return Resources.Load<FlightRegion>($"FlightRegions/{id}");
        }

        private void LaunchToFlight()
        {
            if (currentPlane == null) return;

            var regionToUse = selectedRegionForLaunch ?? Resources.Load<FlightRegion>("FlightRegions/grand_canyon");

            PaperWings.Flight.FlightSessionData.SetSession(currentPlane, regionToUse);

            Debug.Log($"[Folding] Launching {currentPlane.displayName} into {regionToUse?.displayName ?? "default"}...");

            var transition = FindObjectOfType<PaperWings.Core.SceneTransition>();
            if (transition != null)
            {
                transition.LoadSceneWithFade("FlightDemo");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("FlightDemo");
            }
        }

        private void PreviousStep()
        {
            if (currentStep > 0)
            {
                currentStep--;
                StartCoroutine(SmoothStepTransition());
            }
        }

        private IEnumerator SmoothStepTransition()
        {
            // Small breathing room between steps for better feel
            yield return new WaitForSeconds(0.12f);
            UpdateUI();
        }
    }
}