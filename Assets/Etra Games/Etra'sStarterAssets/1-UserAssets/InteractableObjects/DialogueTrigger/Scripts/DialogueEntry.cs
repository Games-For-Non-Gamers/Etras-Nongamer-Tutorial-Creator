using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    [System.Serializable]
    public class DialogueEntry 
    {
        public enum DialogueEvents
        {
            PlayAudioFromManager,
            UpdateLine,
            PlayLineWithAudio,
            Wait,
            MoveObject,
            RotateObject,
            MovePlayer,
            RotatePlayerCam,
            LockPlayer,
            UnlockPlayer

        }

        public DialogueEvents chosenEvent = DialogueEvents.UpdateLine;

        //Play Audio
        public string sfxName = "DemoDialogue1";

        //Update Line
        public string speaker = "Etra:";
        public string dialogueLine = "This is a demo test line";
        public float timeTillNextEvent = 1.5f; //<---Also for wait and moves

        //PlayLine with Audio
        public AudioClip clip;


        //MoveOrRotateObject
        public GameObject savedObject;
        public Vector3 targetVector3;

        //Lock and unlock

    }
}
