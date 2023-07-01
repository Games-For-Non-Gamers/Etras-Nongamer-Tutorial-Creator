using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSplashIfOceanHit : MonoBehaviour
{

    public GameObject splashParticle;
    public Vector3 particleScale = Vector3.one;
    private bool splashed = false;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NonGamerOceanSplashLayer>())
        {
            if (!splashed)
            {
                GameObject sP = Instantiate(splashParticle, this.transform.position, Quaternion.identity);
                sP.transform.localScale = particleScale;
                audioManager.Play("Splash");
                splashed = true;
                StartCoroutine(splashReset());
            }
        }
    }

    IEnumerator splashReset()
    {
        yield return new WaitForSeconds(0.5f);
        splashed = false;
    }
}
