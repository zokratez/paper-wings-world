using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using PaperWings.Demo;
using PaperWings.Folding;

namespace PaperWings.Editor
{
    public static class SetupDemoScene
    {
        [MenuItem("Paper Wings/HIGH INTENSITY - Create Playable Demo Scene")]
        public static void CreateDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.transform.position = new Vector3(0, 1.2f, -3.5f);
            camGO.tag = "MainCamera";

            // Light
            var lightGO = new GameObject("Directional Light");
            lightGO.AddComponent<Light>().type = LightType.Directional;

            // Model root
            var modelRoot = new GameObject("ModelRoot").transform;

            // UI
            var selectionUI = new GameObject("SelectionUI").AddComponent<UIDocument>();
            var foldingUI = new GameObject("FoldingUI").AddComponent<UIDocument>();

            // Bootstrap
            var bootstrapGO = new GameObject("FoldingDemoBootstrap");
            var bootstrap = bootstrapGO.AddComponent<FoldingDemoBootstrap>();
            bootstrap.library = AssetDatabase.LoadAssetAtPath<PaperPlaneLibrary>("Assets/ScriptableObjects/PaperPlanes/PaperPlaneLibrary.asset");
            bootstrap.selectionUI = selectionUI;
            bootstrap.foldingUI = foldingUI;
            bootstrap.modelParent = modelRoot;
            bootstrap.mainCamera = cam;

            // Assign UXMLs
            selectionUI.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Folding/PlaneSelectionScreen.uxml");
            foldingUI.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Folding/FoldingScreen.uxml");

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/FoldingTutorialDemo.unity");
            Debug.Log("HIGH INTENSITY: Playable demo scene created! Open Assets/Scenes/FoldingTutorialDemo.unity and press Play.");
        }
    }
}