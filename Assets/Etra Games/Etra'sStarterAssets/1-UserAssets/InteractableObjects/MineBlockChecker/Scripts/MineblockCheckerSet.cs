using Etra.StarterAssets.Interactables;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

namespace Etra.StarterAssets
{
    public class MineblockCheckerSet : MonoBehaviour
    {
        [Header("Basics:")]
        public MineblockChecker[] mineBlockCheckers;
        float delayBeforeSparkMove = 1;
        float sparkMoveTime = 1;
        [Header("References:")]
        public GameObject sparkParticle;
        bool activateAnimationStarted = false;
        private void Start()
        {
            foreach (MineblockChecker checker in mineBlockCheckers)
            {
                checker.setToActivate = this;
            }
        }

        public void CheckTriggers()
        {
            bool allActivated = true;
            foreach (MineblockChecker checker in mineBlockCheckers)
            {
                if (checker.connectedBlock == null)
                {
                    allActivated = false;
                }
            }

            if (!allActivated || activateAnimationStarted)
            {
                return;
            }

            activateAnimationStarted = true;
            StartCoroutine(ActivateAnimation());
        }

        IEnumerator ActivateAnimation()
        {
            yield return new WaitForSeconds(2); //Wait for initial particles to destory

            GameObject[] particles = new GameObject[mineBlockCheckers.Length];

            for (int i = 0; i < mineBlockCheckers.Length; i++)
            {
                //spawn in particle
                particles[i] = Instantiate(sparkParticle, mineBlockCheckers[i].gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                Destroy(mineBlockCheckers[i].connectedBlock.gameObject); // wil run animation
                Destroy(mineBlockCheckers[i].gameObject);
            }
            MineBlockSystem.Instance.gameObject.GetComponent<AudioManager>().Play("Pop");
            yield return new WaitForSeconds(delayBeforeSparkMove);

            Target target = this.GetComponent<Target>();
            for (int i = 0; i < particles.Length; i++)
            {
                LeanTween.move(particles[i], target.sparker.transform, sparkMoveTime);
            }
            yield return new WaitForSeconds(sparkMoveTime*0.9f);

            for (int i = 0; i < particles.Length; i++)
            {
                LeanTween.scale(particles[i], Vector3.zero, sparkMoveTime * 0.1f);
            }
            yield return new WaitForSeconds(sparkMoveTime * 0.1f);

            foreach (GameObject particle in particles)
            {
                Destroy(particle);
            }

            target.ropeActivate();
        }

    }
}