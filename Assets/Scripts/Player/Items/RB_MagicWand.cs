using MANAGERS;
using UnityEngine;

public class RB_MagicWand : RB_Items
{
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,0, 1);

    }

    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 1);
        _colliderAnimator.SetFloat("WeaponID", 1);
    }

    public override void StartChargingAttack() {
        base.StartChargingAttack();
        RB_AudioManager.Instance.PlaySFX("magicshield_down", RB_PlayerController.Instance.transform.position,0, 1);
    }

    public override void ChargedAttack() {
        base.ChargedAttack();
        RB_AudioManager.Instance.PlaySFX("boom-magic", RB_PlayerController.Instance.transform.position,0, 1);
    }

    public override void DealDamage() {
        base.DealDamage();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,0, 1);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position, .1f, 1);
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("Sheating_magic_wand", RB_PlayerController.Instance.transform.position, 0,.5f);
    }
}
