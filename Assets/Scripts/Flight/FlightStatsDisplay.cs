using UnityEngine;
using UnityEngine.UI;

namespace PaperWings.Flight
{
    /// <summary>
    /// Simple performance stats display for the flight demo.
    /// Shows altitude, distance flown, and flight time.
    /// </summary>
    public class FlightStatsDisplay : MonoBehaviour
    {
        [Header("UI References")]
        public Text altitudeText;
        public Text distanceText;
        public Text timeText;

        private Transform plane;
        private Rigidbody rb;
        private Vector3 startPosition;
        private float startTime;
        private float bestTime = float.MaxValue;
        private const string BestTimeKey = "PaperWings_BestFlightTime";

        // Expose current flight results for progression system
        public float CurrentDistance { get; private set; }
        public float CurrentFlightTime { get; private set; }
        public float MaxAltitude { get; private set; }

        private void Start()
        {
            // Load best time from previous sessions
            bestTime = PlayerPrefs.GetFloat(BestTimeKey, float.MaxValue);

            // Find the plane in the scene
            var physics = FindObjectOfType<PaperAirplanePhysics>();
            if (physics != null)
            {
                plane = physics.transform;
                rb = physics.GetComponent<Rigidbody>();
                startPosition = plane.position;
                startTime = Time.time;
                MaxAltitude = plane.position.y;
            }

            // Create UI if not assigned
            if (altitudeText == null || distanceText == null || timeText == null)
            {
                CreateSimpleStatsUI();
            }
        }

        private void Update()
        {
            if (plane == null) return;

            // Altitude
            if (altitudeText != null)
            {
                float alt = Mathf.Max(0, plane.position.y);
                MaxAltitude = Mathf.Max(MaxAltitude, alt);
                altitudeText.text = $"Alt: {alt:F1}m";
            }

            // Distance
            if (distanceText != null)
            {
                float dist = Vector3.Distance(plane.position, startPosition);
                CurrentDistance = dist;
                distanceText.text = $"Dist: {dist:F0}m";
            }

            // Flight Time + Best Time
            if (timeText != null)
            {
                float elapsed = Time.time - startTime;
                CurrentFlightTime = elapsed;
                string timeStr = $"Time: {elapsed:F1}s";

                if (bestTime < float.MaxValue)
                {
                    timeStr += $"\nBest: {bestTime:F1}s";
                }
                timeText.text = timeStr;

                // Save new best time when landing or on demand (for demo we save on destroy)
                if (elapsed > 3f && elapsed < bestTime)
                {
                    bestTime = elapsed;
                    PlayerPrefs.SetFloat(BestTimeKey, bestTime);
                }
            }
        }

        private void CreateSimpleStatsUI()
        {
            GameObject canvasGO = new GameObject("StatsCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            // Create three text elements
            altitudeText = CreateStatText(canvas.transform, "Altitude", new Vector2(20, -20));
            distanceText = CreateStatText(canvas.transform, "Distance", new Vector2(20, -50));
            timeText = CreateStatText(canvas.transform, "Time", new Vector2(20, -80));
        }

        private Text CreateStatText(Transform parent, string label, Vector2 anchoredPos)
        {
            GameObject go = new GameObject(label);
            go.transform.SetParent(parent);

            var txt = go.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 20;
            txt.color = Color.white;
            txt.text = label + ": 0";

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(220, 28);

            return txt;
        }

        private void OnDestroy()
        {
            // Persist best time across sessions
            if (bestTime < float.MaxValue)
            {
                PlayerPrefs.SetFloat(BestTimeKey, bestTime);
                PlayerPrefs.Save();
            }
        }
    }
}