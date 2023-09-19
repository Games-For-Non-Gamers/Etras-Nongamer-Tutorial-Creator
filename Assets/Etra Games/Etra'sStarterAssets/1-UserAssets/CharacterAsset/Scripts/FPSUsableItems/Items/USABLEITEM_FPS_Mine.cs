using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using System.Collections;
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
        public float distanceToInteract = 5;
        public float swordCooldown = 1f;
        public int swordDamage = 3;
        public Vector2 hitCamShake = new Vector2(1f, 0.1f);

        //Private Variables
        private float _swordTimeoutDelta = 0;
        private bool cooling;
        private string hitAnim;
        private string missAnim;

        //References
        StarterAssetsInputs starterAssetsInputs;
        EtraFPSUsableItemManager etraFPSUsableItemManager;
        Animator mineAnimator;
        ABILITY_CameraMovement camMoveScript;
        AudioManager fpsItemAudioManager;


        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
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

        public void LateUpdate()
        {
            if (inputsLocked)
            {
                starterAssetsInputs.shoot = false;
                return;
            }

            if (starterAssetsInputs.shoot)
            {
                mineAnimator.SetBool(hitAnim, true);

            }
            else
            {
                mineAnimator.SetBool(hitAnim, false);
            }

        }



    }
}
