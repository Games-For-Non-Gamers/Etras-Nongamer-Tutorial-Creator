using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using UnityEngine;

public class RuinStaminaPopups : MonoBehaviour
{
    public ABILITY_Sprint abilitySprint;
    public GameFreezePopupTrigger outOfStam;
    bool firstOneShowed = false;
    public GameFreezePopupTrigger stamRestored;


    // Start is called before the first frame update
    public void setup()
    {
        abilitySprint.OutOfStamina.AddListener(outOfStamFunction);
        abilitySprint.StaminaRecovered.AddListener(stamRestoredFunction);
    }

    // Update is called once per frame
    void outOfStamFunction()
    {
        outOfStam.playEvents();
        firstOneShowed = true;
    }

    void stamRestoredFunction()
    {
        if (!firstOneShowed) {
            return;
        }
        stamRestored.playEvents();
        disable();
    }

    public void disable()
    {
        abilitySprint.OutOfStamina.RemoveListener(outOfStamFunction);
        abilitySprint.StaminaRecovered.RemoveListener(stamRestoredFunction);
    }

  
}
