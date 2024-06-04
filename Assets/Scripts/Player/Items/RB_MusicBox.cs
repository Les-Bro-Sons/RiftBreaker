using MANAGERS;
using UnityEngine;

public class RB_MusicBox : RB_Items
{
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("musicbox", RB_PlayerController.Instance.transform.position, 0.15f, 1);

    }

    public override void ChargedAttack() {
        base.ChargedAttack();
        RB_AudioManager.Instance.PlaySFX("musicbox_Loop", RB_PlayerController.Instance.transform.position, 0, 1);
    }

    public override void StartChargingAttack() {
        base.StartChargingAttack();
        RB_AudioManager.Instance.PlaySFX("musicBoxManivelle", RB_PlayerController.Instance.transform.position, .15f, .05f);
    }
    
    public override void StopChargingAttack() {
        base.StopChargingAttack();
        // RB_AudioManager.Instance.StopSFX();
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        RB_AudioManager.Instance.PlaySFX("musicBoxSpé", RB_PlayerController.Instance.transform.position, 0, 1);
    }

    protected override void Start() {
        base.Start();
        EventOnEndOfAttack.AddListener(EndOfAttack);
    }

    private void EndOfAttack() {
        RB_AudioManager.Instance.StopSFX();
    }
    
    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 2);
        _colliderAnimator.SetFloat("WeaponID", 2);
    }
}
