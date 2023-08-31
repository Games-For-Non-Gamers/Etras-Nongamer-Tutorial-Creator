using Etra.StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin4ButtonDoor : MonoBehaviour
{
    public int buttonsPressed;
    //This is the worst script. Apologies.
    public NewDialogueTrigger ThreeLeft;
    public NewDialogueTrigger TwoLeft;
    public NewDialogueTrigger OneLeft;
    public NewDialogueTrigger Done;
    // Start is called before the first frame updates
    public void buttonPressed()
    {
        buttonsPressed++;
        if (buttonsPressed == 1)
        {
            ThreeLeft.runEvents();
        }
        if (buttonsPressed == 2) { TwoLeft.runEvents(); }
        if (buttonsPressed == 3) { OneLeft.runEvents(); }
        if (buttonsPressed == 4) { Done.runEvents(); this.GetComponent<Door>().doorInteract(); }
    }
}
