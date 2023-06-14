using Cinemachine;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Abilities.FirstPerson;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Items;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using UnityEngine.Networking.Types;
#endif

namespace Etra.StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class EtraCharacterMainController : MonoBehaviour
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //PUBLIC AND PRIVATE VARIABLES
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Instance (this is for easy referencing of the one Etra Character in the scene, Instance is set up in OnValidate())
        [HideInInspector] public static EtraCharacterMainController Instance;

        //************************
        //Gameplay type selector
        //************************
        //Make custom enums
        [HideInInspector]
        public enum GameplayType
        {
            FirstPerson,
            ThirdPerson
        }

        [System.Flags]
        public enum GameplayTypeFlags
        {
            All = 3,
            FirstPerson = 1,
            ThirdPerson = 2
        }

        [HideInInspector]
        public enum Model
        {
            DefaultArmature,
            Capsule,
            Voxel,
            None
        }
        //These public variables let you select what gameplay type you want
        [SerializeField] private GameplayType gameplayType;
        [SerializeField] private Model characterModel;
        [HideInInspector] public GameplayType appliedGameplayType = EtraCharacterMainController.GameplayType.FirstPerson;
        [HideInInspector] public Model appliedCharacterModel = EtraCharacterMainController.Model.Capsule;
        //This is space for the "Apply Gameplay Changes" button generated in UnityEditorForEtraCharacterMainController.cs
        [Space(50)]

        //************************
        //Gravity and floor collision variables
        //************************
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("The maximum slope of an object the character can climb up")]
        public float maxWalkableSlope = 45;
        public float currentSlopeAngle;
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.2f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        [Header("Cam Shake")]
        public bool landingShakeEnabled = true;
        public Vector2 landingShake = new Vector2(1f, 0.1f);
        


        [HideInInspector]
        public float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        [HideInInspector]
        public float _fallTimeoutDelta;
        // animation IDs for floor collision events
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;

        //************************
        //Camera Setup variables
        //************************
        GameObject etraFollowCam;
        private NoiseSettings shake;
        private NoiseSettings handheldNormal;

        //************************
        //References
        //************************
        private bool _hasAnimator;
        private Animator _animator;
        private CharacterController _controller;
        private float characterHeight;
        private ABILITY_Jump abilityJump;
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;

        //************************
        //Externally called function variables
        //************************
        private const int APPLIED_FORCES_AMOUNT = 6;
        private Vector3[] appliedForces = new Vector3[APPLIED_FORCES_AMOUNT]; //Up to six forces can be applied to the character in a single frame. Adjust this to change.


        //************************
        //Variables of child objects
        //************************
        [HideInInspector] public Transform modelParent;
        [HideInInspector] public StarterAssetsCanvas starterAssetCanvas;
        [HideInInspector] public EtraAbilityManager etraAbilityManager;
        [HideInInspector] public EtraFPSUsableItemManager etraFPSUsableItemManager;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //SETUP FUNCTIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Gameplay Type Changing Functions 

        //This function is what you want to run if you wish to change gameplay settings from an outside script
        public void applyGameplayChanges(GameplayType passedGameplayType, Model passedCharacterModel)
        {
            appliedGameplayType = passedGameplayType;
            appliedCharacterModel = passedCharacterModel;
            applyGameplayChanges();

        }

        //This function runs when the inspector button is pressed
        public void applyGameplayChangesInspectorButtonPressed()
        {
            appliedGameplayType = gameplayType;
            appliedCharacterModel = characterModel;
            applyGameplayChanges();
        }

        private void applyGameplayChanges()
        {
            //Update public inspector variables
            gameplayType = appliedGameplayType;
            characterModel = appliedCharacterModel;

            //Destroy the current Cinemachine Virtual Camera
            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            DestroyImmediate(etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>());
            //Create an editable Cinemachine Virtual Camera
            etraFollowCam.GetComponent<CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine3rdPersonFollow>();
            Cinemachine3rdPersonFollow newCamComponent = etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            newCamComponent.VerticalArmLength = 0;
            //Maken the new camera collide with and ignore correct layers
            newCamComponent.CameraCollisionFilter = LayerMask.GetMask("Default");
            newCamComponent.IgnoreTag = "Player";
            newCamComponent.CameraRadius = 0.15f;

            switch (appliedGameplayType)
            {
                case EtraCharacterMainController.GameplayType.FirstPerson:
                    //FPS Default Camera Settings
                    newCamComponent.CameraDistance = 0;
                    newCamComponent.CameraSide = 0.6f;
                    newCamComponent.ShoulderOffset = new Vector3(0, 0, 0);
                    newCamComponent.Damping = new Vector3(0.0f, 0.0f, 0.0f);

                    //FPS Ability Variable Default adjustment
                    if (GetComponentInChildren<ABILITY_CameraMovement>())
                    {
                        GetComponentInChildren<ABILITY_CameraMovement>().setForwardToPlayerLookDirection = true;
                    }
                    if (GetComponentInChildren<ABILITY_CharacterMovement>())
                    {
                        GetComponentInChildren<ABILITY_CharacterMovement>().rotateTowardMoveDirection = false;
                    }
                    break;

                case EtraCharacterMainController.GameplayType.ThirdPerson:
                    //TPS Default Camera Settings
                    newCamComponent.CameraDistance = 4;
                    newCamComponent.CameraSide = 1f;
                    newCamComponent.ShoulderOffset = new Vector3(0.7f, 0.25f, 0);
                    newCamComponent.Damping = new Vector3(0.1f, 0.25f, 0.3f);

                    //TPS Ability Variable Default adjustment
                    if (GetComponentInChildren<ABILITY_CameraMovement>())
                    {
                        GetComponentInChildren<ABILITY_CameraMovement>().setForwardToPlayerLookDirection = false;
                    }
                    if (GetComponentInChildren<ABILITY_CharacterMovement>())
                    {
                        GetComponentInChildren<ABILITY_CharacterMovement>().rotateTowardMoveDirection = true;
                    }
                    break;
            }

            
            //Destroy the current character model
            foreach (Transform child in modelParent)
            {
                DestroyImmediate(child.gameObject);
            }
            GameObject model;
            //Select correct character model
            switch (appliedCharacterModel)
            {

                case EtraCharacterMainController.Model.DefaultArmature:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("DefaultArmatureCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("DefaultArmatureCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.Capsule:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CapsuleCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("CapsuleCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.Voxel:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("VoxelCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("VoxelCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.None:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("InvisibleCapsuleModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("InvisibleCapsuleModel").transform.localPosition;
                    break;
            }
        }

        #endregion
        #region Setup, Reset, and OnValidate functions
        private void Reset()
        {
            setChildObjects();
            GroundLayers = LayerMask.GetMask("Default");
        }

        public void setChildObjects()
        {
            modelParent = GetComponentInChildren<ModelParent>().gameObject.transform;
            starterAssetCanvas = GetComponentInChildren<StarterAssetsCanvas>();
            etraAbilityManager = GetComponentInChildren<EtraAbilityManager>();

            if (etraAbilityManager.GetComponent<ABILITY_Jump>() !=null)
            {
                abilityJump = etraAbilityManager.GetComponent<ABILITY_Jump>();
            }

            if (GetComponentInChildren<EtraFPSUsableItemManager>() != null)
            {
                etraFPSUsableItemManager = GetComponentInChildren<EtraFPSUsableItemManager>();
            }
        }

        private void setUpCinemachineScreenShakeNoiseProfile()
        {
            //Set up cinemachine screen shake profile
            if (Resources.Load<NoiseSettings>("6DShake") == null)
            {
                Debug.LogError("6DShake Cinemachine Noise Settings not found.");
                return;
            }
            shake = Resources.Load<NoiseSettings>("6DShake");


            if (Resources.Load<NoiseSettings>("HandheldNormal") == null)
            {
                Debug.LogError("HandheldNormal Cinemachine Noise Settings not found.");
                return;
            }
            handheldNormal = Resources.Load<NoiseSettings>("HandheldNormal");

            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = shake;
        }

        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
        }
        #endregion
        #region Grounded and Gravity Functions

        public RaycastHit hit;
        bool stuckOnWallSlope = false;
        Vector3 flatBeamToTarget;
        Vector3 lastStableStandingPosition;
        public void GroundedCheck()
        {
            //Get ground slope angle for slide ability
            updateSlope();
            //set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);

            //Get every object that the player is colliding with
            Collider[] hitcolliders = new Collider[5];//max colliders to check
            int hitColliderCount = Physics.OverlapSphereNonAlloc(spherePosition, GroundedRadius, hitcolliders, GroundLayers, QueryTriggerInteraction.Ignore);
            if (hitColliderCount > 0)
            {
                Grounded = false; //Start grounded as false and it should be proven

                //This whole section exists to make sure the character is not grounded on any 46-89 degree surface. 
                //Without a special clause for this, the player can jump up objects with sharp angles.
                for(int i = 0; i< hitColliderCount; i++)
                {
                    Debug.Log("Collider:"+ hitcolliders[i].name);

                    Vector3 target;
                    if (hitcolliders[i] is MeshCollider meshCollider && !meshCollider.convex)
                    {
                        //If a non-convex mesh collider is hit, it is hard to get it's angle since you can't get its contact point.
                        //The next best thing we can go off of is the position of the object
                        target = hitcolliders[i].transform.position;
                    }
                    else
                    {
                        //for all other colliders we can get the contact point of the object
                        target = hitcolliders[i].ClosestPoint(transform.position);
                    }
                   
                    //Make a flat raycast toward the target
                    flatBeamToTarget = new Vector3(target.x, 0, target.z)  - new Vector3(transform.position.x, 0, transform.position.z);
                    //The range of the raycast is only in the sphere collider where the character can start climbing walls.
                    //The beam starts from 5% of player height

                    Vector3 raycastOrigin = transform.position + new Vector3(0, _controller.height * 0.02f, 0);

                    if (i ==0 )
                    {
                        Debug.DrawRay(raycastOrigin, flatBeamToTarget * _controller.radius, Color.red);
                    }
                    if (i == 1)
                    {
                        Debug.DrawRay(raycastOrigin, flatBeamToTarget * _controller.radius, Color.blue);
                    }
                    if (i == 2)
                    {
                        Debug.DrawRay(raycastOrigin, flatBeamToTarget * _controller.radius, Color.green);
                    }

                    if (Physics.Raycast(raycastOrigin, flatBeamToTarget, out hit, _controller.radius))
                    {
                        Debug.Log(Vector3.Angle(Vector3.up, hit.normal));
                        //Check if the angle is larger than the max walkable slope
                        if (Vector3.Angle(Vector3.up, hit.normal) > maxWalkableSlope)
                        {
                            Grounded = false;
                            stuckOnWallSlope = true;
                        }
                        //If the angle is walkable, then apply grounded and break from the loop. There is no need to check the other colliders
                        else
                        {
                            Grounded = true;
                            break;
                        }
                    }
                    else // if the Raycast is out of range, apply grounded and break from the loop.There is no need to check the other colliders
                    {
                        Grounded = true;
                        break;
                    }

                }
            }
            else //If the sphere is hitting no colliders then set grounded to false
            {
                stuckOnWallSlope = false; //They are not stuck on the wall if they are not touching anything
                Grounded = false;
            }

            //Make stuckOnWallSlope false if an earlier object homehow triggered this and was later corrected
            if (Grounded)
            {
                stuckOnWallSlope = false;
            }

            if (!stuckOnWallSlope)
            {
                lastStableStandingPosition = this.transform.position;
            }



            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }



        void updateSlope()
        {
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, characterHeight * 0.5f + 0.3f))
            {
                currentSlopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            }
        }

        bool pauseGravityGain = false;
        bool jumpReset = true;
        bool startGameLandSfxOff = true;
        private void ApplyGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (jumpReset == true)
                {

                    if (abilityJump != null)
                    {
                        abilityJump.lockJump = false;
                    }

                    if (startGameLandSfxOff)
                    {
                        startGameLandSfxOff = false;
                        jumpReset = false;
                    }
                    else if (!startGameLandSfxOff) 
                    {
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDJump, false);
                        }
                        else
                        {
                            abilitySoundManager.Play("Land");
                        }
                        
                        jumpReset = false;
                        if (landingShakeEnabled) { CinemachineShake.Instance.ShakeCamera(landingShake); }
                    }
                }
                
            }
            else
            {
                jumpReset = true;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                if (!pauseGravityGain)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
                
                if (!stuckOnWallSlope)
                {
                    pauseGravityGain = false;
                    addConstantForceToEtraCharacter(new Vector3(0, _verticalVelocity, 0));
                }
                else
                {
                    Debug.Log("Stuck");
                    Vector3 angledForce = Vector3.ProjectOnPlane(flatBeamToTarget, hit.normal).normalized;
                    if (angledForce != Vector3.zero) // If we are not cornered on a flat surface slide down with the slope angle
                    {
                        Debug.Log("tilt");
                        addConstantForceToEtraCharacter(angledForce * _verticalVelocity);
                    }
                    else
                    {
                        Debug.Log("90");
                        //adds a fixed force of negative one to slide the character off the edge if they are in trapped in the corner of a square obstacle
                         pauseGravityGain = true;
                        Vector3 awayFromLastStablePoint = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(lastStableStandingPosition.x, 0, lastStableStandingPosition.z) ;

                        addConstantForceToEtraCharacter((flatBeamToTarget.normalized) * -1) ;
                      //  addConstantForceToEtraCharacter((awayFromLastStablePoint.normalized) * 1);
                    }
                    
                }
                
            }

        }



        private void OnDrawGizmosSelected()
        {
            _controller = GetComponent<CharacterController>();
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        #endregion

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //MAIN FUNCTIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Awake()
        {
            Physics.autoSyncTransforms = true;

            //Set up Instance so it can be easily referenced. 
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Instance = this;
            }

            setChildObjects();
        }

        private void Start()
        {
            //Run setup functions
            setUpCinemachineScreenShakeNoiseProfile();
            AssignAnimationIDs();
            //Set reference variables
            _hasAnimator = TryGetComponent(out _animator);
            _hasAnimator = EtrasResourceGrabbingFunctions.TryGetComponentInChildren<Animator>(modelParent);
            if (_hasAnimator) { _animator = modelParent.GetComponentInChildren<Animator>(); }
            _controller = GetComponent<CharacterController>();
            maxWalkableSlope = _controller.slopeLimit;
            characterHeight = _controller.height;
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
        }

        private void Update()
        {
            //Calculate Grounded and gravity
            GroundedCheck();
            ApplyGravity();
            //Apply vertical velocity from gravity or jump every frame
   

            
            updateImpulseVariables();

        }

        private void LateUpdate()
        {
            //Apply movement and forces to the character controller
            Vector3 overallForce = Vector3.zero;
            // string log= "";
            for (int i = 0; i < appliedForces.Length; i++)
            {
                //log += appliedForces[i] + ", ";
                overallForce += appliedForces[i];
            }
            //We want to call character controller .Move() once every frame with one overall vector or it acts wonky.  
            // Debug.Log(log);
            _controller.Move(overallForce * Time.deltaTime);
            //Reset all applied forces to Vector3.zero
            appliedForces = new Vector3[APPLIED_FORCES_AMOUNT];
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //COLLISIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool damageOnCollisionBool = false;
        int damageOnCollisionValue;
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (etraAbilityManager.GetComponent<ABILITY_RigidbodyPush>())
            {
                etraAbilityManager.GetComponent<ABILITY_RigidbodyPush>().PushRigidBodies(hit);
            }

            if (damageOnCollisionBool)
            {
                IDamageable<int> isDamageableCheck = hit.gameObject.GetComponent<IDamageable<int>>();
                if (isDamageableCheck != null)
                {
                    isDamageableCheck.TakeDamage(damageOnCollisionValue);
                    damageOnCollisionBool = false;
                }
            }

        }



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //CUSTOM FUNCTIONS TO BE CALLED FROM OTHER SCRIPTS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void addConstantForceToEtraCharacter(Vector3 addedForce)
        {
            int emptyIndex = -1;
            for (int i = 0; i < appliedForces.Length; i++)
            {
                if (appliedForces[i].Equals(Vector3.zero))
                {
                    emptyIndex = i;
                    i = appliedForces.Length;
                }

                if (i == appliedForces.Length - 1)
                {
                    Debug.Log("Cannot add any more forces to character controller in this singular frame. To add more forces to the character" +
                        " please increase APPLIED_FORCES_AMOUNT in EtraCharacterMainController.cs .");
                    return;
                }

            }
            appliedForces[emptyIndex] = addedForce;
        }


        Vector3 impact = Vector3.zero;
        public void updateImpulseVariables()
        {
            if (impact.magnitude > 0.2f)
            {
                _controller.Move(impact * Time.deltaTime);
            }

            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        }

        public void addImpulseForceToEtraCharacter(Vector3 direction, float force)
        {
            direction.Normalize();
            if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
            impact += direction.normalized * force / 3.0f;
        }


        public void addImpulseForceWithDamageToEtraCharacter(Vector3 direction, float force, int damage, float cooldown)
        {
            direction.Normalize();
            if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
            impact += direction.normalized * force / 3.0f;

            damageOnCollisionValue = damage;
            damageOnCollisionBool = true;
            StartCoroutine(damageOnCollisionCooldown(cooldown));

        }

        IEnumerator damageOnCollisionCooldown(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);
            damageOnCollisionBool = false;
        }


        public void disableAllActiveAbilities()
        {
            etraAbilityManager.disableAllActiveAbilities();
            if (etraFPSUsableItemManager != null)
            {
                etraFPSUsableItemManager.disableFPSItemInputs();
            }
        }

        public void enableAllActiveAbilities()
        {
            etraAbilityManager.enableAllActiveAbilities();
            if (etraFPSUsableItemManager != null)
            {
                etraFPSUsableItemManager.enableFPSItemInputs();
            }
            
        }

    }

}

