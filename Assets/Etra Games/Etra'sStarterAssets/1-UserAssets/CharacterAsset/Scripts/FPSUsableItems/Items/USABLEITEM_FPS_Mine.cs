using Codice.CM.Client.Differences;
using Codice.CM.Common;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_Mine : EtraFPSUsableItemBaseClass
    {
        //Name of Prefab to load and required function
        public string nameOfPrefabToLoadFromAssets = "FpsMineHandGroup";
        public override string getNameOfPrefabToLoad() { return nameOfPrefabToLoadFromAssets; }

        [Header("Basics")]
        public bool isHand = false;
        public GameObject blockToLoad;
        public float blockInteractDistance = 5;
        public float damageDistance = 2;
        public float hitDamage = 1;
        public float swordCooldown = 1f;
        public int swordDamage = 3;
        public Vector2 hitCamShake = new Vector2(1f, 0.1f);
        public GameObject mineBlockSystem;

        //Private Variables
        private float _swordTimeoutDelta = 0;
        private bool cooling;
        private string hitAnim;
        private string missAnim;

        [Header("Prefab refs")]
        private GameObject scalerAndAnimMover;

        //References
        EtraCharacterMainController mainController;
        StarterAssetsInputs starterAssetsInputs;
        Animator mineAnimator;
        ABILITY_CameraMovement camMoveScript;
        AudioManager fpsItemAudioManager;
        AudioManager mineBlockAudioManager;

        GameObject blockOutline;
        GameObject blockDestroyParticle; // 6?


        private bool isAiming = false; // New boolean flag to track aiming
        private float destroyCooldown = 0.367f; // Cooldown for destroy action
        private float buildCooldown = 0.367f; // Cooldown for build action



        private void Awake()
        {
            enabled = false;
            if (MineBlockSystem.Instance == null)
            {
                mineBlockSystem = (GameObject)Resources.Load("MineBlockSystem");
                mineBlockSystem = Instantiate(mineBlockSystem, Vector3.zero, Quaternion.identity);
                mineBlockSystem.name = "MineBlockSystem";
            }     
        }


        GameObject heldItem;
        private void Start()
        {
            if(heldItem != null)
            {
                return;
            }


            HideOutline();
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
            mainController = FindObjectOfType<EtraCharacterMainController>();
            camMoveScript = FindObjectOfType<ABILITY_CameraMovement>();

            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            if (isHand)
            {
                hitAnim = "ArmHit";
                missAnim = "ArmMiss";
            }
            else
            {
                heldItem = Instantiate(blockToLoad, Vector3.zero, Quaternion.Euler(0, 0, 0), MineBlockSystem.Instance.systemParent.transform);
                heldItem.GetComponent<Collider>().enabled = false;
                heldItem.GetComponent<Renderer>().enabled = false;
                heldItem.GetComponent<MineBlock>().enabled = false;
                heldItem.layer = LayerMask.NameToLayer("EtraFPSUsableItem");
                hitAnim = "BlockHit";
                missAnim = "BlockMiss";

            }

        }

        public override void  runEquipAnimation() //return true when Animation is complete
        {
            StartCoroutine(equipWait());
        }
        IEnumerator equipWait()
        {
            while (!itemReady)
            {
                yield return new WaitForEndOfFrame();
            }
            base.runEquipAnimation();
        }

        bool itemReady = false;
        public void OnEnable()
        {
            itemReady = false;
            blockOutline = MineBlockSystem.Instance.blockOutline;
            GameObject activeItem = EtraCharacterMainController.Instance.etraFPSUsableItemManager.activeItemPrefab;
            if (isHand)
            {
                GameObject hand = EtrasResourceGrabbingFunctions.FindObjectByNameRecursive(activeItem, "Hand");
                hand.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                GameObject scalerAndAnimMover = EtrasResourceGrabbingFunctions.FindObjectByNameRecursive(activeItem, "ScalerAndAnimMover");
                if (heldItem == null)
                {
                    Start();
                }
                heldItem.transform.SetParent(scalerAndAnimMover.transform,false);
                heldItem.GetComponent<Renderer>().enabled = true;
            }
            Transform referenceToMineTransform = activeItem.transform;
            mineBlockAudioManager = activeItem.GetComponentInChildren<AudioManager>();
            mineAnimator = referenceToMineTransform.GetComponentInChildren<Animator>();
            itemReady = true;
        }

        private void OnDisable()
        {
            HideOutline();
            itemReady = false;
            if (heldItem!= null)
            {
                heldItem.transform.SetParent(MineBlockSystem.Instance.systemParent.transform, false);
                heldItem.GetComponent<Renderer>().enabled=false;
            }

        }

        private void Reset()
        {
            equipSfxName = "";
        }
        RaycastHit hitInfo;
        GameObject hitObject;
        public void Update()
        {

            if (inputsLocked || !itemReady)
            {
                starterAssetsInputs.shoot = false;
                starterAssetsInputs.aim = false;
                return;
            }

            if (camMoveScript.objectHit)
            {
                hitInfo = camMoveScript.raycastHit;
                hitObject = camMoveScript.raycastHit.transform.gameObject;

                if (inInteractDistance() && hitObject.GetComponent<MineBlock>())
                {
                    ShowOutline();
                }
                else
                {
                   HideOutline();
                }

                if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
                {
                    if (Time.time - _swordTimeoutDelta >= destroyCooldown)
                    {
                        if (inInteractDistance())
                        {
                            mineAnimator.SetBool(hitAnim, true);
                            DestroyBlock();
                        }
                        else
                        {
                            mineAnimator.SetBool(missAnim, true);
                        }
                        _swordTimeoutDelta = Time.time;
                    }
                }

                if (!starterAssetsInputs.shoot || isAiming)
                {
                    mineAnimator.SetBool(hitAnim, false);
                    mineAnimator.SetBool(missAnim, false);
                }

                if (!starterAssetsInputs.aim)
                {
                    isAiming = false;
                }

                if (starterAssetsInputs.aim && !isAiming && !isHand)
                {
                    if (Time.time - _swordTimeoutDelta >= buildCooldown && inInteractDistance())
                    {
                        mineAnimator.SetBool(hitAnim, true);
                        BuildBlock(blockToLoad);
                        _swordTimeoutDelta = Time.time;
                        isAiming = true;
                    }
                }

            }
            else
            {
                HideOutline();

                if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
                {
                    if (Time.time - _swordTimeoutDelta >= destroyCooldown)
                    {
                        mineAnimator.SetBool(missAnim, true);
                        _swordTimeoutDelta = Time.time;
                    }
                }

                if (!starterAssetsInputs.shoot || isAiming)
                {
                    mineAnimator.SetBool(hitAnim, false);
                    mineAnimator.SetBool(missAnim, false);
                }

            }

        }


        bool inInteractDistance()
        {
            return blockInteractDistance > Vector3.Distance(mainController.modelParent.transform.position, hitInfo.point);
        }

        void DestroyBlock()
        {
            if (camMoveScript.objectHit)
            {
                if (hitObject.GetComponent<MineBlock>())
                {
                    mineBlockAudioManager.Play("WoolBreak");
                    Destroy(hitObject.gameObject);
                }
            }
        }

        void BuildBlock(GameObject block)
        {

            //Distance Check here?
            Vector3 spawnPosition;
            if (hitObject.GetComponent<MineBlock>())
            {
                spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
            }
            else
            {
                spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
            }

            if (!blockCollidingWithPlayer(spawnPosition))
            {
                Instantiate(block, spawnPosition, Quaternion.identity, MineBlockSystem.Instance.blocksParent.transform);
                mineBlockAudioManager.Play("WoolPlace");
            }

        }


        bool blockCollidingWithPlayer(Vector3 spawnPosition)
        {
            //Put this in outline validity?
            Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);
            Collider[] colliders = Physics.OverlapBox(spawnPosition, halfExtents); // Box shape

            bool playerCollision = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerCollision = true;
                    break;
                }
            }
            return playerCollision;
        }

        public void ShowOutline()
        {
            Vector3 spawnPosition;
            if (hitObject.GetComponent<MineBlock>())
            {
                spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
            }
            else
            {
                spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
            }

            blockOutline.transform.position = hitObject.transform.position;


            Vector3 incomingVec = hitInfo.normal - Vector3.up;
            if (incomingVec == new Vector3(0, -1, -1))
            {
                blockOutline.transform.rotation = Quaternion.Euler(0,0,0);
            }
            if (incomingVec == new Vector3(0, -1, 1))
            {
                blockOutline.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            if (incomingVec == new Vector3(0, 0, 0))
            {
                blockOutline.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            if (incomingVec == new Vector3(0, -2, 0))
            {
                blockOutline.transform.rotation = Quaternion.Euler(270, 0, 0);
            }

            if (incomingVec == new Vector3(-1, -1, 0))
            {
                blockOutline.transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            if (incomingVec == new Vector3(1, -1, 0))
            {
                blockOutline.transform.rotation = Quaternion.Euler(0, 270, 0);
            }

            if (blockCollidingWithPlayer(spawnPosition))
            {
                HideOutline();
            }
            else
            {
                MineBlockSystem.Instance.ShowOutline();
            }


        }

        void HideOutline()
        {
            if (MineBlockSystem.Instance)
            {
                MineBlockSystem.Instance.HideOutline();
            }

        }

    }
}
