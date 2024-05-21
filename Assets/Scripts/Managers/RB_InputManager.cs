using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RB_InputManager : MonoBehaviour
{
    public static RB_InputManager Instance;

    public UnityEvent EventMoveStarted;
    public UnityEvent EventMovePerformed;
    public UnityEvent EventMoveCanceled;

    public UnityEvent EventAttackStarted;
    public UnityEvent EventAttackCanceled;

    public UnityEvent EventSpecialAttackStarted;
    public UnityEvent EventSpecialAttackCanceled;

    public UnityEvent EventDashStarted;
    public UnityEvent EventDashCanceled;

    public UnityEvent EventRewindStarted;
    public UnityEvent EventRewindCanceled;

    public Vector2 MoveValue;

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
        MoveValue = context.ReadValue<Vector2>(); //make the value available for PlayerMovement
        if (context.started)
            EventMoveStarted?.Invoke();
        else if (context.performed)
            EventMovePerformed?.Invoke();
        else if (context.canceled)
            EventMoveCanceled?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            EventAttackStarted?.Invoke();
        else if (context.canceled)
            EventAttackCanceled?.Invoke();
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            EventSpecialAttackStarted?.Invoke();
        else if (context.canceled)
            EventSpecialAttackCanceled?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            EventDashStarted?.Invoke();
        else if (context.canceled)
            EventDashCanceled?.Invoke();
    }

    public void OnRewind(InputAction.CallbackContext context)
    {
        if (context.started)
            EventRewindStarted?.Invoke();
        else if (context.canceled)
            EventRewindCanceled?.Invoke();
    }
}
