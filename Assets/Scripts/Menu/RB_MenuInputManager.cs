using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RB_MenuInputManager : MonoBehaviour {
    public static RB_MenuInputManager Instance;

    [Header("Navigation")]
    public UnityEvent EventNavigateStarted;
    public UnityEvent EventNavigatePerformed;
    public UnityEvent EventNavigateCanceled;
    public Vector2 NavigationValue;

    [Header("Submit")]
    public UnityEvent EventSubmitStarted;
    public UnityEvent EventSubmitCanceled;

    [Header("Cancel")]
    public UnityEvent EventCancelStarted;
    public UnityEvent EventCancelCanceled;

    [Header("Pause")]
    public UnityEvent EventPauseStarted;
    public UnityEvent EventPauseCanceled;

    [Header("Any")] 
    public UnityEvent EventAnyStarted;
    public UnityEvent EventAnyCanceled;

    [Header("MouseMoving")]
    public UnityEvent EventMouseMovingStarted;
    public UnityEvent EventMouseMovingCanceled;
    public bool IsLastInputMouse;

    [Header("Next")]
    public UnityEvent EventNextStarted;
    public UnityEvent EventNextCanceled;

    public bool IsKeyBoard = true;


    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);//make this object don't destroy on load
        }
        else {
            DestroyImmediate(gameObject); //destroy if another RB_MenuInputManager is already in the scene
        }
    }

    public void OnNavigate(InputAction.CallbackContext context){
        NavigationValue = context.ReadValue<Vector2>();
        if (context.started) { EventNavigateStarted?.Invoke(); }
        else if (context.performed) { EventNavigatePerformed?.Invoke(); }
        else if (context.canceled) { EventNavigateCanceled?.Invoke(); }
    }

    public void OnSubmit(InputAction.CallbackContext context) {
        if (context.started) { EventSubmitStarted?.Invoke(); }
        else if (context.canceled) { EventSubmitCanceled?.Invoke(); }
    }

    public void OnCancel(InputAction.CallbackContext context) {
        if (context.started) { EventCancelStarted?.Invoke(); }
        else if (context.canceled) { EventCancelCanceled?.Invoke(); }
     }

    public void OnPause(InputAction.CallbackContext context) {
        if (context.started) { EventPauseStarted?.Invoke(); }
        else if (context.canceled) { EventPauseCanceled?.Invoke(); }
    }
    public void OnNext(InputAction.CallbackContext context) {
        if (context.started) { EventNextStarted?.Invoke(); }
        else if (context.canceled) { EventNextCanceled?.Invoke(); }
    }

    public void OnAny(InputAction.CallbackContext context) {
        if (context.action.activeControl.device.name == "Mouse" || context.action.activeControl.device.name == "Keyboard")
            IsKeyBoard = true;
        else IsKeyBoard = false;

        if (context.action.activeControl.device.name != "Mouse")
        {
            IsLastInputMouse = false;
        }
            

        if (context.started) { 
            EventAnyStarted?.Invoke();
        }
        else if (context.canceled) { EventAnyCanceled?.Invoke(); }
    }

    public void OnMouseMoving(InputAction.CallbackContext context){
        if (context.action.activeControl.device.name == "Mouse") {
            IsLastInputMouse = true;
        }
            

        if (context.started) {
            EventMouseMovingStarted?.Invoke(); 
        }
        else if (context.canceled) { EventMouseMovingCanceled?.Invoke(); }
    }
}
