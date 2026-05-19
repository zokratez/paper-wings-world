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
            // Floor
            CreatePlane("Canyon Floor", new Vector3(0, 0, 0), new Vector3(canyonWidth, 1, canyonLength), canyonFloorColor, 0f);

            // Left Wall
            CreatePlane("Left Wall", new Vector3(-canyonWidth / 2f, wallHeight / 2f, 0), 
                        new Vector3(1, wallHeight, canyonLength), canyonWallColor, 90f);

            // Right Wall
            CreatePlane("Right Wall", new Vector3(canyonWidth / 2f, wallHeight / 2f, 0), 
                        new Vector3(1, wallHeight, canyonLength), canyonWallColor, -90f);

            // Back Wall (distant)
            CreatePlane("Back Wall", new Vector3(0, wallHeight / 2f, canyonLength / 2f - 5), 
                        new Vector3(canyonWidth, wallHeight, 1), canyonWallColor, 0f);

            // Add some layered "mesa" feel with smaller platforms
            CreatePlane("Mesa 1", new Vector3(-18, 8, 25), new Vector3(22, 1, 18), canyonWallColor, 0f);
            CreatePlane("Mesa 2", new Vector3(24, 12, -30), new Vector3(16, 1, 14), canyonWallColor, 0f);

            // Add pleasant thermals (rising air) for fun gliding
            CreateThermal(new Vector3(-12, 5, 15), 14f, 7.5f);
            CreateThermal(new Vector3(18, 4, -22), 12f, 6.8f);
            CreateThermal(new Vector3(0, 6, 40), 16f, 5.5f);

            // Lighting & Fog for depth
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f);
            RenderSettings.fogDensity = 0.008f;
            RenderSettings.fogMode = FogMode.Exponential;

            // Sky color (approximate)
            Camera.main.backgroundColor = skyColor;
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