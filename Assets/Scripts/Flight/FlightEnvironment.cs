using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// Creates a simple but beautiful Grand Canyon-style test environment for the flight demo.
    /// This gives a strong sense of height and place without needing real terrain assets yet.
    /// </summary>
    public class FlightEnvironment : MonoBehaviour
    {
        [Header("Environment Settings")]
        public Color canyonFloorColor = new Color(0.55f, 0.45f, 0.35f);
        public Color canyonWallColor = new Color(0.65f, 0.55f, 0.45f);
        public Color skyColor = new Color(0.55f, 0.75f, 0.95f);

        [Header("Canyon Dimensions")]
        public float canyonWidth = 80f;
        public float canyonLength = 120f;
        public float wallHeight = 35f;

        public void BuildEnvironment()
        {
            BuildEnvironment(null);
        }

        /// <summary>
        /// Builds an environment adapted to the given region.
        /// This is the key to making the three flying areas feel like different places
        /// even before we have real 3D Tiles terrain.
        /// </summary>
        public void BuildEnvironment(FlightRegion region)
        {
            string theme = region != null ? region.regionId : "grand_canyon";

            Color floorCol = region != null ? region.environmentPrimaryColor : canyonFloorColor;
            Color wallCol = region != null ? region.environmentSecondaryColor : canyonWallColor;
            Color skyCol = skyColor;
            string[] extraProps = new string[0];

            switch (theme)
            {
                case "fuji_foothills":
                    skyCol = new Color(0.65f, 0.82f, 0.95f);    // soft misty sky
                    extraProps = new[] { "forest", "shrine" };
                    break;

                case "norwegian_coast":
                    skyCol = new Color(0.58f, 0.72f, 0.85f);    // cool overcast
                    extraProps = new[] { "water", "island", "cliff" };
                    break;

                default: // grand_canyon
                    // keep the warm sandstone defaults from region or fallback
                    extraProps = new[] { "mesa" };
                    break;
            }

            // Floor (the "ground" — canyon, forest floor, or sea)
            CreatePlane(theme + " Floor", new Vector3(0, 0, 0), new Vector3(canyonWidth, 1, canyonLength), floorCol, 0f);

            // Main walls / cliffs (rotated)
            CreatePlane("Left Cliff", new Vector3(-canyonWidth / 2f, wallHeight / 2f, 0), 
                        new Vector3(1, wallHeight, canyonLength), wallCol, 90f);
            CreatePlane("Right Cliff", new Vector3(canyonWidth / 2f, wallHeight / 2f, 0), 
                        new Vector3(1, wallHeight, canyonLength), wallCol, -90f);
            CreatePlane("Back Cliff", new Vector3(0, wallHeight / 2f, canyonLength / 2f - 5), 
                        new Vector3(canyonWidth, wallHeight, 1), wallCol, 0f);

            // Region-specific props for visual identity
            if (System.Array.IndexOf(extraProps, "mesa") >= 0)
            {
                CreatePlane("Mesa 1", new Vector3(-18, 8, 25), new Vector3(22, 1, 18), wallCol, 0f);
                CreatePlane("Mesa 2", new Vector3(24, 12, -30), new Vector3(16, 1, 14), wallCol, 0f);
            }

            if (System.Array.IndexOf(extraProps, "forest") >= 0)
            {
                // Simple green "tree cluster" hints
                CreatePlane("Forest Patch 1", new Vector3(-25, 3, 18), new Vector3(14, 0.8f, 14), new Color(0.22f, 0.38f, 0.18f), 0f);
                CreatePlane("Forest Patch 2", new Vector3(22, 4, -15), new Vector3(18, 0.9f, 12), new Color(0.25f, 0.42f, 0.20f), 0f);
            }

            if (System.Array.IndexOf(extraProps, "water") >= 0 || System.Array.IndexOf(extraProps, "island") >= 0)
            {
                // Water plane (lower)
                CreatePlane("Sea", new Vector3(0, -1.5f, 10), new Vector3(canyonWidth * 1.2f, 1, canyonLength * 0.8f), new Color(0.18f, 0.32f, 0.48f), 0f);
                // Small island / rock
                CreatePlane("Island", new Vector3(18, 1.5f, -8), new Vector3(12, 0.6f, 9), new Color(0.38f, 0.36f, 0.34f), 0f);
            }

            // Thermals tuned by region (the physics already received the multiplier)
            float tMult = (region != null) ? region.thermalStrengthMultiplier : 1f;
            CreateThermal(new Vector3(-12, 5, 15), 14f * tMult, 7.5f);
            CreateThermal(new Vector3(18, 4, -22), 12f * tMult, 6.8f);
            CreateThermal(new Vector3(0, 6, 40), 16f * tMult, 5.5f);

            // Fog & sky from region or sensible default
            RenderSettings.fog = true;
            RenderSettings.fogColor = (region != null) ? region.fogColor : new Color(0.7f, 0.8f, 0.9f);
            RenderSettings.fogDensity = (region != null) ? region.fogDensity : 0.008f;
            RenderSettings.fogMode = FogMode.Exponential;

            Camera.main.backgroundColor = skyCol;
        }

        private void CreatePlane(string name, Vector3 position, Vector3 scale, Color color, float yRotation)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = name;
            plane.transform.position = position;
            plane.transform.localScale = scale;
            plane.transform.rotation = Quaternion.Euler(0, yRotation, 0);

            var renderer = plane.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.material = mat;
        }

        private void CreateThermal(Vector3 position, float radius, float strength)
        {
            GameObject thermal = new GameObject($"Thermal_{position.x:F0}");
            thermal.transform.position = position;

            var col = thermal.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = radius;

            var tz = thermal.AddComponent<PaperWings.Flight.ThermalZone>();
            tz.upwardForce = strength;
            tz.radius = radius;
        }
    }
}