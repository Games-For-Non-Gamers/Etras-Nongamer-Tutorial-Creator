using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float secondsTillDestroy = 2;
    void Start()
    {
        StartCoroutine(waitToDestroy());
    }

    IEnumerator waitToDestroy()
    {
        yield return new WaitForSeconds(secondsTillDestroy);
        Destroy(this.gameObject);
    }
}
