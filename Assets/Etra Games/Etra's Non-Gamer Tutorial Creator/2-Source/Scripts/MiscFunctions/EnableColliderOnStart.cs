using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class EnableColliderOnStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.GetComponent<BoxCollider>().enabled = true;
        }
    }
}