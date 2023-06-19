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
            RotateToGameObject
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
        public float flashTimes;
        public float flashDelay;

        //FadeIn
        public float fadeInOpacity = 255;
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

        //RotateToGameObject
        public GameObject rotToObjectGameobject;
        public float rotToObjectTime;
    }
}


