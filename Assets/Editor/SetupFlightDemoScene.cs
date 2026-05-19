using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using PaperWings.Demo;
using PaperWings.Flight;

namespace PaperWings.Editor
{
    public static class SetupFlightDemoScene
    {
        [MenuItem("Paper Wings/HIGH INTENSITY - Create FlightDemo Scene")]
        public static void CreateFlightDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Main Camera
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.transform.position = new Vector3(0, 8, -12);
            cam.transform.rotation = Quaternion.Euler(18, 0, 0);
            camGO.tag = "MainCamera";

            // Directional Light (sun)
            var lightGO = new GameObject("Sun");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(55, 35, 0);
            light.intensity = 1.1f;

            // Spawn Point
            var spawn = new GameObject("SpawnPoint").transform;
            spawn.position = new Vector3(0, 12, 0);

            // Bootstrap
            var bootstrapGO = new GameObject("FlightDemoBootstrap");
            var bootstrap = bootstrapGO.AddComponent<FlightDemoBootstrap>();
            bootstrap.spawnPoint = spawn;
            bootstrap.flightCamera = cam;

            // Let the bootstrap build a beautiful canyon-style environment
            var envGO = new GameObject("Environment");
            var env = envGO.AddComponent<PaperWings.Flight.FlightEnvironment>();
            env.BuildEnvironment();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/FlightDemo.unity");

            Debug.Log("FlightDemo scene created at Assets/Scenes/FlightDemo.unity\n" +
                      "You can now launch from the Folding scene using the 'Launch to Flight' button.");
        }
    }
}