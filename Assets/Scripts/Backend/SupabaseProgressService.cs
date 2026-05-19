using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using PaperWings.Progression;

namespace PaperWings.Backend
{
    /// <summary>
    /// Supabase Progress Sync Service (Foundation Phase 5).
    /// 
    /// Responsibilities:
    /// - Automatically push local FlightProgress changes to Supabase when authenticated
    /// - Provide LoadProgress() to fetch and merge cloud data on login
    /// 
    /// Uses REST API + UnityWebRequest (no external SDK).
    /// Designed to be resilient and easy to extend with conflict resolution.
    /// </summary>
    public class SupabaseProgressService : MonoBehaviour
    {
        public static SupabaseProgressService Instance { get; private set; }

        [Header("Config")]
        public SupabaseConfig config;

        private bool isSubscribed = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (!isSubscribed)
            {
                FlightProgress.OnProgressChanged += OnLocalProgressChanged;
                isSubscribed = true;
            }
        }

        private void OnDisable()
        {
            if (isSubscribed)
            {
                FlightProgress.OnProgressChanged -= OnLocalProgressChanged;
                isSubscribed = false;
            }
        }

        private void OnLocalProgressChanged()
        {
            if (SupabaseAuth.Instance != null && SupabaseAuth.Instance.IsAuthenticated)
            {
                SaveProgress();
            }
        }

        /// <summary>
        /// Pushes the current local progress to Supabase (upsert by user_id).
        /// Called automatically via the event or manually after login.
        /// </summary>
        public void SaveProgress()
        {
            if (!SupabaseAuth.Instance.IsAuthenticated || config == null)
            {
                return;
            }

            StartCoroutine(SaveProgressRoutine());
        }

        private IEnumerator SaveProgressRoutine()
        {
            string userId = SupabaseAuth.Instance.CurrentUserId;
            string token = SupabaseAuth.Instance.AccessToken;

            // Use the clean DTO from FlightProgress
            var dto = FlightProgress.ExportForCloud();
            string bestFlightsJson = FlightProgress.ExportForCloudAsJson();

            // Supabase expects the jsonb column to receive a JSON object.
            // We send the DTO directly under best_flights (as object) and array for regions.
            var payloadObj = new ProgressPayload
            {
                user_id = userId,
                best_flights = dto.bestFlights,   // will serialize as array of objects
                unlocked_regions = dto.unlockedRegions.ToArray()
            };

            string json = JsonUtility.ToJson(payloadObj);

            string url = $"{config.RestUrl}/flight_progress?on_conflict=user_id";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("apikey", config.anonKey);
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                request.SetRequestHeader("Prefer", "resolution=merge-duplicates,return=minimal");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("[SupabaseProgress] Progress synced to cloud for user " + userId);
                }
                else
                {
                    Debug.LogWarning($"[SupabaseProgress] Save failed: {request.error}\n{request.downloadHandler.text}");
                }
            }
        }

        /// <summary>
        /// Fetches the player's progress from Supabase and merges it into local data.
        /// Call this after successful login (especially when upgrading from anonymous).
        /// </summary>
        public void LoadProgress()
        {
            if (!SupabaseAuth.Instance.IsAuthenticated || config == null)
            {
                Debug.LogWarning("[SupabaseProgress] Cannot load — not authenticated.");
                return;
            }

            StartCoroutine(LoadProgressRoutine());
        }

        private IEnumerator LoadProgressRoutine()
        {
            string userId = SupabaseAuth.Instance.CurrentUserId;
            string token = SupabaseAuth.Instance.AccessToken;

            // Filter by user_id
            string url = $"{config.RestUrl}/flight_progress?user_id=eq.{userId}&select=best_flights,unlocked_regions";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("apikey", config.anonKey);
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[SupabaseProgress] Load failed: {request.error}");
                    yield break;
                }

                string response = request.downloadHandler.text;

                // Supabase returns an array. We expect 0 or 1 row.
                if (string.IsNullOrEmpty(response) || response == "[]")
                {
                    Debug.Log("[SupabaseProgress] No cloud progress yet — will create on first save.");
                    yield break;
                }

                try
                {
                    // Lightweight parse: expect [{"best_flights":[...], "unlocked_regions":[...]}]
                    // For foundation we use a small wrapper row class.
                    var rows = JsonUtility.FromJson<ProgressRowArrayWrapper>("{\"rows\":" + response + "}");
                    if (rows != null && rows.rows != null && rows.rows.Length > 0)
                    {
                        var row = rows.rows[0];

                        var dto = new FlightProgress.CloudProgressDto
                        {
                            unlockedRegions = row.unlocked_regions != null ? new System.Collections.Generic.List<string>(row.unlocked_regions) : new System.Collections.Generic.List<string>(),
                            bestFlights = new System.Collections.Generic.List<FlightProgress.CloudFlightRecord>()
                        };

                        if (row.best_flights != null)
                        {
                            foreach (var bf in row.best_flights)
                            {
                                dto.bestFlights.Add(new FlightProgress.CloudFlightRecord
                                {
                                    key = bf.key,
                                    bestDistance = bf.bestDistance,
                                    bestGlideTime = bf.bestGlideTime
                                });
                            }
                        }

                        FlightProgress.ImportFromCloud(dto);
                        Debug.Log("[SupabaseProgress] Cloud progress loaded and merged.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SupabaseProgress] Failed to parse cloud progress: {ex.Message}");
                }
            }
        }

        // Small serializable helpers for JsonUtility (no Newtonsoft dependency)
        [Serializable]
        private class ProgressPayload
        {
            public string user_id;
            public FlightProgress.CloudFlightRecord[] best_flights;
            public string[] unlocked_regions;
        }

        [Serializable]
        private class ProgressRow
        {
            public FlightProgress.CloudFlightRecord[] best_flights;
            public string[] unlocked_regions;
        }

        [Serializable]
        private class ProgressRowArrayWrapper
        {
            public ProgressRow[] rows;
        }
    }
}