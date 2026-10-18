using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ServicesManager_MAX : MonoBehaviour
{
    public static ServicesManager_MAX instance { get; set; }

    public bool openShowOpenAD = false;
    public bool openShowBanner = false;
    public bool openMREC = false;
    [Header("InterAD")]
    public bool openInterstLimit = false;
    public int limitTime = 20;

    [Header("Android")]
    public string appIDAndroid;
    //public string appOpenIDAndroid;
    public string bannerIDAndroid;
    public string interstitialID;
    public string rewardedVideoAdsID;
    public string mrecAdUnitIdAndroid;

    bool isRewardAdded;

    [Header("IOS")]
    public string appIDIos;
    //public string appOpenIDIos;
    public string bannerIDIos;
    public string interstitialIDIos;
    public string rewardedVideoAdsIDIos;
    public string mrecAdUnitIdIos;

    private string sdkKey;
    private string appOpenID;
    private string bannerID;
    private string InterstitialsID;
    private string VideoID;
    private string mrecAdUnitId;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

#if ADS_MAX
    void Start()
    {
        Application.targetFrameRate = 60;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            sdkKey = appIDIos;
            bannerID = bannerIDIos;
            InterstitialsID = interstitialIDIos;
            VideoID = rewardedVideoAdsIDIos;
            if (openMREC)
            {

            mrecAdUnitId = mrecAdUnitIdIos;
            }
        }
        else
        {
            sdkKey = appIDAndroid;
            bannerID = bannerIDAndroid;
            InterstitialsID = interstitialID;
            VideoID = rewardedVideoAdsID;
            if (openMREC)
            {

            mrecAdUnitId = mrecAdUnitIdAndroid;
            }
        }

        //---test--start--测试id
        //        sdkKey = "ENTER_MAX_SDK_KEY_HERE";
        //        bannerID = "ENTER_BANNER_AD_UNIT_ID_HERE";
        // 
        //         InterstitialsID = "ENTER_INTERSTITIAL_AD_UNIT_ID_HERE";
        //         VideoID ="ca-app-pub-ENTER_REWARD_AD_UNIT_ID_HERE/5224354917";
        // 
        //---test---end-

        InitMAX();
    }

    void InitMAX()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies)
            {
                //MaxSdk.SetHasUserConsent(true);
                //MaxSdk.SetHasUserConsent(false);
                // Show user consent dialog
                //MaxSdk.UserService.ShowConsentDialog();
            }
            else if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.DoesNotApply)
            {
                // No need to show consent dialog, proceed with initialization
            }
            else
            {
                // Consent dialog state is unknown. Proceed with initialization, but check if the consent
                // dialog should be shown on the next application initialization
            }

            // AppLovin SDK is initialized, start loading ads
            InitAD();
            CheckFirebase();
            //MaxSdk.ShowMediationDebugger();//正式包注释
        };

        MaxSdk.SetSdkKey(sdkKey);
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
        //MaxSdk.InitializeSdk(new[] { "YOUR_AD_UNIT_ID_1", "YOUR_AD_UNIT_ID_2" });

        //MaxSdk.SetVerboseLogging(true);
    }

    void InitAD()
    {
        if (openShowBanner)
        {
            InitializeBannerAds();
        }

        if (openShowOpenAD)
        {
            InitializeOpenAd();
        }
        //
        #region mrec
        if (openMREC)
        {
            InitializeMRecAds();
        }
        #endregion

        Invoke("InitializeInterstitialAds", 0.5f);
        Invoke("InitializeRewardedAds", 1f);
    }


    void CheckFirebase()
    {
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    var dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        // Create and hold a reference to your FirebaseApp,
        //        // where app is a Firebase.FirebaseApp property of your application class.
        //        //app = Firebase.FirebaseApp.DefaultInstance;

        //        // Set a flag here to indicate whether Firebase is ready to use by your app.
        //        Debug.Log("Create and hold a reference to your FirebaseApp");
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.
        //    }
        //});
    }
#endif

    private void Update()
    {
        AdOpenTime += Time.deltaTime;
    }

    //-----------------------------------横幅---------------------------------------------
#if ADS_MAX
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        //MaxSdk.CreateBanner(bannerID, MaxSdkBase.BannerPosition.TopCenter);
        MaxSdk.CreateBanner(bannerID, MaxSdkBase.BannerPosition.BottomCenter);
        //MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true")

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerID, new Color(1, 1, 1, 0));

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //Invoke("ShowBannerAdmob", 1);
        ShowBannerAdmob("", "");
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
#endif

    public void ShowBannerAdmob(string eventName, object data)
    {
        Debug.Log("ShowBannerAdmob loaded succesfully");
#if ADS_MAX
        MaxSdk.ShowBanner(bannerID);
#endif
    }

    public void HideBanner()
    {
#if ADS_MAX
        MaxSdk.HideBanner(bannerID);
#endif
    }

    //-----------------------------------自制开屏（用插屏当开屏）---------------------------------------------
#if ADS_MAX
    bool isOnceOpenAd = false;
    void InitializeOpenAd()
    {
        isOnceOpenAd = true;
    }

#endif

    //-----------------------------------插页---------------------------------------------
