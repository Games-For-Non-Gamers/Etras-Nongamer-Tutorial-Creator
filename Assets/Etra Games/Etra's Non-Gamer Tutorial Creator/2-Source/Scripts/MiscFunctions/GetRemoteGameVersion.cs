using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Etra
{
    public class GetRemoteGameVersion : MonoBehaviour
    {
        [HideInInspector] public struct userAttributes { };
        [HideInInspector] public struct appAttributes { };
        // Start is called before the first frame update
        void Awake()
        {
            RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        }

        // Update is called once per frame
        public string GetGameVersion()
        {
            return RemoteConfigService.Instance.appConfig.GetString("Version");
        }
    }

}