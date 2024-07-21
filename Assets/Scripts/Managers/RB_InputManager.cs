using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RB_InputManager : MonoBehaviour
{
    public static RB_InputManager Instance;

    public enum INPUTSTATE
    {
        Started,
        Performed,
        Canceled,
    }
    public enum INPUTACTIONNAME
    {
        Move,
        Attack,
        DirectAttackController,
        SpecialAttack,
        Dash,
        Rewind,
        Item1,
        Item2,
        Item3,
    }

    public Dictionary<INPUTACTIONNAME, INPUTSTATE> ActionsState = new();


    private bool _inputEnabled = true;
    public bool InputEnabled
    {
        get { return _inputEnabled; }
        set
        {
            _inputEnabled = value;
            if (_inputEnabled == false)
                CancelAllActiveInput();
        }
    }

    #region EventAction
    [Header("Move")]
    public bool MoveEnabled = true;
    [HideInInspector] public UnityEvent EventMoveStarted;
    [HideInInspector] public UnityEvent EventMovePerformed;
    [HideInInspector] public UnityEvent EventMoveCanceled;

    public Vector2 MoveValue;

    [Header("Attack")]
    public bool AttackEnabled = true;
    [HideInInspector] public UnityEvent EventAttackStartedEvenIfDisabled;
    [HideInInspector] public UnityEvent EventAttackStarted;
    [HideInInspector] public UnityEvent EventAttackCanceled;

    [HideInInspector] public UnityEvent EventDirectAttackControllerStarted;
    [HideInInspector] public UnityEvent EventDirectAttackControllerPerformed;
    [HideInInspector] public UnityEvent EventDirectAttackControllerCanceled;

    [Header("Special Attack")]
    public bool SpecialAttackEnabled = true;
    [HideInInspector] public UnityEvent EventSpecialAttackStarted;
    [HideInInspector] public UnityEvent EventSpecialAttackCanceled;

    [Header("Dash")]
    public bool DashEnabled = true;
    [HideInInspector] public UnityEvent EventDashStarted;
    [HideInInspector] public UnityEvent EventDashCanceled;

    [Header("Rewind")]
    public bool RewindEnabled = true;
    [HideInInspector] public UnityEvent EventRewindStarted;
    [HideInInspector] public UnityEvent EventRewindCanceled;

    [Header("Items")]
    public bool ItemsEnabled = true;
    [HideInInspector] public UnityEvent EventItem1Started;
    [HideInInspector] public UnityEvent EventItem1Canceled;

    [HideInInspector] public UnityEvent EventItem2Started;
    [HideInInspector] public UnityEvent EventItem2Canceled;

    [HideInInspector] public UnityEvent EventItem3Started;
    [HideInInspector] public UnityEvent EventItem3Canceled;
    #endregion

    [Header("Console")]
    [HideInInspector] public UnityEvent ConsoleToggleInputEvent;

    public Vector2 DirectAttackControllerValue;

    public bool IsMouse = false;

    private Transform _playerTransform;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject); //make this object last parent don't destroy on load
        }
        else
        {
            DestroyImmediate(gameObject); //destroy if another RB_InputManager is already in the scene
            return;
        }
    }

    public void CancelAllActiveInput()
    {
        foreach (INPUTACTIONNAME actionName in ActionsState.Keys.ToList())
        {
            if (ActionsState[actionName] != INPUTSTATE.Canceled)
            {
                switch(actionName)
                {
                    case INPUTACTIONNAME.Move:
                        EventMoveCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Attack:
                        EventAttackCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.DirectAttackController: 
                        EventDirectAttackControllerCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.SpecialAttack:
                        EventSpecialAttackCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Dash:
                        EventDashCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Rewind:
                        EventRewindCanceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Item1:
                        EventItem1Canceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Item2:
                        EventItem2Canceled?.Invoke();
                        break;
                    case INPUTACTIONNAME.Item3:
                        EventItem3Canceled?.Invoke();
                        break;
                }
                ActionsState[actionName] = INPUTSTATE.Canceled;
            }
        }
    }

    public void OnConsoleToggled(InputAction.CallbackContext context) //When the console is toggled
    {
        if (context.started)
            ConsoleToggleInputEvent?.Invoke(); //Invoke the event that will open / close the console
    }

    #region Move
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !MoveEnabled)
        {
            MoveValue = Vector2.zero;
            return;
        }

        MoveValue = context.ReadValue<Vector2>(); //make the value available for PlayerMovement
        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Move] = INPUTSTATE.Started;
            EventMoveStarted?.Invoke();
        }
        else if (context.performed)
        {
            ActionsState[INPUTACTIONNAME.Move] = INPUTSTATE.Performed;
            EventMovePerformed?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Move] = INPUTSTATE.Canceled;
            EventMoveCanceled?.Invoke();
        }
    }
    #endregion

    #region Attacks
    public void OnDirectionAttackJoystick(InputAction.CallbackContext context)
    {
        if (!InputEnabled) return;
        DirectAttackControllerValue = context.ReadValue<Vector2>(); //make the value available for PlayerMovement
        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.DirectAttackController] = INPUTSTATE.Started;
            EventDirectAttackControllerStarted?.Invoke();
        }
        else if (context.performed)
        {
            ActionsState[INPUTACTIONNAME.DirectAttackController] = INPUTSTATE.Performed;
            EventDirectAttackControllerPerformed?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.DirectAttackController] = INPUTSTATE.Canceled;
            EventDirectAttackControllerCanceled?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!InputEnabled) return;
        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Attack] = INPUTSTATE.Started;
            EventAttackStartedEvenIfDisabled?.Invoke();
        }

        IsMouse = (context.action.activeControl.device.name == "Mouse");
        if (!AttackEnabled) return;

        if (context.started)
            EventAttackStarted?.Invoke();
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Attack] = INPUTSTATE.Canceled;
            EventAttackCanceled?.Invoke();
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !SpecialAttackEnabled) return;

        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.SpecialAttack] = INPUTSTATE.Started;
            EventSpecialAttackStarted?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.SpecialAttack] = INPUTSTATE.Canceled;
            EventSpecialAttackCanceled?.Invoke();
        }
    }
    #endregion

    #region Dash
    public void OnDash(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !DashEnabled) return;
        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Dash] = INPUTSTATE.Started;
            EventDashStarted?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Dash] = INPUTSTATE.Canceled;
            EventDashCanceled?.Invoke();
        }
    }
    #endregion

    #region Rewind
    public void OnRewind(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !RewindEnabled) return;

        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Rewind] = INPUTSTATE.Started;
            EventRewindStarted?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Rewind] = INPUTSTATE.Canceled;
            EventRewindCanceled?.Invoke();
        }
    }
    #endregion

    #region Items
    public void OnItem1(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !ItemsEnabled) return;

        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Item1] = INPUTSTATE.Started;
            EventItem1Started?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Item1] = INPUTSTATE.Canceled;
            EventItem1Canceled?.Invoke();
        }
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !ItemsEnabled) return;

        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Item2] = INPUTSTATE.Started;
            EventItem2Started?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Item2] = INPUTSTATE.Canceled;
            EventItem2Canceled?.Invoke();
        }
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (!InputEnabled || !ItemsEnabled) return;

        if (context.started)
        {
            ActionsState[INPUTACTIONNAME.Item3] = INPUTSTATE.Started;
            EventItem3Started?.Invoke();
        }
        else if (context.canceled)
        {
            ActionsState[INPUTACTIONNAME.Item3] = INPUTSTATE.Canceled;
            EventItem3Canceled?.Invoke();
        }
    }
    #endregion


    public Vector3 GetMouseDirection()
    {
        if (RB_PlayerAction.Instance != null) //If there's a player in the current scene
        {
            _playerTransform = RB_PlayerAction.Instance.transform; //Set it to the current player
        }
        else
            return Vector3.zero; //If there's no player in the current scene return a vector zero
        

        Vector3 direction = new();
        //Mous)
        Vector3 playerPos = _playerTransform.position;
        Vector3 worldMousePos = new();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);

        // Sort hits by distance
        Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == RB_GroundManager.Instance.gameObject)
            {
                //world mouse position by raycast
                worldMousePos = hit.point;
                break;
            }
        }

        //The direction starting from the the position of the mouse on the camera and finishing to the world mouse pos
        Vector3 cameraMouseDirection = -(worldMousePos - ray.origin).normalized;
        
        //Translate the world mouse position withe camera mouse direction acting like a Vector3.Up
        worldMousePos += cameraMouseDirection;

        //The direction from the player to the world mouse pos
        direction = worldMousePos - _playerTransform.position;
        direction.y = 0;

        return direction;
    }

   
}
