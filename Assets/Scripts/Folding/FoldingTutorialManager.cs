using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using PaperWings.Core;

namespace PaperWings.Folding
{
    /// <summary>
    /// Main controller for the entire Folding Tutorial experience.
    /// Handles plane selection, step progression, 3D model control, and UI flow.
    /// Designed to be completely data-driven.
    /// </summary>
    public class FoldingTutorialManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PaperPlaneLibrary planeLibrary;

        [Header("UI Documents (UI Toolkit)")]
        [SerializeField] private UIDocument selectionScreenDocument;
        [SerializeField] private UIDocument foldingScreenDocument;

        [Header("3D View")]
        [SerializeField] private Transform paperModelSpawnPoint;
        [SerializeField] private Camera foldingCamera;

        // Runtime state
        private PaperPlaneDefinition currentPlane;
        private int currentStepIndex = 0;
        private GameObject currentPaperInstance;
        private Animator currentAnimator;

        // UI Elements (cached)
        private VisualElement selectionRoot;
        private VisualElement foldingRoot;
        private Label stepNumberLabel;
        private Label instructionLabel;
        private Button nextButton;
        private Button prevButton;
        private Button printButton;
        private ProgressBar progressBar;

        private void Start()
        {
            InitializeSelectionScreen();
            ShowSelectionScreen();
        }

        #region Selection Screen

        private void InitializeSelectionScreen()
        {
            if (selectionScreenDocument == null) return;

            selectionRoot = selectionScreenDocument.rootVisualElement;
            var grid = selectionRoot.Q<VisualElement>("plane-grid");

            if (grid == null)
            {
                Debug.LogWarning("Plane grid not found in selection UI. Create a VisualElement with name 'plane-grid' in UI Builder.");
                return;
            }

            grid.Clear();

            var planesToShow = planeLibrary.GetFreePlanes(); // For MVP we can expand this later

            foreach (var plane in planesToShow)
            {
                var card = CreatePlaneCard(plane);
                grid.Add(card);
            }
        }

        private VisualElement CreatePlaneCard(PaperPlaneDefinition plane)
        {
            var card = new VisualElement();
            card.AddToClassList("plane-card");
            card.style.width = 180;
            card.style.height = 220;
            card.style.marginBottom = 12;

            // Thumbnail
            var thumb = new VisualElement();
            thumb.style.backgroundImage = new StyleBackground(plane.thumbnail);
            thumb.style.height = 120;
            thumb.style.width = 160;
            thumb.style.marginBottom = 6;
            card.Add(thumb);

            // Name
            var nameLabel = new Label(plane.displayName);
            nameLabel.style.fontSize = 16;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            card.Add(nameLabel);

            // Difficulty + Category
            var meta = new Label($"{plane.difficulty} • {plane.primaryCategory}");
            meta.style.fontSize = 11;
            meta.style.opacity = 0.8f;
            card.Add(meta);

            // Click handler
            card.RegisterCallback<ClickEvent>(evt =>
            {
                StartFoldingTutorial(plane);
            });

            return card;
        }

        #endregion

        #region Folding Screen

        public void StartFoldingTutorial(PaperPlaneDefinition plane)
        {
            currentPlane = plane;
            currentStepIndex = 0;

            ShowFoldingScreen();
            LoadPaperModel();
            UpdateStepUI();
        }

        private void ShowFoldingScreen()
        {
            selectionScreenDocument.rootVisualElement.style.display = DisplayStyle.None;
            foldingScreenDocument.rootVisualElement.style.display = DisplayStyle.Flex;

            foldingRoot = foldingScreenDocument.rootVisualElement;

            // Cache UI elements (these names must match the UI Builder document)
            stepNumberLabel = foldingRoot.Q<Label>("step-number");
            instructionLabel = foldingRoot.Q<Label>("instruction-text");
            nextButton = foldingRoot.Q<Button>("next-button");
            prevButton = foldingRoot.Q<Button>("prev-button");
            printButton = foldingRoot.Q<Button>("print-button");
            progressBar = foldingRoot.Q<ProgressBar>("progress-bar");

            // Wire buttons
            nextButton.clicked += GoToNextStep;
            prevButton.clicked += GoToPreviousStep;
            printButton.clicked += GeneratePrintableTemplate;

            // Back button
            var backButton = foldingRoot.Q<Button>("back-button");
            if (backButton != null)
                backButton.clicked += ShowSelectionScreen;
        }

        private void LoadPaperModel()
        {
            // Destroy previous model
            if (currentPaperInstance != null)
                Destroy(currentPaperInstance);

            if (currentPlane.paperModelPrefab == null)
            {
                Debug.LogWarning($"No 3D model assigned to {currentPlane.displayName}. Using placeholder cube.");
                currentPaperInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else
            {
                currentPaperInstance = Instantiate(currentPlane.paperModelPrefab, paperModelSpawnPoint);
            }

            currentPaperInstance.transform.localPosition = Vector3.zero;
            currentPaperInstance.transform.localRotation = Quaternion.identity;

            // Setup animator if available
            currentAnimator = currentPaperInstance.GetComponent<Animator>();
            if (currentAnimator != null && currentPlane.animatorController != null)
            {
                currentAnimator.runtimeAnimatorController = currentPlane.animatorController;
            }

            // TODO: Add rotation controller script to the model (orbit with touch)
        }

        private void UpdateStepUI()
        {
            if (currentPlane == null || currentStepIndex >= currentPlane.steps.Count) return;

            var step = currentPlane.steps[currentStepIndex];

            stepNumberLabel.text = $"Step {step.stepNumber} / {currentPlane.TotalSteps}";
            instructionLabel.text = step.instructionText;

            // Progress
            float progress = (float)(currentStepIndex + 1) / currentPlane.TotalSteps;
            progressBar.value = progress * 100f;

            // Play the fold animation
            if (currentAnimator != null && !string.IsNullOrEmpty(step.animationClipName))
            {
                currentAnimator.Play(step.animationClipName, 0, 0f);
            }

            // TODO: Highlight fold lines using step.foldLineHighlightTag
            // This will be implemented once we have a proper paper shader + edge highlighting system
        }

        private void GoToNextStep()
        {
            if (currentStepIndex < currentPlane.steps.Count - 1)
            {
                currentStepIndex++;
                UpdateStepUI();
            }
            else
            {
                // Tutorial complete
                Debug.Log($"Congratulations! You finished folding the {currentPlane.displayName}");
                // TODO: Mark as completed in save system, unlock flight mode, etc.
            }
        }

        private void GoToPreviousStep()
        {
            if (currentStepIndex > 0)
            {
                currentStepIndex--;
                UpdateStepUI();
            }
        }

        private void GeneratePrintableTemplate()
        {
            // Placeholder - in real implementation we will use a PDF generation library
            // or load a pre-made high-res image/PDF from Addressables
            Debug.Log($"Generating printable template for {currentPlane.displayName}...");
            // Example: Application.OpenURL or show a modal with the image
        }

        private void ShowSelectionScreen()
        {
            if (foldingScreenDocument != null)
                foldingScreenDocument.rootVisualElement.style.display = DisplayStyle.None;

            if (selectionScreenDocument != null)
                selectionScreenDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        #endregion

        // TODO: Add touch rotation logic for the paper model (two-finger rotate + pinch zoom)
        // TODO: Add step completion tracking / persistence
    }
}