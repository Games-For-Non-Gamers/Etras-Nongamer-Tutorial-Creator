using EtrasStarterAssets;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class PlayAudioManagerAudioOnAwake : MonoBehaviour
    {
        // Play first sound
        void Start()
        {
            GetComponent<AudioManager>().Play(GetComponent<AudioManager>().sounds[0]);
        }

    }
}