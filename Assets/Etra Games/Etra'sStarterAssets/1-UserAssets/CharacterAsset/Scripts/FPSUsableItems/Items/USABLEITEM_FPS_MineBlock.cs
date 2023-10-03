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

        private bool isAiming = false;
        private float destroyCooldown = 0.367f; //Matched with animation timing atm
        private float buildCooldown = 0.367f;

        private GameObject heldItem;

        private bool itemReady = false;
        protected string iconName = "IconDirtBlock";

        public override string getNameOfPrefabToLoad() { return "FpsMineHandGroup"; }
        public override string getEquipSfxName() { return "NoSound"; }

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
                            mineAnimator.SetBool(hitAnim, true);
                            DestroyBlock();
                        }
                        else if (inInteractDistance(damageDistance) && (isDamageableCheck != null || hitObject.GetComponent<Rigidbody>()))
                        {
                            if (isDamageableCheck != null)
                            {
                                isDamageableCheck.TakeDamage(hitDamage);
                                mineAnimator.SetBool(hitAnim, true);
                            }
                            if (hitObject.GetComponent<Rigidbody>())
                            {
                                mineAnimator.SetBool(hitAnim, true);
                                var charController = EtraCharacterMainController.Instance.GetComponent<CharacterController>();
                                hitObject.GetComponent<Rigidbody>().AddForce(charController.transform.forward * rigidBodyKnockback, ForceMode.Impulse);
                            }
                        }
                        else
                        {
                            mineAnimator.SetBool(missAnim, true);
                        }
                        _hitTimeoutDelta = Time.time;
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
                    if (Time.time - _hitTimeoutDelta >= buildCooldown && inInteractDistance(blockInteractDistance))
                    {
                        mineAnimator.SetBool(hitAnim, true);
                        BuildBlock(blockToLoad.blockPrefab);
                        _hitTimeoutDelta = Time.time;
                        isAiming = true;
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
                        mineAnimator.SetBool(missAnim, true);
                        _hitTimeoutDelta = Time.time;
                    }
                }

                if (!starterAssetsInputs.shoot || isAiming)
                {
                    mineAnimator.SetBool(hitAnim, false);
                    mineAnimator.SetBool(missAnim, false);
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
                mineBlockAudioManager.Play("WoolBreak");
                Destroy(hitObject.gameObject);
            }
        }

        private void BuildBlock(GameObject block)
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

            if (!blockCollidingWithPlayer(spawnPosition))
            {
                Instantiate(block, spawnPosition, Quaternion.identity, MineBlockSystem.Instance.blocksParent.transform);
                mineBlockAudioManager.Play("WoolPlace");
            }
        }

        private bool blockCollidingWithPlayer(Vector3 spawnPosition)
        {
            Vector3 halfExtents = new Vector3(0.495f, 0.495f, 0.495f);
            Collider[] colliders = Physics.OverlapBox(spawnPosition, halfExtents, Quaternion.Euler(0, 0, 0), ~0,  QueryTriggerInteraction.Ignore);


            foreach (var collider in colliders)
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

            if (blockCollidingWithPlayer(spawnPosition))
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
