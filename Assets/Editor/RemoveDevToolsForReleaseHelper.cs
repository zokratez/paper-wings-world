using UnityEditor;
using UnityEngine;

namespace PaperWings.Editor
{
    /// <summary>
    /// One-click helper to guarantee the Phase 5 dev tools panel and its debug buttons
    /// are completely removed from release builds.
    /// 
    /// Works together with the #if UNITY_EDITOR || DEVELOPMENT_BUILD guard in
    /// FoldingTutorialManager.ShowMainMenu().
    /// 
    /// When you run this helper + the other release preparation steps and then build
    /// with Development Build turned OFF, the entire dev panel (anonymous sign-in,
    /// manual cloud load/save buttons, etc.) is stripped from the final player binary.
    /// 
    /// Run this as the final preparation step before a production build candidate.
    /// </summary>
    public static class RemoveDevToolsForReleaseHelper
    {
        [MenuItem("Paper Wings / HIGH INTENSITY - Remove Dev Tools for Release")]
        public static void RemoveDevToolsForRelease()
        {
            Debug.Log("[RemoveDevToolsForReleaseHelper] Removing dev tools for production release build...");

            // Ensure we are building a non-development release
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.allowDebugging = false;

            // Re-apply release scripting backend for good measure (iOS/Android)
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (group == BuildTargetGroup.iOS || group == BuildTargetGroup.Android)
            {
                PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[RemoveDevToolsForReleaseHelper] ✅ Dev tools removal configured.");
            Debug.Log("   • Development Build = false (DEVELOPMENT_BUILD define will be absent)");
            Debug.Log("   • The Phase 5 Dev Tools panel + all its debug buttons are now");
            Debug.Log("     excluded from the player via the #if guard in FoldingTutorialManager.cs");
            Debug.LogWarning("[RemoveDevToolsForReleaseHelper] IMPORTANT: After this, run the full release chain:");
            Debug.LogWarning("   1. Prepare for Device Build (iOS + Android)");
            Debug.LogWarning("   2. Prepare Release Build");
            Debug.LogWarning("   3. Remove Dev Tools for Release");
            Debug.LogWarning("   Then open Build Settings and produce your final candidate build.");
            Debug.Log("[RemoveDevToolsForReleaseHelper] The dev panel will never appear to end users in this build.");
        }
    }
}