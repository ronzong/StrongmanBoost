//
//  Copyright (c) 2022 Tenjin. All rights reserved.
//

using System;
using System.Collections.Generic;
using UnityEngine;

public class TenjinAdMobIntegration
{
    // TODO: Potential issue with multiple ad types being subscribed to at once TENJIN-16020
    private static bool _subscribed_admob = false;
    public TenjinAdMobIntegration()
    {
    }
    public static void ListenForBannerViewImpressions(object bannerView, string adUnitId, Action<string> callback)
    {
#if tenjin_admob_enabled
        if (_subscribed_admob)
        {
            Debug.Log("Ignoring duplicate admob bannerView subscription");
            return;
        }
        _subscribed_admob = true;
#endif
    }
    public static void ListenForRewardedAdImpressions(object rewardedAd, string adUnitId, Action<string> callback)
    {
#if tenjin_admob_enabled
        if (_subscribed_admob)
        {
            Debug.Log("Ignoring duplicate admob rewardedAd subscription");
            return;
        }
        _subscribed_admob = true;
#endif
    }
    public static void ListenForInterstitialAdImpressions(object interstitialAd, string adUnitId, Action<string> callback)
    {
#if tenjin_admob_enabled
        if (_subscribed_admob)
        {
            Debug.Log("Ignoring duplicate admob interstitialAd subscription");
            return;
        }
        _subscribed_admob = true;
#endif
    }

    public static void ListenForRewardedInterstitialAdImpressions(object rewardedInterstitialAd, string adUnitId, Action<string> callback)
    {
#if tenjin_admob_enabled
        if (_subscribed_admob)
        {
            Debug.Log("Ignoring duplicate admob rewardedInterstitialAd subscription");
            return;
        }
        _subscribed_admob = true;
#endif
    }
}

[System.Serializable]
internal class AdMobImpressionDataToJSON
{
    public string currency_code;
    public string ad_unit_id;
    public string response_id;
    public string precision_type;
    public string mediation_adapter_class_name;
#if UNITY_IPHONE
    public double value_micros;
#elif UNITY_ANDROID
    public long value_micros;
#else
    public long value_micros;
#endif
}
