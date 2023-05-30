using UnityEngine;

    [System.Serializable]
    public class EtraUiAnimation 
    {



    public float Speed = 1;
    public Animation Animation;
    //public int FPS;
    public AnimationFPS FPS;

    public RectTransform objectToMove;
        public float time;
        public Vector3 Position;
        public Vector3 NormalRotation;




    /*
    public enum Status { A, B, C };

    public Status state;

    public int valForAB;

    public int valForA;
    public int valForC;

    public bool controllable;
    */  
    }
public enum AnimationFPS { WEB = 15, Film = 24, PAL = 25, NTSC = 30, HDTV = 60, UHDTV = 120 }




