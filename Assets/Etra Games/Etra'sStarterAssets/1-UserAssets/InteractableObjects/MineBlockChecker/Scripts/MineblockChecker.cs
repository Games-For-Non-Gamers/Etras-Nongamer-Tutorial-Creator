using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineblockChecker : MonoBehaviour
    {
        [Header("Basics")]
        public MineBlockData blockToCheckFor;
        [Header("References")]
        public GameObject successParticle;
        public GameObject outline;
        //private
        Renderer thisRenderer;
        [HideInInspector]public MineBlock connectedBlock;//This is our success check.
        [HideInInspector]public MineblockCheckerSet setToActivate = null;
        [HideInInspector] public bool successFreeze = false;

        void Start()
        {
            thisRenderer = GetComponent<Renderer>();
            Renderer blockRenderer = blockToCheckFor.blockPrefab.GetComponent<Renderer>();

            int loopAmount = Mathf.Min(thisRenderer.sharedMaterials.Length, blockRenderer.sharedMaterials.Length);


            for (int i = 0; i < loopAmount; i++)
            {
                thisRenderer.materials[i] = new Material(thisRenderer.sharedMaterials[i]);
                thisRenderer.materials[i].mainTexture = blockRenderer.sharedMaterials[i].mainTexture;
                Color altBlockColor = blockRenderer.sharedMaterials[i].color; ;
                thisRenderer.materials[i].color = new Color(altBlockColor.r, altBlockColor.g, altBlockColor.b, thisRenderer.materials[i].color.a);
            }

        }


        public void BlockPlaced(MineBlock block)
        {
            connectedBlock = block;
            StartCoroutine(BlockJudge(block));

        }

        public void BlockDestroyed()
        {
            connectedBlock = null;
            OutlineVisibility(true);
        }


        IEnumerator BlockJudge(MineBlock block)
        {
            OutlineVisibility(false);
            if (block.blockData.blockPrefab == blockToCheckFor.blockPrefab)
            {
                MineBlockSystem.Instance.gameObject.GetComponent<AudioManager>().Play("PlaceSuccess");
                GameObject particle =  Instantiate(successParticle, this.transform.position, Quaternion.identity);
                successFreeze = true;
                yield return new WaitForSeconds(2);
                Destroy(particle);
                if (setToActivate)
                {
                    setToActivate.CheckTriggers();
                }
                successFreeze = false;
            }
            else
            {
                GameObject tempDestroyBlock = connectedBlock.gameObject;
                connectedBlock = null;
                yield return new WaitForSeconds(0.5f);
                MineBlockSystem.Instance.gameObject.GetComponent<AudioManager>().Play("PlaceFail");
                OutlineVisibility(true);
                yield return new WaitForSeconds(1f);

                if (tempDestroyBlock != null)
                {
                    Destroy(tempDestroyBlock);
                }

            }

        }


        void OutlineVisibility(bool visible)
        {
            thisRenderer.enabled = visible;
            outline.SetActive(visible);
        }

    }
}
