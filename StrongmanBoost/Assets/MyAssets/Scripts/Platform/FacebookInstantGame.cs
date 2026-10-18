#if FB_INSTANT && UNITY_WEBGL
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Meta.InstantGames;
using UnityEngine;

namespace StrongmanBoost.Platform
{
    // FB 生命周期由网页模板负责，Unity 只接入已启动的 SDK。
    public sealed class FacebookInstantGame : MonoBehaviour
    {
        // 网页先读取外部 JSON；二进制不固化 App ID 或广告位。
        [DllImport("__Internal")] private static extern string StrongmanBoost_ConfigValue(string key);
        private static string ConfigValue(string key) {
#if UNITY_EDITOR
            return "";
#else
            return StrongmanBoost_ConfigValue(key);
#endif
        }
        public bool IsShowing => showing;
        public static string AppId => ConfigValue("appId");
        public static string InterstitialId => ConfigValue("interstitialPlacementId");
        public static string RewardedId => ConfigValue("rewardedPlacementId");
        public static FacebookInstantGame Instance { get; private set; }
        public bool RewardReady => rewarded != null && !showing;
        public event Action RewardStateChanged;
        private AdInstance interstitial, rewarded;
        private bool loadingInterstitial, loadingRewarded, showing, ready;
        private string supportedApis = "";
        private float nextInterstitial, nextRewarded, lastInterstitial = -60f;
        private bool audioPausedBeforeAd;
        private float timeScaleBeforeAd;

        [DllImport("__Internal")] private static extern void StrongmanBoost_RegisterLifecycle(string objectName);
        [DllImport("__Internal")] private static extern int StrongmanBoost_PlatformReady();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Instance = new GameObject("FacebookInstantGame").AddComponent<FacebookInstantGame>();
            DontDestroyOnLoad(Instance.gameObject);
        }

        private async void Start()
        {
#if !UNITY_EDITOR
            try
            {
                float deadline = Time.realtimeSinceStartup + 60f;
                while (StrongmanBoost_PlatformReady() == 0)
                {
                    if (Time.realtimeSinceStartup > deadline) throw new TimeoutException("Platform start timed out");
                    await Task.Delay(100);
                }
                StrongmanBoost_RegisterLifecycle(gameObject.name);
                supportedApis = await FBInstant.GetSupportedAPIs();
                ready = true;
                Preload(false);
                Preload(true);
            }
            catch (Exception error) { Debug.LogWarning("[FBInstant] Startup: " + error.Message); }
#endif
        }

        private bool Supports(string api) => supportedApis.Contains("\"" + api + "\"");

        private void Update()
        {
            if (!ready || showing) return;
            if (interstitial == null && !loadingInterstitial && Time.realtimeSinceStartup >= nextInterstitial) Preload(false);
            if (rewarded == null && !loadingRewarded && Time.realtimeSinceStartup >= nextRewarded) Preload(true);
        }

        private async void Preload(bool reward)
        {
            if (!Supports(reward ? "getRewardedVideoAsync" : "getInterstitialAdAsync")) return;
            if (reward) loadingRewarded = true; else loadingInterstitial = true;
            try
            {
                var ad = reward ? await FBInstant.GetRewardedVideoAsync(RewardedId) : await FBInstant.GetInterstitialAdAsync(InterstitialId);
                await ad.LoadAsync();
                if (reward) rewarded = ad; else interstitial = ad;
            }
            catch (Exception error) { Debug.LogWarning("[FBInstant] Preload: " + error.Message); }
            finally
            {
                // 无填充时延迟重试，避免每帧请求和按钮卡死。
                if (reward) { loadingRewarded = false; nextRewarded = Time.realtimeSinceStartup + 30f; }
                else { loadingInterstitial = false; nextInterstitial = Time.realtimeSinceStartup + 30f; }
                RewardStateChanged?.Invoke();
            }
        }

        public async void ShowInterstitial()
        {
            if (showing || interstitial == null || Time.realtimeSinceStartup - lastInterstitial < 60f) return;
            var ad = interstitial;
            interstitial = null;
            showing = true;
            lastInterstitial = Time.realtimeSinceStartup;
            PauseForAd();
            RewardStateChanged?.Invoke();
            try { await ad.ShowAsync(); }
            catch (Exception error) { Debug.LogWarning("[FBInstant] Interstitial: " + error.Message); }
            finally { ResumeAfterAd(); showing = false; Preload(false); RewardStateChanged?.Invoke(); }
        }

        public async Task<bool> ShowRewardedAsync()
        {
            if (!RewardReady) return false;
            var ad = rewarded;
            rewarded = null;
            showing = true;
            PauseForAd();
            RewardStateChanged?.Invoke();
            try { await ad.ShowAsync(); return true; }
            catch (Exception error) { Debug.LogWarning("[FBInstant] Rewarded: " + error.Message); return false; }
            finally { ResumeAfterAd(); showing = false; Preload(true); RewardStateChanged?.Invoke(); }
        }

        private void PauseForAd()
        {
            FacebookPlayerPrefs.Save();
            timeScaleBeforeAd = Time.timeScale;
            Time.timeScale = 0f;
            audioPausedBeforeAd = AudioListener.pause;
            AudioListener.pause = true;
        }
        private void ResumeAfterAd() { Time.timeScale = timeScaleBeforeAd; AudioListener.pause = audioPausedBeforeAd; }
        public void OnPageHidden(string value) { FacebookPlayerPrefs.Save(); }
        public void OnPlatformPause(string value) { AudioListener.pause = true; FacebookPlayerPrefs.Save(); }
        public void OnPageVisible(string value) { if (!showing) AudioListener.pause = false; }
    }
}
#endif
