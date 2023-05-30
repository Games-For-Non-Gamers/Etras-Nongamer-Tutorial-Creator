using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokiEvents : MonoBehaviour
{
    //Currently just for format reference
    public bool isDebugOn = false;


    public GameplayUIAnimator ui;
    public int eventNumber = 0;
    public bool bridge1Down=false;

    public int debugEventNum=1;
    private void Start()
    {
        //player.lockAllPlayerActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    private void Update()
    {
        if (isDebugOn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                runEvent(debugEventNum);
                debugEventNum++;
            }
        }
       
    }

    public void runEvent(int eventNum)
    {
      eventNumber = eventNum;
      StartCoroutine("event" + eventNum);
    }


    /*
     *dUnlocked = false;
        playerShoot.shootUnlocked = false;
        aUnlocked = false;
        sUnlocked = false;
        wUnlocked = false;
        mouseSensitivityY = 0;
        jumpUnlocked = false;
        dashUnlocked = false;
        mouseSensitivityX = 0; 
     *
    */

    /// 1. Right
    /// 2. Shoot
    /// 3. Left
    /// 4. Back
    /// 5. Forward
    /// 6. Crouch
    /// 7. Mouse up/down
    /// 8. Jump
    /// 9. Dash
    /// 10. Mouse Left Right
    /// //special mouse move anim later
    /// 11. Interact
    /// 12. Congrats

    /*
    private IEnumerator event1()
    {
        //Opening cutscene time
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent1();
        ui.calledEvent(1);
    }

    private IEnumerator event2()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent2();
        ui.calledEvent(2);
    }

    private IEnumerator event3()
    {
        yield return new WaitForSeconds(4);
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent3();
        ui.calledEvent(3);
    }

    private IEnumerator event4()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent4();
        ui.calledEvent(4);
    }

    private IEnumerator event5()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent5();
        ui.calledEvent(5);
    }

    private IEnumerator event6()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent6();
        ui.calledEvent(7);
    }

    private IEnumerator event7()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent7();
        ui.calledEvent(8);
    }

    private IEnumerator event8()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent8();
        ui.calledEvent(9);
    }

    private IEnumerator event9()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent9();
        ui.calledEvent(10);
    }

    private IEnumerator event10()
    {
        yield return new WaitForSeconds(0);
        playerEvents.tokiEvent9();
        ui.calledEvent(12);
    }
    */

}
