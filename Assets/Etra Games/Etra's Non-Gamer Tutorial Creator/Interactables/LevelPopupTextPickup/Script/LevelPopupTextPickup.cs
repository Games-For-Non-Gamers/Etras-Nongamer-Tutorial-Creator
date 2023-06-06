using Etra.NonGamerTutorialCreator;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source;
using Etra.StarterAssets;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPopupTextPickup : MonoBehaviour
{
    public string textLabelToShow = "";
    public bool showPopupAnimation = true;
    LevelPopupTextUi textUi;

    //Set the correct selected ability
    private void Start()
    {
        if (GameObject.Find(textLabelToShow).GetComponent<LevelPopupTextUi>() !=null)
        {
            textUi = GameObject.Find(textLabelToShow).GetComponent<LevelPopupTextUi>();
        }
        else
        {
            Debug.LogWarning("Text label:" + textLabelToShow + " not found or " + textLabelToShow + " does not have a LevelPopupTextUi script attatched.");
        }

    }


    //If the player collides with the pickup...
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && textUi != null)
        {
            if (showPopupAnimation)
            {
                textUi.runUiEvent();
            }
            else
            {
                textUi.showAllUiObjects();
            }
           
            Destroy(gameObject);
        }

    }
}
