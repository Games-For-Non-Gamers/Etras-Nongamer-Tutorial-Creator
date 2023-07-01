using Etra.StarterAssets.Input;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class NonGamerTutorialCredits : MonoBehaviour
{
    public GameObject credits;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LerpPosition(howLongToScroll));
        audioManager = GetComponent<AudioManager>();
        audioManager.Play("Music");
        Cursor.lockState =  CursorLockMode.None;
    }


    public Vector3 pointA = new Vector3(0, -400, 0);
    public Vector3 pointB = new Vector3(0, 2800, 0);
    public float howLongToScroll = 60;
    public float fastSpeedMultiplier = 3f;

    private float currentSpeed;

    private void Update()
    {
        // Check if spacebar is pressed
        if (Keyboard.current != null && (Keyboard.current.anyKey.isPressed  || Mouse.current.leftButton.isPressed)
            || Gamepad.current != null && IsAnyGamepadButtonPressed()

            )
        {
            fastSpeedMultiplier = 3;
        }
        else
        {
            fastSpeedMultiplier = 1;
        }


        // Move the object towards point B
        credits.transform.localPosition = Vector3.Lerp(credits.transform.localPosition, pointB, currentSpeed * Time.deltaTime);

        if (credits.transform.localPosition.y > pointB.y)
        {
            credits.transform.localPosition = pointA;
        }

    }



    IEnumerator LerpPosition( float duration)
    {
        float time = 0;
        Vector3 startPosition = credits.transform.localPosition;
        while (time < duration)
        {
            credits.transform.localPosition = Vector3.Lerp(startPosition, pointB, time / duration);
            time += Time.deltaTime * fastSpeedMultiplier;
            yield return null;
        }
        credits.transform.localPosition = pointB;
        credits.transform.localPosition = pointA;
        StartCoroutine(LerpPosition(howLongToScroll));

    }


public void backToMain()
    {
        SceneManager.LoadScene("NonGamerTutorialBaseRoomWithPrefabs");
    }

    private bool IsAnyGamepadButtonPressed()
    {
        foreach (var button in Gamepad.current.allControls)
        {
            if (button is ButtonControl buttonControl && buttonControl.isPressed)
            {
                return true;
            }
        }
        return false;
    }

}
