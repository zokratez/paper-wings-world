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
    /// Supports: Anonymous login, Email Sign Up, Email Sign In.
    /// Anonymous → Email upgrade is supported by simply signing up with email while having an anonymous session (creates a permanent account).
    /// </summary>
    public class SupabaseAuth : MonoBehaviour
    {
        public static SupabaseAuth Instance { get; private set; }

        [Header("Config")]
        public SupabaseConfig config;

        public string CurrentUserId { get; private set; }
        public string AccessToken { get; private set; }
        public string CurrentEmail { get; private set; }
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

        public void SignUpWithEmail(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Debug.LogWarning("[SupabaseAuth] Email and password are required for sign up.");
                return;
            }
            StartCoroutine(SignUpWithEmailRoutine(email, password));
        }

        public void SignInWithEmail(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Debug.LogWarning("[SupabaseAuth] Email and password are required for sign in.");
                return;
            }
            StartCoroutine(SignInWithEmailRoutine(email, password));
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

        private IEnumerator SignUpWithEmailRoutine(string email, string password)
        {
            if (config == null || string.IsNullOrEmpty(config.anonKey))
            {
                Debug.LogError("[SupabaseAuth] SupabaseConfig is missing or incomplete.");
                yield break;
            }

            string url = $"{config.AuthUrl}/signup";

            var payload = new
            {
                email = email,
                password = password
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
                    Debug.LogError($"[SupabaseAuth] Email sign up failed: {request.error}\n{request.downloadHandler.text}");
                    yield break;
                }

                string responseText = request.downloadHandler.text;
                ParseSessionFromResponse(responseText);

                Debug.Log("[SupabaseAuth] Email sign up successful. User: " + CurrentEmail + " (ID: " + CurrentUserId + ")");
                SaveSession();
                OnSignedIn?.Invoke();
            }
        }

        private IEnumerator SignInWithEmailRoutine(string email, string password)
        {
            if (config == null || string.IsNullOrEmpty(config.anonKey))
            {
                Debug.LogError("[SupabaseAuth] SupabaseConfig is missing or incomplete.");
                yield break;
            }

            // Password grant flow for email sign in
            string url = $"{config.AuthUrl}/token?grant_type=password";

            var payload = new
            {
                email = email,
                password = password
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
                    Debug.LogError($"[SupabaseAuth] Email sign in failed: {request.error}\n{request.downloadHandler.text}");
                    yield break;
                }

                string responseText = request.downloadHandler.text;
                ParseSessionFromResponse(responseText);

                Debug.Log("[SupabaseAuth] Email sign in successful. User: " + CurrentEmail + " (ID: " + CurrentUserId + ")");
                SaveSession();
                OnSignedIn?.Invoke();
            }
        }

        public void SignOut()
        {
            CurrentUserId = null;
            AccessToken = null;
            CurrentEmail = null;
            PlayerPrefs.DeleteKey("SupabaseUserId");
            PlayerPrefs.DeleteKey("SupabaseAccessToken");
            PlayerPrefs.DeleteKey("SupabaseEmail");
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
                        CurrentEmail = session.user.email;
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
            public string email;
        }

        private void SaveSession()
        {
            if (!string.IsNullOrEmpty(CurrentUserId))
                PlayerPrefs.SetString("SupabaseUserId", CurrentUserId);
            if (!string.IsNullOrEmpty(AccessToken))
                PlayerPrefs.SetString("SupabaseAccessToken", AccessToken);
            if (!string.IsNullOrEmpty(CurrentEmail))
                PlayerPrefs.SetString("SupabaseEmail", CurrentEmail);
            PlayerPrefs.Save();
        }

        private void LoadSession()
        {
            CurrentUserId = PlayerPrefs.GetString("SupabaseUserId", null);
            AccessToken = PlayerPrefs.GetString("SupabaseAccessToken", null);
            CurrentEmail = PlayerPrefs.GetString("SupabaseEmail", null);

            if (IsAuthenticated)
            {
                Debug.Log("[SupabaseAuth] Restored session for user: " + CurrentUserId + (string.IsNullOrEmpty(CurrentEmail) ? " (anonymous)" : " (" + CurrentEmail + ")"));
                OnSignedIn?.Invoke();
            }
        }
    }
}