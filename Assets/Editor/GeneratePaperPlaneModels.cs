using UnityEngine;
using UnityEditor;
using PaperWings.Folding;

namespace PaperWings.Editor
{
    /// <summary>
    /// Generates low-poly but clean 3D paper plane prefabs with proper bone hierarchy.
    /// 
    /// These can be used as the first "authored" models. Later the user can replace
    /// the meshes while keeping the exact bone names for seamless animation compatibility.
    /// </summary>
    public static class GeneratePaperPlaneModels
    {
        private const string OutputPath = "Assets/Prefabs/Planes/";

        [MenuItem("Paper Wings/Generate Low-Poly Rigged Paper Planes (All 8)")]
        public static void GenerateAllEight()
        {
            if (!AssetDatabase.IsValidFolder(OutputPath))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Planes");

            // Beginner
            GeneratePlane("Classic Dart", "classic_dart", PlaneStyle.Dart);
            GeneratePlane("The Ring", "the_ring", PlaneStyle.Ring);
            GeneratePlane("Loop Plane", "loop_plane", PlaneStyle.Stunt);

            // Intermediate
            GeneratePlane("Nakamichi Glider", "nakamichi_glider", PlaneStyle.Glider);
            GeneratePlane("Stealth Glider", "stealth_glider", PlaneStyle.Glider);
            GeneratePlane("Light Spinner", "light_spinner", PlaneStyle.Spinner);

            // Advanced
            GeneratePlane("Canard", "canard", PlaneStyle.Canard);
            GeneratePlane("The Bird", "the_bird", PlaneStyle.Bird);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Generated all 8 low-poly rigged paper plane prefabs in " + OutputPath);
        }

