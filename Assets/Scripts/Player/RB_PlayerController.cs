using UnityEngine;

public class RB_PlayerController : MonoBehaviour
{
    public static RB_PlayerController Instance;

    //Components
    RB_PlayerMovement _playerMovement;
    RB_PlayerAction _playerAction;
    RB_Items _item;

    //States
    [HideInInspector] public PLAYERSTATES CurrentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerAction = GetComponent<RB_PlayerAction>();
        _item = GetComponentInChildren<RB_Items>();
    }

    private void Start()
    {
        if(_item != null)
        {
            RB_InputManager.Instance.EventAttackStarted.RemoveAllListeners();
            RB_InputManager.Instance.EventAttackStarted.AddListener(OnChargeAttackStart);
            RB_InputManager.Instance.EventAttackCanceled.AddListener(OnChargeAttackStop);
        }
        else
        {
            RB_InputManager.Instance.EventAttackStarted.RemoveAllListeners();
            RB_InputManager.Instance.EventAttackStarted.AddListener(Interact);
        }
        
        RB_InputManager.Instance.EventMovePerformed.AddListener(OnMoveStart);
        RB_InputManager.Instance.EventMoveCanceled.AddListener(OnMoveStop);
        RB_InputManager.Instance.EventDashStarted.AddListener(OnStartDash);
        RB_InputManager.Instance.EventSpecialAttackStarted.AddListener(OnSpecialAttack);
        RB_InputManager.Instance.EventRewindStarted.AddListener(OnStartRewind);
        RB_InputManager.Instance.EventRewindCanceled.AddListener(OnStopRewind);

        _playerAction.EventItemGathered.AddListener(BindToAttack);
    }

    public void OnChargeAttackStart()
    {
        //Start charging attack
        print("attacking");
        _playerAction.StartChargeAttack();
    }

    public void Interact()
    {
        _playerAction.Interact();
    }

    public void BindToAttack()
    {
        _item = GetComponentInChildren<RB_Items>();
        RB_InputManager.Instance.EventAttackStarted.RemoveAllListeners();
        RB_InputManager.Instance.EventAttackStarted.AddListener(OnChargeAttackStart);
        RB_InputManager.Instance.EventAttackCanceled.AddListener(OnChargeAttackStop);
        _playerAction.Rebind();
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

    public void OnSpecialAttack()
    {
        //Start special attack
        _playerAction.SpecialAttack();
    }

    public void OnStartRewind()
    {
        //start rewind in playeraction
    }

    public void OnStopRewind()
    {
        //stop rewind in playeraction
    }
}
