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

    public UnityEvent EventItem1Started;
    public UnityEvent EventItem1Canceled;

    public UnityEvent EventItem2Started;
    public UnityEvent EventItem2Canceled;

    public UnityEvent EventItem3Started;
    public UnityEvent EventItem3Canceled;

    public Vector2 MoveValue;

    public bool IsMouse = false;

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
        IsMouse = (context.action.activeControl.device.name == "Mouse");
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

    public void OnItem1(InputAction.CallbackContext context)
    {
        if (context.started)
            EventItem1Started?.Invoke();
        else if (context.canceled)
            EventItem1Canceled?.Invoke();
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (context.started)
            EventItem2Started?.Invoke();
        else if (context.canceled)
            EventItem2Canceled?.Invoke();
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (context.started)
            EventItem3Started?.Invoke();
        else if (context.canceled)
            EventItem3Canceled?.Invoke();
    }

    public Vector3 GetMouseDirection()
    {
        Vector3 direction = new();
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Camera.main.nearClipPlane;
        Vector3 adjustedWorldMousePos = Camera.main.ScreenToWorldPoint(screenMousePos) - Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, screenMousePos.z));
        print(adjustedWorldMousePos);
        direction = new Vector3((adjustedWorldMousePos - Vector3.zero).x, 0, (adjustedWorldMousePos - Vector3.zero).z);

        return direction;
    }
}
