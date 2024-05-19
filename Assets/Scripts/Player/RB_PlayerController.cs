using UnityEngine;
using UnityEngine.Events;

public class RB_PlayerController : MonoBehaviour
{
    public static RB_PlayerController Instance;

    //Components
    RB_PlayerMovement _playerMovement;
    RB_PlayerAction _playerAction;

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
    }

    private void Start()
    {
        RB_InputManager.Instance.EventAttackStarted.AddListener(OnAttackStart);
        RB_InputManager.Instance.EventMovePerformed.AddListener(OnMoveStart);
        RB_InputManager.Instance.EventMoveCanceled.AddListener(OnMoveStop);
        RB_InputManager.Instance.EventDashStarted.AddListener(OnStartDash);
        RB_InputManager.Instance.EventRewindStarted.AddListener(OnStartRewind);
        RB_InputManager.Instance.EventRewindCanceled.AddListener(OnStopRewind);
    }


    public void OnAttackStart()
    {
        //Start attack
        _playerAction.Attack();
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

    public void OnStartRewind()
    {
        //start rewind in playeraction
    }

    public void OnStopRewind()
    {
        //stop rewind in playeraction
    }
}
