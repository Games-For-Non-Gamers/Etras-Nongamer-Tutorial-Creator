using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineBlock : MonoBehaviour
    {
        Texture savedTexture;
        Vector3 test;
        private void Start()
        {
            savedTexture = GetComponent<Renderer>().material.mainTexture;
            test = transform.position;
        }

        private void OnDestroy()
        {
            MineBlockSystem.Instance.PlayDestroyParticle(test, savedTexture);
        }
    }
}
