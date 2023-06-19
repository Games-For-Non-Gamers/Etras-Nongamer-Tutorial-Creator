using Etra.NonGamerTutorialCreator;
using UnityEngine;

public class AnimationTriggerPickup : MonoBehaviour
{
    public string nameOfObjectWithAnimationHolder = "";
    public bool runAnimation = true;
    EtraAnimationHolder objectWithEtraAnimationScript;

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

        if (GameObject.Find(nameOfObjectWithAnimationHolder).GetComponent<EtraAnimationHolder>() !=null)
        {
            objectWithEtraAnimationScript = GameObject.Find(nameOfObjectWithAnimationHolder).GetComponent<EtraAnimationHolder>();
        }
        else
        {
            Debug.LogWarning("Text label:" + nameOfObjectWithAnimationHolder + " not found or " + nameOfObjectWithAnimationHolder + " does not have a EtraAnimationHolder script attatched.");
        }

    }


    //If the player collides with the pickup...
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && objectWithEtraAnimationScript != null)
        {
            if (runAnimation)
            {
                objectWithEtraAnimationScript.runAnimation();
            }
            else
            {
                objectWithEtraAnimationScript.showAllAnimatedObjects();
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
