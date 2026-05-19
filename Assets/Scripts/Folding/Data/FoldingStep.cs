using UnityEngine;
using System;

namespace PaperWings.Folding
{
    /// <summary>
    /// Represents a single step in a paper airplane folding tutorial.
    /// This is the core data unit for the data-driven system.
    /// </summary>
    [Serializable]
    public class FoldingStep
    {
        [Header("Step Info")]
        public int stepNumber;
        public string title;                    // e.g. "Fold the left corner to the center line"
        [TextArea(2, 4)]
        public string instructionText;          // Kid-friendly instructions

        [Header("3D Animation")]
        public string animationClipName;        // Name of the AnimationClip in the plane's Animator (or Addressable reference later)
        public float animationDuration = 2.5f;  // Default duration for the fold animation

        [Header("Visual Guidance")]
        public string foldLineHighlightTag;     // Used to highlight specific edges on the 3D model
        public bool showGhostNextFold = true;   // Show a ghosted preview of the next fold position

        [Header("Validation (Future)")]
        public bool requiresUserInteraction = false; // For touch-to-fold validation in later phases
        public string validationZoneName;       // Name of the collider zone on the paper model
    }
}