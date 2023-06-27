using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    [System.Serializable]
    public class EtraAnimationEvent
    {
        public GameObject tweenedObject;
        public RectTransform rectTransform;
        public enum AnimationEvents
        {
            MoveAndScale,
            Move,
            Scale,
            Wait,
            ToStartTransform,
            Flash,
            FadeIn,
            FadeOut,
            Show,
            Hide,
            ShowAll,
            HideAll,
            InstantCenterAndZeroScaleObject,
            PlaySfx,
            UnlockAbilityOrItem,
            UnlockPlayer,
            LockPlayer,
            Rotate,
            MoveToGameObject,
            RotateToGameObject,
            RunEtraAnimationActivatedScript,
            BasicUiGrowAndToStartWithUnlock
        }

        public AnimationEvents chosenEvent;

        //MoveAndScale
        public Vector3 scaleAndMovePosition;
        public Vector3 scaleAndMoveScale;
        public float scaleAndMoveTime;

        //Move
        public Vector3 movePosition;
        public float moveTime;

        //Scale
        public Vector3 scaleScale;
        public float scaleTime;

        //Wait
        public float secondsToWait;

        //ToStartPositionAndScale,
        public float toStartTime;

        //Flash
        public float flashTimes = 3;
        public float flashDelay = 0.5f;

        //FadeIn
        public float fadeInOpacity = 1;
        public float fadeInTime;

        //FadeOut
        public float fadeOutTime;

        //PlaySfx
        public string sfxName;

        //Rotate
        public Vector3 rotation;
        public float rotateTime;

        //MoveToGameObject
        public GameObject moveToObjectGameobject;
        public float moveToObjectTime;
        public Vector3 addedPosition;

        //RotateToGameObject
        public GameObject rotToObjectGameobject;
        public float rotToObjectTime;

        //EtraAnimationActivatedScript
        public EtraAnimationActivatedScript etraAnimationActivatedScript;
        public string passedString;

        //BasicUiGrowAndToStart
        public Vector3 basicGrowPos = Vector3.zero;
        public Vector3 basicGrowScale = new Vector3(2, 2, 2);
        public float basicGrowWait = 3f;



    }
}


