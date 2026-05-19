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

            ShowMainMenu();
        }

        #region Main Menu / Hub

        private VisualElement mainMenuContainer;

        private void ShowMainMenu()
        {
            if (foldingDocument) foldingDocument.rootVisualElement.style.display = DisplayStyle.None;
            if (selectionDocument) selectionDocument.rootVisualElement.style.display = DisplayStyle.Flex;

            selectionRoot = selectionDocument.rootVisualElement;

            // Hide the plane grid for now
            var grid = selectionRoot.Q<VisualElement>("plane-grid");
            if (grid != null) grid.style.display = DisplayStyle.None;

            // Remove old menu if exists
            if (mainMenuContainer != null && mainMenuContainer.parent != null)
                mainMenuContainer.parent.Remove(mainMenuContainer);

            // Create Hub container
            mainMenuContainer = new VisualElement();
            mainMenuContainer.name = "main-hub";
            mainMenuContainer.style.flexDirection = FlexDirection.Column;
            mainMenuContainer.style.alignItems = Align.Center;
            mainMenuContainer.style.justifyContent = Justify.Center;
            mainMenuContainer.style.flexGrow = 1;
            mainMenuContainer.style.paddingTop = 40;
            mainMenuContainer.style.paddingBottom = 40;

            // Title
            var title = new Label("Paper Wings World");
            title.style.fontSize = 42;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = new Color(0.15f, 0.35f, 0.55f);
            title.style.marginBottom = 8;
            mainMenuContainer.Add(title);

            // Friendly subtitle
            var subtitle = new Label("Fold amazing planes. Fly them across the world.");
            subtitle.style.fontSize = 18;
            subtitle.style.color = new Color(0.3f, 0.4f, 0.5f);
            subtitle.style.marginBottom = 30;
            mainMenuContainer.Add(subtitle);

            // Icon / Logo area (kid-friendly placeholder)
            var iconArea = new VisualElement();
            iconArea.style.width = 120;
            iconArea.style.height = 120;
            iconArea.style.backgroundColor = new Color(0.3f, 0.65f, 0.85f);
            iconArea.style.borderRadius = 60;
            iconArea.style.alignItems = Align.Center;
            iconArea.style.justifyContent = Justify.Center;
            iconArea.style.marginBottom = 40;

            var icon = new Label("🪁");
            icon.style.fontSize = 64;
            iconArea.Add(icon);
            mainMenuContainer.Add(iconArea);

            // Buttons container
            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Column;
            buttonRow.style.alignItems = Align.Center;
            buttonRow.style.width = Length.Percent(80);
            mainMenuContainer.Add(buttonRow);

            // Start New Flight button
            var startBtn = new Button { text = "✈️  Start New Flight" };
            StyleBigButton(startBtn, new Color(0.2f, 0.55f, 0.85f));
            startBtn.clicked += () =>
            {
                HideMainMenu();
                ShowSelectionScreen();
            };
            buttonRow.Add(startBtn);

            // My Progress button
            var progressBtn = new Button { text = "📊  My Progress" };
            StyleBigButton(progressBtn, new Color(0.85f, 0.55f, 0.2f));
            progressBtn.clicked += () =>
            {
                ShowMyProgressScreen();
            };
            buttonRow.Add(progressBtn);

            selectionRoot.Add(mainMenuContainer);
        }

        private void HideMainMenu()
        {
            if (mainMenuContainer != null && mainMenuContainer.parent != null)
            {
                mainMenuContainer.parent.Remove(mainMenuContainer);
                mainMenuContainer = null;
            }

            // Show the grid again
            var grid = selectionRoot.Q<VisualElement>("plane-grid");
            if (grid != null) grid.style.display = DisplayStyle.Flex;
        }

        private void StyleBigButton(Button btn, Color bgColor)
        {
            btn.style.width = Length.Percent(100);
            btn.style.maxWidth = 420;
            btn.style.height = 72;
            btn.style.marginBottom = 16;
            btn.style.backgroundColor = bgColor;
            btn.style.color = Color.white;
            btn.style.fontSize = 22;
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.borderRadius = 16;
            btn.style.paddingLeft = 20;
            btn.style.paddingRight = 20;
        }

        #endregion

        #region Selection Screen

        private void ShowSelectionScreen()
        {
            if (foldingDocument) foldingDocument.rootVisualElement.style.display = DisplayStyle.None;
            if (selectionDocument) selectionDocument.rootVisualElement.style.display = DisplayStyle.Flex;

            selectionRoot = selectionDocument.rootVisualElement;

            // Add a friendly header with back to hub
            var header = selectionRoot.Q<VisualElement>("selection-header");
            if (header == null)
            {
                header = new VisualElement();
                header.name = "selection-header";
                header.style.flexDirection = FlexDirection.Row;
                header.style.justifyContent = Justify.SpaceBetween;
                header.style.alignItems = Align.Center;
                header.style.marginBottom = 16;
                header.style.paddingLeft = 20;
                header.style.paddingRight = 20;

                var backBtn = new Button { text = "← Hub" };
                backBtn.style.fontSize = 16;
                backBtn.style.backgroundColor = new Color(0.7f, 0.7f, 0.75f);
                backBtn.style.color = Color.white;
                backBtn.style.borderRadius = 8;
                backBtn.clicked += () =>
                {
                    if (selectionDocument) selectionDocument.rootVisualElement.style.display = DisplayStyle.None;
                    ShowMainMenu();
                };

                var headerTitle = new Label("Choose Your Plane");
                headerTitle.style.fontSize = 24;
                headerTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
                headerTitle.style.color = new Color(0.15f, 0.3f, 0.5f);

                header.Add(backBtn);
                header.Add(headerTitle);

                // Insert at top of the document root
                selectionRoot.Insert(0, header);
            }

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

            // Kid-friendly styling
            card.style.backgroundColor = new Color(0.95f, 0.97f, 1f);
            card.style.borderRadius = 16;
            card.style.padding = 16;
            card.style.margin = 8;
            card.style.minWidth = 160;
            card.style.minHeight = 90;
            card.style.flexDirection = FlexDirection.Column;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.Center;

            var nameLabel = new Label(plane.displayName);
            nameLabel.style.fontSize = 20;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = new Color(0.1f, 0.25f, 0.45f);

            var meta = new Label($"{plane.difficulty}  •  {plane.primaryCategory}");
            meta.style.fontSize = 13;
            meta.style.color = new Color(0.4f, 0.5f, 0.6f);

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
            if (audioPlayer != null) audioPlayer.PlayFoldSound(Mathf.Clamp(currentStep, 1, 5));

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

        private System.Collections.IEnumerator LaunchModelPop()
        {
            if (modelParent == null) yield break;

            Vector3 originalScale = modelParent.localScale;
            float duration = 0.35f;
            float t = 0;

            // Quick scale up for satisfying "launch away" pop (kid-friendly)
            while (t < duration)
            {
                t += Time.deltaTime;
                float progress = t / duration;
                float eased = 1f - Mathf.Pow(1f - progress, 3f);

                modelParent.localScale = originalScale * Mathf.Lerp(1f, 1.4f, eased);
                yield return null;
            }

            if (modelParent != null)
                modelParent.localScale = originalScale;
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

            // Add "My Progress" button (simple access to full best scores)
            AddMyProgressButton();

            if (audioPlayer != null)
                audioPlayer.PlaySuccessSound();

            Debug.Log($"[Folding] {currentPlane.displayName} folding tutorial completed successfully.");
        }

        private void AddMyProgressButton()
        {
            if (successPanel == null) return;

            // Avoid duplicates
            if (successPanel.Q<Button>("my-progress-btn") != null) return;

            var progressBtn = new Button { text = "📊 My Progress", name = "my-progress-btn" };
            progressBtn.style.marginTop = 16;
            progressBtn.style.paddingLeft = 20;
            progressBtn.style.paddingRight = 20;
            progressBtn.style.paddingTop = 8;
            progressBtn.style.paddingBottom = 8;
            progressBtn.style.fontSize = 16;
            progressBtn.style.backgroundColor = new Color(0.2f, 0.4f, 0.6f);
            progressBtn.style.color = Color.white;
            progressBtn.style.borderRadius = 8;

            progressBtn.clicked += ShowMyProgressScreen;

            successPanel.Add(progressBtn);
        }

        private void ShowMyProgressScreen()
        {
            if (foldingRoot == null || currentPlane == null || planeLibrary == null) return;

            // Create a simple modal-like progress panel
            var progressPanel = new VisualElement();
            progressPanel.name = "my-progress-panel";
            progressPanel.style.position = Position.Absolute;
            progressPanel.style.top = 0;
            progressPanel.style.left = 0;
            progressPanel.style.width = Length.Percent(100);
            progressPanel.style.height = Length.Percent(100);
            progressPanel.style.backgroundColor = new Color(0, 0, 0, 0.85f);
            progressPanel.style.flexDirection = FlexDirection.Column;
            progressPanel.style.alignItems = Align.Center;
            progressPanel.style.justifyContent = Justify.Center;
            progressPanel.style.padding = 20;

            // Title
            var title = new Label("My Flight Progress");
            title.style.fontSize = 28;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            title.style.marginBottom = 20;
            progressPanel.Add(title);

            // Content container (scrollable feel via max height)
            var content = new VisualElement();
            content.style.backgroundColor = new Color(1, 1, 1, 0.95f);
            content.style.borderRadius = 12;
            content.style.padding = 16;
            content.style.maxHeight = 420;
            content.style.width = Length.Percent(85);
            content.style.flexDirection = FlexDirection.Column;
            content.style.overflow = Overflow.Hidden;
            progressPanel.Add(content);

            // Regions for display
            var regions = new[]
            {
                new { Id = "grand_canyon", Name = "Grand Canyon" },
                new { Id = "fuji_foothills", Name = "Fuji Foothills" },
                new { Id = "norwegian_coast", Name = "Norwegian Coast" }
            };

            foreach (var planeDef in planeLibrary.allPlanes)
            {
                // Plane header
                var planeHeader = new Label(planeDef.displayName);
                planeHeader.style.fontSize = 18;
                planeHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                planeHeader.style.color = new Color(0.15f, 0.25f, 0.35f);
                planeHeader.style.marginTop = 12;
                planeHeader.style.marginBottom = 6;
                content.Add(planeHeader);

                foreach (var region in regions)
                {
                    var (dist, time) = PaperWings.Progression.FlightProgress.GetBest(planeDef.planeId, region.Id);
                    bool unlocked = PaperWings.Progression.FlightProgress.IsRegionUnlocked(region.Id);

                    string badge = "";
                    if (unlocked && (dist > 0 || time > 0))
                    {
                        var tier = PaperWings.Progression.FlightProgress.GetMasteryTier(planeDef.planeId, region.Id);
                        badge = PaperWings.Progression.FlightProgress.GetBadgeEmoji(tier) + " ";
                    }

                    string text = unlocked 
                        ? (dist > 0 || time > 0 ? $"{badge}{region.Name}: {dist:F0}m / {time:F1}s" : $"{region.Name}: No flights yet")
                        : $"{region.Name}: 🔒 Locked";

                    var line = new Label(text);
                    line.style.fontSize = 14;
                    line.style.color = unlocked ? new Color(0.2f, 0.3f, 0.2f) : new Color(0.5f, 0.5f, 0.5f);
                    line.style.marginLeft = 12;
                    content.Add(line);
                }
            }

            // Close button
            var closeBtn = new Button { text = "Close" };
            closeBtn.style.marginTop = 20;
            closeBtn.style.paddingLeft = 40;
            closeBtn.style.paddingRight = 40;
            closeBtn.style.paddingTop = 10;
            closeBtn.style.paddingBottom = 10;
            closeBtn.style.fontSize = 18;
            closeBtn.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
            closeBtn.style.color = Color.white;
            closeBtn.style.borderRadius = 8;

            closeBtn.clicked += () =>
            {
                if (progressPanel.parent != null)
                    progressPanel.parent.Remove(progressPanel);
            };

            progressPanel.Add(closeBtn);

            foldingRoot.Add(progressPanel);
        }

        private void ShowRegionSelection()
        {
            // Phase 3 Region Selection "screen" - appears after successful fold, before launch.
            // Clean, tablet-friendly, with short descriptions so the player understands the personality of each place.

            var regionContainer = foldingRoot.Q<VisualElement>("region-selection");
            if (regionContainer == null)
            {
                regionContainer = new VisualElement { name = "region-selection" };
                regionContainer.style.flexDirection = FlexDirection.Column;
                regionContainer.style.alignItems = Align.Center;
                regionContainer.style.marginTop = 16;
                foldingRoot.Add(regionContainer);
            }

            regionContainer.Clear();

            // Header
            var header = new Label("Choose Your Flying Region");
            header.style.fontSize = 22;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.color = new Color(0.15f, 0.25f, 0.35f);
            header.style.marginBottom = 12;
            regionContainer.Add(header);

            // Sub header hint
            var hint = new Label("Each region feels different — wind, lift, and scenery change how your plane flies.");
            hint.style.fontSize = 14;
            hint.style.color = new Color(0.4f, 0.45f, 0.5f);
            hint.style.marginBottom = 16;
            regionContainer.Add(hint);

            // Horizontal row of region choices (simple cards via buttons for now)
            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.justifyContent = Justify.Center;
            regionContainer.Add(buttonRow);

            // Show all 3 regions, with locked state for progression
            bool gcUnlocked = PaperWings.Progression.FlightProgress.IsRegionUnlocked("grand_canyon");
            bool fujiUnlocked = PaperWings.Progression.FlightProgress.IsRegionUnlocked("fuji_foothills");
            bool norUnlocked = PaperWings.Progression.FlightProgress.IsRegionUnlocked("norwegian_coast");

            CreateRegionChoice(buttonRow, "Grand Canyon", "grand_canyon", 
                "Balanced canyons • Reliable thermals • Perfect starter", "🏜️", new Color(0.85f, 0.55f, 0.35f), !gcUnlocked);

            CreateRegionChoice(buttonRow, "Fuji Foothills", "fuji_foothills", 
                "Strong volcanic lift • Misty forests • Long graceful flights", "🗻", new Color(0.35f, 0.65f, 0.45f), !fujiUnlocked);

            CreateRegionChoice(buttonRow, "Norwegian Coast", "norwegian_coast", 
                "Powerful sea winds • Dramatic fjords • Fast distance runs", "🌊", new Color(0.35f, 0.55f, 0.75f), !norUnlocked);
        }

        private void CreateRegionChoice(VisualElement container, string displayName, string regionId, string description, string iconEmoji, Color iconBg, bool isLocked)
        {
            // Polished region "card" using VisualElement + labels.
            // Supports best scores (from FlightProgress) and locked state for progression.

            var card = new VisualElement();
            card.AddToClassList("region-card");

            if (isLocked)
            {
                card.AddToClassList("region-card-locked");
            }

            // Icon
            var icon = new Label(iconEmoji);
            icon.AddToClassList("region-card-icon");
            icon.style.backgroundColor = iconBg;
            icon.style.color = Color.white;
            icon.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(icon);

            // Title
            var title = new Label(displayName);
            title.AddToClassList("region-card-title");
            card.Add(title);

            // Description / personality hint
            var desc = new Label(description);
            desc.AddToClassList("region-card-desc");
            card.Add(desc);

            if (isLocked)
            {
                // Lock state
                var lockLabel = new Label("🔒 Locked");
                lockLabel.AddToClassList("region-card-lock");
                card.Add(lockLabel);

                string unlockHint = regionId == "fuji_foothills" 
                    ? "Reach 500m in Grand Canyon to unlock" 
                    : "Reach 600m in Fuji Foothills to unlock";
                var hint = new Label(unlockHint);
                hint.style.fontSize = 10;
                hint.style.color = new Color(0.6f, 0.6f, 0.6f);
                hint.style.whiteSpace = WhiteSpace.Normal;
                card.Add(hint);

                // No click handler for locked
            }
            else
            {
                // Show best scores + mastery badge if we have a current plane
                if (currentPlane != null)
                {
                    var (bestDist, bestTime) = PaperWings.Progression.FlightProgress.GetBest(currentPlane.planeId, regionId);
                    if (bestDist > 0 || bestTime > 0)
                    {
                        string bestText = $"Best: {bestDist:F0}m / {bestTime:F1}s";
                        var bestLabel = new Label(bestText);
                        bestLabel.AddToClassList("region-card-best");
                        card.Add(bestLabel);
                    }
                    else
                    {
                        var bestLabel = new Label("Best: --");
                        bestLabel.style.fontSize = 10;
                        bestLabel.style.color = new Color(0.6f, 0.6f, 0.6f);
                        card.Add(bestLabel);
                    }

                    // Mastery badge
                    var tier = PaperWings.Progression.FlightProgress.GetMasteryTier(currentPlane.planeId, regionId);
                    string badgeEmoji = PaperWings.Progression.FlightProgress.GetBadgeEmoji(tier);
                    if (!string.IsNullOrEmpty(badgeEmoji))
                    {
                        var badgeLabel = new Label($"{badgeEmoji} {PaperWings.Progression.FlightProgress.GetBadgeLabel(tier)}");
                        badgeLabel.style.fontSize = 12;
                        badgeLabel.style.color = new Color(0.85f, 0.65f, 0.1f);
                        badgeLabel.style.marginTop = 2;
                        card.Add(badgeLabel);
                    }
                }

                // Click handler (only for unlocked)
                card.RegisterCallback<ClickEvent>(evt =>
                {
                    selectedRegionForLaunch = LoadRegionById(regionId);

                    if (launchToFlightBtn != null)
                        launchToFlightBtn.style.display = DisplayStyle.Flex;

                    Debug.Log($"[Region Selection] Selected: {displayName}");
                });
            }

            container.Add(card);
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

            // Play satisfying launch sound
            if (audioPlayer != null)
            {
                audioPlayer.PlayLaunchSound();
            }

            // Simple launch visual pop on the model (kid-friendly "whoosh away" feel)
            if (modelParent != null)
            {
                StartCoroutine(LaunchModelPop());
            }

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