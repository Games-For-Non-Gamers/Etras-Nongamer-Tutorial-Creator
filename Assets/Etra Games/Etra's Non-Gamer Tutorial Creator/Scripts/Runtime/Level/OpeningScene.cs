using Etra.StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningScene : MonoBehaviour
{


    [Header("CutsceneCheckpoints")]
    public GameObject star;
    public GameObject playerSpawn;
    public GameObject camRoot;

    // Start is called before the first frame update
    private void Reset()
    {
        initialize();
    }

    public void initialize()
    {
        star = GameObject.Find("Star");
        playerSpawn = GameObject.Find("PlayerSpawn");
        camRoot = GameObject.Find("EtraPlayerCameraRoot");
    }

    // Update is called once per frame
    /*
    void Awake()
    {
        camRoot.transform.position = star.transform.position + new Vector3(0,50,-30);
        EtraCharacterMainController.Instance.disableAllActiveAbilities();
    }

    private void Start()
    {
        runOpeningScene();
    }

    public void runOpeningScene()
    {
        LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -30), 5).setEaseInOutSine();
    }
    */
}
