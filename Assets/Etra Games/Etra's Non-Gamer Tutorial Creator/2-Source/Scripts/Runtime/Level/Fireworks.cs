using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class Fireworks : MonoBehaviour
    {
        // Start is called before the first frame update
        ParticleSystem particle;
        AudioManager audioManager;

        bool inQueue;
        bool playingFireworks;
        public void runFireworks()
        {

            if (playingFireworks)
            {
                inQueue = true;
            }
            else
            {
                StartCoroutine(fireWorkInit());
            }
        }

        private void Start()
        {
            this.gameObject.SetActive(true);
            particle = GetComponent<ParticleSystem>();
            audioManager = GetComponent<AudioManager>();
            particle.Stop();
        }


        IEnumerator fireWorkInit()
        {
            playingFireworks = true;
            particle.Play();
            audioManager.Play("FireworkHiss");
            yield return new WaitForSeconds(4);
            fireWorkPopAudio();
            StartCoroutine(LoopingFireworks());
        }

        IEnumerator LoopingFireworks()
        {

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(3);
                fireWorkPopAudio();
            }


            if (!inQueue)
            {
                particle.Stop();
                yield return new WaitForSeconds(3);
                fireWorkPopAudio();
                audioManager.Stop("FireworkHiss");
                playingFireworks = false;
            }
            else
            {
                inQueue = false;
                StartCoroutine(LoopingFireworks());
            }
        }

        void fireWorkPopAudio()
        {
            audioManager.Stop("FireworkHiss");
            audioManager.Play("FireworkPop");
            audioManager.Play("FireworkHiss");
        }
    }
}