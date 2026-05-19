using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

namespace PaperWings.Backend
{
    /// <summary>
    /// Foundation Supabase Auth client (email + anonymous).
    /// Uses REST API + UnityWebRequest so it works on all Unity platforms.
    /// 
    /// Current state: Basic anonymous login + session storage.
    /// Email signup/login and proper token refresh will be added next.
    /// </summary>
    public class SupabaseAuth : MonoBehaviour
    {
        public static SupabaseAuth Instance { get; private set; }

        [Header("Config")]
        public SupabaseConfig config;

        public string CurrentUserId { get; private set; }
        public string AccessToken { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        public event Action OnSignedIn;
        public event Action OnSignedOut;

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

        private void Start()
        {
            // Try to restore previous anonymous session
            LoadSession();
        }

        public void SignInAnonymously()
        {
            StartCoroutine(SignInAnonymouslyRoutine());
        }

        private IEnumerator SignInAnonymouslyRoutine()
        {
            if (config == null || string.IsNullOrEmpty(config.anonKey))
            {
                Debug.LogError("[SupabaseAuth] SupabaseConfig is missing or incomplete.");
                yield break;
            }

            string url = $"{config.AuthUrl}/signup";

            var payload = new
            {
                data = new { is_anonymous = true }
            };

            string json = JsonUtility.ToJson(payload);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("apikey", config.anonKey);
                request.SetRequestHeader("Authorization", $"Bearer {config.anonKey}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[SupabaseAuth] Anonymous signup failed: {request.error}\n{request.downloadHandler.text}");
                    yield break;
                }

                // Parse response for access_token and user id (simple string parsing for foundation)
                string responseText = request.downloadHandler.text;
                ParseSessionFromResponse(responseText);

                Debug.Log("[SupabaseAuth] Anonymous login successful. User ID: " + CurrentUserId);
                SaveSession();
                OnSignedIn?.Invoke();
            }
        }

        public void SignOut()
        {
            CurrentUserId = null;
            AccessToken = null;
            PlayerPrefs.DeleteKey("SupabaseUserId");
            PlayerPrefs.DeleteKey("SupabaseAccessToken");
            PlayerPrefs.Save();

            Debug.Log("[SupabaseAuth] Signed out.");
            OnSignedOut?.Invoke();
        }

        private void ParseSessionFromResponse(string json)
        {
            // Use proper DTO + JsonUtility (no external JSON library)
            try
            {
                var session = JsonUtility.FromJson<SupabaseSessionResponse>(json);
                if (session != null)
                {
                    AccessToken = session.access_token;
                    if (session.user != null)
                    {
                        CurrentUserId = session.user.id;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SupabaseAuth] Failed to parse session response cleanly: " + e.Message);
            }
        }

        [Serializable]
        private class SupabaseSessionResponse
        {
            public string access_token;
            public SupabaseUser user;
        }

        [Serializable]
        private class SupabaseUser
        {
            public string id;
        }

        private void SaveSession()
        {
            if (!string.IsNullOrEmpty(CurrentUserId))
                PlayerPrefs.SetString("SupabaseUserId", CurrentUserId);
            if (!string.IsNullOrEmpty(AccessToken))
                PlayerPrefs.SetString("SupabaseAccessToken", AccessToken);
            PlayerPrefs.Save();
        }

        private void LoadSession()
        {
            CurrentUserId = PlayerPrefs.GetString("SupabaseUserId", null);
            AccessToken = PlayerPrefs.GetString("SupabaseAccessToken", null);

            if (IsAuthenticated)
            {
                Debug.Log("[SupabaseAuth] Restored session for user: " + CurrentUserId);
                OnSignedIn?.Invoke();
            }
        }
    }
}