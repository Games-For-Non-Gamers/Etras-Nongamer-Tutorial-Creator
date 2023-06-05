using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_GamePlay_BasicUIEvents : MonoBehaviour
{
    public GameObject deathScreen;


    public GameplayUIAnimator controllerUI;

 
    public int currentLanguage;
    public Font arial;
    public Font cartoon;



    [Header("Keyboard Text Objects")]
    public Text moveText;
    public Text lookText;
    public Text runText;
    public Text crouchText;
    public Text spaceText;
    public Text interactText;
    public Text fireText;
    public Text runHold;
    public Text crouchHold;

    [Header("Control Text Objects")]
    public Text CONTROLmoveText;
    public Text CONTROLactionsText;
    public Text CONTROLlookText;
    public Text CONTROLrunText;
    public Text CONTROLcrouchText;
    public Text CONTROLinteractText;
    public Text CONTROLjumpText;
    public Text CONTROLfireText;

    [Header("Congrats Text Objects")]
    public Text congratsText;
    public Text congratsSubText;

    private List<string> eachLine;
    private string theWholeFileAsOneLongString;

    private void Start()
    {
        /*
        //Save data setup
        savedSettingsScript.loadSettings();

        //Languages Setup
        savedLanguagesScript.loadLanguages();
        languagesLoadedWithKey = savedLanguagesScript.savedLanguages.languages;
        updateLanguage(savedSettingsScript.savedSettings.selectedLanguage);

        //Controller Setup
        controllerUI.setControllerImages(savedSettingsScript.savedSettings.selectedController);
        */
    }

    /*
    public void deathSwipe()
    {
        StartCoroutine(actualDeathSwipe());

    }

    IEnumerator actualDeathSwipe()
    {
        LeanTween.moveLocal(deathScreen, new Vector3(-2200, 0, 0), 1f);
        yield return new WaitForSeconds(1f);
        LeanTween.moveLocal(deathScreen, new Vector3(2200, 0, 0), 0);

    }

    //Languages
    public void updateLanguage(int index)
    {
        

        //Set current Language
        currentLanguage = index;

        //Split the language text string into lines again
        theWholeFileAsOneLongString = languagesLoadedWithKey[currentLanguage].languageText;
        eachLine = new List<string>();
        eachLine.AddRange(theWholeFileAsOneLongString.Split("\n"[0]));

        //update each in game text object



        moveText.text = eachLine[4];
        lookText.text = eachLine[8];
        runText.text = eachLine[10];
        crouchText.text = eachLine[11];
        spaceText.text = eachLine[7];
        interactText.text = eachLine[14];
        fireText.text = eachLine[15];
        runHold.text = eachLine[13];
        crouchHold.text = eachLine[13];

        CONTROLmoveText.text = eachLine[4];
        CONTROLactionsText.text = eachLine[5];
        CONTROLlookText.text = eachLine[8];
        CONTROLrunText.text = eachLine[10];
        CONTROLcrouchText.text = eachLine[11];
        CONTROLinteractText.text = eachLine[14];
        CONTROLjumpText.text = eachLine[7];
        CONTROLfireText.text = eachLine[15];

        congratsText.text = eachLine[16];
        congratsSubText.text = eachLine[17];

        //induce auto size then set font
        if (eachLine[60] == "0")
        {
            setToFont(cartoon);
        }
        else
        {
            setToFont(arial);
        }


    }

    private void setToFont(Font newFont)
    {


        moveText.GetComponent<Text>().font = newFont;
        lookText.GetComponent<Text>().font = newFont;
        runText.GetComponent<Text>().font = newFont;
        crouchText.GetComponent<Text>().font = newFont;
        spaceText.GetComponent<Text>().font = newFont;
        interactText.GetComponent<Text>().font = newFont;
        fireText.GetComponent<Text>().font = newFont;
        runHold.GetComponent<Text>().font = newFont;
        crouchHold.GetComponent<Text>().font = newFont;

        CONTROLmoveText.GetComponent<Text>().font = newFont;
        CONTROLactionsText.GetComponent<Text>().font = newFont;
        CONTROLlookText.GetComponent<Text>().font = newFont;
        CONTROLrunText.GetComponent<Text>().font = newFont;
        CONTROLcrouchText.GetComponent<Text>().font = newFont;
        CONTROLinteractText.GetComponent<Text>().font = newFont;
        CONTROLjumpText.GetComponent<Text>().font = newFont;
        CONTROLfireText.GetComponent<Text>().font = newFont;

        congratsText.GetComponent<Text>().font = newFont;
        congratsSubText.GetComponent<Text>().font = newFont;


    }
        */

}
