using Etra.StarterAssets.Abilities;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Etra.StarterAssets.Interactables
{
  public class ChasePlayer : MonoBehaviour
  {

    //From Krissy#1337
    /*
    The MIT License (MIT)
    Copyright 2023 Krissy
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */


    Transform playerCamRoot;
    public GameObject currentTarget;
    public GameObject[] followTargets;
    public int targetNum = 0;
    NavMeshAgent agent;

    public float walkSpeed = 1f;
    public float chaseSpeed = 2.5f;
    public bool spotted;
    public bool searchStarted;
    public float searchTime;
    public float viewDistance;
    public float viewAngle;
    public Material NoneFound;
    public Material PlayerFound;
    public bool jumpscare = true;
    public static bool enemyInJumpscare = false;
    public bool respawnAfterJumpscare = true;
    private Animator animator;
    private Transform startTransform;
    GameObject enemyEye;
    EtraCharacterMainController etraCharacterMainController;
    Vector3 startPos;
    AudioManager audioManager;

    //Unity Events
    [HideInInspector] public UnityEvent playerSpotted; // Play music
    [HideInInspector] public UnityEvent playerCaught;


        private void Start()
        {

            startPos = this.transform.localPosition;
            enemyEye = transform.Find("TiltRoot").Find("Base").Find("BaseTop").Find("Center").Find("Body").Find("NeckRoatator").Find("Neck").Find("Head").Find("Eye").gameObject;
            etraCharacterMainController = EtraCharacterMainController.Instance;
            agent = this.GetComponent<NavMeshAgent>();
            animator = this.GetComponent<Animator>();
            playerCamRoot = GameObject.Find("EtraPlayerCameraRoot").transform;
            agent.enabled = true;
            audioManager = GetComponent<AudioManager>();
            startTransform = this.transform;
            nextLocation();
        }

        void Update()
        {
            if (enemyInJumpscare)
            {
                return;
            }

            agent.destination = currentTarget.transform.position;

            if (this.transform.hasChanged)
            {
                animator.SetBool("Moving", true);
                if (!audioManager.IsPlaying("Move"))
                {
                    audioManager.Play("Move");
                }

            }
            else
            {
                animator.SetBool("Moving", false);
                if (audioManager.IsPlaying("Move"))
                {
                    audioManager.Stop("Move");
                }
            }

              findThePlayer();
              if (searchStarted)
              {
                // Set eye material to player found material, and set flashlight to material color
                enemyEye.GetComponent<MeshRenderer>().material = PlayerFound;
                enemyEye.transform.Find("Spot Light").GetComponent<Light>().color = Color.red;
                    if (!audioManager.IsPlaying("Alarm") && Time.timeScale != 0)
                    {
                            currentTarget = playerCamRoot.gameObject;
                            agent.speed = chaseSpeed;
                            audioManager.Play("Spotted");
                            audioManager.Play("Alarm");
                    }
              }
              else
              {
                    // Set eye material to none found material, and set flashlight to material color
                    enemyEye.GetComponent<MeshRenderer>().material = NoneFound;
                    enemyEye.transform.Find("Spot Light").GetComponent<Light>().color = Color.white;

                    if (audioManager.IsPlaying("Alarm"))
                    {
                        nextLocation();
                        agent.speed = walkSpeed;
                        audioManager.Stop("Alarm");
                    }
              }
              

            if (Vector3.Distance(transform.position, playerCamRoot.position) < 1.65f && !enemyInJumpscare)
            {
                if (jumpscare)
                {
                        enemyInJumpscare = true;
                        StopAllCoroutines();
                        StartCoroutine(jumpScare());
                }
                else
                {
                            respawnPlayer();
                }

            }

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1 && !enemyInJumpscare)
            {
                nextLocation();
            }

        }



        void nextLocation()
        {
            currentTarget = followTargets[targetNum++ % followTargets.Length];
        }


        IEnumerator jumpScare()
    {
            etraCharacterMainController.disableAllActiveAbilities();
            animator.SetBool("Moving", false);
            animator.SetBool("Jumpscare", true);
            audioManager.Stop("Move");
            Transform savedParent = this.transform.parent;
            agent.enabled = false;
            audioManager.Stop("Spotted");
            audioManager.Play("Jumpscare");
            this.transform.parent = playerCamRoot;
            this.transform.localPosition = new Vector3( 0, -1.24f, 1.25f);
            LeanTween.rotateLocal(this.gameObject, new Vector3(7, 180, 0 ),0 );
            yield return new WaitForSeconds(0.95f);
            etraCharacterMainController.enableAllActiveAbilities();
            yield return new WaitForSeconds(0.05f);
            audioManager.Stop("Alarm");
            this.transform.parent = savedParent;
            this.transform.localPosition = startPos;
            LeanTween.rotateLocal(this.gameObject, new Vector3(0, 180, 0), 0);
            agent.enabled = true;
            respawnPlayer();
            yield return new WaitForSeconds(2f);
            animator.SetBool("Jumpscare", false);
            animator.SetBool("Moving", true);
            enemyInJumpscare = false;
            if (respawnAfterJumpscare)
            {
                respawn();
            }
            else{
                nextLocation();
            }
            playerCaught.Invoke(); 
            yield return new WaitForSeconds(3f);
            etraCharacterMainController.enableAllActiveAbilities();
        }

        public void respawn()
        {
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;
            nextLocation();
        }
        void respawnPlayer()
        {
        // "Kill" player, respawn at last checkpoint.
        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>())
        {
            EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>().teleportToCheckpoint();
        }
        else
        {
            Debug.LogWarning("To Use the Checkpoint Teleporter ABILITY_CheckpointRespawn must be added to your character's ability mannager. ");
        }
            searchStarted = false;
        }

    IEnumerator search()
    {
      yield return new WaitForSeconds(searchTime);
      searchStarted = false;
    }

    void findThePlayer()
    {
      if (Vector3.Distance(transform.position, playerCamRoot.position) < viewDistance)
      {
        var directionToPlayer = (playerCamRoot.position - transform.position).normalized;
        float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.green);
        if (angleBetweenGuardAndPlayer < viewAngle / 2)
        {
          RaycastHit objectHit;
          if (Physics.Raycast(transform.position, directionToPlayer, out objectHit, viewDistance))
          {
            if (objectHit.transform.CompareTag("Player"))
            {
              searchStarted = true;
              StartCoroutine(search());
            }
          }
        }
      }
    }

    public bool ifPlayerFound()
    {
      if (Vector3.Distance(transform.position, playerCamRoot.position) < viewDistance)
      {
        var directionToPlayer = (playerCamRoot.position - transform.position).normalized;
        float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.green);
        if (angleBetweenGuardAndPlayer < viewAngle / 2)
        {
          RaycastHit objectHit;
          if (Physics.Raycast(transform.position, directionToPlayer, out objectHit, viewDistance))
          {
            if (objectHit.transform.CompareTag("Player"))
            {
              return true;
            }
          }
        }
      }
      return false;
    }
  }
}
