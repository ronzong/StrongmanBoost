//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Services.RemoteConfig;
//using Unity.Services.Authentication;
//using Unity.Services.Core;
//using System.Threading.Tasks;

//public class UnityRemote : MonoBehaviour
//{
//    public struct userAttribute { }
//    public struct appAttribute { }


//    async Task InitRemoteConfigAsync()
//    {
//        await UnityServices.InitializeAsync();
//        if (!AuthenticationService.Instance.IsSignedIn)
//        {
//            await AuthenticationService.Instance.SignInAnonymouslyAsync();
//        }

//    }
//    async Task Awake()
//    {
//        if (Utilities.CheckForInternetConnection())
//        {
//            await InitRemoteConfigAsync();
//        }
//        RemoteConfigService.Instance.FetchCompleted += LoadConfig;
//        RemoteConfigService.Instance.FetchConfigs<userAttribute, appAttribute>(new userAttribute(),new appAttribute());

//    }

//    void LoadConfig(ConfigResponse response)
//    {
//        //RemoteConfigService.Instance.appConfig.GetBool("");
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    private void OnDestroy()
//    {
//        RemoteConfigService.Instance.FetchCompleted -= LoadConfig;
//    }
//}
