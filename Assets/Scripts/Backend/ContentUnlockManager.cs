using UnityEngine;
using System;
using System.Collections.Generic;
using PaperWings.Flight;
using PaperWings.Folding.Data;

namespace PaperWings.Backend
{
    /// <summary>
    /// Central authority for freemium content gating (Phase 5 foundation).
    /// 
    /// Rules (current):
    /// - A plane is unlocked if:
    ///     1. PaperPlaneDefinition.isFree == true, OR
    ///     2. It has been purchased (via IAP / RevenueCat in future)
    /// 
    /// - A region is unlocked if:
    ///     1. FlightRegion.isFree == true, OR
    ///     2. It was unlocked via in-game milestones (FlightProgress), OR
    ///     3. It has been purchased
    /// 
    /// This manager is the single place the UI and selection screens should ask:
    ///     ContentUnlockManager.IsPlaneUnlocked(planeDefinition)
    ///     ContentUnlockManager.IsRegionUnlocked(region)
    /// 
    /// Later: integrate with PurchaseManager + Supabase entitlements.
    /// </summary>
    public static class ContentUnlockManager
    {
        /// <summary>
        /// Raised when a purchase or cloud sync grants new content.
        /// UI can refresh selection screens.
        /// </summary>
        public static event Action OnContentUnlocked;

        // Simple local "purchased" cache for demo / testing (later replaced by RevenueCat + cloud)
        private static HashSet<string> purchasedProductIds = new HashSet<string>();

        static ContentUnlockManager()
        {
            LoadPurchasedFromPrefs();
        }

        // ============================================================
        // Plane Unlocks
        // ============================================================

        public static bool IsPlaneUnlocked(PaperPlaneDefinition def)
        {
            if (def == null) return false;
            if (def.isFree) return true;

            // Check individual product
            if (!string.IsNullOrEmpty(def.unlockProductId) && HasProduct(def.unlockProductId))
                return true;

            // Full content pack unlocks everything
            if (HasFullContentAccess())
                return true;

            return false;
        }

        // ============================================================
        // Region Unlocks
        // ============================================================

        public static bool IsRegionUnlocked(FlightRegion region)
        {
            if (region == null) return false;
            if (region.isFree) return true;
            if (FlightProgress.IsRegionUnlocked(region.regionId)) return true;

            if (!string.IsNullOrEmpty(region.unlockProductId) && HasProduct(region.unlockProductId))
                return true;

            if (HasFullContentAccess())
                return true;

            return false;
        }

        /// <summary>
        /// ID-based version for UI code that hasn't loaded the FlightRegion asset yet.
        /// Tries asset load for isFree/purchase check + always respects milestone unlocks in FlightProgress.
        /// </summary>
        public static bool IsRegionUnlocked(string regionId)
        {
            if (string.IsNullOrEmpty(regionId)) return false;

            // Milestone unlocks from gameplay always take precedence
            if (FlightProgress.IsRegionUnlocked(regionId)) return true;

            // Try to load the region definition to respect isFree + purchased product
            var region = Resources.Load<FlightRegion>($"FlightRegions/{regionId}");
            if (region != null)
            {
                if (region.isFree) return true;
                if (!string.IsNullOrEmpty(region.unlockProductId) && HasProduct(region.unlockProductId))
                    return true;
            }

            if (HasFullContentAccess())
                return true;

            return false;
        }

        // ============================================================
        // Purchase Integration Points (Foundation)
        // ============================================================

        /// <summary>
        /// Call this after a successful IAP purchase (or after restoring from RevenueCat).
        /// </summary>
        public static void GrantPurchase(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return;

            purchasedProductIds.Add(productId);
            SavePurchasedToPrefs();

            Debug.Log($"[ContentUnlockManager] Purchase granted: {productId}");
            OnContentUnlocked?.Invoke();
        }

        /// <summary>
        /// For testing / debug in editor or early builds.
        /// </summary>
        public static void GrantAllForDebug()
        {
            // In a real build this would be removed or behind a dev flag
            purchasedProductIds.Add("full_library");
            purchasedProductIds.Add("all_regions");
            SavePurchasedToPrefs();
            OnContentUnlocked?.Invoke();
            Debug.LogWarning("[ContentUnlockManager] DEBUG: All content unlocked locally.");
        }

        public static void ClearAllPurchasesForDebug()
        {
            purchasedProductIds.Clear();
            PlayerPrefs.DeleteKey("PurchasedProductIds");
            PlayerPrefs.Save();
            OnContentUnlocked?.Invoke();
        }

        // ============================================================
        // Persistence for local purchased state (temporary until cloud entitlements)
        // ============================================================

        private static void SavePurchasedToPrefs()
        {
            string joined = string.Join(",", purchasedProductIds);
            PlayerPrefs.SetString("PurchasedProductIds", joined);
            PlayerPrefs.Save();
        }

        private static void LoadPurchasedFromPrefs()
        {
            purchasedProductIds.Clear();
            string saved = PlayerPrefs.GetString("PurchasedProductIds", "");
            if (!string.IsNullOrEmpty(saved))
            {
                foreach (var id in saved.Split(','))
                {
                    if (!string.IsNullOrEmpty(id))
                        purchasedProductIds.Add(id);
                }
            }
        }

        // ============================================================
        // PurchaseManager Integration (Phase 5 IAP foundation)
        // ============================================================

        /// <summary>
        /// Returns true if the given product has been purchased (via PurchaseManager or legacy cache).
        /// </summary>
        public static bool HasProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return false;

            PurchaseManager.EnsureExists();

            // Prefer the real PurchaseManager when present
            if (PurchaseManager.Instance != null && PurchaseManager.Instance.HasPurchased(productId))
                return true;

            return purchasedProductIds.Contains(productId);
        }

        /// <summary>
        /// Returns true if the user owns the full content pack (unlocks everything).
        /// </summary>
        public static bool HasFullContentAccess()
        {
            PurchaseManager.EnsureExists();

            if (PurchaseManager.Instance != null && PurchaseManager.Instance.HasFullContentAccess())
                return true;

            return purchasedProductIds.Contains(PurchaseManager.FullContentProductId);
        }
    }
}