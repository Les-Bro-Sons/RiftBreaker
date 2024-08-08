using MANAGERS;
using System.Collections;
using UnityEngine;

public class RB_ShurikenPatrick : RB_Items
{

    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("LittleSwoosh", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }
    
    public override void Bind()
    {
        base.Bind();
        if (RobertShouldTalk && !RB_LevelManager.SavedData.HasReachedWeapon)
        {
            RobertPickupDialogue(7);
        }
        //Set the current weapon on the animators
        _playerAnimator.SetFloat("WeaponID", 7);
        _colliderAnimator.SetFloat("WeaponID", 7);
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
        RB_AudioManager.Instance.PlaySFX("BigSwoosh", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public IEnumerator WaitForEndOfFrameToPlaySFX()
    {
        yield return new WaitForSeconds(0.6f);
        RB_AudioManager.Instance.PlaySFX("Jump_Attack_Viking_Horn", RB_PlayerController.Instance.transform.position, false, 0, 1);
    }
    public override void SpecialAttack() {
        base.SpecialAttack();
        
        RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position,false, 0, 1);
        StartCoroutine(WaitForEndOfFrameToPlaySFX());
        
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_Katana", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }
}
