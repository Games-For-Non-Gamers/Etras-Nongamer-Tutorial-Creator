using Etra.StarterAssets;
using Etra.StarterAssets.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineblockCheckerSet : MonoBehaviour
{
    public MineblockChecker[] mineBlockCheckers; 

    //SOMETHING tirggers the check, maybe the indidiual checkers
    void checkTriggers()
    {
        bool allActivated = true;
        foreach (MineblockChecker checker in mineBlockCheckers)
        {
            if (checker.blockToCheckFor == null)
            {
                allActivated = false;
            }
        }

        if (!allActivated)
        {
            return;
        }

        foreach (MineblockChecker checker in mineBlockCheckers)
        {
            Destroy(checker.blockToCheckFor); // wil run animation
            //Play terget pop sfx????
            //Add in new object of orbs look at flare head as base at each cube location.

            //wait one second then they all move to rope bottom I need ref to over ??? seconds.
        }


        this.GetComponent<Target>().targetHit();
    }
    

}
