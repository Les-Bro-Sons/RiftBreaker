using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class RB_PlayerAction : MonoBehaviour
{
    //Conditions
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool IsChargedAttacking;
    [HideInInspector] public bool IsSpecialAttacking;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsOnCooldown; //Cannot attack


    [Range(0, 100)] public float SpecialAttackCharge; //from 0 to 100
    private float _currentDashCooldown;
    private float _chargeAttackPressTime;

    //Components
    private RB_PlayerMovement _playerMovement;
    private RB_PlayerController _playerController;
    [SerializeField] private GameObject _ChargedAttackReadyMark;
    private Transform _transform;
    RB_Items _item;

    //Charge attack
    private Coroutine _currentChargedAttack;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerController = GetComponent<RB_PlayerController>();
        _item = GetComponentInChildren<RB_Items>();
        _transform = transform;
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
            if(_currentChargedAttack != null)
                StopCoroutine(_currentChargedAttack);
            _currentChargedAttack = StartCoroutine(ChargeAttack());
        }
    }

    public bool IsDoingAnyAttack()
    {
        //If the player is attacking in any way possible (normal attack, charging attack, charged attack or special attack)
        return IsChargingAttack || IsAttacking || IsChargedAttacking || IsSpecialAttacking;
    }

    public void StopChargeAttack()
    {
        //Stop charging attack
        IsChargingAttack = false;
        if(_currentChargedAttack != null)
            StopCoroutine(_currentChargedAttack);
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
            GameObject instantiatedChargedAttackReadyMark = Instantiate(_ChargedAttackReadyMark, _transform);
            instantiatedChargedAttackReadyMark.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            //When the charge of the attack is ready
            print("attaque chargée prête");
        }
    }

    public void SpecialAttack()
    {
        if(CanAttack() && SpecialAttackCharge >= 100)
        {
            //Special Attack
            IsSpecialAttacking = true;
            SpecialAttackCharge = 0;
            _item.SpecialAttack();
        }
    }

    public void StopSpecialAttack()
    {
        IsSpecialAttacking = false;
    }

    public bool CanAttack()
    {
        //If there's no cooldown left and is not attacking
        return !IsAttacking && !IsChargingAttack && !IsChargedAttacking;
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
