using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Etra.NonGamerTutorialCreator.EtraAnimationEvent;

namespace Etra.NonGamerTutorialCreator
{
    public class EtraAnimationHolder : MonoBehaviour
    {
        public GameObject[] animatedObjects;
        public bool hideAllObjectsAtStart = true;
        public AudioManager sfxPlayer;
        public bool separateKeyboardControllerAnimations = false;
        public List<EtraAnimationEvent> standardAnimation;
        public List<EtraAnimationEvent> keyboardAnimation;
        public List<EtraAnimationEvent> controllerAnimation;
        [HideInInspector]
        public bool objectsHidden = false;
        List<ObjectStarterTransform> startPositions = new List<ObjectStarterTransform>();
        EtraCharacterMainController etraCharacterMainController;

        [Header("Duplication Ranges")]
        public int duplicateStartIndex;
        public int duplicateEndIndex;
        [ContextMenu("Duplicate range Standard")]
        void duplicateRangeStandard()
        {
            duplicateRange(standardAnimation);
        }
        [ContextMenu("Duplicate range Keyboard")]
        void duplicateRangeKeyboard()
        {
            duplicateRange(keyboardAnimation);
        }
        [ContextMenu("Duplicate range Controller")]
        void duplicateRangeController()
        {
            duplicateRange(controllerAnimation);
        }


        void duplicateRange(List<EtraAnimationEvent> l)
        {
            for (int i = duplicateStartIndex; i < duplicateEndIndex +1; i++)
            {
                EtraAnimationEvent elementToDuplicate = l[i];
                EtraAnimationEvent duplicatedElement = new EtraAnimationEvent();
                duplicatedElement = elementToDuplicate;

                // Insert the duplicated element at the next index in the list
                l.Add(duplicatedElement);
            }
 
        }

        private void Reset()
        {
            OnValidate();
        }
        private void OnValidate()
        {
            if (sfxPlayer == null && transform.parent) //so the controller and keyboard sfx don't play over each other
            {
                if (transform.parent.parent)
                {
                    if (transform.parent.parent.GetComponent<NonGamerTutorialUi>())
                    {
                        sfxPlayer = transform.parent.parent.GetComponent<AudioManager>();
                    }
                }
                
            }
        }


        private void Start()
        {
            etraCharacterMainController = EtraCharacterMainController.Instance;
            if (hideAllObjectsAtStart)
            {
                hideAllAnimatedObjects();
            }

            //Get the transforms of all ui objects for AnimationEvents.ToStartTransform:
            foreach (GameObject uiObject in animatedObjects)
            {
                //If rect transform
                if (uiObject.GetComponent<RectTransform>())
                {
                    startPositions.Add(new ObjectStarterTransform(uiObject.GetComponent<RectTransform>().name, uiObject.GetComponent<RectTransform>().position, uiObject.GetComponent<RectTransform>().localScale));
                }
                //If gameobject
                else
                {
                    startPositions.Add(new ObjectStarterTransform(uiObject.name, uiObject.transform.localPosition, uiObject.transform.localScale));
                }
            }

            //load rect transform if object is rect transform
            foreach (EtraAnimationEvent eventFrame in standardAnimation)
            {
                if (eventFrame.tweenedObject.GetComponent<RectTransform>())
                {
                    eventFrame.rectTransform = eventFrame.tweenedObject.GetComponent<RectTransform>();
                }
            }
            foreach (EtraAnimationEvent eventFrame in keyboardAnimation)
            {
                if (eventFrame.tweenedObject.GetComponent<RectTransform>())
                {
                    eventFrame.rectTransform = eventFrame.tweenedObject.GetComponent<RectTransform>();
                }
            }
            foreach (EtraAnimationEvent eventFrame in controllerAnimation)
            {
                if (eventFrame.tweenedObject.GetComponent<RectTransform>())
                {
                    eventFrame.rectTransform = eventFrame.tweenedObject.GetComponent<RectTransform>();
                }
            }


        }

        public void showAllAnimatedObjects()
        {

            foreach (GameObject uiObject in animatedObjects)
            {
                showOrHideUiObject(uiObject, true);
            }
            objectsHidden = false;
        }

        public void hideAllAnimatedObjects()
        {
            foreach (GameObject uiObject in animatedObjects)
            {
                showOrHideUiObject(uiObject, false);
            }
            objectsHidden = true;
        }

