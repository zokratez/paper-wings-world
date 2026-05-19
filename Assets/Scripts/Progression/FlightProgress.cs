using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PaperWings.Progression
{
    /// <summary>
    /// Simple, self-contained local progress system for Phase 3.
    /// 
    /// Tracks:
    /// - Best distance and glide time per (planeId + regionId) combination
    /// - Set of unlocked region IDs
    /// 
    /// Persists to JSON in persistentDataPath (works on device and editor).
    /// 
    /// Designed to be extended later with more planes/regions, cloud sync, etc.
    /// </summary>
    public static class FlightProgress
    {
        [Serializable]
        private class FlightRecord
        {
            public float bestDistance;
            public float bestGlideTime;
        }

        [Serializable]
        private class SaveData
        {
            public List<string> unlockedRegions = new List<string>();
            public Dictionary<string, FlightRecord> bestFlights = new Dictionary<string, FlightRecord>();
        }

        private static SaveData data;
        private static string SavePath => Path.Combine(Application.persistentDataPath, "flight_progress.json");

        private const string GrandCanyon = "grand_canyon";
        private const string Fuji = "fuji_foothills";
        private const string Norwegian = "norwegian_coast";

        static FlightProgress()
        {
            Load();
        }

        public static void Load()
        {
            data = new SaveData();

            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    var loaded = JsonUtility.FromJson<SaveData>(json);
                    if (loaded != null)
                    {
                        data = loaded;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[FlightProgress] Failed to load progress: {e.Message}. Starting fresh.");
                }
            }

            // Ensure Grand Canyon is always unlocked (the default starter region)
            if (!data.unlockedRegions.Contains(GrandCanyon))
            {
                data.unlockedRegions.Add(GrandCanyon);
            }
        }

        public static void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[FlightProgress] Failed to save progress: {e.Message}");
            }
        }

        /// <summary>
        /// Returns true if the region is currently unlocked for the player.
        /// </summary>
        public static bool IsRegionUnlocked(string regionId)
        {
            if (string.IsNullOrEmpty(regionId)) return false;
            return data.unlockedRegions.Contains(regionId);
        }

        public static List<string> GetUnlockedRegions()
        {
            return new List<string>(data.unlockedRegions);
        }

        /// <summary>
        /// Records a completed flight. Updates personal bests if improved.
        /// Checks and applies unlock milestones.
        /// </summary>
        public static void RecordFlight(string planeId, string regionId, float distance, float glideTime)
        {
            if (string.IsNullOrEmpty(planeId) || string.IsNullOrEmpty(regionId)) return;
            if (distance <= 0 && glideTime <= 0) return;

            string key = $"{planeId}_{regionId}";

            if (!data.bestFlights.ContainsKey(key))
            {
                data.bestFlights[key] = new FlightRecord();
            }

            var record = data.bestFlights[key];

            bool improved = false;

            if (distance > record.bestDistance)
            {
                record.bestDistance = distance;
                improved = true;
            }

            if (glideTime > record.bestGlideTime)
            {
                record.bestGlideTime = glideTime;
                improved = true;
            }

            // Apply unlock rules (simple but effective for v1)
            TryUnlockNextRegions(regionId, distance);

            if (improved)
            {
                Save();
                Debug.Log($"[FlightProgress] New best for {planeId} in {regionId}: {record.bestDistance:F0}m / {record.bestGlideTime:F1}s");
            }
        }

        private static void TryUnlockNextRegions(string justFlownRegion, float distanceFlown)
        {
            // Example milestone: 500m in Grand Canyon unlocks Fuji Foothills
            if (justFlownRegion == GrandCanyon && distanceFlown >= 500f)
            {
                if (!data.unlockedRegions.Contains(Fuji))
                {
                    data.unlockedRegions.Add(Fuji);
                    Debug.Log("[FlightProgress] Fuji Foothills unlocked! (500m+ in Grand Canyon)");
                }
            }

            // Example: 600m in Fuji unlocks Norwegian Coast
            if (justFlownRegion == Fuji && distanceFlown >= 600f)
            {
                if (!data.unlockedRegions.Contains(Norwegian))
                {
                    data.unlockedRegions.Add(Norwegian);
                    Debug.Log("[FlightProgress] Norwegian Coast unlocked!");
                }
            }

            // Could add more rules here (e.g. total flights, specific planes, etc.)
        }

        /// <summary>
        /// Gets the player's personal best for a specific plane + region combination.
        /// Returns 0 / 0 if no flight recorded yet.
        /// </summary>
        public static (float distance, float time) GetBest(string planeId, string regionId)
        {
            string key = $"{planeId}_{regionId}";
            if (data.bestFlights.TryGetValue(key, out var record))
            {
                return (record.bestDistance, record.bestGlideTime);
            }
            return (0f, 0f);
        }

        /// <summary>
        /// Simple mastery tiers based on best distance achieved for a plane + region.
        /// Thresholds are tuned for the current v1.0 regions (easy to adjust).
        /// </summary>
        public enum MasteryTier
        {
            None,
            Bronze,
            Silver,
            Gold
        }

        public static MasteryTier GetMasteryTier(string planeId, string regionId)
        {
            var (dist, _) = GetBest(planeId, regionId);
            if (dist >= 950f) return MasteryTier.Gold;
            if (dist >= 650f) return MasteryTier.Silver;
            if (dist >= 400f) return MasteryTier.Bronze;
            return MasteryTier.None;
        }

        public static string GetBadgeEmoji(MasteryTier tier)
        {
            return tier switch
            {
                MasteryTier.Gold => "🥇",
                MasteryTier.Silver => "🥈",
                MasteryTier.Bronze => "🥉",
                _ => ""
            };
        }

        public static string GetBadgeLabel(MasteryTier tier)
        {
            return tier switch
            {
                MasteryTier.Gold => "Gold",
                MasteryTier.Silver => "Silver",
                MasteryTier.Bronze => "Bronze",
                _ => ""
            };
        }

        /// <summary>
        /// Debug helper - reset everything (useful during development)
        /// </summary>
        public static void ResetAllProgress()
        {
            data = new SaveData();
            data.unlockedRegions.Add(GrandCanyon);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            Debug.Log("[FlightProgress] All progress reset.");
        }
    }
}