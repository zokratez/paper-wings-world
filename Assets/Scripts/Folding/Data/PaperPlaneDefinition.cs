using UnityEngine;
using System.Collections.Generic;

namespace PaperWings.Folding
{
    /// <summary>
    /// ScriptableObject that defines everything needed for one paper airplane in the tutorial system.
    /// This is the heart of the data-driven architecture.
    /// Add new planes by creating new assets of this type (no code changes required).
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlaneDefinition", menuName = "Paper Wings / Paper Plane Definition")]
    public class PaperPlaneDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string planeId;                    // Unique ID e.g. "classic_dart", "the_ring"
        public string displayName;                // "Classic Dart"
        public Sprite thumbnail;                  // For selection grid

        [Header("Classification")]
        public Core.DifficultyLevel difficulty;
        public Core.PlaneCategory primaryCategory;
        public string[] tags;                     // e.g. "japanese", "record", "forgiving"

        [Header("Folding Tutorial")]
        public List<FoldingStep> steps = new List<FoldingStep>();
        public GameObject paperModelPrefab;       // The 3D paper model with Animator + bones/blendshapes
        public RuntimeAnimatorController animatorController;

        [Header("Flight Tuning (for later phases)")]
        public FlightCharacteristics flightData;

        [Header("Content")]
        [TextArea(2, 3)]
        public string shortDescription;
        [TextArea(2, 4)]
        public string educationalNote;
        public string culturalNote;

        [Header("Unlock & Monetization")]
        public bool isFree = false;
        public string unlockProductId;            // RevenueCat / IAP id

        [Header("Printable")]
        public string printableTemplatePath;      // Addressable or Resources path to PDF/image

        public int TotalSteps => steps != null ? steps.Count : 0;
    }

    [System.Serializable]
    public class FlightCharacteristics
    {
        public float baseSpeed = 1f;
        public float glideEfficiency = 1f;
        public float stability = 1f;
        public float windSensitivity = 1f;
    }
}