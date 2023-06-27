using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndChunkEvent : EtraAnimationActivatedScript
{
    bool gameEnded = false;
    StarterAssetsInputs _inputs;
    public override void runScript(string passedString)
    {
        gameEnded = true;
    }

    private void Start()
    {
        _inputs = EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>();
    }

    bool startedLeave = false;
    private void Update()
    {
        if (!gameEnded)
        {
            if (_inputs.select)
            {
                if (startedLeave)
                {
                    StartCoroutine(toMainMenu());
                    return;
                }


            }

            if (_inputs.start)
            {
                SceneManager.LoadScene("CreditsNonGamerTutorial");
            }

        }
    }

    IEnumerator toMainMenu()
    {
        Debug.LogWarning("Click me for the code that should load back to the main menu. After a few seconds this will just go to scene zero. This code is in the EndChunkEvent.cs script.");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0); //
    }
}
