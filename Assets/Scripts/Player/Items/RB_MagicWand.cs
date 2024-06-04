using MANAGERS;
using UnityEngine;

public class RB_MagicWand : RB_Items
{
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,0);

    }

    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 1);
        _colliderAnimator.SetFloat("WeaponID", 1);
    }

    public override void ChargedAttack()
    {
        base.ChargedAttack();
    }
}
