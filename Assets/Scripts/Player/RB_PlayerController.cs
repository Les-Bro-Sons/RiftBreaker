using System.Collections;
using UnityEngine;

public class RB_PlayerController : MonoBehaviour
{
    //Components
    RB_PlayerMovement _playerMovement;
    RB_PlayerAction _playerAction;
    RB_Items _item;

    //States
    [HideInInspector] public PLAYERSTATES CurrentState;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerAction = GetComponent<RB_PlayerAction>();
        _item = GetComponentInChildren<RB_Items>();
    }

    private void Start()
    {

        RB_InputManager.Instance.EventAttackStarted.AddListener(OnChargeAttackStart);
        RB_InputManager.Instance.EventAttackCanceled.AddListener(OnChargeAttackStop);
        RB_InputManager.Instance.EventMovePerformed.AddListener(OnMoveStart);
        RB_InputManager.Instance.EventMoveCanceled.AddListener(OnMoveStop);
        RB_InputManager.Instance.EventDashStarted.AddListener(OnStartDash);
    }

    public void OnChargeAttackStart()
    {
        //Start charging attack
        _playerAction.StartChargeAttack();
    }

    public void OnChargeAttackStop()
    {
        //If charge attack completed start charged attack otherwise start normal attack
        _playerAction.StopChargeAttack();
    }

    public void OnStartDash()
    {
        //Start dash
        _playerAction.StartDash();
    }

    public void OnMoveStart()
    {
        //Start movement
        _playerMovement.StartMove();
    }

    public void OnMoveStop()
    {
        //Stop movement
        _playerMovement.StopMove();
    }


}