        public void showOrHideUiObject(GameObject uiObject, bool visibility)
        {
            if (uiObject.GetComponent<Image>())
            {
                uiObject.GetComponent<Image>().enabled = visibility;
            }

            if (uiObject.GetComponent<MeshRenderer>())
            {
                uiObject.GetComponent<MeshRenderer>().enabled = visibility;
            }

            if (uiObject.GetComponent<Text>())
            {
                uiObject.GetComponent<Text>().enabled = visibility;
            }

            if (uiObject.GetComponent<TextMeshPro>())
            {
                uiObject.GetComponent<TextMeshPro>().enabled = visibility;
            }

            if (uiObject.GetComponent<TextMeshProUGUI>())
            {
                uiObject.GetComponent<TextMeshProUGUI>().enabled = visibility;
            }

        }

        bool getVisibility(GameObject uiObject)
        {
            if (uiObject.GetComponent<Image>())
            {
                if (!uiObject.GetComponent<Image>().enabled)
                {
                    return false;
                }
            }

            if (uiObject.GetComponent<MeshRenderer>())
            {
                if (!uiObject.GetComponent<MeshRenderer>().enabled)
                {
                    return false;
                }
            }

            if (uiObject.GetComponent<Text>())
            {
                if (!uiObject.GetComponent<Text>().enabled)
                {
                    return false;
                }
            }

            if (uiObject.GetComponent<TextMeshPro>())
            {
                if (!uiObject.GetComponent<TextMeshPro>().enabled)
                {
                    return false;
                }
            }

            if (uiObject.GetComponent<TextMeshProUGUI>())
            {
                if (!uiObject.GetComponent<TextMeshProUGUI>().enabled)
                {
                    return false;
                }
            }
            return true;

        }



        //Ran by most scripts
        public void runAnimation()
        {
            runAnimation(null, null, true);
        }

        //Ran by non-gamer tutorial pickup
        public void runAnimation(AbilityScriptAndNameHolder abilityToActivate, ItemScriptAndNameHolder selectedItem, bool isAbility) //have parameter of event to unlock string?
        {
            if (separateKeyboardControllerAnimations)
            {
                StartCoroutine(runAnimationCoroutine(abilityToActivate, selectedItem, isAbility, true));
                StartCoroutine(runAnimationCoroutine(abilityToActivate, selectedItem, isAbility, false));
            }
            else
            {
                StartCoroutine(runAnimationCoroutine(abilityToActivate, selectedItem, isAbility, true));
            }

        }

