using Etra.StarterAssets.Interactables;
using Etra.StarterAssets.Source.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Etra.NonGamerTutorialCreator
{
    public class MagicPhysicsBox : MonoBehaviour
    {
        Vector3 startLocation;
        public bool hasSetRange = false;
        public Vector2 xRange = new Vector2(30, -30);
        public Vector2 zRange = new Vector2(30, -30);
        void Start()
        {
            startLocation = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CheckpointTeleporter>())
            {
                respawn();
            }
        }

        void respawn()
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = startLocation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasSetRange)
            {
                //When this object lands check if it is within range, if not, teleport to start
                if (transform.position.x > startLocation.x + xRange.x)
                {
                    respawn();
                }

                if (transform.position.x < startLocation.x + xRange.y)
                {
                    respawn();
                }

                if (transform.position.z > startLocation.z + zRange.x)
                {
                    respawn();
                }

                if (transform.position.z < startLocation.z + zRange.y)
                {
                    respawn();
                }

            }
        }
    }
}
