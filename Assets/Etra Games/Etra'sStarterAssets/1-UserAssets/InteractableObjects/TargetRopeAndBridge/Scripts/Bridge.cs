using EtrasStarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Etra.StarterAssets.Interactables
{
    public class Bridge : MonoBehaviour
    {
        //Variables
        public Target[] targets;
        public float startRotation = -70;
        public float loweredRotation = 0;
        public GameObject dumbHitbox;
        public float delayBeforeLower = 0.5f;
        public float timeToLower = 1;
        private bool bridgeLowered = false;
        EtrasStarterAssets.AudioManager audioManager;
        public UnityEvent bridgeActivateStart;
        private void Start()
        {
            LeanTween.rotateLocal(transform.GetChild(0).gameObject, new Vector3 (startRotation,0,0), 0);
            // Check this at start so the bridge automatically lowers if no targets are connected
            checkActivate();
            audioManager = GetComponentInChildren<EtrasStarterAssets.AudioManager>();
        }

        //This is called each time as connected target is destoryed
        public void checkActivate()
        {
            if (bridgeLowered)
            {
                return;
            }

            bool allActivated = true;
            for (int i = 0; i < targets.Length; i++)
            {
                if (!targets[i].activated)
                {
                    allActivated = false;
                }
            }

            if (allActivated)
            {
                bridgeActivateStart.Invoke();
                StartCoroutine(lowerBridge());
            }

        }

        //Animation for bridge lowering
        IEnumerator lowerBridge()
        {
            dumbHitbox.SetActive(false);
            bridgeLowered = true;
            //Second delay before the bridge lowers
            yield return new WaitForSeconds(delayBeforeLower);
            audioManager.Play("BridgeFall");
            //Rotate correct anchor point
            LeanTween.rotateLocal(transform.GetChild(0).gameObject, new Vector3 (loweredRotation, 0,0), timeToLower).setEaseInOutSine();
            yield return new WaitForSeconds(timeToLower);
            audioManager.Play("MedSparkle");
        }

    }
}

