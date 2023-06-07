using Etra.StarterAssets.Source;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace Etra.NonGamerTutorialCreator{
    public class NonGamerTutorialUi : MonoBehaviour
    {
        public enum ActiveUi
        {
            AutoSwitch,
            Xbox360,
            XboxOne,
            XboxSeries,
            Ps4,
            Ps5,
            SwitchPro,
            Steam,
            Ouya,
            Keyboard
        }

        public enum UiTypes
        {
            Xbox360,
            XboxOne,
            XboxSeries,
            Ps4,
            Ps5,
            SwitchPro,
            Steam,
            Ouya,
            Keyboard
        }


        [Header("Current UI")]
        public ActiveUi activeUi;
        public bool allowAutoSwitchToKeyboard = true;

        [Header("General References")]
        public GameObject keyboardUi;
        public GameObject controllerUi;
        public GameObject AbilityAndItemAnimationData;

        [Header("Controller Button References")]
        public RectTransform leftJoystick;
        public RectTransform rightJoystick;
        public RectTransform leftShoulder;
        public RectTransform leftTrigger;
        public RectTransform rightShoulder;
        public RectTransform rightTrigger;
        public RectTransform start;
        public RectTransform select;
        public RectTransform upFace;
        public RectTransform downFace;
        public RectTransform leftFace;
        public RectTransform rightFace;
        public RectTransform fadedUpFace;
        public RectTransform fadedDownFace;
        public RectTransform fadedLeftFace;
        public RectTransform fadedRightFace;
        public RectTransform upDpad;
        public RectTransform downDpad;
        public RectTransform leftDpad;
        public RectTransform rightDpad;
        public RectTransform dpadBase;
        //need to add dpad and start buttons here in the future, along with additions to the 

        public ControllerUIImages[] controllerImages;

        private void OnValidate()
        {
            switchFromActiveUi();
        }

        void switchFromActiveUi()
        {
            UiTypes translatedNum = (UiTypes)(int)activeUi - 1;
            if ((int)translatedNum != -1)
            {
                switchUi(translatedNum);
            }
        }

        public void switchUi(UiTypes uiType)
        {
            if (uiType == UiTypes.Keyboard)
            {
                keyboardUi.SetActive(true);
                controllerUi.SetActive(false);

            }
            else //Controller
            {
                keyboardUi.SetActive(false);
                controllerUi.SetActive(true);
                int enumValue = (int)uiType;

                leftJoystick.GetComponent<Image>().sprite = controllerImages[enumValue].leftJoystick;
                rightJoystick.GetComponent<Image>().sprite = controllerImages[enumValue].rightJoystick;
                leftShoulder.GetComponent<Image>().sprite = controllerImages[enumValue].leftShoulder;
                leftTrigger.GetComponent<Image>().sprite = controllerImages[enumValue].leftTrigger;
                rightShoulder.GetComponent<Image>().sprite = controllerImages[enumValue].rightShoulder;
                rightTrigger.GetComponent<Image>().sprite = controllerImages[enumValue].rightTrigger;
                start.GetComponent<Image>().sprite = controllerImages[enumValue].start;
                select.GetComponent<Image>().sprite = controllerImages[enumValue].select;
                upFace.GetComponent<Image>().sprite = controllerImages[enumValue].upFace;
                downFace.GetComponent<Image>().sprite = controllerImages[enumValue].downFace;
                leftFace.GetComponent<Image>().sprite = controllerImages[enumValue].leftFace;
                rightFace.GetComponent<Image>().sprite = controllerImages[enumValue].rightFace;
                fadedUpFace.GetComponent<Image>().sprite = controllerImages[enumValue].upFace;
                fadedDownFace.GetComponent<Image>().sprite = controllerImages[enumValue].downFace;
                fadedLeftFace.GetComponent<Image>().sprite = controllerImages[enumValue].leftFace;
                fadedRightFace.GetComponent<Image>().sprite = controllerImages[enumValue].leftFace;
                upDpad.GetComponent<Image>().sprite = controllerImages[enumValue].upDpad;
                downDpad.GetComponent<Image>().sprite = controllerImages[enumValue].downDpad;
                leftDpad.GetComponent<Image>().sprite = controllerImages[enumValue].leftDpad;
                rightDpad.GetComponent<Image>().sprite = controllerImages[enumValue].rightDpad;
                dpadBase.GetComponent<Image>().sprite = controllerImages[enumValue].dpadBase;
            }
        }



        [ContextMenu("Hide All Components On Active UI Objects")]
        public void hideAllComponentsOnActiveUiObjects()
        {
            AbilityOrItemUI[] controlComponentUi = GetComponentsInChildren<AbilityOrItemUI>();
            foreach (AbilityOrItemUI component in controlComponentUi)
            {
                Debug.Log(component.name);
                component.hideAllUiObjects();
            }
            //same for new text object event thing

        }

        [ContextMenu("Show All Components On Active UI Objects")]
        public void showAllComponentsOnActiveUiObjects()
        {
            AbilityOrItemUI[] controlComponentUi = GetComponentsInChildren<AbilityOrItemUI>();
            foreach (AbilityOrItemUI component in controlComponentUi)
            {
                component.showAllUiObjects();
            }
            //same for new text object event thing

        }


        private bool isUsingKeyboard = false;
        private bool isUsingGamepad = false;
        private string currentDeviceName = "";
        private string previousDevice = "";
        private string previousGamepad = "";

        private void OnEnable()
        {
            // Subscribe to input events
            Keyboard.current.onTextInput += OnTextInput;
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            Keyboard.current.onTextInput -= OnTextInput;
        }

        private void Update()
        {
            // Update for auto and controller swap
            if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
            {
                if (!isUsingKeyboard)
                {
                    isUsingKeyboard = true;
                    isUsingGamepad = false;
                    currentDeviceName = "Keyboard";
                }
            }
            else if (Gamepad.current != null && IsAnyGamepadButtonPressed())
            {
                if (previousGamepad != Gamepad.current.name)
                {
                    previousGamepad = Gamepad.current.name;
                    currentDeviceName = Gamepad.current.name;
                }

                if (!isUsingGamepad)
                {
                    isUsingGamepad = true;
                    isUsingKeyboard = false;
                    currentDeviceName = Gamepad.current.name;
                }
            }


            if (previousDevice != currentDeviceName)
            {
                previousDevice = currentDeviceName;
                //Run change events
                if (activeUi == ActiveUi.AutoSwitch)
                {
                    if (currentDeviceName == "Keyboard")
                    {
                        switchUi(UiTypes.Keyboard);
                    }
                    else if (currentDeviceName.Contains("XInput"))
                    {
                        switchUi(UiTypes.XboxOne);
                    }
                    else if (currentDeviceName.Contains("Ouya"))
                    {
                        switchUi(UiTypes.Ouya);
                    }
                    else if (currentDeviceName.Contains("DualShock4"))
                    {
                        switchUi(UiTypes.Ps4);
                    }
                    else if (currentDeviceName.Contains("DualShock5")) //untested
                    {
                        switchUi(UiTypes.Ps5);
                    }
                    else if (currentDeviceName.Contains("Pro"))
                    {
                        switchUi(UiTypes.SwitchPro);
                    }
                    else
                    {
                        switchUi(UiTypes.XboxOne);
                    }
                }
                else if (allowAutoSwitchToKeyboard)
                {
                    if (currentDeviceName == "Keyboard")
                    {
                        switchUi(UiTypes.Keyboard);
                    }
                    else
                    {
                        switchFromActiveUi();
                    }
                }
                else
                {
                    switchFromActiveUi();
                }
            }
        }

        private void OnTextInput(char character)
        {
            if (!isUsingKeyboard && Keyboard.current.anyKey.isPressed)
            {
                // If not using the keyboard UI and a key is pressed, switch to the keyboard UI
                isUsingKeyboard = true;
                isUsingGamepad = false;
                currentDeviceName = "Keyboard";
            }
            else if (!isUsingGamepad && Gamepad.current != null)
            {
                if (!isUsingKeyboard && IsAnyGamepadButtonPressed())
                {
                    // If not using the gamepad UI and any button on the gamepad is pressed, switch to the gamepad UI
                    isUsingGamepad = true;
                    isUsingKeyboard = false;
                    currentDeviceName = Gamepad.current.displayName;
                }
            }
        }

        private bool IsAnyGamepadButtonPressed()
        {
            foreach (var button in Gamepad.current.allControls)
            {
                if (button is ButtonControl buttonControl && buttonControl.isPressed)
                {
                    return true;
                }
            }
            return false;
        }



    }  
}

