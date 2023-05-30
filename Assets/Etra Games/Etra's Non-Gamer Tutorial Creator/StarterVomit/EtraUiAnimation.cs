using UnityEngine;

    [System.Serializable]
    public class EtraUiAnimation 
    {
        public RectTransform tweenedObject;
        public enum AnimationEvents 
        { 
            MoveAndScale, 
            Move, 
            Scale, 
            Flash,
            Wait, 
            Fade,
            UnlockAbility
        }

        public AnimationEvents chosenTweenEvent;

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

        //Flash
        public Vector3 flashTimes;
        public float flashDelay;

        //Wait
        public float secondsToWait;

        //Fade
        public float opacity;
        public float fadeTime;

}




