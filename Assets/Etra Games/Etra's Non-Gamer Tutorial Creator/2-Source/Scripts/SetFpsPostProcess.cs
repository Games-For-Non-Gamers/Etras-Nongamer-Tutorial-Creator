using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
