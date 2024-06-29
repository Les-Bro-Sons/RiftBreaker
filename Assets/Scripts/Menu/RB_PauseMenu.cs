using UnityEngine;

public class RB_PauseMenu : MonoBehaviour
{

    [SerializeField] float _timeScaleSpeed;   // Speed at which time scale changes during pause/unpause
    public bool IsPaused;                     // Flag indicating if the game is currently paused
    [SerializeField] bool _isUnpausing;       // Flag indicating if the game is currently in the process of unpausing
    CanvasGroup _canvasGroup;                 // Reference to the CanvasGroup component attached to this GameObject

    float _oldTimeScale = 1;                  // Previous time scale value before pausing

    void Start()
    {
        // Subscribe to the pause event from RB_MenuInputManager
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(Pause);
        _canvasGroup = GetComponent<CanvasGroup>();  // Get the CanvasGroup component
    }

    private void Update()
    {
        if (IsPaused)
        {
            // During pause:
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * _timeScaleSpeed);  // Smoothly fade in the pause menu
        }
        else if (_isUnpausing)
        {
            // During unpause:

            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0f, Time.unscaledDeltaTime * _timeScaleSpeed);  // Smoothly fade out the pause menu
            RB_MenuManager.Instance.UnPauseAnim();  // Trigger unpause animation

            if (_canvasGroup.alpha < 0.1f)
            {
                _isUnpausing = false;  // Reset unpause flag
                _canvasGroup.alpha = 0f;  // Set canvas alpha to 0 (fully transparent)
                RB_MenuManager.Instance.CloseOption();  // Close any open pause menu options
                RB_ButtonSelectioner.Instance.SelectMainButton(0);  // Select the main button (index 0) using RB_ButtonSelectioner
                RB_MenuManager.Instance.Animator.Play("UnPause");  // Play unpause animation in RB_MenuManager's Animator
            }
        }
        else
        {
            // If not paused and not unpause, hide the pause menu
            _canvasGroup.alpha = 0f;  // Set canvas alpha to 0 (fully transparent)
        }
    }

    public void Pause()
    {
        RB_InputManager.Instance.InputEnabled = false;  // Disable input using RB_InputManager
        if (!IsPaused)
        {
            IsPaused = true;  // Set game state to paused
            RB_MenuManager.Instance.PauseAnim();  // Trigger pause animation in RB_MenuManager
            RB_ButtonSelectioner.Instance.SelectMainButton(0);  // Select the main button (index 0) using RB_ButtonSelectioner
            RB_TimescaleManager.Instance.SetModifier(gameObject, "PauseTimescale", 0, 1000);
        }
        else
        {
            UnPause();  // If already paused, unpause instead
        }
    }

    public void UnPause()
    {
        RB_InputManager.Instance.InputEnabled = true;  // Enable input using RB_InputManager
        RB_ButtonSelectioner.Instance.SelectMainButton(0);  // Select the main button (index 0) using RB_ButtonSelectioner
        RB_MenuManager.Instance.BackMainMenu();  // Return to main menu using RB_MenuManager
        RB_MenuManager.Instance.CancelQuit();  // Cancel any ongoing quit action using RB_MenuManager
        IsPaused = false;  // Set game state to not paused
        _isUnpausing = true;  // Set flag to indicate unpause process is active
        RB_TimescaleManager.Instance.RemoveModifier("PauseTimescale");
    }

}
