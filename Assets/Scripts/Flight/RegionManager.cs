using UnityEngine;
using System.Collections.Generic;

namespace PaperWings.Flight
{
    /// <summary>
    /// Runtime manager for flight regions.
    /// 
    /// Responsibilities:
    /// - Holds reference to the central FlightRegionLibrary
    /// - Provides easy lookup for regions at runtime
    /// - Central place to apply a region's settings to the current scene (visuals, physics, environment)
    /// 
    /// This keeps the region logic out of FlightDemoBootstrap and makes the system extensible.
    /// Add one to a scene (or bootstrap it) and assign the library asset.
    /// </summary>
    public class RegionManager : MonoBehaviour
    {
        public static RegionManager Instance { get; private set; }

        [Header("Data")]
        [Tooltip("The central library containing all available FlightRegion assets")]
        public FlightRegionLibrary regionLibrary;

        [Header("Current")]
        public FlightRegion CurrentRegion { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public FlightRegion GetRegionById(string id)
        {
            if (regionLibrary == null) return null;
            return regionLibrary.GetRegionById(id);
        }

        public List<FlightRegion> GetAllRegions()
        {
            if (regionLibrary == null) return new List<FlightRegion>();
            return regionLibrary.regions;
        }

        public FlightRegion GetDefaultRegion()
        {
            if (regionLibrary == null) return null;
            return regionLibrary.GetDefaultRegion();
        }

        /// <summary>
        /// Applies all settings from the given region to the active scene elements.
        /// This centralizes the "magic" of making a region feel unique.
        /// </summary>
        public void ApplyRegion(FlightRegion region, Camera mainCamera, PaperAirplanePhysics physics, FlightEnvironment environment = null)
        {
            if (region == null) return;

            CurrentRegion = region;

            // 1. Visuals (sky / fog / ambient)
            ApplyVisuals(region, mainCamera);

            // 2. Flight physics tuning
            if (physics != null)
            {
                physics.windDirection = region.baseWindDirection;
                physics.baseWindStrength = region.baseWindStrength;
                physics.thermalMultiplier = region.thermalStrengthMultiplier;
            }

            // 3. Environment (terrain style / props)
            if (environment != null)
            {
                environment.BuildEnvironment(region);
            }

            Debug.Log($"[RegionManager] Applied region: {region.displayName}");
        }

        private void ApplyVisuals(FlightRegion region, Camera cam)
        {
            if (region.skyboxMaterial != null)
            {
                RenderSettings.skybox = region.skyboxMaterial;
            }

            RenderSettings.ambientLight = region.ambientLightColor;
            RenderSettings.ambientIntensity = region.ambientIntensity;

            RenderSettings.fog = true;
            RenderSettings.fogColor = region.fogColor;
            RenderSettings.fogDensity = region.fogDensity;
            RenderSettings.fogMode = FogMode.Exponential;

            if (cam != null)
            {
                cam.backgroundColor = region.fogColor;
            }
        }

        public void ClearCurrentRegion()
        {
            CurrentRegion = null;
        }
    }
}