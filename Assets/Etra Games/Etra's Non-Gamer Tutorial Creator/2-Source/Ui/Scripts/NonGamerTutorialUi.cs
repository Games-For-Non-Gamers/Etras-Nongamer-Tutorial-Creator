using UnityEngine;
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


        public void switchFromActiveUi(ActiveUi activeUi)
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
            EtraAnimationHolder[] controlComponentUi = GetComponentsInChildren<EtraAnimationHolder>();
            foreach (EtraAnimationHolder component in controlComponentUi)
            {
                Debug.Log(component.name);
                component.hideAllAnimatedObjects();
            }
            //same for new text object event thing

        }

        [ContextMenu("Show All Components On Active UI Objects")]
        public void showAllComponentsOnActiveUiObjects()
        {
            EtraAnimationHolder[] controlComponentUi = GetComponentsInChildren<EtraAnimationHolder>();
            foreach (EtraAnimationHolder component in controlComponentUi)
            {
                component.showAllAnimatedObjects();
            }
            //same for new text object event thing

        }




    }  
}

