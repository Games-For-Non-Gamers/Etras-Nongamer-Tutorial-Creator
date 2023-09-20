using Codice.CM.Client.Differences;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
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
        public GameObject blockOutline;
        public GameObject blockParticle; // 6?
        public float interactDistance = 5;
        public float swordCooldown = 1f;
        public int swordDamage = 3;
        public Vector2 hitCamShake = new Vector2(1f, 0.1f);
        public Transform blocksParent;

        //Private Variables
        private float _swordTimeoutDelta = 0;
        private bool cooling;
        private string hitAnim;
        private string missAnim;

        //References
        EtraCharacterMainController mainController;
        StarterAssetsInputs starterAssetsInputs;
        EtraFPSUsableItemManager etraFPSUsableItemManager;
        Animator mineAnimator;
        ABILITY_CameraMovement camMoveScript;
        AudioManager fpsItemAudioManager;


        private bool isAiming = false; // New boolean flag to track aiming
        private float destroyCooldown = 0.367f; // Cooldown for destroy action
        private float buildCooldown = 0.367f; // Cooldown for build action


        public enum MCFace
        {
            None,
            Up,
            Down,
            East,
            West,
            North,
            South
        }

        public MCFace test;


        private void Awake()
        {
            enabled = false;
        }
        MeshRenderer [] outlineMeshComponents;
        const int AMOUNT_OF_PARTICLES = 7;
        GameObject[] blockParticles = new GameObject[AMOUNT_OF_PARTICLES];
        int currentParticle;
        private void Start()
        {
            blockOutline = Instantiate(blockOutline, Vector3.zero, Quaternion.identity, blocksParent);
            for (int i = 0; i < blockParticles.Length; i++)
            {
                blockParticles[i] = Instantiate(blockParticle, Vector3.zero, Quaternion.Euler(-90,0,0), blocksParent);
            }
            outlineMeshComponents = blockOutline.GetComponentsInChildren<MeshRenderer>();
            HideOutline();
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
            mainController = FindObjectOfType<EtraCharacterMainController>();
        }

        public void OnEnable()
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            etraFPSUsableItemManager = GetComponent<EtraFPSUsableItemManager>();
            etraFPSUsableItemManager = GetComponent<EtraFPSUsableItemManager>();
            Transform referenceToMineTransform = etraFPSUsableItemManager.activeItemPrefab.transform;
            mineAnimator = referenceToMineTransform.GetComponentInChildren<Animator>();

            camMoveScript = GameObject.Find("EtraAbilityManager").GetComponent<ABILITY_CameraMovement>();

            if (isHand)
            {
                hitAnim = "ArmHit";
                missAnim = "ArmMiss";
            }
            else
            {
                hitAnim = "BlockHit";
                missAnim = "BlockMiss";
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
            if (inputsLocked)
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

                if (starterAssetsInputs.aim && !isAiming)
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
            return interactDistance > Vector3.Distance(mainController.modelParent.transform.position, hitInfo.point);
        }

        void DestroyBlock()
        {
            if (camMoveScript.objectHit)
            {
                if (hitObject.GetComponent<MineBlock>())
                {
                    if (hitObject.GetComponent<BoxCollider>())
                    {
                        hitObject.GetComponent<BoxCollider>().enabled = false;
                    }
                    if (hitObject.GetComponent<MeshCollider>())
                    {
                        hitObject.GetComponent<MeshCollider>().enabled = false;
                    }

                    blockParticles[currentParticle].transform.position = hitObject.transform.position;
                    Destroy(hitObject.gameObject);
                    blockParticles[currentParticle].GetComponent<ParticleSystem>().Play();
                    currentParticle++;
                    if (currentParticle >= blockParticles.Length)
                    {
                        currentParticle = 0;
                    }
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



            //Put this in outline validity?
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f); // Adjust the radius as needed

            bool playerCollision = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerCollision = true;
                    break;
                }
            }

            if (!playerCollision)
            {
                Instantiate(block, spawnPosition, Quaternion.identity, blocksParent);
            }

        }


        public void ShowOutline()
        {
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
            foreach (MeshRenderer mesh in outlineMeshComponents)
            {
                mesh.enabled = true;
            }

        }

        void HideOutline()
        {
            foreach (MeshRenderer mesh in outlineMeshComponents)
            {
                mesh.enabled= false;
            }
        }

    }
}
