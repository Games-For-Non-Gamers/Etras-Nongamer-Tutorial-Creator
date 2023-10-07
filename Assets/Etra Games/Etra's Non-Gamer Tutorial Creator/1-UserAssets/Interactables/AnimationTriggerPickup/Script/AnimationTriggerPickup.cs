using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
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
            GameObject hostObject;

            GameObject tutorialUi = GameObject.Find("NonGamerTutorialManager");


            if (tutorialUi.transform.Find("NonGamerTutorialUI/UiRectTransforms/AdditionalGeneralEvents/" + nameOfObjectWithAnimationHolder))
            {
                hostObject = tutorialUi.gameObject.transform.Find("NonGamerTutorialUI/UiRectTransforms/AdditionalGeneralEvents/" + nameOfObjectWithAnimationHolder).gameObject;

            }
            else
            {
                Debug.LogWarning(nameOfObjectWithAnimationHolder + " not found");
                return;
            }

            if (hostObject.GetComponent<EtraAnimationHolder>() != null)
            {
                objectWithEtraAnimationScript = hostObject.GetComponent<EtraAnimationHolder>();
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
                RunTriggerPickupEvents();
            }

        }

        public void RunTriggerPickupEvents()
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
}