        [MenuItem("Paper Wings/Assign Real Models to Key PaperPlaneDefinitions")]
        public static void AssignModelsToDefinitions()
        {
            string[] keyPlanes = { "classic_dart", "the_ring", "nakamichi_glider", "the_bird" };

            foreach (string id in keyPlanes)
            {
                string defPath = $"Assets/ScriptableObjects/PaperPlanes/{id}.asset";
                string prefabPath = $"Assets/Prefabs/Planes/{id}.prefab";

                var definition = AssetDatabase.LoadAssetAtPath<PaperPlaneDefinition>(defPath);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (definition != null && prefab != null)
                {
                    definition.paperModelPrefab = prefab;
                    EditorUtility.SetDirty(definition);
                    Debug.Log($"Assigned real model to {id}");
                }
                else
                {
                    Debug.LogWarning($"Could not assign model for {id}. Make sure both the definition and prefab exist.");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private enum PlaneStyle
        {
            Dart,
            Ring,
            Stunt,
            Glider,
            Spinner,
            Canard,
            Bird
        }

        private static void GeneratePlane(string displayName, string id, PlaneStyle style)
        {
            GameObject root = new GameObject(displayName);

            // Create improved low-poly paper-like hierarchy
            var body = CreateBone(root, "Body", PrimitiveType.Cube, GetBodyScale(style), Vector3.zero);

            // Main wings with better paper-like proportions
            var leftWing = CreateBone(root, "LeftWing", PrimitiveType.Quad, GetWingScale(style), GetLeftWingPos(style));
            var rightWing = CreateBone(root, "RightWing", PrimitiveType.Quad, GetWingScale(style), GetRightWingPos(style));

            var leftTip = CreateBone(root, "LeftWingTip", PrimitiveType.Quad, GetTipScale(style), GetLeftTipPos(style));
            var rightTip = CreateBone(root, "RightWingTip", PrimitiveType.Quad, GetTipScale(style), GetRightTipPos(style));

            var tail = CreateBone(root, "Tail", PrimitiveType.Quad, GetTailScale(style), GetTailPos(style));

            // Apply plane-specific shaping for more realistic folded paper look
            ApplyPlaneStyle(style, body, leftWing, rightWing, leftTip, rightTip, tail);

            // Add the animator
            root.AddComponent<PaperPlaneAnimator>();

            // Paper-like material
            MakePaperMaterial(body);
            MakePaperMaterial(leftWing);
            MakePaperMaterial(rightWing);
            MakePaperMaterial(leftTip);
            MakePaperMaterial(rightTip);
            MakePaperMaterial(tail);

            // Save prefab
            string path = OutputPath + id + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        // --- Improved scales for more paper-like appearance ---

        private static Vector3 GetBodyScale(PlaneStyle style)
        {
            return style switch
            {
                PlaneStyle.Ring => new Vector3(0.09f, 0.09f, 0.28f),
                PlaneStyle.Glider => new Vector3(0.06f, 0.022f, 0.68f),
                PlaneStyle.Spinner => new Vector3(0.055f, 0.02f, 0.48f),
                PlaneStyle.Canard => new Vector3(0.065f, 0.025f, 0.58f),
                PlaneStyle.Bird => new Vector3(0.065f, 0.028f, 0.55f),
                _ => new Vector3(0.075f, 0.024f, 0.52f) // Dart / Stunt default
            };
        }

        private static Vector3 GetWingScale(PlaneStyle style)
        {
            return style switch
            {
                PlaneStyle.Glider or PlaneStyle.Bird => new Vector3(0.58f, 0.36f, 1f),
                PlaneStyle.Ring => new Vector3(0.28f, 0.28f, 1f),
                PlaneStyle.Spinner => new Vector3(0.36f, 0.32f, 1f),
                _ => new Vector3(0.46f, 0.32f, 1f)
            };
        }

        private static Vector3 GetLeftWingPos(PlaneStyle style)
        {
            float x = style == PlaneStyle.Ring ? -0.18f : -0.20f;
            return new Vector3(x, 0.008f, 0.04f);
        }

        private static Vector3 GetRightWingPos(PlaneStyle style) => new Vector3(-GetLeftWingPos(style).x, GetLeftWingPos(style).y, GetLeftWingPos(style).z);

        private static Vector3 GetTipScale(PlaneStyle style)
        {
            return style == PlaneStyle.Glider || style == PlaneStyle.Bird 
                ? new Vector3(0.22f, 0.18f, 1f) 
                : new Vector3(0.18f, 0.15f, 1f);
        }

        private static Vector3 GetLeftTipPos(PlaneStyle style)
        {
            float x = style == PlaneStyle.Glider || style == PlaneStyle.Bird ? -0.38f : -0.40f;
            return new Vector3(x, 0.004f, 0.06f);
        }

        private static Vector3 GetRightTipPos(PlaneStyle style) => new Vector3(-GetLeftTipPos(style).x, GetLeftTipPos(style).y, GetLeftTipPos(style).z);

        private static Vector3 GetTailScale(PlaneStyle style)
        {
            return style == PlaneStyle.Glider || style == PlaneStyle.Bird 
                ? new Vector3(0.12f, 0.26f, 1f) 
                : new Vector3(0.13f, 0.20f, 1f);
        }

        private static Vector3 GetTailPos(PlaneStyle style) => new Vector3(0, 0.10f, -0.26f);

        private static Transform CreateBone(GameObject parent, string name, PrimitiveType type, Vector3 scale, Vector3 localPos)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;

            // Make it a "bone" (no collider in final model usually)
            if (go.GetComponent<Collider>() != null)
                Object.DestroyImmediate(go.GetComponent<Collider>());

            return go.transform;
        }

        private static void ApplyPlaneStyle(PlaneStyle style, Transform body, Transform lw, Transform rw, Transform ltip, Transform rtip, Transform tail)
        {
            switch (style)
            {
                case PlaneStyle.Ring:
                    // More tubular, ring-like
                    body.localRotation = Quaternion.Euler(0, 0, 0);
                    lw.localRotation = Quaternion.Euler(0, 0, 25);
                    rw.localRotation = Quaternion.Euler(0, 0, -25);
                    tail.localScale = new Vector3(0.05f, 0.05f, 1f);
                    break;

                case PlaneStyle.Glider:
                    // Long, elegant dihedral
                    lw.localRotation = Quaternion.Euler(0, 0, 12);
                    rw.localRotation = Quaternion.Euler(0, 0, -12);
                    ltip.localRotation = Quaternion.Euler(0, 0, 18);
                    rtip.localRotation = Quaternion.Euler(0, 0, -18);
                    break;

                case PlaneStyle.Spinner:
                    // Asymmetric for spin character
                    lw.localRotation = Quaternion.Euler(0, 0, 35);
                    rw.localRotation = Quaternion.Euler(0, 0, -22);
                    break;

                case PlaneStyle.Canard:
                    // Prominent forward surfaces (simulated on tips)
                    ltip.localRotation = Quaternion.Euler(0, 0, 95);
                    rtip.localRotation = Quaternion.Euler(0, 0, -95);
                    lw.localScale *= 0.85f;
                    rw.localScale *= 0.85f;
                    break;

                case PlaneStyle.Bird:
                    // Layered, accordion-like wings
                    lw.localRotation = Quaternion.Euler(0, 0, 15);
                    rw.localRotation = Quaternion.Euler(0, 0, -15);
                    ltip.localRotation = Quaternion.Euler(0, 0, 28);
                    rtip.localRotation = Quaternion.Euler(0, 0, -28);
                    break;

                default:
                    // Classic dart / stunt - clean and sharp
                    lw.localRotation = Quaternion.Euler(0, 0, 6);
                    rw.localRotation = Quaternion.Euler(0, 0, -6);
                    break;
            }
        }

        private static void MakePaperMaterial(Transform t)
        {
            var renderer = t.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = Color.white;
                renderer.material = mat;
            }
        }
    }
}