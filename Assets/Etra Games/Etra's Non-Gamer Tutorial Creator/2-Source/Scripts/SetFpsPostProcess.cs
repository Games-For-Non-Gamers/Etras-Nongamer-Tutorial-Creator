using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Etra.NonGamerTutorialCreator
{
    public class SetFpsPostProcess : MonoBehaviour
    {
        //Switch the active post process layer
        private void Start()
        {
            if (Camera.main.GetComponent<PostProcessLayer>())
            {
                Camera.main.GetComponent<PostProcessLayer>().enabled = false;
                this.GetComponent<PostProcessLayer>().enabled = true;
            }
        }
    }
}