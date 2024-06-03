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

    private void Awake(){
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

}
