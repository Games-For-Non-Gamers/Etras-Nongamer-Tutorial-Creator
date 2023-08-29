using Etra.StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin4ButtonDoor : MonoBehaviour
{
    public int buttonsPressed;
    //This is the worst script. Apologies.
    public BoxCollider ThreeLeft;
    public BoxCollider TwoLeft;
    public BoxCollider OneLeft;
    public BoxCollider Done;
    // Start is called before the first frame updates
    public void buttonPressed()
    {
        buttonsPressed++;
        if (buttonsPressed == 1)
        {
            ThreeLeft.enabled = true;
        }
        if (buttonsPressed == 2) { TwoLeft.enabled = true; }
        if (buttonsPressed == 3) { OneLeft.enabled = true; }
        if (buttonsPressed == 4) { Done.enabled = true; this.GetComponent<Door>().doorInteract(); }
    }
}
