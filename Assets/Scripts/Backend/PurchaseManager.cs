using UnityEngine;
using System;
using System.Collections.Generic;

namespace PaperWings.Backend
{
    /// <summary>
    /// Foundation PurchaseManager for RevenueCat / IAP (Phase 5).
    /// 
    /// Current implementation: Local simulation + PlayerPrefs persistence.
    /// This allows the UI and gating to be built and tested immediately.
    /// 
    /// When the real RevenueCat SDK is imported:
    /// - Replace the internal purchased set with RevenueCat's PurchaserInfo / Entitlements.
    /// - Call Purchases.PurchasePackage() in BuyProduct().
    /// - Listen to Purchases.OnPurchaseCompleted and Purchases.OnRestoreCompleted.
    /// - Call Purchases.RestorePurchases() in RestorePurchases().
    /// 
    /// The public API (BuyProduct, HasPurchased, events) stays stable.
    /// </summary>
    public class PurchaseManager : MonoBehaviour
    {
        public static PurchaseManager Instance { get; private set; }

        public static void EnsureExists()
        {
            if (Instance != null) return;

            var go = new GameObject("PurchaseManager (Phase 5 IAP)");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<PurchaseManager>();
        }

        // Example premium products for the foundation demo.
        public const string FullContentProductId = "full_content_pack";      // Unlocks all planes + regions
        public const string AllRegionsProductId = "all_regions_pack";        // Unlocks premium regions only

        // Display prices used in UI until we have a real product catalog.
        public const string FullContentDisplayPrice = "$4.99";
        public const string AllRegionsDisplayPrice = "$2.99";

        public event Action<string> OnPurchaseCompleted;
        public event Action<string> OnPurchaseFailed;
        public event Action OnRestoreCompleted;

        private HashSet<string> purchasedProductIds = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadPurchases();
        }

        /// <summary>
        /// Starts a purchase for the given RevenueCat product identifier.
        /// In simulation mode this immediately grants the product.
        /// </summary>
        public void BuyProduct(string productId)
        {
            EnsureExists(); // self-healing

            if (string.IsNullOrEmpty(productId))
            {
                OnPurchaseFailed?.Invoke("Invalid product");
                return;
            }

            if (HasPurchased(productId))
            {
                Debug.Log("[PurchaseManager] Product already owned: " + productId);
                OnPurchaseCompleted?.Invoke(productId);
                return;
            }

            // === SIMULATION (remove when real SDK is wired) ===
            Debug.LogWarning($"[PurchaseManager] SIMULATED PURCHASE of {productId} (no real money charged)");
            GrantPurchaseInternal(productId);
            OnPurchaseCompleted?.Invoke(productId);
        }

        public void RestorePurchases()
        {
            // In real implementation this would call the RevenueCat restore API.
            Debug.Log("[PurchaseManager] Restore requested (simulation: no change)");
            OnRestoreCompleted?.Invoke();
        }

        public bool HasPurchased(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return false;
            return purchasedProductIds.Contains(productId);
        }

        /// <summary>
        /// Returns true if the user has purchased the full content pack.
        /// Used by ContentUnlockManager to unlock everything.
        /// </summary>
        public bool HasFullContentAccess()
        {
            return HasPurchased(FullContentProductId);
        }

        /// <summary>
        /// Returns true if the user has purchased the All Regions pack.
        /// </summary>
        public bool HasAllRegionsAccess()
        {
            return HasPurchased(AllRegionsProductId);
        }

        /// <summary>
        /// Debug helper — grants a product locally without going through purchase flow.
        /// Useful during development.
        /// </summary>
        public void DebugGrantPurchase(string productId)
        {
            GrantPurchaseInternal(productId);
            OnPurchaseCompleted?.Invoke(productId);
            Debug.LogWarning("[PurchaseManager] DEBUG grant: " + productId);
        }

        private void GrantPurchaseInternal(string productId)
        {
            purchasedProductIds.Add(productId);
            SavePurchases();

            // Notify the central unlock manager so selection screens can refresh
            ContentUnlockManager.GrantPurchase(productId);
        }

        private void SavePurchases()
        {
            string joined = string.Join(",", purchasedProductIds);
            PlayerPrefs.SetString("PurchasedProductIds", joined);
            PlayerPrefs.Save();
        }

        private void LoadPurchases()
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
    }
}