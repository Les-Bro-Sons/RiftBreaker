using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RB_InputManager : MonoBehaviour
{
    public static RB_InputManager Instance;

    [Header("Move")]
    public bool MoveEnabled = true;
    [HideInInspector] public UnityEvent EventMoveStarted;
    [HideInInspector] public UnityEvent EventMovePerformed;
    [HideInInspector] public UnityEvent EventMoveCanceled;

    [Header("Attack")]
    public bool AttackEnabled = true;
    [HideInInspector] public UnityEvent EventAttackStarted;
    [HideInInspector] public UnityEvent EventAttackCanceled;

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

    public Vector2 MoveValue;

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
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!MoveEnabled) return;

        MoveValue = context.ReadValue<Vector2>(); //make the value available for PlayerMovement
        if (context.started)
        {
            EventMoveStarted?.Invoke();
        }
        else if (context.performed)
            EventMovePerformed?.Invoke();
        else if (context.canceled)
            EventMoveCanceled?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        IsMouse = (context.action.activeControl.device.name == "Mouse");
        if (!AttackEnabled) return;

        if (context.started)
            EventAttackStarted?.Invoke();
        else if (context.canceled)
            EventAttackCanceled?.Invoke();
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (!SpecialAttackEnabled) return;

        if (context.started)
            EventSpecialAttackStarted?.Invoke();
        else if (context.canceled)
            EventSpecialAttackCanceled?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!DashEnabled) return;
        if (context.started) {
            EventDashStarted?.Invoke();
        }
        else if (context.canceled)
        EventDashCanceled?.Invoke();
    }

    public void OnRewind(InputAction.CallbackContext context)
    {
        if (!RewindEnabled) return;

        if (context.started)
            EventRewindStarted?.Invoke();
        else if (context.canceled)
            EventRewindCanceled?.Invoke();
    }

    public void OnItem1(InputAction.CallbackContext context)
    {
        if (!ItemsEnabled) return;

        if (context.started)
            EventItem1Started?.Invoke();
        else if (context.canceled)
            EventItem1Canceled?.Invoke();
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (!ItemsEnabled) return;

        if (context.started)
            EventItem2Started?.Invoke();
        else if (context.canceled)
            EventItem2Canceled?.Invoke();
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (!ItemsEnabled) return;

        if (context.started)
            EventItem3Started?.Invoke();
        else if (context.canceled)
            EventItem3Canceled?.Invoke();
    }

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
