using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineBlockSystem : MonoBehaviour
    {
        [HideInInspector] public static MineBlockSystem Instance;

        public GameObject systemParent;
        public GameObject blocksParent;

        MeshRenderer[] outlineMeshComponents;
        const int AMOUNT_OF_PARTICLES = 7;
        public GameObject[] blockParticles = new GameObject[AMOUNT_OF_PARTICLES];
        public int currentParticle;
        public GameObject blockOutline;

        public void Awake()
        {
            //Set up Instance so it can be easily referenced. 
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Instance = this;
            }

            //Block Outline
            blockOutline = (GameObject)Resources.Load("BlockOutline");
            blockOutline = Instantiate(blockOutline, Vector3.zero, Quaternion.identity, systemParent.transform);
            outlineMeshComponents = blockOutline.GetComponentsInChildren<MeshRenderer>();

            //Block Particles
            GameObject blockDestroyParticle = (GameObject)Resources.Load("BlockDestroyParticle");
            for (int i = 0; i < blockParticles.Length; i++)
            {
                blockParticles[i] = Instantiate(blockDestroyParticle, Vector3.zero, Quaternion.Euler(-90, 0, 0), systemParent.transform);
                blockParticles[i].GetComponent<ParticleSystem>().GetComponent<Renderer>().material = new Material(blockParticles[i].GetComponent<ParticleSystem>().GetComponent<Renderer>().material);
            }



        }

        public void HideOutline()
        {
            foreach (MeshRenderer mesh in outlineMeshComponents)
            {
                mesh.enabled = false;
            }
        }

        public void ShowOutline()
        {
            foreach (MeshRenderer mesh in outlineMeshComponents)
            {
                mesh.enabled = true;
            }
        }

        public void PlayDestroyParticle(Vector3 worldPosition, Texture mainTexture)
        {
            blockParticles[currentParticle].transform.position = worldPosition;

            blockParticles[currentParticle].GetComponent<ParticleSystem>().GetComponent<Renderer>().material.mainTexture = mainTexture;
            blockParticles[currentParticle].GetComponent<ParticleSystem>().Play();
            currentParticle++;
            if (currentParticle >= blockParticles.Length)
            {
                currentParticle = 0;
            }
        }


    }
}
