using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class RB_PlayerAction : MonoBehaviour
{
    //Conditions
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool IsChargedAttacking;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsOnCooldown; //Cannot attack


    public float SpecialAttackCharge; //from 0 to 100
    private float _currentDashCooldown;
    private float _chargeAttackPressTime;

    //Components
    private RB_PlayerMovement _playerMovement;
    private RB_PlayerController _playerController;
    RB_Items _item;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerController = GetComponent<RB_PlayerController>();
        _item = GetComponentInChildren<RB_Items>();

    }

    public void StartDash()
    {
        if (_playerMovement.CanDash())
        {
            //Start Dash
            _playerMovement.StartDash();
        }
    }

    public void Attack()
    {
        if (CanAttack())
        {
            //Attack
            IsAttacking = true;
            _item.Attack();
            print("charge attack annulé et attaque commencé");
        }
    }

    public void ChargedAttack()
    {
        //Charge attack
        _item.ChargedAttack();
        IsChargedAttacking = true;
    }

    public void StopChargedAttack()
    {
        //Stop charged attack
        IsChargedAttacking = false;
    }

    public void StopAttack()
    {
        //Stop normal attack
        IsAttacking = false;
    }

    public void StartChargeAttack()
    {
        if(CanAttack())
        {
            //Start charging attack
            IsChargingAttack = true;
            _chargeAttackPressTime = 0;
            StartCoroutine(ChargeAttack());
        }
    }

    public bool IsDoingAnyAttack()
    {
        //If the player is attacking in any way possible (normal attack, charging attack, charged attack or special attack)
        return IsChargingAttack || IsAttacking || IsChargedAttacking;
    }

    public void StopChargeAttack()
    {
        //Stop charging attack
        IsChargingAttack = false;
        StopCoroutine(ChargeAttack());
        if(_chargeAttackPressTime < _item.ChargeTime)
        {
            //If the player didn't press long enough, normal attack
            Attack();
        }
        else
        {
            //Otherwise do the charged attack
            ChargedAttack();
        }
    }

    private IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(_item.ChargeTime);
        if (IsChargingAttack)
        {
            //When the charge of the attack is ready
            print("attaque chargée prête");
        }
    }

    public void SpecialAttack()
    {
        if(CanAttack() && SpecialAttackCharge >= 100)
        {
            //Special Attack
        }
    }

    public bool CanAttack()
    {
        //If there's no cooldown left and is not attacking
        return !IsAttacking && !IsChargingAttack;
    }

    public bool CanSpecialAttack()
    {
        //If there's no cooldown left and is not attacking
        return !(IsOnCooldown && (IsAttacking || IsChargingAttack));
    }

    public bool CanRewind()
    {
        //If can rewind
        return false;
    }

    private void Update()
    {
        //count the time the player press the attack button
        TimerChargeAttack();
    }

    private void TimerChargeAttack()
    {
        if (IsChargingAttack)
        {
            //count the time the player press the attack button
            _chargeAttackPressTime += Time.deltaTime;
        }
    }
}
