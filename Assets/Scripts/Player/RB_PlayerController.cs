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

        RB_InputManager.Instance.EventItem1Started.AddListener(delegate { ChoseItem(0); });
        RB_InputManager.Instance.EventItem2Started.AddListener(delegate { ChoseItem(1); });
        RB_InputManager.Instance.EventItem3Started.AddListener(delegate { ChoseItem(2); });

        _playerAction.EventItemGathered.AddListener(BindToAttack);
    }

    public void OnChargeAttackStart()
    {
        //Start charging attack
        print("attacking");
        _playerAction.StartChargeAttack();
    }

    public void ChoseItem(int id)
    {
        //Chose the item wanted
        if(_playerAction.Items.Count-1 >= id)
        {
            _playerAction.SetCurrentWeapon(_playerAction.Items[id].name);
            _playerAction.Items[id].Bind();
        }
        
    }

    public void Interact()
    {
        //Interact with the object nearby
        _playerAction.Interact();
    }

    public void BindToAttack()
    {
        //When the item is gathed, get it
        _item = GetComponentInChildren<RB_Items>();
        //Set the attack event to the charge attack
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
