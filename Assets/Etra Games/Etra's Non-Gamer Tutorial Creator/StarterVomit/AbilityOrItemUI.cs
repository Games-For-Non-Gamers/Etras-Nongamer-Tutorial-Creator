using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static EtraUiAnimation;

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
            if (uiObject.GetComponent<Image>())
            {
                uiObject.GetComponent<Image>().enabled = true;
            }

            if (uiObject.GetComponent<Text>())
            {
                uiObject.GetComponent<Text>().enabled = true;
            }

        }
        objectsHidden = false;
    }

    public void hideAllUiObjects()
    {
        foreach (RectTransform uiObject in uiObjects)
        {
            if (uiObject.GetComponent<Image>())
            {
                uiObject.GetComponent<Image>().enabled = false;
            }

            if (uiObject.GetComponent<Text>())
            {
                uiObject.GetComponent<Text>().enabled = false;
            }
        }
        objectsHidden = true;
    }


    [Header("UI Animation Events")]
    public List<EtraUiAnimation> keyboardAnimation;
    public List<EtraUiAnimation> controllerAnimation;


    public void runUiEvent()
    {
        StartCoroutine(runKeyboardEventArray());
        //StartCoroutine(runControllerEventArray());
    }

    //Making these two enumerators right now, I'm certain this code could be better, but just making something functional rn
    IEnumerator runKeyboardEventArray()
    {

        foreach (EtraUiAnimation UiEvent in keyboardAnimation)
        {
            RectTransform obj = UiEvent.tweenedObject;
            switch (UiEvent.chosenTweenEvent)
            {
                case AnimationEvents.MoveAndScale:
                    LeanTween.move(obj, UiEvent.scaleAndMovePosition, UiEvent.scaleAndMoveTime);
                    LeanTween.scale(obj, UiEvent.scaleAndMoveScale, UiEvent.scaleAndMoveTime);
                    break;

                case AnimationEvents.Move:
                    LeanTween.move(obj, UiEvent.movePosition, UiEvent.moveTime);
                    break;
                case AnimationEvents.Scale:
                    LeanTween.scale(obj, UiEvent.scaleAndMoveScale, UiEvent.scaleAndMoveTime);
                    break;
                case AnimationEvents.Flash:


                    for (int i = 0; i < UiEvent.flashTimes; i++)
                    {
                        element.GetComponent<Image>().enabled = true;
                        yield return new WaitForSeconds(UiEvent.flashDelay);
                        element.GetComponent<Image>().enabled = false;
                        yield return new WaitForSeconds(UiEvent.flashDelay);
                    }
                    break;
                case AnimationEvents.Wait:
                    print("Ulg, glib, Pblblblblb");
                    break;
                case AnimationEvents.Fade:
                    print("Whadya want?");
                    break;
                case AnimationEvents.UnlockAbility:
                    print("Grog SMASH!");
                    break;
                case AnimationEvents.ToStartTransform:
                    print("Ulg, glib, Pblblblblb");
                    break;
                case AnimationEvents.InstantCenterObject:
                    print("Whadya want?");
                    break;
                default:
                    print("Incorrect intelligence level.");
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }


    class ObjectStarterTransform
    {
        public string objectName;
        public Transform startTransform;


        public ObjectStarterTransform(string n, Transform trans)
        {
            objectName = n;
            startTransform = trans;
        }
    }
}
