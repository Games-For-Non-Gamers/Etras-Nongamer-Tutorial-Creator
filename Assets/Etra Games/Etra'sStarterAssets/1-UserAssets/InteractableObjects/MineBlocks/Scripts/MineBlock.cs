using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineBlock : MonoBehaviour
    {
        private void OnDestroy()
        {
            Vector3 test = transform.position;
            MineBlockSystem.Instance.PlayDestroyParticle(test, GetComponent<Renderer>().material.mainTexture);
        }
    }
}
