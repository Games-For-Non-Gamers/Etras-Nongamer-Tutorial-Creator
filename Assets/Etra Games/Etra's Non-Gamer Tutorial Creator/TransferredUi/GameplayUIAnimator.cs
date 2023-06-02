using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIAnimator : MonoBehaviour
{

    [Header("Vars")]
    public int controllerNumber = -1;
    public bool debugMode = false;
    public int debugEventNum = 1;
    public UIElement[] allElements;
    public ControllerUIImages[] controllerImages;
    public GameObject keyboardUI;
    public GameObject controllerUI;
    public GameObject congratsUI;

    [Header("Keyboard Objects")]
    public RectTransform wKey;
    public RectTransform aKey;
    public RectTransform sKey;
    public RectTransform dKey;

    public RectTransform eKey;

    public RectTransform FADEDwKey;
    public RectTransform FADEDaKey;
    public RectTransform FADEDsKey;
    public RectTransform FADEDdKey;
    public RectTransform spaceKey;
    public RectTransform shiftKey;
    public RectTransform ctrlKey;
    public RectTransform mouseNoClick;
    public RectTransform mouseLeftClick;

    [Header("Keyboard Arrows")]
    public RectTransform wArrow;
    public RectTransform aArrow;
    public RectTransform sArrow;
    public RectTransform dArrow;
    public RectTransform lMouseArrow;
    public RectTransform rMouseArrow;
    public RectTransform uMouseArrow;
    public RectTransform dMouseArrow;

    [Header("Keyboard Labels")]
    public RectTransform moveText;

    public RectTransform lookText;

    public RectTransform runText;
    public RectTransform crouchText;
    public RectTransform spaceText;
    public RectTransform interactArrow;
    public RectTransform interactText;
    public RectTransform fireArrow;
    public RectTransform fireText;
    public RectTransform runHold;
    public RectTransform crouchHold;


    [Header("Controller Objects")]
    public RectTransform leftJoystick;
    public RectTransform leftShoulder;
    public RectTransform leftFace;
    public RectTransform downFace;
    public RectTransform FADEDleftFace;
    public RectTransform FADEDrightFace;
    public RectTransform FADEDupFace;
    public RectTransform FADEDdownFace;
    public RectTransform rightShoulder;
    public RectTransform rightJoystick;

    [Header("Controller Arrows")]
    public RectTransform lsLeftArrow;
    public RectTransform lsRightArrow;
    public RectTransform lsUpArrow;
    public RectTransform lsDownArrow;
    public RectTransform rsLeftArrow;
    public RectTransform rsRightArrow;
    public RectTransform rsUpArrow;
    public RectTransform rsDownArrow;


    [Header("Controller Labels")]
    public RectTransform CONTROLmoveText;
    public RectTransform CONTROLactionsText;
    public RectTransform CONTROLlookText;
    public RectTransform CONTROLrunArrow;
    public RectTransform CONTROLrunText;
    public RectTransform CONTROLcrouchArrow;
    public RectTransform CONTROLcrouchText;
    public RectTransform CONTROLinteractArrow;
    public RectTransform CONTROLinteractText;
    public RectTransform CONTROLjumpArrow;
    public RectTransform CONTROLjumpText;
    public RectTransform CONTROLfireArrow;
    public RectTransform CONTROLfireText;


    [Header("Win Message")]
    public RectTransform congratsText;
    public RectTransform congratsSubtitleText;

    //All Ui elements builder functions
    void setupAllUIElementsArray()
    {
        addToAllUIElementsArray(0, wKey);
        addToAllUIElementsArray(1, aKey);
        addToAllUIElementsArray(2, sKey);
        addToAllUIElementsArray(3, dKey);
        addToAllUIElementsArray(4, eKey);
        addToAllUIElementsArray(5, FADEDwKey);
        addToAllUIElementsArray(6, FADEDaKey);
        addToAllUIElementsArray(7, FADEDsKey);
        addToAllUIElementsArray(8, FADEDdKey);
        addToAllUIElementsArray(9, spaceKey);
        addToAllUIElementsArray(10, shiftKey);
        addToAllUIElementsArray(11, ctrlKey);
        addToAllUIElementsArray(12, mouseNoClick);
        addToAllUIElementsArray(13, mouseLeftClick);
        addToAllUIElementsArray(14, wArrow);
        addToAllUIElementsArray(15, aArrow);
        addToAllUIElementsArray(16, sArrow);
        addToAllUIElementsArray(17, dArrow);
        addToAllUIElementsArray(18, lMouseArrow);
        addToAllUIElementsArray(19, rMouseArrow);
        addToAllUIElementsArray(20, uMouseArrow);
        addToAllUIElementsArray(21, dMouseArrow);
        addToAllUIElementsArray(22, moveText);
        addToAllUIElementsArray(23, lookText);
        addToAllUIElementsArray(24, runText);
        addToAllUIElementsArray(25, crouchText);
        addToAllUIElementsArray(26, spaceText);
        addToAllUIElementsArray(27, interactArrow);
        addToAllUIElementsArray(28, interactText);
        addToAllUIElementsArray(29, fireArrow);
        addToAllUIElementsArray(30, fireText);
        addToAllUIElementsArray(31, runHold);
        addToAllUIElementsArray(32, crouchHold);
        addToAllUIElementsArray(33, leftJoystick);
        addToAllUIElementsArray(34, leftShoulder);
        addToAllUIElementsArray(35, leftFace);
        addToAllUIElementsArray(36, downFace);
        addToAllUIElementsArray(37, FADEDleftFace);
        addToAllUIElementsArray(38, FADEDrightFace);
        addToAllUIElementsArray(39, FADEDupFace);
        addToAllUIElementsArray(40, FADEDdownFace);
        addToAllUIElementsArray(41, rightShoulder);
        addToAllUIElementsArray(42, rightJoystick);
        addToAllUIElementsArray(43, lsLeftArrow);
        addToAllUIElementsArray(44, lsRightArrow);
        addToAllUIElementsArray(45, lsUpArrow);
        addToAllUIElementsArray(46, lsDownArrow);
        addToAllUIElementsArray(47, rsLeftArrow);
        addToAllUIElementsArray(48, rsRightArrow);
        addToAllUIElementsArray(49, rsUpArrow);
        addToAllUIElementsArray(50, rsDownArrow);
        addToAllUIElementsArray(51, CONTROLmoveText);
        addToAllUIElementsArray(52, CONTROLactionsText);
        addToAllUIElementsArray(53, CONTROLlookText);
        addToAllUIElementsArray(54, CONTROLrunArrow);
        addToAllUIElementsArray(55, CONTROLrunText);
        addToAllUIElementsArray(56, CONTROLcrouchArrow);
        addToAllUIElementsArray(57, CONTROLcrouchText);
        addToAllUIElementsArray(58, CONTROLinteractArrow);
        addToAllUIElementsArray(59, CONTROLinteractText);
        addToAllUIElementsArray(60, CONTROLjumpArrow);
        addToAllUIElementsArray(61, CONTROLjumpText);
        addToAllUIElementsArray(62, CONTROLfireArrow);
        addToAllUIElementsArray(63, CONTROLfireText);
        addToAllUIElementsArray(64, congratsText);
        addToAllUIElementsArray(65, congratsSubtitleText);
    }
    void addToAllUIElementsArray(int index, RectTransform element)
    {

        allElements[index].nameOfObject = element.gameObject.name;

        if (element.gameObject.GetComponent<Image>() != null)
        {
            allElements[index].imageComponent = element.gameObject.GetComponent<Image>();
        }

        if (element.gameObject.GetComponent<Text>() != null)
        {
            allElements[index].textComponent = element.gameObject.GetComponent<Text>();
        }

        allElements[index].startingLocation = element.localPosition;

    }
    //Functions that control all objects
    void disableAllUIvisualElements()
    {
        for (int i = 0; i < allElements.Length; i++)
        {
            if (allElements[i].imageComponent != null)
            {
                allElements[i].imageComponent.enabled=false;
            }

            if (allElements[i].textComponent != null)
            {
                allElements[i].textComponent.enabled = false ;
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        //Need the following made in inspector
        // allElements = new UIElement[66];
        //Setup all ui objects array
        setupAllUIElementsArray();
        //diable all ui objects visual elements
        disableAllUIvisualElements();

    }



    int debugControllerNum=0;
    int activeSelectionNum = 0;
    // Update is called once per frame
    void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                if (activeSelectionNum == 0)
                {
                    keyboardUI.SetActive(false);
                    controllerUI.SetActive(false);
                    congratsUI.SetActive(false);
                    
                }
                else if (activeSelectionNum == 1)
                {
                    keyboardUI.SetActive(true);
                    controllerUI.SetActive(false);
                    congratsUI.SetActive(true);
                }
                else if (activeSelectionNum == 2)
                {
                    keyboardUI.SetActive(false);
                    controllerUI.SetActive(true);
                    congratsUI.SetActive(true);
                }

                activeSelectionNum++;
                if (activeSelectionNum == 3)
                {
                    activeSelectionNum = 0;
                }

            }
                //run events

            //swap controllers
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                setControllerImages(debugControllerNum);
                debugControllerNum++;
                if (debugControllerNum== controllerImages.Length)
                {
                    debugControllerNum = 0;
                }
            }
        }    
    }

    public void setControllerImages(int controllerNum)
    {
        if (controllerNum == -1)
        {
            keyboardUI.SetActive(true);
            controllerUI.SetActive(false);
        }
        else
        {
            keyboardUI.SetActive(false);
            controllerUI.SetActive(true);

            leftJoystick.GetComponent<Image>().sprite = controllerImages[controllerNum].leftJoystick;
            rightJoystick.GetComponent<Image>().sprite = controllerImages[controllerNum].rightJoystick;
            leftShoulder.GetComponent<Image>().sprite = controllerImages[controllerNum].leftShoulder;
            rightShoulder.GetComponent<Image>().sprite = controllerImages[controllerNum].rightShoulder;
            leftFace.GetComponent<Image>().sprite = controllerImages[controllerNum].leftFace;
            downFace.GetComponent<Image>().sprite = controllerImages[controllerNum].downFace;
            FADEDleftFace.GetComponent<Image>().sprite = controllerImages[controllerNum].FADEDleftFace;
            FADEDrightFace.GetComponent<Image>().sprite = controllerImages[controllerNum].FADEDrightFace;
            FADEDupFace.GetComponent<Image>().sprite = controllerImages[controllerNum].FADEDupFace;
            FADEDdownFace.GetComponent<Image>().sprite = controllerImages[controllerNum].FADEDdownFace;
        }

    }

    /* We'll get to this
    public void setControllerAxis(int controllerValue)
    {
        //-1 Keyboard //0-Ps4 //1-Xbox1 //2-Xbox360 //3-Steam//4 SwitchPro controller //5 OUYAAAAA
        if (controllerValue == 0)
        {
            //Ps4
            controllerRotateCameraXInput = "PS4 X";
            controllerRotateCameraYInput = "PS4 Y";
            controllerJumpInput = KeyCode.JoystickButton1;
            controllerBlipInput = KeyCode.JoystickButton2;


        }
        if (controllerValue == 1)
        {
            //Xbox 1
            controllerRotateCameraXInput = "Xbox One X";
            controllerRotateCameraYInput = "Xbox One Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }
        if (controllerValue == 2)
        {
            //Xbox 360
            controllerRotateCameraXInput = "Xbox 360 X";
            controllerRotateCameraYInput = "Xbox 360 Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }
        if (controllerValue == 3)
        {
            //Steam
            controllerRotateCameraXInput = "Xbox 360 X";
            controllerRotateCameraYInput = "Xbox 360 Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }
        if (controllerValue == 4)
        {
            //SWitch Pro
            controllerRotateCameraXInput = "Nintendo X";
            controllerRotateCameraYInput = "Nintendo Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }

        if (controllerValue == 5)
        {
            //Ouya
            controllerRotateCameraXInput = "Xbox 360 X";
            controllerRotateCameraYInput = "Xbox 360 Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }


        if (controllerValue == 6)
        {
            //Off Brand
            controllerRotateCameraXInput = "Xbox 360 X";
            controllerRotateCameraYInput = "Xbox 360 Y";
            controllerJumpInput = KeyCode.JoystickButton0;
            controllerBlipInput = KeyCode.JoystickButton1;
        }
        

    }
    */

    /// <EVENTS>
    /// 1. Right
    /// 2. Shoot
    /// 3. Left-Basic Grow and Flash
    /// 4. Back-Basic Grow and flash
    /// 5. Forward-Basic Grow and flash 
    /// 6. Crouch
    /// 7. Mouse up/down
    /// 8. Jump - Basic Grow and flash WITH Text
    /// 9. Dash
    /// 10. Mouse Left Right
    /// //special mouse move anim later
    /// 11. Interact
    /// 12. Congrats
    /// </summary>

    bool doEvents = true;
    #region calledEvents
    public void calledEvent(int eventNum)
    {
        //run both but have one set be inactive? That way easy switch?

        if (doEvents)
        {
            if (eventNum < 12)
            {
                StartCoroutine("keyboardEvent" + eventNum);
                StartCoroutine("controllerEvent" + eventNum);
            }
            else
            {
                StartCoroutine(congratsTextAppear());
                doEvents = false;
            }
        }
       

    }

    #endregion

    #region ui Tweening functions
    private IEnumerator basicCenterGrowToPlaceAnim(RectTransform element)
    {
        //get default pos and set center invisible
        Vector3 startPos = element.localPosition;
        LeanTween.scale(element, Vector3.zero, 0);
        LeanTween.move(element, new Vector3(50, -50, 0), 0);
        element.GetComponent<Image>().enabled = true;

        LeanTween.scale(element, new Vector3(2, 2, 2), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, startPos, 1).setEaseInOutSine();

    }

    private IEnumerator basicCenterGrowToPlaceAnim(RectTransform element, Vector3 newLocToGoTo)
    {
        //get default pos and set center invisible
        LeanTween.scale(element, Vector3.zero, 0);
        LeanTween.move(element, new Vector3(50, -50, 0), 0);
        element.GetComponent<Image>().enabled = true;

        LeanTween.scale(element, new Vector3(2, 2, 2), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, newLocToGoTo, 1).setEaseInOutSine();

    }

    private IEnumerator basicCenterGrowToPlaceAnim(RectTransform element, Vector3 startLoc, Vector3 newLocToGoTo)
    {
        //get default pos and set center invisible
        LeanTween.scale(element, Vector3.zero, 0);
        LeanTween.move(element, startLoc, 0);
        element.GetComponent<Image>().enabled = true;

        LeanTween.scale(element, new Vector3(2, 2, 2), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, newLocToGoTo, 1).setEaseInOutSine();

    }

    private IEnumerator mouseCenterGrowToPlaceAnim(RectTransform element)
    {
        //get default pos and set center invisible
        Vector3 startingPos = element.localPosition;
        LeanTween.scale(element, Vector3.zero, 0);
        LeanTween.move(element, new Vector3(-50, -50, 0), 0);
        element.GetComponent<Image>().enabled = true;

        LeanTween.scale(element, new Vector3(2, 2, 2), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, startingPos, 1).setEaseInOutSine();
    }

    private IEnumerator bringMouseToCenter(RectTransform element)
    {
        Vector3 startingPos = element.localPosition;
        LeanTween.move(element, new Vector3(-50, -50, 1), 1).setEaseInOutSine();
        LeanTween.scale(element, new Vector3(1.5f, 1.5f, 1.5f), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, startingPos, 1).setEaseInOutSine();
    }

    private IEnumerator bringJoystickToCenter(RectTransform element, Vector3 locToReturnTo)
    {
        LeanTween.move(element, new Vector3(50, -50, 1), 1).setEaseInOutSine();
        LeanTween.scale(element, new Vector3(1.5f, 1.5f, 1.5f), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, locToReturnTo, 1).setEaseInOutSine();
    }

    private IEnumerator bringJoystickToCenter(RectTransform element, Vector3 locGoTo, Vector3 locToReturnTo)
    {
        LeanTween.move(element, locGoTo, 1).setEaseInOutSine();
        LeanTween.scale(element, new Vector3(1.5f, 1.5f, 1.5f), 1).setEaseInOutSine();
        yield return new WaitForSeconds(3);
        LeanTween.scale(element, new Vector3(1, 1, 1), 1).setEaseInOutSine();
        LeanTween.move(element, locToReturnTo, 1).setEaseInOutSine();
    }




    private IEnumerator fadeInText(RectTransform element)
    {
        LeanTween.colorText(element, new Color(255, 255, 255, 0), 0);
        element.GetComponent<Text>().enabled = true;
        LeanTween.colorText(element, new Color(255, 255, 255, 1), 1);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator fadeInImage(RectTransform element)
    {
        LeanTween.color(element, new Color(255, 255, 255, 0), 0);
        element.GetComponent<Image>().enabled = true;
        LeanTween.color(element, new Color(255, 255, 255, 1), 1);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator flashImage(RectTransform element)
    {
        element.GetComponent<Image>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        element.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        element.GetComponent<Image>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        element.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        element.GetComponent<Image>().enabled = true;
    }
    #endregion

    #region keyboardEvents
    /// 1. Right
    private IEnumerator keyboardEvent1()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(dKey));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(moveText));
        StartCoroutine(fadeInImage(FADEDwKey));
        StartCoroutine(fadeInImage(FADEDaKey));
        StartCoroutine(fadeInImage(FADEDsKey));
        StartCoroutine(fadeInImage(FADEDdKey));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(dArrow));
    }
    /// 2. Shoot
    private IEnumerator keyboardEvent2()
    {
        StartCoroutine(mouseCenterGrowToPlaceAnim(mouseLeftClick));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(fireText));
        StartCoroutine(fadeInImage(fireArrow));
        yield return new WaitForSeconds(1);
        mouseNoClick.GetComponent<Image>().enabled = true;
        StartCoroutine(flashImage(mouseLeftClick));
        yield return new WaitForSeconds(2.5f);
        mouseLeftClick.GetComponent<Image>().enabled = false;
    }
    /// 3. Left
    private IEnumerator keyboardEvent3()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(aKey));
        yield return new WaitForSeconds(4);
        StartCoroutine(flashImage(aArrow));
    }

    /// 4. Back
    private IEnumerator keyboardEvent4()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(sKey));
        yield return new WaitForSeconds(4);
        StartCoroutine(flashImage(sArrow));
    }

    /// 5. Forward
    private IEnumerator keyboardEvent5()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(wKey));
        yield return new WaitForSeconds(4);
        StartCoroutine(flashImage(wArrow));
    }

    /// 6. Crouch
    private IEnumerator keyboardEvent6()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(ctrlKey));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(crouchHold));
        StartCoroutine(fadeInText(crouchText));
    }

    /// 7. Mouse up/down
    private IEnumerator keyboardEvent7()
    {
        StartCoroutine(bringMouseToCenter(mouseNoClick));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(uMouseArrow));
        StartCoroutine(flashImage(dMouseArrow));
        yield return new WaitForSeconds(2);
        StartCoroutine(fadeInText(lookText));
    }

    /// 8. Jump
    private IEnumerator keyboardEvent8()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(spaceKey));
        yield return new WaitForSeconds(4);
        StartCoroutine(fadeInText(spaceText));
    }

    /// 9. Dash
    private IEnumerator keyboardEvent9()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(shiftKey));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(runText));
        StartCoroutine(fadeInText(runHold));
    }

    /// 10. Mouse Left Right
    private IEnumerator keyboardEvent10()
    {
        StartCoroutine(bringMouseToCenter(mouseNoClick));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(lMouseArrow));
        StartCoroutine(flashImage(rMouseArrow));
    }

    /// 11. Interact
    private IEnumerator keyboardEvent11()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(eKey));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInImage(interactArrow));
        StartCoroutine(fadeInText(interactText));
    }

    #endregion

    #region controllerEvents
    /// 1. Right
    private IEnumerator controllerEvent1()
    {
        //have to manually overide jotstick loc for some reason
        StartCoroutine(basicCenterGrowToPlaceAnim(leftJoystick, new Vector3(-560,178,0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLmoveText));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(lsRightArrow));
    }
    /// 2. Shoot
    private IEnumerator controllerEvent2()
    {
        //May just have to manually overide a lot of events for controller I guess :(
        StartCoroutine(basicCenterGrowToPlaceAnim(rightShoulder, new Vector3(-50, -50, 0), new Vector3(650, 255, 0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLactionsText));
        StartCoroutine(fadeInText(CONTROLfireText));
        StartCoroutine(fadeInImage(CONTROLfireArrow));
    }
    /// 3. Left
    private IEnumerator controllerEvent3()
    {
        StartCoroutine(bringJoystickToCenter(leftJoystick, new Vector3(-560, 178, 0)));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(lsLeftArrow));
    }

    /// 4. Back
    private IEnumerator controllerEvent4()
    {
        StartCoroutine(bringJoystickToCenter(leftJoystick, new Vector3(-560, 178, 0)));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(lsDownArrow));
    }

    /// 5. Forward
    private IEnumerator controllerEvent5()
    {
        StartCoroutine(bringJoystickToCenter(leftJoystick, new Vector3(-560, 178, 0)));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(lsUpArrow));
    }

    /// 6. Crouch
    private IEnumerator controllerEvent6()
    {
        StartCoroutine(bringJoystickToCenter(leftJoystick, new Vector3(-560, 178, 0)));
        yield return new WaitForSeconds(1);
        StartCoroutine(fadeInText(CONTROLcrouchText));
        StartCoroutine(fadeInImage(CONTROLcrouchArrow));
    }

    /// 7. Mouse up/down
    private IEnumerator controllerEvent7()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(rightJoystick, new Vector3(-50, 0, 0), new Vector3(571, -133, 0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLlookText));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(rsUpArrow));
        StartCoroutine(flashImage(rsDownArrow));
    }

    /// 8. Jump
    private IEnumerator controllerEvent8()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(downFace, new Vector3(-175, -125, 0), new Vector3(570, 125, 0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLjumpText));
        StartCoroutine(fadeInImage(CONTROLjumpArrow));
        StartCoroutine(fadeInImage(FADEDleftFace));
        StartCoroutine(fadeInImage(FADEDrightFace));
        StartCoroutine(fadeInImage(FADEDupFace));
        StartCoroutine(fadeInImage(FADEDdownFace));
    }

    /// 9. Dash
    private IEnumerator controllerEvent9()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(leftShoulder, new Vector3(50, -50, 0), new Vector3(-665, 252, 0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLrunText));
        StartCoroutine(fadeInImage(CONTROLrunArrow));
    }

    /// 10. Mouse Left Right
    private IEnumerator controllerEvent10()
    {
        StartCoroutine(bringJoystickToCenter(rightJoystick, new Vector3(-50, 0, 0),  new Vector3(571, -133, 0)));
        yield return new WaitForSeconds(1);
        StartCoroutine(flashImage(rsLeftArrow));
        StartCoroutine(flashImage(rsRightArrow));
    }

    /// 11. Interact
    private IEnumerator controllerEvent11()
    {
        StartCoroutine(basicCenterGrowToPlaceAnim(leftFace, new Vector3(-175, -125, 0), new Vector3(500, 195, 0)));
        yield return new WaitForSeconds(3);
        StartCoroutine(fadeInText(CONTROLinteractText));
        StartCoroutine(fadeInImage(CONTROLinteractArrow));
    }

    #endregion

    #region sharedEvents
    private IEnumerator congratsTextAppear()
    {
        //have to manually overide jotstick loc for some reason
        LeanTween.scale(congratsText,Vector3.zero, 0);
        congratsText.GetComponent<Text>().enabled = true;
        LeanTween.scale(congratsText, Vector3.one, 1);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(fadeInText(congratsSubtitleText));
    }
    #endregion
}
