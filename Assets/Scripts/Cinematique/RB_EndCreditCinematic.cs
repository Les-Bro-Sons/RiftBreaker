using MANAGERS;
using UnityEngine;

public class RB_EndCreditCinematic : MonoBehaviour
{
    [SerializeField] private RectTransform _creditTransform;  // Reference to the RectTransform of the credits
    [SerializeField] private float _creditSpeed;  // Speed at which the credits scroll

    /// <summary>
    /// Starts the end credit cinematic: plays music, listens for skip input events.
    /// </summary>
    private void Start()
    {
        RB_AudioManager.Instance.PlayMusic("Credit_Music");  // Start playing the credit music
        RB_MenuInputManager.Instance.EventNextStarted.AddListener(OnStartCreditSkip);  // Listen for input to speed up credits
        RB_MenuInputManager.Instance.EventNextCanceled.AddListener(OnStopCreditSkip);  // Listen for input to slow down credits
    }

    /// <summary>
    /// Updates the position of the credits based on their scrolling speed.
    /// Triggers actions when the credits reach the end.
    /// </summary>
    private void Update()
    {
        // Scroll the credits upwards based on the credit speed
        _creditTransform.localPosition += Vector3.up * _creditSpeed * Time.deltaTime;

        // Check if the credits have scrolled past a certain point
        if (_creditTransform.localPosition.y > 4600)
        {
            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0); // Transition to another scene (assuming scene index 0)
            RB_SaveManager.Instance.ResetSave(); // Reset save data
            RB_SaveManager.Instance.SaveObject.IsGameFinish = true; // Mark the game as finished in the save data
            RB_SaveManager.Instance.SaveToJson(); //Save that the game is finish
        }
    }

    /// <summary>
    /// Increases the credit scroll speed when skip input is detected.
    /// </summary>
    private void OnStartCreditSkip()
    {
        _creditSpeed *= 10;  // Increase credit scroll speed
    }

    /// <summary>
    /// Decreases the credit scroll speed when skip input is cancelled.
    /// </summary>
    private void OnStopCreditSkip()
    {
        _creditSpeed /= 10;  // Decrease credit scroll speed
    }
}