        //Making these two enumerators right now, I'm certain this code could be better, but just making something functional rn
        IEnumerator runAnimationCoroutine(AbilityScriptAndNameHolder abilityToActivate, ItemScriptAndNameHolder selectedItem, bool isAbility, bool isKeyboard)
        {
            int indexTracker = 0;
            List<EtraAnimationEvent> animToScroll;

            if (!separateKeyboardControllerAnimations)
            {
                animToScroll = standardAnimation;
            }
            else if (isKeyboard)
            {
                animToScroll = keyboardAnimation;
            }
            else
            {
                animToScroll = controllerAnimation;
            }


            foreach (EtraAnimationEvent animEvent in animToScroll)
            {
                bool isRectTransform = false;
                if (animEvent.rectTransform != null)
                {
                    isRectTransform = true;
                }

                switch (animEvent.chosenEvent)
                {
                    case AnimationEvents.MoveAndScale:
                        if (isRectTransform)
                        {
                            LeanTween.move(animEvent.rectTransform, animEvent.scaleAndMovePosition, animEvent.scaleAndMoveTime).setEaseInOutSine();
                            LeanTween.scale(animEvent.rectTransform, animEvent.scaleAndMoveScale, animEvent.scaleAndMoveTime).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.move(animEvent.tweenedObject, animEvent.scaleAndMovePosition, animEvent.scaleAndMoveTime).setEaseInOutSine();
                            LeanTween.scale(animEvent.tweenedObject, animEvent.scaleAndMoveScale, animEvent.scaleAndMoveTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.Move:
                        if (isRectTransform)
                        {
                            LeanTween.move(animEvent.rectTransform, animEvent.movePosition, animEvent.moveTime).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.move(animEvent.tweenedObject, animEvent.movePosition, animEvent.moveTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.Scale:
                        if (isRectTransform)
                        {
                            LeanTween.scale(animEvent.rectTransform, animEvent.scaleScale, animEvent.scaleTime).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.scale(animEvent.tweenedObject, animEvent.scaleScale, animEvent.scaleTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.Wait:
                        yield return new WaitForSeconds(animEvent.secondsToWait);
                        break;

                    case AnimationEvents.ToStartTransform:

                        //Find the element

                        ObjectStarterTransform foundObjectTransform1 = new ObjectStarterTransform();

                        foreach (ObjectStarterTransform objTransform in startPositions)
                        {
                            if (objTransform.objectName == animEvent.tweenedObject.name)
                            {
                                foundObjectTransform1 = objTransform;
                            }
                        }


                        LeanTween.move(animEvent.tweenedObject, foundObjectTransform1.startPosition, 1).setEaseInOutSine();
                        LeanTween.scale(animEvent.tweenedObject, foundObjectTransform1.startScale, 1).setEaseInOutSine();



                        break;

                    case AnimationEvents.Flash:

                        //(GameObject obj, float flashTimes, float delay)
                        StartCoroutine(flashCoroutine(animEvent.tweenedObject, animEvent.flashTimes, animEvent.flashDelay));
                        break;

                    case AnimationEvents.FadeIn:
                        fadeInObject(animEvent.tweenedObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                        break;

                    case AnimationEvents.FadeOut:
                        if (animEvent.tweenedObject.GetComponent<MeshRenderer>())
                        {
                            Material material = animEvent.tweenedObject.GetComponent<MeshRenderer>().material;
                            Color color = material.color;
                            LeanTween.value(this.gameObject, color.a, 0, animEvent.fadeOutTime).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });

                        }

                        if (animEvent.tweenedObject.GetComponent<Image>())
                        {
                            Image image = animEvent.tweenedObject.GetComponent<Image>();
                            LeanTween.color(animEvent.rectTransform, new Color(image.color.r, image.color.g, image.color.b, 0), animEvent.fadeOutTime);
                        }

                        if (animEvent.tweenedObject.GetComponent<Text>() && animEvent.rectTransform != null)
                        {
                            Text text = animEvent.tweenedObject.GetComponent<Text>();
                            LeanTween.colorText(animEvent.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 0), animEvent.fadeOutTime).setEaseInOutSine();
                        }

                        //showOrHideUiObject(animEvent.tweenedObject, false);
                        break;


                    case AnimationEvents.Show:
                        showOrHideUiObject(animEvent.tweenedObject, true);
                        break;

                    case AnimationEvents.Hide:
                        showOrHideUiObject(animEvent.tweenedObject, false);
                        break;

                    case AnimationEvents.ShowAll:
                        showAllAnimatedObjects();
                        break;

                    case AnimationEvents.HideAll:
                        hideAllAnimatedObjects();
                        break;


                    case AnimationEvents.InstantCenterAndZeroScaleObject:
                        if (isRectTransform)
                        {
                            LeanTween.move(animEvent.rectTransform, Vector3.zero, 0).setEaseInOutSine();
                            LeanTween.scale(animEvent.rectTransform, Vector3.zero, 0).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.move(animEvent.tweenedObject, Vector3.zero, 0).setEaseInOutSine();
                            LeanTween.scale(animEvent.tweenedObject, Vector3.zero, 0).setEaseInOutSine();
                        }

                        break;

                    case AnimationEvents.PlaySfx:
                        if (separateKeyboardControllerAnimations)
                        {
                            keyOrContSfx(isKeyboard, indexTracker, animEvent.sfxName);
                        }
                        else
                        {
                            if (sfxPlayer != null)
                            {
                                sfxPlayer.Play(animEvent.sfxName);
                            }
                            else{
                                Debug.LogWarning("You must set the sfx player variable to play sfx");
                            }
                           
                        }

                        break;

                    case AnimationEvents.UnlockAbilityOrItem:
                        unlockAbility(abilityToActivate, selectedItem, isAbility);
                        break;

                    case AnimationEvents.UnlockPlayer:
                        etraCharacterMainController.enableAllActiveAbilities();
                        break;

                    case AnimationEvents.LockPlayer:
                        etraCharacterMainController.disableAllActiveAbilities();
                        break;

                    case AnimationEvents.Rotate:
                        if (isRectTransform)
                        {
                            LeanTween.rotate(animEvent.rectTransform, animEvent.rotation, animEvent.rotateTime).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.rotate(animEvent.tweenedObject, animEvent.rotation, animEvent.rotateTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.MoveToGameObject:
                            LeanTween.move(animEvent.tweenedObject, animEvent.moveToObjectGameobject.transform.position + animEvent.addedPosition, animEvent.moveToObjectTime).setEaseInOutSine();

                        break;

                    case AnimationEvents.RotateToGameObject:
                            LeanTween.rotate(animEvent.tweenedObject, animEvent.rotToObjectGameobject.transform.rotation.eulerAngles, animEvent.rotToObjectTime).setEaseInOutSine();
                        break;

                    case AnimationEvents.RunEtraAnimationActivatedScript:
                        animEvent.etraAnimationActivatedScript.runScript(animEvent.passedString);
                        break;

                    case AnimationEvents.BasicUiGrowAndToStartWithMidUnlock:
                        StartCoroutine(basicUiGrowAndToStartCoroutine(animEvent.tweenedObject, animEvent.basicGrowPos, animEvent.basicGrowScale, animEvent.basicGrowWait, abilityToActivate, selectedItem, isAbility, true, isKeyboard, indexTracker, animEvent.sfxName));
                        break;
                    case AnimationEvents.BasicUiGrowAndToStartWithInstantUnlock:
                        unlockAbility(abilityToActivate, selectedItem, isAbility);
                        StartCoroutine(basicUiGrowAndToStartCoroutine(animEvent.tweenedObject, animEvent.basicGrowPos, animEvent.basicGrowScale, animEvent.basicGrowWait, abilityToActivate, selectedItem, isAbility, false, isKeyboard, indexTracker, animEvent.sfxName));
                        break;
                    case AnimationEvents.BasicUiGrowAndToStartWithNoUnlock:
                        StartCoroutine(basicUiGrowAndToStartCoroutine(animEvent.tweenedObject, animEvent.basicGrowPos, animEvent.basicGrowScale, animEvent.basicGrowWait, abilityToActivate, selectedItem, isAbility, false, isKeyboard, indexTracker, animEvent.sfxName));
                        break;

                    case AnimationEvents.FadeInIfNotVisible:

                        if (getVisibility(animEvent.tweenedObject) == false)
                        {
                            fadeInObject(animEvent.tweenedObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                        }
                        break;

                    case AnimationEvents.FadeInSelfAndChildren:
                        fadeInObject(animEvent.tweenedObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                        foreach (Transform child in animEvent.tweenedObject.transform)
                        {
                            fadeInObject(child.gameObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                        }
                        break;

                    case AnimationEvents.FadeInSelfAndChildrenIfNotVisible:

                        if (getVisibility(animEvent.tweenedObject) == false)
                        {
                            fadeInObject(animEvent.tweenedObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                        }

                        foreach (Transform child in animEvent.tweenedObject.transform)
                        {
                            if (getVisibility(child.gameObject) == false)
                            {
                                fadeInObject(child.gameObject, animEvent.fadeInOpacity, animEvent.fadeInTime);
                            }


                        }
                        break;

                    default:
                        Debug.Log("Invalid Animation Event");
                        break;
                }
                indexTracker++;

            }
        }

        void fadeInObject(GameObject tweenedObject, float fadeInOpacity, float fadeInTime)
        {
            if (tweenedObject.GetComponent<MeshRenderer>())
            {
                Material material = tweenedObject.GetComponent<MeshRenderer>().material;
                Color color = material.color;
                color.a = 0;
                material.color = color;
                tweenedObject.GetComponent<MeshRenderer>().enabled = true;
                LeanTween.value(this.gameObject, 0, fadeInOpacity, fadeInTime).setEaseInOutSine().setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });

            }

            if (tweenedObject.GetComponent<Image>())
            {
                Image image = tweenedObject.GetComponent<Image>();
                LeanTween.color(tweenedObject.GetComponent<RectTransform>(), new Color(image.color.r, image.color.g, image.color.b, 0), 0);
                image.enabled = true;
                LeanTween.color(tweenedObject.GetComponent<RectTransform>(), new Color(image.color.r, image.color.g, image.color.b, fadeInOpacity), fadeInTime).setEaseInOutSine();
            }

            if (tweenedObject.GetComponent<Text>())
            {
                Text text = tweenedObject.GetComponent<Text>();
                LeanTween.colorText(tweenedObject.GetComponent<RectTransform>(), new Color(text.color.r, text.color.g, text.color.b, 0), 0);
                text.enabled = true;
                LeanTween.colorText(tweenedObject.GetComponent<RectTransform>(), new Color(text.color.r, text.color.g, text.color.b, fadeInOpacity), fadeInTime).setEaseInOutSine();
            }
        }

        IEnumerator flashCoroutine(GameObject obj, float flashTimes, float delay)
        {
            //Shouldn't need an "isRectTransform", I think...
            for (int i = 0; i < flashTimes; i++)
            {
                showOrHideUiObject(obj, true);
                yield return new WaitForSeconds(delay);
                showOrHideUiObject(obj, false);
                yield return new WaitForSeconds(delay);
            }
            showOrHideUiObject(obj, true);
        }

        IEnumerator basicUiGrowAndToStartCoroutine(GameObject obj, Vector3 basicGrowPos, Vector3 basicGrowScale, float basicGrowWait, AbilityScriptAndNameHolder abilityToActivate, ItemScriptAndNameHolder selectedItem, bool isAbility, bool unlockAbilityBool, bool isKeyboard, int indexTracker, string sfxName)
        {
            if (basicGrowWait < 2)
            {
                Debug.LogError("basicUiGrowAndToStart's wait must be above 2 seconds");
                yield break;
            }

            RectTransform rect = obj.GetComponent<RectTransform>();
            if (obj.GetComponent<Image>().enabled == false)
            {
                
                LeanTween.move(rect, basicGrowPos, 0).setEaseInOutSine();
                LeanTween.scale(rect, Vector3.zero, 0).setEaseInOutSine();
                showOrHideUiObject(obj, true);
            }

            LeanTween.move(rect, basicGrowPos, 1).setEaseInOutSine();
            LeanTween.scale(rect, basicGrowScale, 1).setEaseInOutSine();
            yield return new WaitForSeconds(basicGrowWait-2);

            if (unlockAbilityBool)
            {
                unlockAbility(abilityToActivate, selectedItem, isAbility);
            }

            if (separateKeyboardControllerAnimations)
            {
                keyOrContSfx(isKeyboard, indexTracker, sfxName);
            }
            else
            {
                if (sfxPlayer != null)
                {
                    sfxPlayer.Play(sfxName);
                }
                else
                {
                    Debug.LogWarning("You must set the sfx player variable to play sfx");
                }

            }

            //Find the element
            ObjectStarterTransform foundObjectTransform1 = new ObjectStarterTransform();

            foreach (ObjectStarterTransform objTransform in startPositions)
            {
                if (objTransform.objectName == obj.name)
                {
                    foundObjectTransform1 = objTransform;
                }
            }


            LeanTween.move(obj, foundObjectTransform1.startPosition, 1).setEaseInOutSine();
            LeanTween.scale(obj, foundObjectTransform1.startScale, 1).setEaseInOutSine();


        }

        void unlockAbility(AbilityScriptAndNameHolder abilityToActivate, ItemScriptAndNameHolder selectedItem, bool isAbility)
        {
            if (abilityToActivate == null && selectedItem == null)
            {
                Debug.LogWarning("Please use the non-gamer tutorial pickup to unlock abilities or items.");
            }
            else
            {
                if (isAbility)
                {
                    EtraAbilityBaseClass abilityScriptOnCharacter = (EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(abilityToActivate.script.GetType());
                    abilityScriptOnCharacter.unlockAbility(abilityToActivate.name);
                }
                else //is Item
                {
                    //Add the script to the item manager
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(selectedItem.script.GetType());
                    //Update the items array
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();
                    //Equip the new item
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipLastItem();
                }
            }
        }


        private List<int> keyIndexes = new List<int>();
        private List<int> contIndexes = new List<int>();
        bool sfxWillPlayIfPossible = false;
        void keyOrContSfx(bool isKeyboard, int index, string sfxName)
        {

            if (sfxPlayer == null)
            {
                Debug.LogWarning("Cannot play SFX in " + this.name + " without sfxPlayer set to a value");
                return;
            }

            if (sfxName == "" || sfxName == null)
            {
                if (transform.parent.parent.GetComponent<AudioManager>() != null)
                {
                    if (sfxPlayer == transform.parent.parent.GetComponent<AudioManager>())
                    {
                        sfxName = "UiElementMove";
                    }
                }
            }

            sfxWillPlayIfPossible = false;
            if (isKeyboard && FindObjectOfType<NonGamerTutorialUi>().keyboardUi.activeInHierarchy)
            {
                sfxWillPlayIfPossible = true;
            }
            else if (!isKeyboard && FindObjectOfType<NonGamerTutorialUi>().controllerUi.activeInHierarchy)
            {
                sfxWillPlayIfPossible = true;
            }


            if (isKeyboard && !keyIndexes.Contains(index))
            {
                keyIndexes.Add(index);

                if (sfxWillPlayIfPossible)
                {
                    sfxPlayer.Play(sfxName);
                }
            }
            if (!isKeyboard && !contIndexes.Contains(index))
            {
                contIndexes.Add(index);

                if (sfxWillPlayIfPossible)
                {
                    sfxPlayer.Play(sfxName);
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