using UnityEngine;

public class RB_PlayerAction : MonoBehaviour
{
    //Conditions
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsOnCooldown; //Cannot attack


    public float SpecialAttackCharge; //from 0 to 100
    private float _currentDashCooldown;

    //Components
    private RB_PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();

    }

    public void StartDash()
    {
        if (_playerMovement.CanDash())
        {
            _playerMovement.StartDash();
        }
    }

    public void Attack()
    {
        if (CanAttack())
        {
            IsAttacking = true;
            //Attack
        }
    }

    public void StartChargeAttack()
    {
        if(CanAttack())
        {
            IsChargingAttack = true;
        }
    }

    public void StopChargeAttack()
    {
        IsChargingAttack = false;
    }

    public void SpecialAttack()
    {
        if(CanAttack() && SpecialAttackCharge >= 100)
        {
            IsChargingAttack=true;
            //Special Attack
        }
    }

    public bool CanAttack()
    {
        //If there's no cooldown left and is not attacking
        return !(IsOnCooldown && (IsAttacking || IsChargingAttack));
    }

    public bool CanSpecialAttack()
    {
        //If there's no cooldown left and is not attacking
        return !(IsOnCooldown && (IsAttacking || IsChargingAttack));
    }

    public bool CanRewind()
    {
        return false;
    }
}
