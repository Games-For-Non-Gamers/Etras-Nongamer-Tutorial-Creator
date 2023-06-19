using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Etra.NonGamerTutorialCreator.EtraAnimationEvent;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

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
        private void Reset()
        {
            OnValidate();
        }
        private void OnValidate()
        {
            if (sfxPlayer == null && transform.parent.parent.GetComponent<NonGamerTutorialUi>()) //so the controller and keyboard sfx don't play over each other
            {
                sfxPlayer = transform.parent.parent.GetComponent<AudioManager>();
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
                        ObjectStarterTransform foundObjectTransform = new ObjectStarterTransform();

                        foreach (ObjectStarterTransform objTransform in startPositions)
                        {
                            if (objTransform.objectName == animEvent.tweenedObject.name)
                            {
                                foundObjectTransform = objTransform;
                            }
                        }

                        if (isRectTransform)
                        {
                            LeanTween.move(animEvent.tweenedObject, foundObjectTransform.startPosition, animEvent.toStartTime).setEaseInOutSine();
                            LeanTween.scale(animEvent.tweenedObject, foundObjectTransform.startScale, animEvent.toStartTime).setEaseInOutSine();
                        }
                        else
                        {
                            LeanTween.move(animEvent.tweenedObject, foundObjectTransform.startPosition, animEvent.toStartTime).setEaseInOutSine();
                            LeanTween.scale(animEvent.tweenedObject, foundObjectTransform.startScale, animEvent.toStartTime).setEaseInOutSine();
                        }
                        break;

                    case AnimationEvents.Flash:
                        //Shouldn't need an "isRectTransform", I think...
                        for (int i = 0; i < animEvent.flashTimes; i++)
                        {
                            showOrHideUiObject(animEvent.tweenedObject, true);
                            yield return new WaitForSeconds(animEvent.flashDelay);
                            showOrHideUiObject(animEvent.tweenedObject, false);
                            yield return new WaitForSeconds(animEvent.flashDelay);
                        }
                        showOrHideUiObject(animEvent.tweenedObject, true);
                        break;

                    case AnimationEvents.FadeIn:


                        showOrHideUiObject(animEvent.tweenedObject, true);
                        if (animEvent.tweenedObject.GetComponent<MeshRenderer>())
                        {
                            Material material = animEvent.tweenedObject.GetComponent<MeshRenderer>().material;
                            Color color = material.color;
                            color.a = 0;
                            material.color = color;
                            LeanTween.value(this.gameObject, 0, 255, animEvent.fadeInTime).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });

                        }

                        if (animEvent.tweenedObject.GetComponent<Image>())
                        {
                            Image image = animEvent.tweenedObject.GetComponent<Image>();
                            LeanTween.color(animEvent.tweenedObject, new Color(image.color.r, image.color.g, image.color.b, 0), 0);
                            image.enabled = true;
                            LeanTween.color(animEvent.tweenedObject, new Color(image.color.r, image.color.g, image.color.b, animEvent.fadeInOpacity), animEvent.fadeInTime).setEaseInOutSine();
                        }

                        if (animEvent.tweenedObject.GetComponent<Text>() && animEvent.rectTransform != null)
                        {
                            Text text = animEvent.tweenedObject.GetComponent<Text>();
                            LeanTween.colorText(animEvent.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 0), 0);
                            text.enabled = true;
                            LeanTween.colorText(animEvent.rectTransform, new Color(text.color.r, text.color.g, text.color.b, animEvent.fadeInOpacity), animEvent.fadeInTime).setEaseInOutSine();
                        }

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
                            LeanTween.color(animEvent.tweenedObject, new Color(image.color.r, image.color.g, image.color.b, 0), animEvent.fadeOutTime).setEaseInOutSine();
                        }

                        if (animEvent.tweenedObject.GetComponent<Text>() && animEvent.rectTransform != null)
                        {
                            Text text = animEvent.tweenedObject.GetComponent<Text>();
                            LeanTween.colorText(animEvent.rectTransform, new Color(text.color.r, text.color.g, text.color.b, 0), animEvent.fadeOutTime).setEaseInOutSine();
                        }

                        showOrHideUiObject(animEvent.tweenedObject, false);
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


                        if (abilityToActivate == null &&  selectedItem == null)
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
                    default:
                        Debug.Log("Invalid Animation Event");
                        break;
                }
                indexTracker++;

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