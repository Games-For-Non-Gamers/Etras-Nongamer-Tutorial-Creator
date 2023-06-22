using Etra.StarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class OpeningScene : MonoBehaviour
    {

        [Header("CutsceneCheckpoints")]
        public Star star;
        public GameObject playerSpawn;
        public GameObject camRoot;
        public GameObject cursorCanvas;

        // Start is called before the first frame update
        private void Reset()
        {
            initialize();
        }

        public void initialize()
        {
            star = GameObject.Find("Star").GetComponent<Star>();
            playerSpawn = GameObject.Find("PlayerSpawn");
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
            cursorCanvas = GameObject.Find("CursorCanvas");
        }

        // Update is called once per frame
    
        void Awake()
        {
            cursorCanvas.SetActive(false);  
            camRoot.transform.position = star.transform.position + new Vector3(0,50,-30);
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
            EtraCharacterMainController.Instance.etraFPSUsableItemManager.weaponInitHandledElsewhere = true;
        }

        private void Start()
        {
            GameObject scoutStar = new GameObject("ScoutStar");
            scoutStar.transform.parent = star.transform;
            scoutStar.transform.localPosition = Vector3.zero;
            GameObject scoutSpawn = new GameObject("ScoutSpawn");
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
                        Debug.Log("Raycast hit target object!");
                        heightCorrect = true;
                    }
                }
            }


            runOpeningScene();
        }

        public void runOpeningScene()
        {
            StartCoroutine(openingScene());
        
        }

        IEnumerator openingScene()
        {
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -30), 4).setEaseInOutSine();
            yield return new WaitForSeconds(4.2f);
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0.3f, -7.37f), 1).setEaseInOutSine();
            yield return new WaitForSeconds(0.5f);
            //pLAY TEXT ANIM
            yield return new WaitForSeconds(0.6f);
            star.TakeDamage(0);
            yield return new WaitForSeconds(1.1f);

        }
    
    }
}