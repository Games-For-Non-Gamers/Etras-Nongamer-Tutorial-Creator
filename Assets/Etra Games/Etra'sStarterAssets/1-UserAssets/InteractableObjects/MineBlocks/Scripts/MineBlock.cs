using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineBlock : MonoBehaviour
    {
        public MineBlockData blockData;
        Texture savedTexture;
        Vector3 test;
        private void Start()
        {
            savedTexture = GetComponent<Renderer>().material.mainTexture;
            test = transform.position;
        }

        private void OnDestroy()
        {
            if (Application.isPlaying) { 
                MineBlockSystem.Instance.PlayDestroyParticle(test, savedTexture);
            }
        }
    }
}
