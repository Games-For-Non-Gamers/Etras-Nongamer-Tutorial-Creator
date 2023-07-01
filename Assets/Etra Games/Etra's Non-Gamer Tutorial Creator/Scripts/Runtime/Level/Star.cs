using Etra.StarterAssets.Interactables.Enemies;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class Star : MonoBehaviour, IDamageable<int>
    {
        public GameObject impactParticle;
        public Fireworks firework1;
        public Fireworks firework2;
        public Fireworks firework3;
        public ParticleSystem light1;
        public ParticleSystem light2;

        private float speed = 50;

        private void Start()
        {
            light1.Stop();
            light2.Stop();
        }

        void Update()
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }

        public void TakeDamage(int damage)
        {
            this.GetComponent<AudioManager>().Stop("HitStar");
            this.GetComponent<AudioManager>().Play("HitStar");
            speed = 500;
            speedSpin(500, 50);
            StartCoroutine(fireworks());
            Instantiate(impactParticle, transform.position, Quaternion.identity);
        }

        public void endCelebration()
        {
            light1.Play();
            light2.Play();
            //Maybe an sfx
            StartCoroutine(fireworks());
        }

        IEnumerator fireworks()
        {
            if (firework1 != null)
            {
                firework1.runFireworks();
            }
            yield return new WaitForSeconds(Random.Range(0.2f, 3f));
            if (firework2 != null)
            {
                firework2.runFireworks();
            }
            yield return new WaitForSeconds(Random.Range(0.2f, 3f));
            if (firework3 != null)
            {
                firework3.runFireworks();
            }
        }


        public void speedSpin(int inital, int final)
        {

            speed = inital;
            LeanTween.value(this.gameObject, inital, final, 3f).setOnUpdate((float speedValue) => { speed = speedValue; });

        }
    }
}
