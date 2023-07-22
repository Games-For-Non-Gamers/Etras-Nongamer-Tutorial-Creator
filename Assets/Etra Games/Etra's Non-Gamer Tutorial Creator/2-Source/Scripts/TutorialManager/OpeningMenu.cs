using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using EtrasStarterAssets;
using UnityEngine.EventSystems;
using Etra.StandardMenus;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class OpeningMenu : MonoBehaviour
    {
        [Header("Basic")]
        public bool skipMenu;
        public GameObject openingMenuUi;
        private OpeningScene opening;
        private Button[] allButtons;
        private RectTransform[] allRectTransforms;
        AudioManager audioManager;
        [HideInInspector]public bool canBeginGame = true;
        private void Start()
        {
            // Find the OpeningMenuUi script in the scene
            opening = GetComponent<OpeningScene>();
            //Get all ui objects to fade out later
            openingMenuUi.gameObject.SetActive(true);
            allButtons = openingMenuUi.GetComponentsInChildren<Button>();
            allRectTransforms = openingMenuUi.GetComponentsInChildren<RectTransform>();

            audioManager = GetComponent<AudioManager>();
            audioManager.Play("BackgroundMusic");

            if (skipMenu)
            {
                // If skipMenu is true, disable the menu, run the opening scene, and return
                toEndState();
                return;
            }

            // Activate the openingMenuUi object

            // Unlock Cursor
            EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(false);

        }

        public void startPressed()
        {
            if (!canBeginGame){return;}
            // Lock cursor
            EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(true);
            // Disable all the buttons in the menu
            disableButtons();
            // Start the coroutine for the menu closing animation
            StartCoroutine(CloseMenuAnimation());
        }

        private IEnumerator CloseMenuAnimation()
        {
            // Iterate through all the RectTransforms in the allRectTransforms array
            foreach (RectTransform rect in allRectTransforms)
            {
                Image image = rect.GetComponent<Image>();
                TextMeshProUGUI text = rect.GetComponent<TextMeshProUGUI>();

                // Check if the current RectTransform has an Image component
                if (image != null)
                {
                    // Use LeanTween to fade out the image by modifying its alpha value over time
                    LeanTween.value(rect.gameObject, image.color.a, 0, 1f).setOnUpdate((float newAlpha) =>
                    {
                        Color e = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
                        image.color = e;
                    });
                }
                // Check if the current RectTransform has a TextMeshProUGUI component
                else if (text != null)
                {
                    // Use LeanTween to fade out the TextMeshProUGUI text by modifying its color alpha value over time
                    LeanTween.value(rect.gameObject, text.color.a, 0, 1f).setOnUpdate((float newAlpha) =>
                    {
                        Color e = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
                        text.color = e;
                    });
                }
            }
            yield return new WaitForSeconds(1f);
            toEndState();
        }

        void toEndState()
        {
            //Make sure all values get to end state in case menu is skipped
            EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(true);
            disableButtons();
            openingMenuUi.gameObject.SetActive(false);
            opening.RunOpeningScene();
        }

        void disableButtons()
        {
            // Disable all the buttons in the allButtons array
            foreach (Button button in allButtons)
            {
                button.enabled = false;
            }
        }
    }
}