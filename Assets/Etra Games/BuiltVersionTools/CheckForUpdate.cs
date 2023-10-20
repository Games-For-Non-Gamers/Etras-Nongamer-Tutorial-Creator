using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Etra.StarterAssets;
using UnityEditor;

public class CheckForUpdate : MonoBehaviour
{
    public GameFreezePopupTrigger updatePopup;
    [HideInInspector] public struct userAttributes { };
    [HideInInspector] public struct appAttributes { };
    // Start is called before the first frame update
    void Awake()
    {
        RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }



    // Declare any Settings variables you�ll want to configure remotely:
    public int enemyVolume;
    public string remoteVersion;

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        // options can be passed in the initializer, e.g if you want to set AnalyticsUserId or an EnvironmentName use the lines from below:
        // var options = new InitializationOptions()
        // .SetEnvironmentName("testing")
        // .SetAnalyticsUserId("test-user-id-12345");
        // await UnityServices.InitializeAsync(options);

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    async Task Start()
    {
        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());

        // -- Example on how to fetch configuration settings using filter attributes:
        // var fAttributes = new filterAttributes();
        // fAttributes.key = new string[] { "sword","cannon" };
        // RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes(), fAttributes);

        // -- Example on how to fetch configuration settings if you have dedicated configType:
        // var configType = "specialConfigType";
        // RemoteConfigService.Instance.FetchConfigs(configType, new userAttributes(), new appAttributes());
        // -- Configuration can be fetched with both configType and fAttributes passed
        // RemoteConfigService.Instance.FetchConfigs(configType, new userAttributes(), new appAttributes(), fAttributes);

        // -- All examples from above will also work asynchronously, returning Task<RuntimeConfig>
        // await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
        // await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes(), fAttributes);
        // await RemoteConfigService.Instance.FetchConfigsAsync(configType, new userAttributes(), new appAttributes());
        // await RemoteConfigService.Instance.FetchConfigsAsync(configType, new userAttributes(), new appAttributes(), fAttributes);


        Debug.Log(Application.version);
        Debug.Log(remoteVersion);
        if (updatePopup == null)
        {
            Debug.LogWarning("Add in the update popup Luke!");
        }
        else
        {
            if (Application.version != remoteVersion)
            {
                updatePopup.playEvents();
            }
        }

    }

    void ApplyRemoteConfig(ConfigResponse configResponse)
    {

        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session and no local cache file exists; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("New settings loaded this session; update values accordingly.");
                Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());
                break;
        }

        enemyVolume = RemoteConfigService.Instance.appConfig.GetInt("enemyVolume");
        remoteVersion = RemoteConfigService.Instance.appConfig.GetString("Version");
        // These calls could also be used with the 2nd optional arg to provide a default value, e.g:
        // enemyVolume = RemoteConfigService.Instance.appConfig.GetInt("enemyVolume", 100);

    }
}

