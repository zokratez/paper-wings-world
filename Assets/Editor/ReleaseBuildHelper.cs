using UnityEditor;
using UnityEngine;

namespace PaperWings.Editor
{
    /// <summary>
    /// One-click helper to prepare a true production release build configuration.
    /// 
    /// Turns off all development / debugging features and ensures the build is
    /// optimized for App Store / Google Play submission:
    /// - Development Build = OFF
    /// - Profiler / Connect Profiler = OFF
    /// - Scripts Only = OFF
    /// - IL2CPP (release performance and 64-bit)
    /// 
    /// Run this right before clicking "Build" in Build Settings for any candidate
    /// that will go to TestFlight or Play Internal/Production track.
    /// </summary>
    public static class ReleaseBuildHelper
    {
        [MenuItem("Paper Wings / HIGH INTENSITY - Prepare Release Build")]
        public static void PrepareReleaseBuild()
        {
            Debug.Log("[ReleaseBuildHelper] Preparing production release build configuration...");

            // Core release flags
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildScriptsOnly = false;
            EditorUserBuildSettings.allowDebugging = false;

            // Ensure release scripting backend on current target (IL2CPP recommended)
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android)
            {
                if (PlayerSettings.GetScriptingBackend(targetGroup) != ScriptingImplementation.IL2CPP)
                {
                    PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
                    Debug.Log($"[ReleaseBuildHelper] Forced IL2CPP scripting backend for {targetGroup}");
                }
            }

            // Strip unused data for smaller, faster builds
            PlayerSettings.stripUnusedMeshComponents = true;

            // Make sure we are not showing Unity splash in final release (we have custom)
            PlayerSettings.SplashScreen.show = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ReleaseBuildHelper] ✅ Release build settings applied:");
            Debug.Log("   • Development Build = OFF");
            Debug.Log("   • Profiler / Debugging = OFF");
            Debug.Log("   • IL2CPP enforced where applicable");
            Debug.Log("   • Ready for clean production build.");
            Debug.LogWarning("[ReleaseBuildHelper] IMPORTANT: After running this, open Build Settings, select your platform (iOS or Android), and click Build. Do NOT use 'Build and Run' for store candidates.");
            Debug.Log("[ReleaseBuildHelper] Tip: Run 'Prepare for Device Build (iOS + Android)' first if you haven't set bundle ID / version yet.");
        }
    }
}