using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineBlock : MonoBehaviour
    {
        private void OnDestroy()
        {
            MineBlockSystem.Instance.PlayDestroyParticle(transform.position, GetComponent<Renderer>().material.mainTexture);
        }
    }
}
