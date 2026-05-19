using UnityEngine;
using System.Collections.Generic;

namespace PaperWings.Folding
{
    /// <summary>
    /// High-quality component that drives folding animations on any 3D paper plane model.
    /// 
    /// Required Hierarchy (by name):
    /// - Body (root or main fuselage)
    /// - LeftWing
    /// - RightWing
    /// - LeftWingTip (optional but recommended)
    /// - RightWingTip (optional)
    /// - Tail
    /// 
    /// This allows real authored low-poly models to be dropped in without changing any code,
    /// as long as they follow the same bone/part naming.
    /// </summary>
    public class PaperPlaneAnimator : MonoBehaviour
    {
        private Dictionary<string, Transform> bones = new Dictionary<string, Transform>();

        public void Initialize()
        {
            bones.Clear();

            // Find bones by name (case sensitive, direct children or deep search)
            FindBone("Body");
            FindBone("LeftWing");
            FindBone("RightWing");
            FindBone("LeftWingTip");
            FindBone("RightWingTip");
            FindBone("Tail");

            // If no Body found, assume this transform is the body
            if (!bones.ContainsKey("Body"))
            {
                bones["Body"] = transform;
            }
        }

        private void FindBone(string name)
        {
            var t = transform.Find(name);
            if (t != null)
            {
                bones[name] = t;
            }
            else
            {
                // Deep search as fallback
                t = FindDeep(transform, name);
                if (t != null) bones[name] = t;
            }
        }

        private Transform FindDeep(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var found = FindDeep(child, name);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>
        /// Drives a specific fold animation. Called by FoldingTutorialManager.
        /// </summary>
        public void AnimateFold(string foldName, float t)
        {
            if (bones.Count == 0) Initialize();

            float eased = EaseOutCubic(t);

            // This logic mirrors (and can eventually replace) the one in ProceduralPaperPlane
            // We apply it to whatever transforms we found by name.

            switch (foldName)
            {
                case "Fold_CenterLine":
                    if (bones.TryGetValue("Body", out var body))
                        body.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1, 0.92f, 1), eased * 0.4f);
                    break;

                case "Fold_TopCorners":
                    ApplyWingRotation("LeftWing", 0, 0, Mathf.Lerp(0, 58, eased));
                    ApplyWingRotation("RightWing", 0, 0, Mathf.Lerp(0, -58, eased));
                    break;

                case "Fold_TopPoint":
                    ApplyWingRotation("LeftWing", 0, 0, Mathf.Lerp(58, 85, eased));
                    ApplyWingRotation("RightWing", 0, 0, Mathf.Lerp(-58, -85, eased));
                    break;

                case "Fold_Half":
                    ApplyWingRotation("LeftWing", 0, 0, Mathf.Lerp(85, 0, eased));
                    ApplyWingRotation("RightWing", 0, 0, Mathf.Lerp(-85, 0, eased));
                    break;

                case "Fold_Wings":
                    ApplyWingRotation("LeftWing", 0, 0, Mathf.Lerp(0, 95, eased));
                    ApplyWingRotation("RightWing", 0, 0, Mathf.Lerp(0, -95, eased));
                    break;

                case "Fold_WingTips":
                    ApplyWingRotation("LeftWingTip", 0, 0, Mathf.Lerp(0, 42, eased));
                    ApplyWingRotation("RightWingTip", 0, 0, Mathf.Lerp(0, -42, eased));
                    break;

                // Add cases for other planes as needed (Ring, Glider, Spinner, Canard, Bird, etc.)
                // The system is designed so we can extend this switch without touching the manager.
            }
        }

        private void ApplyWingRotation(string boneName, float x, float y, float z)
        {
            if (bones.TryGetValue(boneName, out var bone))
            {
                bone.localRotation = Quaternion.Euler(x, y, z);
            }
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }
    }
}