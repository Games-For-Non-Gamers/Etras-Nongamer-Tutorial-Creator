using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static EtraUiAnimation;
using static UnityEditor.Experimental.GraphView.Port;

public class AbilityOrItemUI : MonoBehaviour
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
            startPositions.Add(new ObjectStarterTransform(uiObject.name, uiObject.transform));
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
    public List<EtraUiAnimation> keyboardAnimation;
    public List<EtraUiAnimation> controllerAnimation;


    //Run into object.
    //Based off ability name get ui event.
    //PAss ability name to ui event.
    //Unlock ability by ui event sending string to character. 
    //need character ability unlock function
    //Also need sub ability unlocks.

    public void runUiEvent(AbilityScriptAndNameHolder abilityToActivate) //have parameter of event to unlock string?
    {
        StartCoroutine(runKeyboardEventArray(abilityToActivate));
        //StartCoroutine(runControllerEventArray());
    }

    //Making these two enumerators right now, I'm certain this code could be better, but just making something functional rn
    IEnumerator runKeyboardEventArray(AbilityScriptAndNameHolder abilityToActivate)
    {

        foreach (EtraUiAnimation UiEvent in keyboardAnimation)
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
                        Image image= obj.GetComponent<Image>();
                        LeanTween.color(obj, new Color(image.color.r, image.color.g, image.color.b, UiEvent.opacity), UiEvent.fadeTime).setEaseInOutSine();
                    }

                    if (obj.GetComponent<Text>())
                    {
                        Text text= obj.GetComponent<Text>();
                        LeanTween.colorText(obj, new Color(text.color.r, text.color.g, text.color.b, UiEvent.opacity), UiEvent.fadeTime).setEaseInOutSine();
                    }
                    break;

                case AnimationEvents.UnlockAbility:
                    abilityToActivate.script.unlockAbility(abilityToActivate.name);
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
                    LeanTween.move(obj, foundObjectTransform.startTransform.position, UiEvent.toStartTime).setEaseInOutSine();
                    LeanTween.rotate(obj, foundObjectTransform.startTransform.rotation.eulerAngles, UiEvent.toStartTime).setEaseInOutSine();
                    LeanTween.scale(obj, foundObjectTransform.startTransform.localScale, UiEvent.toStartTime).setEaseInOutSine();
                    break;

                case AnimationEvents.InstantCenterObject:
                    LeanTween.move(obj, Vector3.zero, 0);
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
        public Transform startTransform;


        public ObjectStarterTransform()
        {

        }

        public ObjectStarterTransform(string n, Transform trans)
        {
            objectName = n;
            startTransform = trans;
        }
    }
}
