using UnityEditor;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level { 
    public class BridgeIdentifier : MonoBehaviour
    {
        private void Awake()
        {
            PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
    }
}