#if ADS_MAX
    int retryAttemptInterstitial;

    DateTime lastNow = DateTime.Now;
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialsID);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttemptInterstitial = 0;

        DateTime newTime = DateTime.Now;
        TimeSpan ts1 = new TimeSpan(lastNow.Ticks);
        TimeSpan ts2 = new TimeSpan(newTime.Ticks);
        TimeSpan tsSub = ts1.Subtract(ts2).Duration();
        if (tsSub.TotalSeconds < 5 && isOnceOpenAd)
        {
            Debug.Log("OpenAd loaded succesfully");
            isOnceOpenAd = false;
            ShowInterstitialAdmob("ShowInterstitialAdmob", null);
        }
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        print("OnInterstitialLoadFailedEvent Name: " + errorInfo.WaterfallInfo.Name + " and Test Name: " + errorInfo.WaterfallInfo.TestName);

        retryAttemptInterstitial++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptInterstitial));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
#endif

    //带回掉的展示插屏
    public void ShowInterstitialAdmob(string eventName, object data)
    {
#if ADS_MAX
        if (data != null)
        {
            click = (System.Action)data;
        }


        if (MaxSdk.IsInterstitialReady(InterstitialsID))
        {
            Debug.Log("Interstitial was loaded succesfully! interstialId=" + InterstitialsID[0]);
            MaxSdk.ShowInterstitial(InterstitialsID);
        }
        else
        {
            Debug.Log("Interstitial Loding " + InterstitialsID);
        }
#endif
    }

    //限制时间的回掉插屏
    float AdOpenTime = 120f;
    public void ShowInterstitialAdmobTime(string eventName, object data)
    {
        if (AdOpenTime < (int)data)
        {
            return;
        }

#if ADS_MAX
        click = null;
        if (MaxSdk.IsInterstitialReady(InterstitialsID))
        {
            AdOpenTime = 0;
            Debug.Log("Interstitial was loaded succesfully! interstialId=" + InterstitialsID[0]);
            MaxSdk.ShowInterstitial(InterstitialsID);
        }
        else
        {
            Debug.Log("Interstitial Loding " + InterstitialsID);
        }
#endif
    }
    public void ShowInterstitial()
    {
        if (openInterstLimit)
        {
            ShowInterstitialAdmobTime("", limitTime);
        }
        else
        {
            ShowInterstitialAdmob("",null);
        }
    }

    //-----------------------------------激励视频---------------------------------------------
#if ADS_MAX
    System.Action click;

    int retryAttemptRewarded = 0;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(VideoID);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptRewarded++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptRewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (click != null)
        {
            click();
        }
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
#endif

    /// <summary>
    /// 展示视频广告
    /// </summary>
    /// <param name="_newPage">标签页</param>
    /// <param name="action">奖励回掉</param>
    /// <param name="action2">失败回掉</param>
    public void ShowRewardedAd(string eventName, object data)
    {
#if ADS_MAX
        click = (System.Action)data;

        isRewardAdded = false;
        if (MaxSdk.IsRewardedAdReady(VideoID))
        {
            AdOpenTime = 0;
            Debug.Log("Rewarded was loaded succesfully! " + VideoID);
            MaxSdk.ShowRewardedAd(VideoID);
        }
        else
        {
            Debug.Log("Rewarded Loding " + VideoID);

        }
#endif
    }
    public bool IsRewardedAdReady()
    {
        return MaxSdk.IsRewardedAdReady(VideoID);
    }
    public bool IsInterstitialReady()
    {
        return MaxSdk.IsInterstitialReady(InterstitialsID);
    }

    #region MREC

//#if UNITY_IOS
//string mrecAdUnitId = "YOUR_IOS_AD_UNIT_ID"; // Retrieve the ID from your account
//#else // UNITY_ANDROID
//    string mrecAdUnitId = "YOUR_ANDROID_AD_UNIT_ID"; // Retrieve the ID from your account
//#endif

    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(mrecAdUnitId, MaxSdkBase.AdViewPosition.CenterRight);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
        Debug.Log("mrec Loaded");
        ShowMRec();
    }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) 
    {
        Debug.Log("mrec loadFail");
        //StopMRecAutoRefresh();
        //LoadMRec();
    }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }



    public void ShowMRec()
    {
        //StartMRecAutoRefresh();
        
        Debug.Log(" MaxSdk.ShowMRe");
        MaxSdk.ShowMRec(mrecAdUnitId);
        //StartMRecAutoRefresh();

    }
    public void HideMRec()
    {
        MaxSdk.HideMRec(mrecAdUnitId);
    }
    public void DestroyMRec()
    {
        if (openMREC)
        {

        MaxSdk.DestroyMRec(mrecAdUnitId);
        }
    }
    public void StopMRecAutoRefresh()
    {
        MaxSdk.StopMRecAutoRefresh(mrecAdUnitId);
    }
    public void StartMRecAutoRefresh()
    {
        Debug.Log("MaxSdk.StartMRecAutoRefres");
        MaxSdk.StartMRecAutoRefresh(mrecAdUnitId);
    }
    public void LoadMRec()
    {
        MaxSdk.LoadMRec(mrecAdUnitId);
    }
    #endregion

    private void OnDestroy()
    {
        DestroyMRec();
    }

}