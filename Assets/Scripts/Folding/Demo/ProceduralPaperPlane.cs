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

        public void Initialize(string planeId = "classic_dart")
        {
            CreateParts();
            RegisterParts();
            ConfigureForPlane(planeId);
            ResetToFlat();
        }

        /// <summary>
        /// Configures the model proportions to visually match the target plane type.
        /// This gives each of the 8 planes a distinct, recognizable silhouette.
        /// </summary>
        public void ConfigureForPlane(string planeId)
        {
            switch (planeId)
            {
                case "the_ring":
                    // Short, wide, tubular look
                    if (body) body.localScale = new Vector3(0.12f, 0.12f, 0.35f);
                    if (leftWing) leftWing.localScale = new Vector3(0.35f, 0.35f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.35f, 0.35f, 1f);
                    if (tail) tail.localScale = new Vector3(0.08f, 0.08f, 1f);
                    break;

                case "loop_plane":
                    // Sleek dart with pronounced elevators
                    if (body) body.localScale = new Vector3(0.06f, 0.025f, 0.65f);
                    if (leftWing) leftWing.localScale = new Vector3(0.42f, 0.28f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.42f, 0.28f, 1f);
                    break;

                case "nakamichi_glider":
                case "stealth_glider":
                    // Long, wide glider profile
                    if (body) body.localScale = new Vector3(0.07f, 0.025f, 0.75f);
                    if (leftWing) leftWing.localScale = new Vector3(0.65f, 0.38f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.65f, 0.38f, 1f);
                    if (tail) tail.localScale = new Vector3(0.12f, 0.28f, 1f);
                    break;

                case "light_spinner":
                    // Light and slightly asymmetric for spin
                    if (body) body.localScale = new Vector3(0.05f, 0.02f, 0.5f);
                    if (leftWing) leftWing.localScale = new Vector3(0.38f, 0.42f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.38f, 0.28f, 1f); // asymmetric
                    break;

                case "canard":
                    // Add visual forward wing emphasis (we'll scale the existing wings to suggest canards)
                    if (body) body.localScale = new Vector3(0.07f, 0.03f, 0.6f);
                    if (leftWing) leftWing.localScale = new Vector3(0.32f, 0.22f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.32f, 0.22f, 1f);
                    // Simulate canards by making wing tips more prominent
                    if (leftWingTip) leftWingTip.localScale = new Vector3(0.18f, 0.12f, 1f);
                    if (rightWingTip) rightWingTip.localScale = new Vector3(0.18f, 0.12f, 1f);
                    break;

                case "the_bird":
                    // Distinct accordion/bird wing look - wider and layered feel
                    if (body) body.localScale = new Vector3(0.07f, 0.035f, 0.58f);
                    if (leftWing) leftWing.localScale = new Vector3(0.58f, 0.42f, 1f);
                    if (rightWing) rightWing.localScale = new Vector3(0.58f, 0.42f, 1f);
                    if (tail) tail.localScale = new Vector3(0.14f, 0.32f, 1f);
                    break;

                default: // classic_dart and fallbacks
                    // Default dart proportions (already set in CreateParts)
                    break;
            }
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
        /// Animates a specific fold step. Uses easing for natural, satisfying motion.
        /// Each plane has tailored animation logic so folding feels distinct and realistic.
        /// </summary>
        public void AnimateFold(string foldName, float t)
        {
            float eased = EaseOutCubic(t); // Natural easing for paper-like movement

            switch (foldName)
            {
                // === Classic Dart ===
                case "Fold_CenterLine":
                    // Subtle body compression for "crease" feel
                    if (body) body.localScale = Vector3.Lerp(new Vector3(0.08f, 0.03f, 0.55f), new Vector3(0.08f, 0.025f, 0.55f), eased * 0.3f);
                    break;

                case "Fold_TopCorners":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 58, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -58, eased));
                    break;

                case "Fold_TopPoint":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(58, 85, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-58, -85, eased));
                    break;

                case "Fold_Half":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(85, 0, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-85, 0, eased));
                    break;

                case "Fold_Wings":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 95, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -95, eased));
                    break;

                case "Fold_WingTips":
                    if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 42, eased));
                    if (rightWingTip) rightWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -42, eased));
                    break;

                // === The Ring ===
                case "Ring_First":
                case "Ring_Second":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 165, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -165, eased));
                    break;

                case "Ring_Lock":
                    if (body) body.localScale = Vector3.Lerp(new Vector3(0.12f, 0.12f, 0.35f), new Vector3(0.11f, 0.11f, 0.38f), eased);
                    break;

                // === Loop Plane ===
                case "Loop_Base":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 35, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -35, eased));
                    break;

                case "Loop_Elevators":
                    if (tail) tail.localRotation = Quaternion.Euler(Mathf.Lerp(90, 125, eased), 0, 0);
                    break;

                case "Loop_Wings":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(35, 48, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-35, -48, eased));
                    break;

                // === Gliders (Nakamichi + Stealth + The Bird) ===
                case "Glider_Fuselage":
                case "Stealth_Base":
                case "Bird_Accordion":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 18, eased));  // gentle dihedral start
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -18, eased));
                    break;

                case "Glider_Wings":
                case "Stealth_Stabilize":
                case "Bird_Shaping":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(18, 32, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-18, -32, eased));
                    if (tail) tail.localRotation = Quaternion.Euler(Mathf.Lerp(90, 78, eased), 0, 0);
                    break;

                case "Glider_Tail":
                case "Stealth_Trim":
                case "Bird_Trim":
                case "Glider_Trim":
                    if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 25, eased));
                    if (rightWingTip) rightWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -25, eased));
                    break;

                // === Light Spinner ===
                case "Spinner_Base":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 25, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -18, eased)); // asymmetric
                    break;

                case "Spinner_Wings":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(25, 95, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-18, -82, eased));
                    break;

                case "Spinner_Calibrate":
                    if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 35, eased));
                    break;

                // === Canard ===
                case "Canard_Main":
                    if (leftWing) leftWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 28, eased));
                    if (rightWing) rightWing.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -28, eased));
                    break;

                case "Canard_Canards":
                    // Simulate small forward wings by rotating wing tips more extremely
                    if (leftWingTip) leftWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 115, eased));
                    if (rightWingTip) rightWingTip.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -115, eased));
                    break;

                case "Canard_Trim":
                    if (tail) tail.localRotation = Quaternion.Euler(Mathf.Lerp(90, 82, eased), 0, 0);
                    break;
            }
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        /// <summary>
        /// Adds subtle visual flutter and turning feedback to the paper model.
        /// Called from the flight controller or physics when in flight.
        /// </summary>
        public void ApplyFlightVisuals(float speed, float turnRate)
        {
            float flutter = Mathf.Sin(Time.time * 18f) * 0.8f + Mathf.Sin(Time.time * 27f) * 0.5f;
            float turnWobble = turnRate * 2.5f;

            if (leftWing)
            {
                Vector3 rot = leftWing.localRotation.eulerAngles;
                rot.z += (flutter + turnWobble) * 0.015f;
                leftWing.localRotation = Quaternion.Euler(rot);
            }

            if (rightWing)
            {
                Vector3 rot = rightWing.localRotation.eulerAngles;
                rot.z += (-flutter + turnWobble) * 0.015f;
                rightWing.localRotation = Quaternion.Euler(rot);
            }

            // Light body flex when turning hard or going fast
            if (body)
            {
                float flex = Mathf.Sin(Time.time * 12f) * speed * 0.008f;
                body.localScale = new Vector3(
                    1f + flex * 0.3f,
                    1f + flex * 0.15f,
                    1f
                );
            }
        }
    }
}