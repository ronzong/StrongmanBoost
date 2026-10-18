using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinInitEntry : MonoBehaviour
{
    // Start is called before the first frame update
    public string SDK_KEY = "";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        TenjinConnect();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            TenjinConnect();
        }
    }

    public void TenjinConnect()
    {
#if tenjin_applovin_enabled
        if (string.IsNullOrEmpty(SDK_KEY))
        {
            Debug.LogError("tenjin SDK_KEY Œ™ø’£¨«Î…Ë÷√SDK_KEY");
            return;
        }
        BaseTenjin instance = Tenjin.getInstance(SDK_KEY);

        instance.SetAppStoreType(AppStoreType.googleplay);
        // Sends install/open event to Tenjin
        instance.Connect();
        Debug.Log("TenjinInitEntry TenjinConnect");
#endif
    }

}
