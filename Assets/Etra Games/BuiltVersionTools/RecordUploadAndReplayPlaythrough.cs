using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordUploadAndReplayPlaythrough : MonoBehaviour
{

    //Maybe some replay tools or gui?
    //Make a public variable for input trace here
    [ContextMenu("Play Back Input Trace")]
    void PlayBackInputTrace()
    {

    }

    //Record should start after any cutscenes/opening stuff
    //Replay should start from a button press


    //Disable the unused control scheme, that is what will be used to fly the camera with the old input system.
    //Camera movement and inpout disable should be a seperate script. Also should timescale and cool stuff.

    //
    /*

        public InputActionAsset inputActions; // Reference to your Input Actions asset.
    public string controlSchemeName = "YourControlSchemeName"; // Replace with your control scheme's name.

    private InputActionMap controlScheme;

    private void Start()
    {
        // Get the control scheme from the Input Actions asset by name.
        controlScheme = inputActions.FindActionMap(controlSchemeName);

        if (controlScheme != null)
        {
            // Disable the control scheme.
            controlScheme.Enable();
            controlScheme.Disable();
        }
        else
        {
            Debug.LogWarning("Control scheme not found: " + controlSchemeName);
        }
    }


     */



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.quitting += OnGameQuit;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= OnGameQuit;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Your code to execute on scene change
        Debug.Log("Scene changed to: " + scene.name); //<---NOT WORKING, runs at start which is not wanted.
        UploadInputTrace();
    }
    private void OnGameQuit()
    {
        UploadInputTrace(); //<--- WORKING
    }

    private void UploadInputTrace()
    {

        //FIX SCENE CHANGE TRIGGER
        //Upload player id and trace file to filestack
        Debug.Log("eeeeee");
    }
}
