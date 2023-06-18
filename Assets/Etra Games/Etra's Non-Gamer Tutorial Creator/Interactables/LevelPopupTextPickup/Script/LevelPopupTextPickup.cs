using Etra.NonGamerTutorialCreator;
using UnityEngine;

public class LevelPopupTextPickup : MonoBehaviour
{
    public string textLabelToShow = "";
    public bool runAnimation = true;
    LevelPopupTextUi objectWithEtraAnimationScript;

    [Header("Rendering")]
    public bool showInEditor = true;
    public bool showInGame = false;

    //Set the correct selected ability
    private void Start()
    {
        if (showInGame)
        {
            showRenderers();
        }
        else
        {
            hideRenderers();
        }

        if (GameObject.Find(textLabelToShow).GetComponent<LevelPopupTextUi>() !=null)
        {
            objectWithEtraAnimationScript = GameObject.Find(textLabelToShow).GetComponent<LevelPopupTextUi>();
        }
        else
        {
            Debug.LogWarning("Text label:" + textLabelToShow + " not found or " + textLabelToShow + " does not have a LevelPopupTextUi script attatched.");
        }

    }


    //If the player collides with the pickup...
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && objectWithEtraAnimationScript != null)
        {
            if (runAnimation)
            {
                objectWithEtraAnimationScript.runUiEvent();
            }
            else
            {
                objectWithEtraAnimationScript.showAllUiObjects();
            }
           
            Destroy(gameObject);
        }

    }


    private void OnValidate()
    {
        if (showInEditor)
        {
            showRenderers();
        }
        else
        {
            hideRenderers();
        }

    }

    void showRenderers()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
    }

    void hideRenderers()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }

}
