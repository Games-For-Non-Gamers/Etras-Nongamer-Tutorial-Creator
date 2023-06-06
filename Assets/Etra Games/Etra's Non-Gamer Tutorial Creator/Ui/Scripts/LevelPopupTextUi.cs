using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Etra.NonGamerTutorialCreator.EtraUiAnimation;
using UnityEngine.UI;

namespace Etra.NonGamerTutorialCreator
{
    public class LevelPopupTextUi : MonoBehaviour
    {
        public RectTransform[] uiObjects;
        public bool showUiIfAbilityIsActive = true;
        bool objectsHidden = false;
        List<ObjectStarterTransform> startPositions = new List<ObjectStarterTransform>();


        private void Start()
        {
            if (!showUiIfAbilityIsActive)
            {
                hideAllUiObjects();
            }
            //Get the transforms of all ui objects for AnimationEvents.ToStartTransform:
            foreach (RectTransform uiObject in uiObjects)
            {
                startPositions.Add(new ObjectStarterTransform(uiObject.name, uiObject.localPosition, uiObject.localScale));
            }


        }

        public void showAllUiObjects()
        {

            foreach (RectTransform uiObject in uiObjects)
            {
                showOrHideUiObject(uiObject, true);
            }
            objectsHidden = false;
        }

        public void hideAllUiObjects()
        {
            foreach (RectTransform uiObject in uiObjects)
            {
                showOrHideUiObject(uiObject, false);
            }
            objectsHidden = true;
        }

        public void showOrHideUiObject(RectTransform uiObject, bool visibility)
        {
            if (uiObject.GetComponent<Image>())
            {
                uiObject.GetComponent<Image>().enabled = visibility;
            }

            if (uiObject.GetComponent<Text>())
            {
                uiObject.GetComponent<Text>().enabled = visibility;
            }
        }


        [Header("UI Animation Events")]
        public List<EtraUiAnimation> textAnimation;


        public void runUiEvent() //have parameter of event to unlock string?
        {
            StartCoroutine(runUiEventArray());
        }

        //Making these two enumerators right now, I'm certain this code could be better, but just making something functional rn
        IEnumerator runUiEventArray()
        {

            foreach (EtraUiAnimation UiEvent in textAnimation)
            {
                RectTransform obj = UiEvent.tweenedObject;
                switch (UiEvent.chosenTweenEvent)
                {
                    case AnimationEvents.MoveAndScale:
                        LeanTween.move(obj, UiEvent.scaleAndMovePosition, UiEvent.scaleAndMoveTime).setEaseInOutSine();
                        LeanTween.scale(obj, UiEvent.scaleAndMoveScale, UiEvent.scaleAndMoveTime).setEaseInOutSine();
                        break;

                    case AnimationEvents.Move:
                        LeanTween.move(obj, UiEvent.movePosition, UiEvent.moveTime).setEaseInOutSine();
                        break;

                    case AnimationEvents.Scale:
                        LeanTween.scale(obj, UiEvent.scaleAndMoveScale, UiEvent.scaleAndMoveTime).setEaseInOutSine();
                        break;

                    case AnimationEvents.Flash:
                        for (int i = 0; i < UiEvent.flashTimes; i++)
                        {
                            showOrHideUiObject(obj, true);
                            yield return new WaitForSeconds(UiEvent.flashDelay);
                            showOrHideUiObject(obj, false);
                            yield return new WaitForSeconds(UiEvent.flashDelay);
                        }
                        break;

                    case AnimationEvents.Wait:
                        yield return new WaitForSeconds(UiEvent.secondsToWait);
                        break;

                    case AnimationEvents.Fade:

                        if (obj.GetComponent<Image>())
                        {
                            Image image = obj.GetComponent<Image>();
                            LeanTween.color(obj, new Color(image.color.r, image.color.g, image.color.b, UiEvent.opacity), UiEvent.fadeTime).setEaseInOutSine();
                        }

                        if (obj.GetComponent<Text>())
                        {
                            Text text = obj.GetComponent<Text>();
                            LeanTween.colorText(obj, new Color(text.color.r, text.color.g, text.color.b, UiEvent.opacity), UiEvent.fadeTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.UnlockAbilityOrItem:

                        Debug.LogWarning("Text popup does not unlock any abilities");
                        break;

                    case AnimationEvents.ToStartTransform:

                        //Find the element
                        ObjectStarterTransform foundObjectTransform = new ObjectStarterTransform();

                        foreach (ObjectStarterTransform uitransform in startPositions)
                        {
                            if (uitransform.objectName == obj.name)
                            {
                                foundObjectTransform = uitransform;
                            }
                        }
                        //Move to that pos, rot, and scale
                        LeanTween.move(obj, foundObjectTransform.startPosition, UiEvent.toStartTime).setEaseInOutSine();
                        // LeanTween.rotate(obj, foundObjectTransform.startPosition.rotation.eulerAngles, UiEvent.toStartTime).setEaseInOutSine();
                        LeanTween.scale(obj, foundObjectTransform.startScale, UiEvent.toStartTime).setEaseInOutSine();
                        break;

                    case AnimationEvents.InstantCenterAndZeroScaleObject:
                        LeanTween.move(obj, Vector3.zero, 0);
                        LeanTween.scale(obj, Vector3.zero, 0);
                        break;

                    case AnimationEvents.MakeVisible:
                        showOrHideUiObject(obj, true);
                        break;

                    case AnimationEvents.PlaySfx:
                        if (UiEvent.sfxName == "" || UiEvent.sfxName == null)
                        {
                            transform.parent.parent.GetComponent<AudioManager>().Play("UiElementMove");
                        }
                        else
                        {
                            transform.parent.parent.GetComponent<AudioManager>().Play(UiEvent.sfxName);
                        }
                        break;

                    default:
                        Debug.Log("Invalid Animation Event");
                        break;
                }

            }
        }




        class ObjectStarterTransform
        {
            public string objectName;
            public Vector3 startPosition;
            public Vector3 startScale;


            public ObjectStarterTransform()
            {

            }

            public ObjectStarterTransform(string n, Vector3 pos, Vector3 scale)
            {
                objectName = n;
                startPosition = pos;
                startScale = scale;
            }
        }
    }
}