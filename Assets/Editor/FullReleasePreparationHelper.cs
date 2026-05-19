using UnityEditor;
using UnityEngine;

namespace PaperWings.Editor
{
    /// <summary>
    /// Ultimate one-click helper for Phase 7 release preparation.
    /// 
    /// Executes the entire chain of previous release helpers in the correct order:
    /// 1. Prepare Build Settings for Mobile Testing
    /// 2. Prepare for Device Build (iOS + Android)
    /// 3. Prepare Release Build
    /// 4. Remove Dev Tools for Release
    /// 
    /// This is the recommended final step before producing any v1.0 candidate build.
    /// It ensures scenes are registered, all PlayerSettings are production-grade,
    /// Development Build is off, and the Phase 5 dev panel + debug buttons are stripped.
    /// 
    /// After running this, simply open Build Settings and hit Build.
    /// </summary>
    public static class FullReleasePreparationHelper
    {
        [MenuItem("Paper Wings / HIGH INTENSITY - Full Release Preparation")]
        public static void RunFullReleasePreparation()
        {
            Debug.Log("[FullReleasePreparationHelper] ============================================");
            Debug.Log("[FullReleasePreparationHelper] Starting FULL RELEASE PREPARATION CHAIN...");
            Debug.Log("[FullReleasePreparationHelper] This will run all previous helpers in sequence.");
            Debug.Log("[FullReleasePreparationHelper] ============================================");

            // Step 1
            Debug.Log("\n[FullReleasePreparationHelper] Step 1/4: Prepare Build Settings for Mobile Testing");
            BuildSettingsHelper.PrepareMobileBuildSettings();

            // Step 2
            Debug.Log("\n[FullReleasePreparationHelper] Step 2/4: Prepare for Device Build (iOS + Android)");
            DeviceBuildHelper.PrepareForDeviceBuild();

            // Step 3
            Debug.Log("\n[FullReleasePreparationHelper] Step 3/4: Prepare Release Build");
            ReleaseBuildHelper.PrepareReleaseBuild();

            // Step 4
            Debug.Log("\n[FullReleasePreparationHelper] Step 4/4: Remove Dev Tools for Release");
            RemoveDevToolsForReleaseHelper.RemoveDevToolsForRelease();

            // Final summary
            Debug.Log("\n[FullReleasePreparationHelper] ============================================");
            Debug.Log("[FullReleasePreparationHelper] ✅ FULL RELEASE PREPARATION COMPLETE!");
            Debug.Log("[FullReleasePreparationHelper] All four release helpers executed successfully.");
            Debug.Log("[FullReleasePreparationHelper] - Scenes added to Build Settings");
            Debug.Log("[FullReleasePreparationHelper] - Device/PlayerSettings configured (bundle, IL2CPP, ARM64, etc.)");
            Debug.Log("[FullReleasePreparationHelper] - Development Build = OFF, profiler/debug disabled");
            Debug.Log("[FullReleasePreparationHelper] - Phase 5 Dev Tools panel and buttons will be stripped (via #if guard)");
            Debug.LogWarning("[FullReleasePreparationHelper] NEXT ACTION: Open File > Build Settings, select your target platform (iOS or Android), and click Build.");
            Debug.Log("[FullReleasePreparationHelper] Then follow the iOS/Android export steps in notes/Roadmap/Phase-7-Release.md");
            Debug.Log("[FullReleasePreparationHelper] ============================================");
        }
    }
}