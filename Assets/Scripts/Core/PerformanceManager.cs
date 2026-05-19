using UnityEngine;

namespace PaperWings.Core
{
    /// <summary>
    /// Centralized performance manager for Phase 6.
    /// Ensures consistent 60 FPS target, quality settings, and device-tier optimizations
    /// across all entry points (bootstraps and transitions).
    /// 
    /// Call EnsurePerformanceSettings() early in scene bootstraps.
    /// Low-end devices get reduced shadows, quality level, and particle rates (via FlightEffects).
    /// </summary>
    public static class PerformanceManager
    {
        public static bool IsLowEndDevice { get; private set; }

        /// <summary>
        /// Call this once early (e.g. in Awake/Start of persistent or bootstrap objects)
        /// to apply all performance settings. Safe to call multiple times.
        /// </summary>
        public static void EnsurePerformanceSettings()
        {
            if (IsLowEndDevice) return; // already initialized this session

            IsLowEndDevice = SystemInfo.systemMemorySize < 3000 
                || SystemInfo.processorCount <= 4 
                || SystemInfo.graphicsMemorySize < 512;

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            if (IsLowEndDevice)
            {
                // Aggressive but safe for low-end mobile/tablets
                QualitySettings.SetQualityLevel(1, true);
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowDistance = 20f;
                QualitySettings.shadowCascades = 1;
            }
            else if (QualitySettings.GetQualityLevel() > 3)
            {
                // Balanced high quality for modern tablets
                QualitySettings.SetQualityLevel(3, true);
            }
        }
    }
}