using Etra.StarterAssets.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBotScript : MonoBehaviour
{
    public ChasePlayer playerChaseScript;
    public AudioSource alarm;
    // Start is called before the first frame update
    void Start()
    {
        alarm.Stop();
        StartCoroutine(waitToDissapear());
    }


    IEnumerator waitToDissapear()
    {
        yield return new WaitForSeconds(1);
        playerChaseScript.respawn();
        playerChaseScript.gameObject.SetActive(false);
    }

    public void SilenceAlarm(float time)
    {
        LeanTween.value(this.gameObject, alarm.volume, 0, time).setOnUpdate((float volume) => { alarm.volume = volume;});
    }

}
