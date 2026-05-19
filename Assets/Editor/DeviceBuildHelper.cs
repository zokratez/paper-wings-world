using UnityEditor;
using UnityEngine;

namespace PaperWings.Editor
{
    /// <summary>
    /// One-click helper to prepare a production-ready build for real devices and store submission.
    /// 
    /// Configures the critical PlayerSettings required for App Store and Google Play:
    /// - Bundle identifiers (MUST be customized to match your App Store Connect / Play Console records)
    /// - Version and build number
    /// - Company / Product names
    /// - IL2CPP scripting backend (better performance and 64-bit compliance)
    /// - iOS minimum version, Android SDK targets and architectures
    /// 
    /// Run this immediately before every TestFlight, internal track, or final submission build.
    /// </summary>
    public static class DeviceBuildHelper
    {
        // ============================================================
        // CUSTOMIZE THESE VALUES BEFORE ANY REAL SUBMISSION
        // ============================================================
        private const string COMPANY_NAME = "ooabisabi LLC";
        private const string PRODUCT_NAME = "Paper Wings World";
        private const string VERSION = "1.0.0";
        private const int BUILD_NUMBER = 1;

        // Replace with your actual registered bundle identifier
        private const string BUNDLE_ID_IOS = "com.ooabisabi.paperwingsworld";
        private const string BUNDLE_ID_ANDROID = "com.ooabisabi.paperwingsworld";

        // Minimum supported OS versions (adjust after device testing matrix)
        private const string MIN_IOS_VERSION = "15.0";
        private const int MIN_ANDROID_SDK = 24;      // Android 7.0
        private const int TARGET_ANDROID_SDK = 34;   // Modern target (update as Google requires)

        [MenuItem("Paper Wings / HIGH INTENSITY - Prepare for Device Build (iOS + Android)")]
        public static void PrepareForDeviceBuild()
        {
            Debug.Log("[DeviceBuildHelper] Starting production device build preparation...");

            // Core identity (critical for store records)
            ApplyIdentityAndBundleSettings();

            // Versioning
            ApplyVersionSettings();

            // iOS production settings
            ApplyIOSSettings();

            // Android production settings (64-bit, modern SDKs, IL2CPP)
            ApplyAndroidSettings();

            // Shared release-friendly settings
            ApplySharedReleaseSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[DeviceBuildHelper] ✅ Device build settings applied for iOS + Android.");
            Debug.LogWarning("[DeviceBuildHelper] IMPORTANT: Verify and customize the BUNDLE_ID constants in this file before submitting to the stores. They must exactly match your App Store Connect and Google Play Console records.");
            Debug.Log("[DeviceBuildHelper] Reminder: Also run the scene setup tools if needed, then build via File > Build Settings or your CI pipeline.");
        }

        private static void ApplyIdentityAndBundleSettings()
        {
            PlayerSettings.companyName = COMPANY_NAME;
            PlayerSettings.productName = PRODUCT_NAME;

            // iOS bundle
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, BUNDLE_ID_IOS);

            // Android bundle (package name)
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, BUNDLE_ID_ANDROID);

            Debug.Log($"[DeviceBuildHelper] Identity set: {COMPANY_NAME} / {PRODUCT_NAME}");
            Debug.Log($"[DeviceBuildHelper] Bundle IDs → iOS: {BUNDLE_ID_IOS} | Android: {BUNDLE_ID_ANDROID}");
        }

        private static void ApplyVersionSettings()
        {
            PlayerSettings.bundleVersion = VERSION;

            // iOS build number
            PlayerSettings.iOS.buildNumber = BUILD_NUMBER.ToString();

            // Android version code (increment for every submission)
            PlayerSettings.Android.bundleVersionCode = BUILD_NUMBER;

            Debug.Log($"[DeviceBuildHelper] Version: {VERSION} (build {BUILD_NUMBER})");
        }

        private static void ApplyIOSSettings()
        {
            // Minimum iOS version (matches modern device support)
            PlayerSettings.iOS.targetOSVersionString = MIN_IOS_VERSION;

            // Architecture and build settings for release
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.appleEnableAutomaticSigning = false; // Manual or CI signing recommended for production

            // Visual / presentation (consistent with our custom splash)
            PlayerSettings.iOS.statusBarStyle = iOSStatusBarStyle.Default;
            PlayerSettings.SplashScreen.show = false;

            Debug.Log($"[DeviceBuildHelper] iOS: IL2CPP + min iOS {MIN_IOS_VERSION}");
        }

        private static void ApplyAndroidSettings()
        {
            // SDK versions
            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)MIN_ANDROID_SDK;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)TARGET_ANDROID_SDK;

            // 64-bit only (Google requirement since 2021)
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Scripting backend for performance and 64-bit compliance
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            // Release-friendly options
            PlayerSettings.Android.useCustomKeystore = false; // Set to true + configure when you have a real keystore
            PlayerSettings.Android.buildApkPerCpuArchitecture = false; // Single AAB is preferred

            Debug.Log($"[DeviceBuildHelper] Android: IL2CPP + ARM64 + SDK {MIN_ANDROID_SDK}–{TARGET_ANDROID_SDK}");
        }

        private static void ApplySharedReleaseSettings()
        {
            // Performance & size
            PlayerSettings.stripUnusedMeshComponents = true;
            PlayerSettings.use32BitDisplayBuffer = true;

            // Orientation (tablet-first, landscape preferred)
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // Battery / background behavior
            PlayerSettings.runInBackground = false;

            Debug.Log("[DeviceBuildHelper] Shared release settings applied (IL2CPP, orientation, stripping, no background run).");
        }
    }
}