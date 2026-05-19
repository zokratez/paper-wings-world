using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PaperWings.Editor
{
    /// <summary>
    /// One-click helper to prepare the project for mobile build testing.
    /// 
    /// Adds both demo scenes to Build Settings (if missing) and applies
    /// sensible mobile/tablet Player Settings (landscape orientation, performance-friendly defaults).
    /// 
    /// Run this before any real-device build or TestFlight / internal Android testing.
    /// </summary>
    public static class BuildSettingsHelper
    {
        private const string FOLDING_SCENE = "Assets/Scenes/FoldingTutorialDemo.unity";
        private const string FLIGHT_SCENE = "Assets/Scenes/FlightDemo.unity";

        [MenuItem("Paper Wings / HIGH INTENSITY - Prepare Build Settings for Mobile Testing")]
        public static void PrepareMobileBuildSettings()
        {
            Debug.Log("[BuildSettingsHelper] Starting mobile build preparation...");

            // === 1. Add both demo scenes to Build Settings ===
            AddDemoScenesToBuildSettings();

            // === 2. Apply mobile-friendly Player Settings ===
            ApplyMobilePlayerSettings();

            // Save any changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[BuildSettingsHelper] ✅ Mobile build settings ready. Both demo scenes added + landscape orientation + performance defaults applied.");
            Debug.Log("[BuildSettingsHelper] Reminder: Run the scene setup tools first (Paper Wings / HIGH INTENSITY - Setup Demo Scenes) if the .unity files are missing.");
        }

        private static void AddDemoScenesToBuildSettings()
        {
            var currentScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool scenesChanged = false;

            string[] requiredScenes = { FOLDING_SCENE, FLIGHT_SCENE };

            foreach (string scenePath in requiredScenes)
            {
                bool alreadyPresent = currentScenes.Exists(s => s.path == scenePath);

                if (!alreadyPresent)
                {
                    // Add even if file doesn't exist yet (user will be warned on build)
                    currentScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    scenesChanged = true;
                    Debug.Log($"[BuildSettingsHelper] Added to Build Settings: {scenePath}");
                }
                else
                {
                    Debug.Log($"[BuildSettingsHelper] Scene already present: {scenePath}");
                }
            }

            if (scenesChanged)
            {
                EditorBuildSettings.scenes = currentScenes.ToArray();
                Debug.Log("[BuildSettingsHelper] Build Settings scenes list updated.");
            }
        }

        private static void ApplyMobilePlayerSettings()
        {
            // Orientation: Tablet-first, landscape preferred for flight experience
            // Allow both landscape directions, disable portrait
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // Performance / behavior
            PlayerSettings.runInBackground = false;           // Save battery when user switches away
            PlayerSettings.use32BitDisplayBuffer = true;      // Better for URP on mobile
            PlayerSettings.stripUnusedMeshComponents = true;  // Reduce build size

            // iOS specific
            PlayerSettings.iOS.statusBarStyle = iOSStatusBarStyle.Default;
            PlayerSettings.iOS.hideHomeButton = false;        // Keep for modern iPads
            PlayerSettings.iOS.requiresPersistentWiFi = false;

            // Android specific (reasonable modern baseline)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; // Android 7.0+
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Modern target

            // Splash / presentation (we have custom splash in FoldingDemoBootstrap)
            PlayerSettings.SplashScreen.show = false; // We control our own

            Debug.Log("[BuildSettingsHelper] PlayerSettings updated for mobile/tablet (landscape auto-rotate, no background run, modern SDK targets).");
        }
    }
}