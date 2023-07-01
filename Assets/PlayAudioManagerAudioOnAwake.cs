using EtrasStarterAssets;
using UnityEngine;

public class PlayAudioManagerAudioOnAwake : MonoBehaviour
{
    // Play first sound
    void Start()
    {
        GetComponent<AudioManager>().Play(GetComponent<AudioManager>().sounds[0]);    
    }

}
