using UnityEngine;

public class RB_PlayerAction : MonoBehaviour
{
    //Conditions
    public bool IsChargingAttack;
    public bool IsAttacking;
    public bool IsOnCooldown; //Cannot attack


    public float SpecialAttackCharge; //from 0 to 100
    private float _currentDashCooldown;

    //Components
    private RB_PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();

    }
    private void Start()
    {
        RB_InputManager.Instance.EventMovePerformed.AddListener(StartMove);
        RB_InputManager.Instance.EventMoveCanceled.AddListener(StopMove);
        RB_InputManager.Instance.EventDashStarted.AddListener(StartDash);
    }

    private void StartMove()
    {
        _playerMovement.StartMove();
        print("start");
    }

    private void StopMove()
    {
        _playerMovement.StopMove();
    }

    private void StartDash()
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
