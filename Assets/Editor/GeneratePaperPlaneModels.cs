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

        [MenuItem("Paper Wings/Assign Real Models to All PaperPlaneDefinitions")]
        public static void AssignAllModelsToDefinitions()
        {
            string[] allPlanes = { 
                "classic_dart", "the_ring", "loop_plane", 
                "nakamichi_glider", "stealth_glider", "light_spinner", 
                "canard", "the_bird" 
            };

            foreach (string id in allPlanes)
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
            Debug.Log("All 8 PaperPlaneDefinitions now point to real 3D models.");
        }

        [MenuItem("Paper Wings/Refresh All Models in Demo")]
        public static void RefreshAllModelsInDemo()
        {
            // Regenerate models
            GenerateAllEight();

            // Re-assign to all definitions
            AssignAllModelsToDefinitions();

            // Ping the folders so user sees the update
            AssetDatabase.Refresh();

            Debug.Log("All models regenerated and re-assigned. Please reload the demo scenes (FoldingTutorialDemo + FlightDemo) to see changes.");
            EditorUtility.DisplayDialog("Paper Wings", 
                "Models refreshed!\n\nReload the demo scenes to see the latest 3D models in action.", "OK");
        }

        [MenuItem("Paper Wings/Generate Default Flight Regions")]
        public static void GenerateDefaultFlightRegions()
        {
            string regionPath = "Assets/ScriptableObjects/FlightRegions/";
            if (!AssetDatabase.IsValidFolder(regionPath))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "FlightRegions");

            CreateRegion("Grand Canyon", "grand_canyon", new Color(0.6f, 0.75f, 0.9f));
            CreateRegion("Fuji Foothills", "fuji_foothills", new Color(0.7f, 0.85f, 0.7f));
            CreateRegion("Norwegian Coast", "norwegian_coast", new Color(0.55f, 0.75f, 0.85f));

            // Also create a ready-to-use library asset that references all three (for clean runtime loading)
            CreateDefaultRegionLibrary(regionPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Created 3 default FlightRegion assets + FlightRegionLibrary in " + regionPath);
        }

        private static void CreateDefaultRegionLibrary(string regionFolder)
        {
            string libPath = "Assets/ScriptableObjects/FlightRegionLibrary.asset";
            if (AssetDatabase.LoadAssetAtPath<FlightRegionLibrary>(libPath) != null)
            {
                Debug.Log("FlightRegionLibrary already exists. Skipping recreation.");
                return;
            }

            var lib = ScriptableObject.CreateInstance<FlightRegionLibrary>();

            // Load the three we just created (or existing ones)
            var gc = AssetDatabase.LoadAssetAtPath<FlightRegion>(regionFolder + "grand_canyon.asset");
            var fuji = AssetDatabase.LoadAssetAtPath<FlightRegion>(regionFolder + "fuji_foothills.asset");
            var nor = AssetDatabase.LoadAssetAtPath<FlightRegion>(regionFolder + "norwegian_coast.asset");

            if (gc) lib.regions.Add(gc);
            if (fuji) lib.regions.Add(fuji);
            if (nor) lib.regions.Add(nor);

            AssetDatabase.CreateAsset(lib, libPath);
            Debug.Log("Created FlightRegionLibrary with the 3 starter regions.");
        }

        private static void CreateRegion(string name, string id, Color fogColor)
        {
            string path = $"Assets/ScriptableObjects/FlightRegions/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<FlightRegion>(path) != null)
            {
                Debug.Log($"{name} region already exists. Skipping.");
                return;
            }

            var region = ScriptableObject.CreateInstance<FlightRegion>();
            region.regionId = id;
            region.displayName = name;
            region.fogColor = fogColor;

            // Distinct per-region settings for interesting gameplay
            switch (id)
            {
                case "grand_canyon":
                    region.baseWindDirection = new Vector3(0.35f, 0.08f, 0.15f);
                    region.baseWindStrength = 0.85f;
                    region.thermalStrengthMultiplier = 1.0f;
                    region.distanceGoal = 650f;
                    region.glideTimeGoal = 52f;
                    region.defaultSpawnHeight = 22f;
                    region.environmentPrimaryColor = new Color(0.75f, 0.55f, 0.4f);   // warm sandstone
                    region.environmentSecondaryColor = new Color(0.6f, 0.5f, 0.4f);
                    break;

                case "fuji_foothills":
                    // Strong lift from volcanic slopes, calmer winds, long graceful flights
                    region.baseWindDirection = new Vector3(0.15f, 0.12f, 0.45f);
                    region.baseWindStrength = 0.65f;
                    region.thermalStrengthMultiplier = 1.35f;
                    region.distanceGoal = 480f;
                    region.glideTimeGoal = 68f;
                    region.defaultSpawnHeight = 28f;
                    region.ambientLightColor = new Color(0.95f, 0.97f, 1f);
                    region.ambientIntensity = 0.95f;
                    region.fogDensity = 0.006f;
                    region.environmentPrimaryColor = new Color(0.3f, 0.55f, 0.35f);   // deep forest green
                    region.environmentSecondaryColor = new Color(0.45f, 0.5f, 0.4f);
                    break;

                case "norwegian_coast":
                    // Strong consistent sea winds, dramatic cliffs, faster travel, fewer thermals
                    region.baseWindDirection = new Vector3(0.55f, 0.03f, 0.25f);
                    region.baseWindStrength = 1.25f;
                    region.thermalStrengthMultiplier = 0.75f;
                    region.distanceGoal = 720f;
                    region.glideTimeGoal = 41f;
                    region.defaultSpawnHeight = 15f;
                    region.ambientLightColor = new Color(0.85f, 0.9f, 0.95f);
                    region.ambientIntensity = 0.85f;
                    region.fogDensity = 0.011f;
                    region.environmentPrimaryColor = new Color(0.3f, 0.45f, 0.6f);    // cool ocean/granite
                    region.environmentSecondaryColor = new Color(0.4f, 0.45f, 0.5f);
                    break;
            }

            AssetDatabase.CreateAsset(region, path);
            Debug.Log($"Created FlightRegion: {name}");
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

            // === Improved geometry for more realistic folded paper look ===
            AddPaperCreases(style, leftWing, rightWing, leftTip, rightTip, tail);

            // Apply plane-specific shaping
            ApplyPlaneStyle(style, body, leftWing, rightWing, leftTip, rightTip, tail);

            // Give the body a little paper thickness (card stock volume) — all planes benefit
            if (body) body.localScale = new Vector3(body.localScale.x, body.localScale.y * 1.45f, body.localScale.z);

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

        /// <summary>
        /// Adds extra geometry to simulate paper creases, leading-edge rolls, and subtle thickness.
        /// This gives the low-poly models a much more convincing "real folded paper" look
        /// while staying extremely cheap (a few extra quads per plane).
        /// Special care is taken for The Ring (tubular/annular) and The Bird (layered accordion).
        /// </summary>
        private static void AddPaperCreases(PlaneStyle style, Transform leftWing, Transform rightWing, 
                                           Transform leftTip, Transform rightTip, Transform tail)
        {
            // === General paper crease + edge treatment (most planes) ===
            if (style != PlaneStyle.Ring)
            {
                // Primary fold crease running span-wise
                CreateCrease(leftWing, "LeftCrease", new Vector3(0.48f, 0.006f, 0.98f), new Vector3(0, 0.003f, 0));
                CreateCrease(rightWing, "RightCrease", new Vector3(0.48f, 0.006f, 0.98f), new Vector3(0, 0.003f, 0));
            }

            // Leading edge "roll" detail — makes wings look thicker and more aerodynamic
            CreateEdgeDetail(leftWing, "LeftLeadingEdge", new Vector3(0.055f, 0.012f, 1f), new Vector3(-0.23f, 0.006f, 0.01f));
            CreateEdgeDetail(rightWing, "RightLeadingEdge", new Vector3(0.055f, 0.012f, 1f), new Vector3(0.23f, 0.006f, 0.01f));

            // Trailing edge subtle thickness (gives the wing a real paper card feel)
            CreateEdgeDetail(leftWing, "LeftTrailingEdge", new Vector3(0.04f, 0.01f, 1f), new Vector3(-0.23f, -0.002f, 0.01f));
            CreateEdgeDetail(rightWing, "RightTrailingEdge", new Vector3(0.04f, 0.01f, 1f), new Vector3(0.23f, -0.002f, 0.01f));

            // === The Bird — layered accordion / pleated wings ===
            if (style == PlaneStyle.Bird)
            {
                // Second and third layer creases to simulate the bird-like folded pleats
                CreateCrease(leftWing, "LeftLayer2", new Vector3(0.38f, 0.009f, 0.88f), new Vector3(-0.02f, 0.008f, 0.04f));
                CreateCrease(rightWing, "RightLayer2", new Vector3(0.38f, 0.009f, 0.88f), new Vector3(0.02f, 0.008f, 0.04f));

                CreateCrease(leftWing, "LeftLayer3", new Vector3(0.32f, 0.007f, 0.78f), new Vector3(-0.04f, 0.014f, 0.08f));
                CreateCrease(rightWing, "RightLayer3", new Vector3(0.32f, 0.007f, 0.78f), new Vector3(0.04f, 0.014f, 0.08f));

                // Extra tip "feather" layers
                if (leftTip) leftTip.localScale = new Vector3(leftTip.localScale.x, leftTip.localScale.y * 1.35f, leftTip.localScale.z);
                if (rightTip) rightTip.localScale = new Vector3(rightTip.localScale.x, rightTip.localScale.y * 1.35f, rightTip.localScale.z);
            }

            // === The Ring — tubular / annular structure ===
            if (style == PlaneStyle.Ring)
            {
                // Inner tube wall to give real depth to the ring (the "hole" effect)
                CreateCrease(leftWing, "RingInnerLeft", new Vector3(0.16f, 0.022f, 0.82f), new Vector3(0.01f, 0.01f, 0));
                CreateCrease(rightWing, "RingInnerRight", new Vector3(0.16f, 0.022f, 0.82f), new Vector3(-0.01f, 0.01f, 0));

                // Top and bottom "lip" of the ring tube for thickness
                CreateEdgeDetail(leftWing, "RingLipTopL", new Vector3(0.28f, 0.008f, 0.18f), new Vector3(0, 0.018f, 0.42f));
                CreateEdgeDetail(rightWing, "RingLipTopR", new Vector3(0.28f, 0.008f, 0.18f), new Vector3(0, 0.018f, 0.42f));

                // Give the main ring walls more "height" (thickness of the paper tube)
                if (leftWing) leftWing.localScale = new Vector3(leftWing.localScale.x, leftWing.localScale.y * 1.6f, leftWing.localScale.z);
                if (rightWing) rightWing.localScale = new Vector3(rightWing.localScale.x, rightWing.localScale.y * 1.6f, rightWing.localScale.z);
            }

            // === Universal tip thickness pass ===
            if (leftTip) leftTip.localScale = new Vector3(leftTip.localScale.x, leftTip.localScale.y * 1.25f, leftTip.localScale.z);
            if (rightTip) rightTip.localScale = new Vector3(rightTip.localScale.x, rightTip.localScale.y * 1.25f, rightTip.localScale.z);
        }

        private static void CreateEdgeDetail(Transform parent, string name, Vector3 scale, Vector3 localPos)
        {
            GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Quad);
            edge.name = name;
            edge.transform.SetParent(parent);
            edge.transform.localPosition = localPos;
            edge.transform.localScale = scale;

            var renderer = edge.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = new Color(0.9f, 0.9f, 0.9f);
                renderer.material = mat;
            }
        }

        private static void CreateCrease(Transform parent, string name, Vector3 scale, Vector3 localPos)
        {
            GameObject crease = GameObject.CreatePrimitive(PrimitiveType.Quad);
            crease.name = name;
            crease.transform.SetParent(parent);
            crease.transform.localPosition = localPos;
            crease.transform.localScale = scale;
            crease.transform.localRotation = Quaternion.identity;

            var renderer = crease.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = new Color(0.78f, 0.78f, 0.78f); // darker crease line for realistic folded paper shadow
                renderer.material = mat;
            }
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
                    // More tubular, ring-like - enhanced for distinct annular silhouette
                    body.localRotation = Quaternion.Euler(0, 0, 0);
                    body.localScale = new Vector3(0.08f, 0.08f, 0.32f); // thicker tube body
                    lw.localRotation = Quaternion.Euler(0, 0, 22);
                    rw.localRotation = Quaternion.Euler(0, 0, -22);
                    // Slight forward sweep on ring walls for 3D hoop feel
                    lw.localPosition = new Vector3(lw.localPosition.x, lw.localPosition.y, 0.02f);
                    rw.localPosition = new Vector3(rw.localPosition.x, rw.localPosition.y, 0.02f);
                    tail.localScale = new Vector3(0.04f, 0.04f, 0.9f);
                    tail.localPosition = new Vector3(0, 0.08f, -0.30f);
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
                    // Layered, accordion-like wings - more bird-like with pronounced layering and slight anhedral
                    lw.localRotation = Quaternion.Euler(0, 0, 18);
                    rw.localRotation = Quaternion.Euler(0, 0, -18);
                    ltip.localRotation = Quaternion.Euler(0, 0, 32);
                    rtip.localRotation = Quaternion.Euler(0, 0, -32);
                    // Slight downward droop on tips for natural bird wing look
                    if (ltip) ltip.localPosition = new Vector3(ltip.localPosition.x, -0.01f, ltip.localPosition.z);
                    if (rtip) rtip.localPosition = new Vector3(rtip.localPosition.x, -0.01f, rtip.localPosition.z);
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