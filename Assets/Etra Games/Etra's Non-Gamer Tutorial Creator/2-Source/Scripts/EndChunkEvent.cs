using Etra.StandardMenus;
using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Etra.NonGamerTutorialCreator
{
    public class EndChunkEvent : EtraAnimationActivatedScript
    {
        bool gameEnded = false;
        StarterAssetsInputs _inputs;
        Star star;
        public override void runScript(string passedString)
        {
            StartCoroutine(end());

        }

        IEnumerator end()
        {
            star.endCelebration();
            yield return new WaitForSeconds(5);
            gameEnded = true;
            EtraStandardMenusManager menusManager = FindObjectOfType<EtraStandardMenusManager>();
            menusManager.canFreeze = false;
        }


        private void Start()
        {
            _inputs = EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>();
            star = GameObject.Find("Star").GetComponent<Star>();
        }

        bool startedLeave = false;
        private void Update()
        {
            if (gameEnded)
            {
                if (_inputs.select)
                {
                    if (!startedLeave)
                    {
                        startedLeave = true;
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
}