using Etra.StarterAssets.Source.Combat;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class NonGamerOceanSplashLayer : MonoBehaviour
    {

        public GameObject splashParticle;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                standardSplash(other, new Vector2(1.0f, 1.2f));
            }

            if (other.gameObject.GetComponent<BulletProjectile>())
            {

                if (other.gameObject.GetComponent<BulletProjectile>().hitObject)
                {
                    if (other.gameObject.GetComponent<BulletProjectile>().hitObject.GetComponent<NonGamerOceanSplashLayer>())
                    {
                        //Speedy raycast splash
                        Instantiate(splashParticle, other.gameObject.GetComponent<BulletProjectile>().hitPoint, Quaternion.identity);
                    }
                    else
                    {
                        standardSplash(other);
                    }

                }
                else
                {
                    standardSplash(other);
                }

            }
            else
            {
                standardSplash(other);
            }
        }


        void standardSplash(Collider other)
        {
            standardSplash(other, new Vector2(0.7f, 1));
        }

        void standardSplash(Collider other, Vector2 scaleRange)
        {

            if (cooldown)
            {
                return;
            }
            cooldown = true;

            Vector3 contactPoint = other.ClosestPoint(transform.position);
            GameObject sP = Instantiate(splashParticle, contactPoint, Quaternion.identity);
            AddObjectAtRandomScale scaler = sP.AddComponent<AddObjectAtRandomScale>();
            scaler.scaleObjectWithRange(scaleRange);
            StartCoroutine(splashCooldown());
        }

        bool cooldown = false;
        IEnumerator splashCooldown()
        {
            yield return new WaitForSeconds(0.01f);
            cooldown = false;
        }

    }
}