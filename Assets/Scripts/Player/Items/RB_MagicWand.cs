using MANAGERS;

public class RB_MagicWand : RB_Items
{
    public override void Attack() {
        base.Attack();
        ShootProjectile("BasicPowerBall").Damage = AttackDamage;
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,false, 0, 1);

    }

    public override void Bind()
    {
        base.Bind();
        if (RobertShouldTalk)
        {
            RB_PlayerAction.Instance.PickupGathered.StartDialogue(1);
            RobertShouldTalk = false;
        }
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 1);
        _colliderAnimator.SetFloat("WeaponID", 1);
    }

    public override void StartChargingAttack() {
        base.StartChargingAttack();
        RB_AudioManager.Instance.PlaySFX("Charge_Charged_Attack_Magic_Wand", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public override void ChargedAttack() {
        base.ChargedAttack();
        ShootProjectile("ChargePowerBall").Damage = ChargedAttackDamage;
        RB_AudioManager.Instance.PlaySFX("boom-magic", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public override void DealDamage() {
        base.DealDamage();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        ShootProjectile("SpecialPowerBall").Damage = SpecialAttackDamage;
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position, false, .1f, 1);
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("Sheating_magic_wand", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }
}
