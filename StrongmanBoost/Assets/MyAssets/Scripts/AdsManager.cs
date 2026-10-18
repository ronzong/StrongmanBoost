using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour
{
    private string intertestialId = "ca-app-pub-3940256099942544/1033173712";
    private string bannerId = "ca-app-pub-3940256099942544/6300978111";
    private string rewardId = "ca-app-pub-3940256099942544/5224354917";

    //private InterstitialAd interstitialAd;
    //private BannerView bannerView;
    //private RewardedAd rewardedAd;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });

        LoadBannerAd();

        RequestInterstitial();

        LoadRewardedAd();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.K))
        {
            FindObjectOfType<GameManager>().AddCash(50);
        }
#endif
    }

    public void LoadBannerAd()
    {
        //// create an instance of a banner view first.
        //if (bannerView == null)
        //{
        //    CreateBannerView();
        //}
        //// create our request used to load the ad.
        //var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        //// send the request to load the ad.
        //Debug.Log("Loading banner ad.");
        //bannerView.LoadAd(adRequest);
    }

    public void CreateBannerView()
    {
        //Debug.Log("Creating banner view");

        //// If we already have a banner, destroy the old one.
        //if (bannerView != null)
        //{
        //    DestroyAd();
        //}

        //// Create a 320x50 banner at top of the screen
        //bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    public void DestroyAd()
    {
        //if (bannerView != null)
        //{
        //    Debug.Log("Destroying banner ad.");
        //    bannerView.Destroy();
        //    bannerView = null;
        //}
    }


    private void RequestInterstitial()
    {
        //// Clean up the old ad before loading a new one.
        //if (interstitialAd != null)
        //{
        //    interstitialAd.Destroy();
        //    interstitialAd = null;
        //}

        //Debug.Log("Loading the interstitial ad.");

        //// create our request used to load the ad.
        //var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        //// send the request to load the ad.
        //InterstitialAd.Load(intertestialId, adRequest,
        //    (InterstitialAd ad, LoadAdError error) =>
        //    {
        //        // if error is not null, the load request failed.
        //        if (error != null || ad == null)
        //        {
        //            Debug.LogError("interstitial ad failed to load an ad " +
        //                           "with error : " + error);
        //            return;
        //        }

        //        Debug.Log("Interstitial ad loaded with response : "
        //                  + ad.GetResponseInfo());

        //        interstitialAd = ad;
        //    });
    }

    public void ShowInterstitialAd()
    {
        //if (interstitialAd != null && interstitialAd.CanShowAd())
        //{
        //    Debug.Log("Showing interstitial ad.");
        //    interstitialAd.Show();
        //}
        //else
        //{
        //    Debug.LogError("Interstitial ad is not ready yet.");
        //    RequestInterstitial();
        //}
#if FB_INSTANT && UNITY_WEBGL
        StrongmanBoost.Platform.FacebookInstantGame.Instance.ShowInterstitial();
#elif ADS_MAX
        ServicesManager_MAX.instance.ShowInterstitial();
#endif
    }

    public void LoadRewardedAd()
    {
        //// Clean up the old ad before loading a new one.
        //if (rewardedAd != null)
        //{
        //    rewardedAd.Destroy();
        //    rewardedAd = null;
        //}

        //Debug.Log("Loading the rewarded ad.");

        //// create our request used to load the ad.
        //var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        //// send the request to load the ad.
        //RewardedAd.Load(rewardId, adRequest,
        //    (RewardedAd ad, LoadAdError error) =>
        //    {
        //        // if error is not null, the load request failed.
        //        if (error != null || ad == null)
        //        {
        //            Debug.LogError("Rewarded ad failed to load an ad " +
        //                           "with error : " + error);
        //            return;
        //        }

        //        Debug.Log("Rewarded ad loaded with response : "
        //                  + ad.GetResponseInfo());

        //        rewardedAd = ad;
        //    });
    }
    Action callback = null;
    public void ShowRewardedAd()
    {
        //const string rewardMsg =
        //    "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
        //if (rewardedAd != null && rewardedAd.CanShowAd())
        //{
        //    rewardedAd.Show((Reward reward) =>
        //    {
        //        GiveReward();
        //        print("Reward");
        //    });
        //}

        //LoadRewardedAd();
#if FB_INSTANT && UNITY_WEBGL
        ShowFacebookReward();
#elif ADS_MAX
        callback = GiveReward;
        ServicesManager_MAX.instance.ShowRewardedAd(""  , callback);



#endif
    }

#if FB_INSTANT && UNITY_WEBGL
    // 每次展示使用独立结果；取消/失败/重复点击不发奖。
    private async void ShowFacebookReward()
    {
        if (await StrongmanBoost.Platform.FacebookInstantGame.Instance.ShowRewardedAsync()) GiveReward();
    }
#endif
    private void GiveReward()
    {
        FindObjectOfType<GameManager>().AddCash(50);
    }
}
