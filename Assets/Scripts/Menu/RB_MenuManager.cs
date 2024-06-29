using UnityEngine;
using UnityEngine.UI;

public class RB_MenuManager : MonoBehaviour
{

    public static RB_MenuManager Instance;  // Singleton instance reference

    public bool IsOptionOpen;  // Flag indicating whether the options menu is open

    public Animator Animator;  // Reference to the Animator component

    public enum MENUSTATE
    {
        Audio, Video, Control
    }

    public MENUSTATE MenuState;  // Current state of the menu

    [Header("Sliders")]
    [SerializeField] Slider _sliderGeneral;  // Reference to the general settings slider
    [SerializeField] Slider _sliderMusic;    // Reference to the music volume slider
    [SerializeField] Slider _sliderSFX;      // Reference to the sound effects volume slider

    private void Awake()
    {
        if (Instance == null) { Instance = this; }  // Set the singleton instance
        else { Destroy(gameObject); }  // Ensure only one instance exists
        Animator = GetComponent<Animator>();  // Get the Animator component on this GameObject
        Animator.updateMode = AnimatorUpdateMode.UnscaledTime;  // Ensure Animator updates in unscaled time
    }

    private void Start()
    {
        RB_ButtonSelectioner.Instance.SelectMainButton(0);  // Select the main button when the game starts
    }

    // Start a new game
    public void NewGame()
    {
        RB_ButtonSelectioner.Instance.BlockInteraction();  // Block UI interaction
        RB_SaveManager.Instance.ResetSave();  // Reset the game save data
        RB_SaveManager.Instance.SaveObject.IsGameFinish = false;  // Reset game finish flag
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 1);  // Transition to a new scene
        RB_InputManager.Instance.InputEnabled = true;  // Enable input
    }

    // Continue from a saved game
    public void Continue()
    {
        RB_ButtonSelectioner.Instance.BlockInteraction();  // Block UI interaction
        if (RB_SaveManager.Instance.SaveObject.IsGameFinish)
        {  // Check if the game is finished
            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 15);  // Transition to a specific scene
            RB_SaveManager.Instance.SaveObject.HpBossRush = 150;  // Reset specific game data
        }
        else
        {
            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), RB_SaveManager.Instance.SaveObject.CurrentLevel);  // Transition to a saved level
        }
        RB_InputManager.Instance.InputEnabled = true;  // Enable input
    }

    // Return to the main menu
    public void MainMenu()
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0);  // Transition to the main menu scene
        RB_InputManager.Instance.InputEnabled = true;  // Enable input
    }

    // Open the options menu
    public void Options()
    {
        Animator.SetBool("IsOptionOpen", true);  // Set animator parameter to open options menu
        IsOptionOpen = true;  // Set options menu open flag
        MenuState = MENUSTATE.Audio;  // Set menu state to Audio
    }

    // Close the options menu
    public void CloseOption()
    {
        if (IsOptionOpen)
        {
            Animator.SetBool("IsOptionOpen", false);  // Set animator parameter to close options menu
            RB_ButtonSelectioner.Instance.SelectMainButton(2);  // Select a specific button after closing options
            IsOptionOpen = false;  // Set options menu open flag to false
        }
    }

    // Switch to audio options
    public void OptionAudio()
    {
        if (MenuState != MENUSTATE.Audio)
        {
            Animator.SetTrigger("Audio");  // Trigger audio options animation
            MenuState = MENUSTATE.Audio;  // Set menu state to Audio
        }
    }

    // Switch to video options
    public void OptionVideo()
    {
        if (MenuState != MENUSTATE.Video)
        {
            Animator.SetTrigger("Video");  // Trigger video options animation
            MenuState = MENUSTATE.Video;  // Set menu state to Video
        }
    }

    // Switch to control options
    public void OptionControl()
    {
        if (MenuState != MENUSTATE.Control)
        {
            Animator.SetTrigger("Control");  // Trigger control options animation
            MenuState = MENUSTATE.Control;  // Set menu state to Control
        }
    }

    // Display the credits
    public void Credits()
    {
        Animator.SetTrigger("Credits");  // Trigger credits animation
    }

    // Initiate the quit process
    public void Quit()
    {
        Animator.SetBool("IsQuitOpen", true);  // Set animator parameter to open quit menu
    }

    // Confirm quitting the application
    public void ConfirmQuit()
    {
        Application.Quit();  // Quit the application
    }

    // Cancel quitting the application
    public void CancelQuit()
    {
        Animator.SetBool("IsQuitOpen", false);  // Set animator parameter to close quit menu
    }

    // Return to the main menu from options
    public void BackMainMenu()
    {
        Animator.SetBool("IsOptionOpen", false);  // Set animator parameter to close options menu
        CloseOption();  // Close options menu
        _sliderGeneral.GetComponent<RB_MenuSlider>().ResetInteraction();  // Reset interaction for general slider
        _sliderMusic.GetComponent<RB_MenuSlider>().ResetInteraction();  // Reset interaction for music slider
        _sliderSFX.GetComponent<RB_MenuSlider>().ResetInteraction();  // Reset interaction for SFX slider
    }

    // Pause the game animation
    public void PauseAnim()
    {
        Animator.SetBool("IsPaused", true);  // Set animator parameter to pause animation
    }

    // Unpause the game animation
    public void UnPauseAnim()
    {
        Animator.SetBool("IsPaused", false);  // Set animator parameter to unpause animation
    }

    // Select a specific button in the UI
    public void SelectButton(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Button button))
        {
            button.Select();  // Select the button if it has a Button component
        }
    }
}
