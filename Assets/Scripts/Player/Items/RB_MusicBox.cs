using MANAGERS;
using UnityEngine;

public class RB_MusicBox : RB_Items
{
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("musicbox", RB_PlayerController.Instance.transform.position);

    }

    public override void ChargedAttack() {
        base.ChargedAttack();
        RB_AudioManager.Instance.PlaySFX("music-box-loop", RB_PlayerController.Instance.transform.position);
    }

    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 2);
        _colliderAnimator.SetFloat("WeaponID", 2);
    }
}
