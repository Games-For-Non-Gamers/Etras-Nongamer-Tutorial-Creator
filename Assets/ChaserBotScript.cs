using Etra.StarterAssets.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBotScript : MonoBehaviour
{
    public ChasePlayer playerChaseScript;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitToDissapear());
    }



    IEnumerator waitToDissapear()
    {
        yield return new WaitForSeconds(1);
        playerChaseScript.respawn();
        playerChaseScript.gameObject.SetActive(false);
    }

}
