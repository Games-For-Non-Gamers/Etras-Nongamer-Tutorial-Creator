using Etra.NonGamerTutorialCreator.Level;
using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class OpeningScene : MonoBehaviour
    {
        public bool equipAnimForStartingWeapon = true;
        [Header("CutsceneCheckpoints")]
        public Star star;
        public GameObject playerSpawn;
        public GameObject camRoot; // make fov 60
        public GameObject cursorCanvas;
        public GameObject nongamerUi;
        public EtraAnimationHolder starText;
        public LevelController levelController;
        AbilityOrItemPickup[] pickups;
        AnimationTriggerPickup [] animPickups;
        // Start is called before the first frame update
        private void Reset()
        {
            initialize();
        }

        public void initialize()
        {
            //Switch to child refs
            star = GameObject.Find("Star").GetComponent<Star>();
            playerSpawn = GameObject.Find("PlayerSpawn");
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
            cursorCanvas = GameObject.Find("CursorCanvas");
            nongamerUi = GameObject.Find("NonGamerTutorialUI");
            starText = GameObject.Find("StarText").GetComponent<EtraAnimationHolder>();
            levelController = GameObject.Find("Level Controller").GetComponent<LevelController>();
        }

        // Update is called once per frame
        bool openingDone = false;
        private void Update()
        {
            if (starterAssetsInputs.start && !openingDone)
            {
                starterAssetsInputs.start = false;
                skipOpening();
            }
        }
        public void skipOpening()
        {
            LeanTween.cancelAll(this.gameObject);
            LeanTween.cancelAll(camRoot);
            StopAllCoroutines();
            playerSetup();
            starText.gameObject.SetActive(false);
        }

        void Awake()
        {
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
            nongamerUi.SetActive(false);

        }
        float savedFov;
        GameObject scoutStar;
        GameObject scoutSpawn;
        StarterAssetsInputs starterAssetsInputs;
        private void Start()
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
            EtraCharacterMainController.Instance.etraFPSUsableItemManager.weaponInitHandledElsewhere = true;
            EtraCharacterMainController mainController = EtraCharacterMainController.Instance;
            starterAssetsInputs = mainController.GetComponent<StarterAssetsInputs>();
            savedFov = EtraCharacterMainController.Instance.getFov();
            EtraCharacterMainController.Instance.setFov(60);
            scoutStar = new GameObject("ScoutStar");
            scoutStar.transform.parent = star.transform;
            scoutStar.transform.localPosition = Vector3.zero;
            scoutSpawn = new GameObject("ScoutSpawn");
            scoutSpawn.transform.parent = playerSpawn.transform;
            scoutSpawn.transform.localPosition = new Vector3 (0,9,0);
            scoutSpawn.AddComponent<BoxCollider>();
            scoutSpawn.GetComponent<BoxCollider>().isTrigger= true;

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


            runOpeningScene();
        }

        public void runOpeningScene()
        {
            StartCoroutine(openingScene());
        
        }

        IEnumerator openingScene()
        {
            camRoot.transform.position = star.transform.position + new Vector3(0, 50, -30); //Start
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -20), 4).setEaseInOutSine(); //Move down
            yield return new WaitForSeconds(4.5f);
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -4.2f), 3.5f).setEaseInOutSine();//Star zoom
            yield return new WaitForSeconds(4f);
            starText.runAnimation();
            yield return new WaitForSeconds(0.4f);
            star.TakeDamage(0);
            yield return new WaitForSeconds(3.2f); 
            LeanTween.move(camRoot, scoutStar.transform.position + new Vector3(0, 0, -30f), 3f).setEaseInOutSine();//Behind scout
            LeanTween.rotate(camRoot, new Vector3(20,0,0), 2f).setEaseInOutSine();//Behind scout
            yield return new WaitForSeconds(3.2f);

            //Get time vary based off blocks
            //blocks x 1.4

            float timeToBack = levelController.chunks.Count * 1.4f; // in first and last second do ease
            LeanTween.move(camRoot, scoutSpawn.transform.position + new Vector3(0, 0, -10f), timeToBack).setEaseInOutQuad();//Behind scout
            yield return new WaitForSeconds(timeToBack);
            //change y to proper cam y
            LeanTween.move(camRoot, EtraCharacterMainController.Instance.transform.position + new Vector3(0, 1.375f, -10f), 3).setEaseInOutSine();//Behind scout
            LeanTween.rotate(camRoot, new Vector3(0, 0, 0), 2f).setEaseInOutSine();//Behind scout
            yield return new WaitForSeconds(3.2f);
            //CHANGE FOV HERE
            LeanTween.move(camRoot, EtraCharacterMainController.Instance.transform.position + new Vector3(0, 1.375f, 0), 3).setEaseInOutSine();

            LeanTween.value(this.gameObject, EtraCharacterMainController.Instance.getFov(), savedFov, 3).setOnUpdate((float fovValue) => { EtraCharacterMainController.Instance.setFov(fovValue); });

            yield return new WaitForSeconds(2f);
            if (EtraCharacterMainController.Instance.GetComponentInChildren<MeshRenderer>())
            {
                Material material = EtraCharacterMainController.Instance.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 0, 1).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }

            yield return new WaitForSeconds(1f);

            if (EtraCharacterMainController.Instance.GetComponentInChildren<MeshRenderer>())
            {
                Material material = EtraCharacterMainController.Instance.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 255, 0).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }
            playerSetup();
        }
        
        void playerSetup()
        {
            EtraCharacterMainController.Instance.enableAllActiveAbilities(); //also maybe enable collision boxes for trigger?
            EtraCharacterMainController.Instance.setFov(savedFov);
            LeanTween.move(camRoot, EtraCharacterMainController.Instance.transform.position + new Vector3(0, 1.375f, 0), 0);
            camRoot.transform.localPosition = new Vector3(0, 1.375f, 0);
            LeanTween.rotate(camRoot, new Vector3(0, 0, 0), 0);
            //GEt base player cam root pos
            cursorCanvas.SetActive(true);
            foreach (AbilityOrItemPickup a in pickups)
            {
                a.gameObject.SetActive(true);
            }
            foreach (AnimationTriggerPickup a in animPickups)
            {
                a.gameObject.SetActive(true);
            }
            nongamerUi.SetActive(true);
            EtraCharacterMainController.Instance.etraFPSUsableItemManager.weaponInitHandledElsewhere = false;

            if (EtraCharacterMainController.Instance.etraFPSUsableItemManager.usableItems.Length >0)
            {
                if (equipAnimForStartingWeapon)
                {
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipItem(0);
                }
                else
                {
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.instatiateItemAtStart();
                }
                
            }
            
            openingDone = true;
        }
    }
}