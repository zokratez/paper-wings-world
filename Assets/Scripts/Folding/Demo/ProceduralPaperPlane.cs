using UnityEngine;
using System.Collections.Generic;

namespace PaperWings.Demo
{
    /// <summary>
    /// Procedural paper plane with named parts for demo folding animations.
    /// This allows us to simulate folding without real 3D models yet.
    /// </summary>
    public class ProceduralPaperPlane : MonoBehaviour
    {
        [Header("Parts (auto-created)")]
        public Transform body;
        public Transform leftWing;
        public Transform rightWing;
        public Transform leftWingTip;
        public Transform rightWingTip;
        public Transform tail;

        private Dictionary<string, Transform> parts = new Dictionary<string, Transform>();

        public void Initialize()
        {
            CreateParts();
            RegisterParts();
            ResetToFlat();
        }

        private void CreateParts()
        {
            // Body
            body = CreatePart("Body", PrimitiveType.Cube, new Vector3(0.08f, 0.03f, 0.55f), Vector3.zero);

            // Wings
            leftWing = CreatePart("LeftWing", PrimitiveType.Quad, new Vector3(0.48f, 0.32f, 1), new Vector3(-0.22f, 0.02f, 0.02f), Quaternion.Euler(0, 0, 8));
            rightWing = CreatePart("RightWing", PrimitiveType.Quad, new Vector3(0.48f, 0.32f, 1), new Vector3(0.22f, 0.02f, 0.02f), Quaternion.Euler(0, 0, -8));

            // Wing tips (for last step)
            leftWingTip = CreatePart("LeftWingTip", PrimitiveType.Quad, new Vector3(0.22f, 0.18f, 1), new Vector3(-0.42f, 0.01f, 0.05f), Quaternion.Euler(0, 0, 22));
            rightWingTip = CreatePart("RightWingTip", PrimitiveType.Quad, new Vector3(0.22f, 0.18f, 1), new Vector3(0.42f, 0.01f, 0.05f), Quaternion.Euler(0, 0, -22));

            // Tail
            tail = CreatePart("Tail", PrimitiveType.Quad, new Vector3(0.16f, 0.24f, 1), new Vector3(0, 0.14f, -0.26f), Quaternion.Euler(85, 0, 0));
        }

        private Transform CreatePart(string name, PrimitiveType type, Vector3 scale, Vector3 pos, Quaternion? rot = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(transform);
            go.transform.localScale = scale;
            go.transform.localPosition = pos;
            if (rot.HasValue) go.transform.localRotation = rot.Value;

            var renderer = go.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            renderer.material.color = Color.white;

            return go.transform;
        }

        private void RegisterParts()
        {
            parts.Clear();
            parts["Body"] = body;
            parts["LeftWing"] = leftWing;
            parts["RightWing"] = rightWing;
            parts["LeftWingTip"] = leftWingTip;
            parts["RightWingTip"] = rightWingTip;
            parts["Tail"] = tail;
        }

        public void ResetToFlat()
        {
            // Set all parts to a flat starting pose
            if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, 0);
            if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, 0);
            if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, 0);
            if (rightWingTip) rightWingTip.localRotation = Quaternion.Euler(0, 0, 0);
            if (tail) tail.localRotation = Quaternion.Euler(90, 0, 0);
        }

        /// <summary>
        /// Animates a fold by name. Used by the demo system.
        /// </summary>
        public void AnimateFold(string foldName, float t)
        {
            switch (foldName)
            {
                case "Fold_CenterLine":
                    // Already flat
                    break;

                case "Fold_TopCorners":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 55, t));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -55, t));
                    break;

                case "Fold_TopPoint":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(55, 82, t));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-55, -82, t));
                    break;

                case "Fold_Half":
                    // Simulate closing the plane
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(82, 0, t));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-82, 0, t));
                    break;

                case "Fold_Wings":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 92, t));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -92, t));
                    break;

                case "Fold_WingTips":
                    if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 38, t));
                    if (rightWingTip) rightWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -38, t));
                    break;
            }
        }
    }
}