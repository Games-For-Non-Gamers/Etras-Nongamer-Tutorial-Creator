using Etra.StarterAssets.Interactables.Enemies;
using EtrasStarterAssets;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class Star : MonoBehaviour, IDamageable<int>
    {

        private float speed = 50;

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

        }


        public void speedSpin(int inital, int final)
        {

            speed = inital;
            LeanTween.value(this.gameObject, inital, final, 3f).setOnUpdate((float speedValue) => { speed = speedValue; });

        }
    }
}
