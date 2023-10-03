using Codice.CM.Client.Differences;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_MineBlock : EtraFPSUsableItemBaseClass
    {
        [Header("Block")]
        public MineBlockData blockToLoad;
        public bool canPlaceOnObjects = false;
        [Header("Interaction")]
        public float blockInteractDistance = 5;
        public float damageDistance = 3.5f;
        public int hitDamage = 1;
        public float rigidBodyKnockback = 5;
        private Vector2 hitCamShake = new Vector2(1f, 0.1f);

        protected bool isHand = false;


        [Header("Prefab refs")]
        private GameObject scalerAndAnimMover;

        [Header("Private Variables")]
        private float _hitTimeoutDelta = 0;
        private string hitAnim;
        private string missAnim;

        [Header("References")]
        private EtraCharacterMainController mainController;
        private StarterAssetsInputs starterAssetsInputs;
        private Animator mineAnimator;
        private ABILITY_CameraMovement camMoveScript;
        private AudioManager mineBlockAudioManager;

        private GameObject mineBlockSystem;
        private GameObject blockOutline;

        private float destroyCooldown = 0.1835f; //Matched with animation timing atm 0.367f
        private float buildCooldown = 0.1835f;
        private float savedDestroyCooldown;
        private float savedBuildCooldown;
        private GameObject heldItem;

        private bool itemReady = false;
        protected string iconName = "IconDirtBlock";

        public override string getNameOfPrefabToLoad() { return "FpsMineHandGroup"; }
        public override string getEquipSfxName() { return "NoSound"; }

        private enum ScanTypes
        {
            Create,
            Destroy,
            Scan
        }


        private void Reset()
        {
            blockToLoad = (MineBlockData)Resources.Load("DirtBlockDefaultData");
        }

        protected void Awake()
        {
            if (blockToLoad !=null)
            {
                iconName = blockToLoad.blockIcon.name;
            }

            Texture2D temp = (Texture2D)Resources.Load(iconName);
            inventoryImage = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), Vector2.zero);
            enabled = false;
            if (MineBlockSystem.Instance == null)
            {
                mineBlockSystem = (GameObject)Resources.Load("MineBlockSystem");
                mineBlockSystem = Instantiate(mineBlockSystem, Vector3.zero, Quaternion.identity);
                mineBlockSystem.name = "MineBlockSystem";
            }
        }

        protected void Start()
        {
            if (heldItem != null)
            {
                return;
            }

            HideOutline();
            mainController = FindObjectOfType<EtraCharacterMainController>();
            camMoveScript = FindObjectOfType<ABILITY_CameraMovement>();
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            savedDestroyCooldown = destroyCooldown;
            savedBuildCooldown = buildCooldown;

            if (isHand)
            {
                hitAnim = "ArmHit";
                missAnim = "ArmMiss";
            }
            else
            {
                heldItem = Instantiate(blockToLoad.blockPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), MineBlockSystem.Instance.systemParent.transform);
                heldItem.GetComponent<Collider>().enabled = false;
                heldItem.GetComponent<Renderer>().enabled = false;
                heldItem.GetComponent<MineBlock>().enabled = false;
                heldItem.layer = LayerMask.NameToLayer("EtraFPSUsableItem");
                hitAnim = "BlockHit";
                missAnim = "BlockMiss";
            }
        }

        public override void runEquipAnimation()
        {
            StartCoroutine(EquipWait());
        }

        private IEnumerator EquipWait()
        {
            while (!itemReady)
            {
                yield return new WaitForEndOfFrame();
            }
            base.runEquipAnimation();
        }

        protected void OnEnable()
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
                if (heldItem == null)
                {
                    Start();
                }
                scalerAndAnimMover = EtrasResourceGrabbingFunctions.FindObjectByNameRecursive(activeItem, "ScalerAndAnimMover");
                heldItem.transform.SetParent(scalerAndAnimMover.transform, false);
                heldItem.GetComponent<Renderer>().enabled = true;
            }
            Transform referenceToMineTransform = activeItem.transform;
            mineBlockAudioManager = activeItem.GetComponentInChildren<AudioManager>();
            mineAnimator = referenceToMineTransform.GetComponentInChildren<Animator>();
            itemReady = true;


        }

        protected void OnDisable()
        {
            HideOutline();
            itemReady = false;
            if (heldItem != null && !isHand && Application.isPlaying)
            {
                heldItem.transform.SetParent(MineBlockSystem.Instance.systemParent.transform, false);
                heldItem.GetComponent<Renderer>().enabled = false;
            }
        }


        private RaycastHit hitInfo;
        private GameObject hitObject;

        protected void Update()
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

                if (inInteractDistance(blockInteractDistance) && hitObject.GetComponent<MineBlock>())
                {
                    ShowOutline();
                }
                else
                {
                    HideOutline();
                }

                if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
                {
                    if (Time.time - _hitTimeoutDelta >= destroyCooldown)
                    {
                        var isDamageableCheck = hitObject.GetComponent<IDamageable<int>>();
                        if (inInteractDistance(blockInteractDistance) && hitObject.GetComponent<MineBlock>())
                        {
                            mineAnimator.SetTrigger(hitAnim);
                            DestroyBlock();
                            destroyCooldown = savedDestroyCooldown;
                            buildCooldown = savedBuildCooldown;
                        }
                        else if (inInteractDistance(damageDistance) && (isDamageableCheck != null || hitObject.GetComponent<Rigidbody>()))
                        {
                            if (isDamageableCheck != null)
                            {
                                isDamageableCheck.TakeDamage(hitDamage);
                                mineAnimator.SetTrigger(hitAnim);
                            }
                            if (hitObject.GetComponent<Rigidbody>())
                            {
                                mineAnimator.SetTrigger(hitAnim);
                                var charController = EtraCharacterMainController.Instance.GetComponent<CharacterController>();
                                hitObject.GetComponent<Rigidbody>().AddForce(charController.transform.forward * rigidBodyKnockback, ForceMode.Impulse);
                                destroyCooldown = savedDestroyCooldown;
                                buildCooldown = savedBuildCooldown;
                            }
                        }
                        else
                        {
                            mineAnimator.SetTrigger(missAnim);
                            destroyCooldown = savedDestroyCooldown*2;
                            buildCooldown = savedBuildCooldown * 2;

                        }
                        _hitTimeoutDelta = Time.time;
                    }
                }

                if (starterAssetsInputs.aim && !isHand)
                {
                    if (Time.time - _hitTimeoutDelta >= buildCooldown && inInteractDistance(blockInteractDistance))
                    {
                        destroyCooldown = savedDestroyCooldown;
                        buildCooldown = savedBuildCooldown;
                        _hitTimeoutDelta = Time.time;

                        Vector3 spawnPosition = GetSpawnPosition();
                        if (!BlockPlacementCollidingWithNonTrigger(spawnPosition))
                        {
                            GameObject block = Instantiate(blockToLoad.blockPrefab, spawnPosition, Quaternion.identity, MineBlockSystem.Instance.blocksParent.transform);
                            MineBlock mineBlock = block.GetComponent<MineBlock>();
                            mineBlock.blockData = blockToLoad;

                            if (checker != null)
                            {
                                checker.BlockPlaced(mineBlock);
                                checker = null;
                            }

                            mineBlockAudioManager.Play("WoolPlace");
                            mineAnimator.SetTrigger(hitAnim);
                        }
                    }
                }
            }
            else
            {
                HideOutline();

                if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
                {
                    if (Time.time - _hitTimeoutDelta >= destroyCooldown)
                    {
                        mineAnimator.SetTrigger(missAnim);
                        destroyCooldown = savedDestroyCooldown * 2;
                        buildCooldown = savedBuildCooldown * 2;
                        _hitTimeoutDelta = Time.time;
                    }
                }

            }
        }

        private bool inInteractDistance(float customDist)
        {
            return customDist > Vector3.Distance(mainController.modelParent.transform.position, hitInfo.point);
        }

        private void DestroyBlock()
        {
            if (camMoveScript.objectHit && hitObject.GetComponent<MineBlock>())
            {
                BlockPlacementCollidingWithNonTrigger(hitObject.GetComponent<MineBlock>().transform.position);
                if (checker != null)
                {
                    checker.BlockDestroyed();
                    checker = null;
                }
                mineBlockAudioManager.Play("WoolBreak");
                Destroy(hitObject.gameObject);
            }
        }

        private Vector3 GetSpawnPosition()
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
            return spawnPosition;
        }

        private bool BlockPlacementCollidingWithNonTrigger()
        {
            return BlockPlacementCollidingWithNonTrigger(GetSpawnPosition());
        }

        MineblockChecker checker = null;
        private bool BlockPlacementCollidingWithNonTrigger( Vector3 spawnPosition)
        {
            checker = null;
            Vector3 halfExtents = new Vector3(0.495f, 0.495f, 0.495f);
            Collider[] colliders = Physics.OverlapBox(spawnPosition, halfExtents, Quaternion.Euler(0, 0, 0), ~0,  QueryTriggerInteraction.Collide);

            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<MineblockChecker>())
                {
                    checker = collider.gameObject.GetComponent<MineblockChecker>();
                }

                if (!collider.isTrigger)
                {
                    if (!canPlaceOnObjects)
                    {
                        return true;
                    }

                    if (collider.gameObject.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ShowOutline()
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
                blockOutline.transform.rotation = Quaternion.Euler(0, 0, 0);
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

            if (BlockPlacementCollidingWithNonTrigger())
            {
                HideOutline();
            }
            else
            {
                MineBlockSystem.Instance.ShowOutline();
            }
        }

        private void HideOutline()
        {
            if (MineBlockSystem.Instance)
            {
                MineBlockSystem.Instance.HideOutline();
            }
        }
    }
}
