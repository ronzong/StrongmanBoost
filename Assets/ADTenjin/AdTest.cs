using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdTest : MonoBehaviour
{
    public Button banner_btn;
    public Button inter_btn;
    public Button reward_btn;

    // Start is called before the first frame update
    void Start()
    {
        banner_btn.onClick.AddListener(()=> { ServicesManager_MAX.instance.ShowBannerAdmob("",null  ); });
        inter_btn.onClick.AddListener(() => { ServicesManager_MAX.instance.ShowInterstitialAdmob("", null); });
        reward_btn.onClick.AddListener(() => { ServicesManager_MAX.instance.ShowRewardedAd("", null); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
