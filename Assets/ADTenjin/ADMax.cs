using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADMax : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ShowBanner());
        if (ServicesManager_MAX.instance.openMREC)
        {
            StartCoroutine(ShowMRec());
        }
       
    }
    IEnumerator ShowBanner()
    {
        yield return new WaitForSeconds(3f);
        ServicesManager_MAX.instance.ShowBannerAdmob("ShowBannerAdmob", null);

    }
    IEnumerator ShowMRec()
    {
        yield return new WaitForSeconds(2f);
        ServicesManager_MAX.instance.ShowMRec();

    }
}
