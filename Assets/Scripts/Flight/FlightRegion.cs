using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// Defines a distinct flight region with its own visual and environmental settings.
    /// Used for expanding the world in Phase 3+.
    /// </summary>
    [CreateAssetMenu(fileName = "NewFlightRegion", menuName = "Paper Wings / Flight Region")]
    public class FlightRegion : ScriptableObject
    {
        [Header("Identity")]
        public string regionId;
        public string displayName;

        [Header("Visuals")]
        public Material skyboxMaterial;
        public Color ambientLightColor = Color.white;
        public float ambientIntensity = 1f;
        public Color fogColor = new Color(0.7f, 0.8f, 0.9f);
        public float fogDensity = 0.008f;

        [Header("Flight Environment")]
        public Vector3 baseWindDirection = new Vector3(0.3f, 0.05f, 0.2f);
        public float baseWindStrength = 0.9f;
        public float thermalStrengthMultiplier = 1.0f;

        [Header("Starting Conditions")]
        public float defaultSpawnHeight = 18f;
        public Vector3 defaultSpawnOffset = Vector3.zero;

        [Header("Challenges (Future)")]
        public float distanceGoal = 500f;
        public float glideTimeGoal = 45f;
    }
}