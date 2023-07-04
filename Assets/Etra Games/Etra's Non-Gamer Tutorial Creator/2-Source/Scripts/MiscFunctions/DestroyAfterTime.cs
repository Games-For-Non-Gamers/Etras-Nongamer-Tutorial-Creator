using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
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
}