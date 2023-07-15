using UnityEngine;
using static Etra.NonGamerTutorialCreator.NonGamerTutorialUi;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

namespace Etra.NonGamerTutorialCreator
{
    public class UiSwitcher : MonoBehaviour
    {
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


        [Header("Current UI")]
        public ActiveUi activeUi;
        public bool allowAutoSwitchToKeyboard = true;

        [Header("Reference")]
        public NonGamerTutorialUi nonGamerTutorialUi;

        private bool isUsingKeyboard = false;
        private bool isUsingGamepad = false;
        private string currentDeviceName = "";
        private string previousDevice = "";
        private string previousGamepad = "";

        private void OnValidate()
        {
            nonGamerTutorialUi.switchFromActiveUi(activeUi);
        }


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
            if (Keyboard.current != null && (Keyboard.current.anyKey.isPressed || Mouse.current.delta.ReadValue().magnitude > 0 || Mouse.current.leftButton.isPressed))
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
                        nonGamerTutorialUi.switchUi(UiTypes.Keyboard);
                    }
                    else if (currentDeviceName.Contains("XInput"))
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.XboxOne);
                    }
                    else if (currentDeviceName.Contains("Ouya"))
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.Ouya);
                    }
                    else if (currentDeviceName.Contains("DualShock4"))
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.Ps4);
                    }
                    else if (currentDeviceName.Contains("DualShock5")) //untested
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.Ps5);
                    }
                    else if (currentDeviceName.Contains("Pro"))
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.SwitchPro);
                    }
                    else
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.XboxOne);
                    }
                }
                else if (allowAutoSwitchToKeyboard)
                {
                    if (currentDeviceName == "Keyboard")
                    {
                        nonGamerTutorialUi.switchUi(UiTypes.Keyboard);
                    }
                    else
                    {
                        nonGamerTutorialUi.switchFromActiveUi(activeUi);
                    }
                }
                else
                {
                    nonGamerTutorialUi.switchFromActiveUi(activeUi);
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