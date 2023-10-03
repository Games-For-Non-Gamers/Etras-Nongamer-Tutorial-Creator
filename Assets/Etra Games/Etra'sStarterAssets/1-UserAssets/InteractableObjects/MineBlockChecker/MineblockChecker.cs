using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineblockChecker : MonoBehaviour
    {
        public MineBlockData blockToCheckFor;
        public GameObject successParticle;
        Renderer thisRenderer;
        MineBlock blockInSlot = null;  //This is our success check.

        void Start()
        {
            thisRenderer = GetComponent<Renderer>();
            Renderer blockRenderer = blockToCheckFor.blockPrefab.GetComponent<Renderer>();

            int loopAmount = Mathf.Min(thisRenderer.sharedMaterials.Length, blockRenderer.sharedMaterials.Length);


            for (int i = 0; i < loopAmount; i++)
            {
                thisRenderer.materials[i] = new Material(thisRenderer.sharedMaterials[i]);
                thisRenderer.materials[i].mainTexture = blockRenderer.sharedMaterials[i].mainTexture;
            }

        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("a");
            if (other.GetComponent<MineBlock>())
            {
                Debug.Log("e");
                StartCoroutine(BlockJudge(other.GetComponent<MineBlock>()));
            }
        }


        IEnumerator BlockJudge(MineBlock block )
        {
            thisRenderer.enabled = false;
            if (block.blockData.blockPrefab == blockToCheckFor.blockPrefab)
            {
                blockInSlot = block;
                MineBlockSystem.Instance.gameObject.GetComponent<AudioManager>().Play("PlaceSuccess");
                GameObject particle =  Instantiate(successParticle, Vector3.zero, Quaternion.identity, this.transform);
                yield return new WaitForSeconds(2);
                Destroy(particle);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                MineBlockSystem.Instance.gameObject.GetComponent<AudioManager>().Play("PlaceFail");
                Destroy(block.gameObject);
            }

        }

    }
}
