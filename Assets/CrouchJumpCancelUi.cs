using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using System.Collections;

using UnityEngine;

public class CrouchJumpCancelUi : MonoBehaviour
{
    public ABILITY_Jump jumpAbility;
    public EtraAbilityOrItemUi crouchCancelUi;
    bool uiAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        crouchCancelUi = this.GetComponent<EtraAbilityOrItemUi>();
        jumpAbility.FailedCrouchJump.AddListener(showUi);
    }

    void showUi()
    {
        if (uiAnimating)
        {
            return;
        }
        uiAnimating = true;
        StartCoroutine(animateCancel());
    }

    IEnumerator animateCancel()
    {
        float waitTime = 0.5f;
        crouchCancelUi.fadeInUi(waitTime);
        yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(waitTime*6);
        crouchCancelUi.fadeOutUi(waitTime);
        yield return new WaitForSeconds(waitTime);
        uiAnimating = false;
    }
}
