using MANAGERS;

public class RB_MagicWand : RB_Items
{
    public override void Attack() {
        base.Attack();
        var projectile = ShootProjectile("BasicPowerBall");
        projectile.Damage = AttackDamage;
        projectile.KnocbackExplosionForce = _normalKnockbackForce;
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,false, 0, 1);

    }

    public override void Bind()
    {
        base.Bind();
        if (RobertShouldTalk && !RB_LevelManager.SavedData.HasReachedWeapon)
        {
            RobertPickupDialogue(1);
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
        var projectile = ShootProjectile("ChargePowerBall");
        projectile.Damage = ChargedAttackDamage;
        projectile.KnocbackExplosionForce = _chargeAttackKnockbackForce;
        RB_AudioManager.Instance.PlaySFX("boom-magic", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public override void DealDamage() {
        base.DealDamage();
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        var projectile = ShootProjectile("SpecialPowerBall");
        projectile.Damage = SpecialAttackDamage;
        projectile.KnocbackExplosionForce = _specialAttackKnockbackForce;
        RB_AudioManager.Instance.PlaySFX("fireball", RB_PlayerController.Instance.transform.position, false, .1f, 1);
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("Sheating_magic_wand", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }
}
