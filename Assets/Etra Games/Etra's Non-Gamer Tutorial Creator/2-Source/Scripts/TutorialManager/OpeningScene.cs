using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class OpeningScene : MonoBehaviour
    {
        [Header("Basic")]
        public bool skipOpeningScene;
        [Header("Advanced")]
        public bool startCamAtStartPos = true;
        [Header("References")]
        public GameObject openingSceneUi;
        public GameObject nonGamerTutorialUi;

        //Rect transforms
        private AbilityOrItemPickup[] pickups;
        private AnimationTriggerPickup[] animPickups;

        //References
        private Star star;
        private GameObject playerSpawn;
        private GameObject camRoot;
        private GameObject cursorCanvas;
        private LevelController levelController;
        private EtraCharacterMainController character;

        //Wait times for cutscene
        private WaitForSecondsRealtime wait4p5Seconds = new WaitForSecondsRealtime(4.5f);
        private WaitForSecondsRealtime wait4Seconds = new WaitForSecondsRealtime(4f);
        private WaitForSecondsRealtime wait0p4Seconds = new WaitForSecondsRealtime(0.4f);
        private WaitForSecondsRealtime wait3p2Seconds = new WaitForSecondsRealtime(3.2f);
        private WaitForSecondsRealtime wait3Seconds = new WaitForSecondsRealtime(3f);
        private WaitForSecondsRealtime wait2Seconds = new WaitForSecondsRealtime(2f);
        private WaitForSecondsRealtime wait1Second = new WaitForSecondsRealtime(1f);

        //In code variables
        private float savedFov;
        private GameObject scoutStar;
        private GameObject scoutSpawn;

        private void Awake()
        {
            GetReferenceVariables();
            if (character.etraFPSUsableItemManager)
            {
                character.etraFPSUsableItemManager.weaponInitHandledElsewhere = true;
            }

            cursorCanvas.SetActive(false);
            pickups = levelController.chunks[levelController.chunks.Count - 1].gameObject.GetComponentsInChildren<AbilityOrItemPickup>();
            animPickups = levelController.chunks[levelController.chunks.Count - 1].gameObject.GetComponentsInChildren<AnimationTriggerPickup>();
            foreach (AbilityOrItemPickup a in pickups)
            {
                a.gameObject.SetActive(false);
            }
            foreach (AnimationTriggerPickup a in animPickups)
            {
                a.gameObject.SetActive(false);
            }
            nonGamerTutorialUi.gameObject.SetActive(false);
        }

        private void Start()
        {
            character.disableAllActiveAbilities();
            savedFov = character.getFov();
            if (skipOpeningScene)
            {
                return;
            }
            if (startCamAtStartPos)
            {
                camRoot.transform.position = star.transform.position + new Vector3(0, 50, -30); //Start
            }

            character.setFov(60);
            scoutStar = new GameObject("ScoutStar");
            scoutStar.transform.parent = star.transform;
            scoutStar.transform.localPosition = Vector3.zero;
            scoutSpawn = new GameObject("ScoutSpawn");
            scoutSpawn.transform.parent = playerSpawn.transform;
            scoutSpawn.transform.localPosition = new Vector3(0, 9, 0);
            scoutSpawn.AddComponent<BoxCollider>();
            scoutSpawn.GetComponent<BoxCollider>().isTrigger = true;

            bool heightCorrect = false;

            while (!heightCorrect)
            {
                scoutStar.transform.position = scoutStar.transform.position + new Vector3(0, 0.1f, 0);
                RaycastHit hit;
                if (Physics.Raycast(scoutStar.transform.position, scoutSpawn.transform.position - scoutStar.transform.position, out hit))
                {
                    // Check if the ray hits the target object
                    if (hit.collider.gameObject == scoutSpawn)
                    {
                        heightCorrect = true;
                    }
                }
            }
            scoutStar.transform.position = scoutStar.transform.position + new Vector3(0, 3f, 0);
        }

        public void RunOpeningScene()
        {
            character.GetComponent<StarterAssetsInputs>().cursorLocked = true;
            if (skipOpeningScene)
            {
                PlayerSetup();
            }
            else
            {
                openingSceneUi.SetActive(true);
                StartCoroutine(OpeningSceneCoroutine());
            }
        }

        private IEnumerator OpeningSceneCoroutine()
        {
            camRoot.transform.position = star.transform.position + new Vector3(0, 50, -30); //Start
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -20), 4).setEaseInOutSine(); //Move down
            yield return wait4p5Seconds;
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -4.2f), 3.5f).setEaseInOutSine(); //Star zoom
            yield return wait4Seconds;
            openingSceneUi.GetComponent<OpeningSceneUi>().starText.runAnimation();
            yield return wait0p4Seconds;
            star.starSpin();
            yield return wait3p2Seconds;
            LeanTween.move(camRoot, scoutStar.transform.position + new Vector3(0, 0, -30f), 3f).setEaseInOutSine(); //Behind scout
            LeanTween.rotate(camRoot, new Vector3(20, 0, 0), 2f).setEaseInOutSine(); //Behind scout
            yield return wait3p2Seconds;

            float timeToBack = levelController.chunks.Count * 1.4f; // in first and last second do ease
            LeanTween.move(camRoot, scoutSpawn.transform.position + new Vector3(0, 0, -10f), timeToBack).setEaseInOutQuad(); //Behind scout
            yield return new WaitForSecondsRealtime(timeToBack);
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, -10f), 3).setEaseInOutSine(); //Behind scout
            LeanTween.rotate(camRoot, new Vector3(0, 0, 0), 2f).setEaseInOutSine(); //Behind scout
            yield return wait3p2Seconds;
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, 0), 3).setEaseInOutSine();
            LeanTween.value(this.gameObject, character.getFov(), savedFov, 3).setOnUpdate((float fovValue) => { character.setFov(fovValue); });
            yield return wait2Seconds;

            if (character.GetComponentInChildren<MeshRenderer>())
            {
                Material material = character.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 0, 1).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }

            yield return wait1Second;

            if (character.GetComponentInChildren<MeshRenderer>())
            {
                Material material = character.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 255, 0).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }
            PlayerSetup();
        }

        private void GetReferenceVariables()
        {
            star = GameObject.Find("Star").GetComponent<Star>();
            playerSpawn = GameObject.Find("PlayerSpawn");
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
            cursorCanvas = GameObject.Find("CursorCanvas");
            levelController = GetComponentInChildren<LevelController>();
            character = EtraCharacterMainController.Instance;
        }

        private void PlayerSetup()
        {
            
            character.enableAllActiveAbilities();
            character.setFov(savedFov);
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, 0), 0).setEaseInOutSine();

            cursorCanvas.SetActive(true);
            foreach (AbilityOrItemPickup a in pickups)
            {
                a.gameObject.SetActive(true);
            }
            foreach (AnimationTriggerPickup a in animPickups)
            {
                a.gameObject.SetActive(true);
            }
            openingSceneUi.SetActive(true);
            nonGamerTutorialUi.gameObject.SetActive(true);

            if (character.etraFPSUsableItemManager)
            {
                character.etraFPSUsableItemManager.instatiateItemAtStart();
            }

            AudioManager[] managers = FindObjectsOfType<AudioManager>();

            foreach (AudioManager manager in managers)
            {
                if (manager.silenceSoundsUntilTutorialBegins)
                {
                    manager.stopPlayingSounds = false;
                }

            }

        }
    }
}
