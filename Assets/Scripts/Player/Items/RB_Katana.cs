using System.Collections;
using UnityEngine;

public class RB_Katana : RB_Items
{
    public override void Bind()
    {
        base.Bind();
        //Set the current weapon on the animators
        _playerAnimator.SetFloat("WeaponID", 0);
        _colliderAnimator.SetFloat("WeaponID", 0);
    }

    public IEnumerator WaitForEndOfFrameToChargeAttack()
    {
        yield return new WaitForEndOfFrame();
        base.ChargedAttack();
    }

    public override void ChargedAttack()
    {
        //Reset directions
        RB_PlayerMovement.Instance.ResetDirection();
        StartCoroutine(WaitForEndOfFrameToChargeAttack());
    }
}
