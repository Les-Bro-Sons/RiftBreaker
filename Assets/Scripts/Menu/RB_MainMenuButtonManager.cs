using UnityEngine;

public class RB_MainMenuButtonManager : MonoBehaviour
{
    public static RB_MainMenuButtonManager Instance; // Singleton instance

    // Enum for main menu buttons
    public enum BUTTONS { NewGame, Continue, Options, Credits, Quit }

    // Enum for pause menu buttons
    public enum PAUSE_BUTTONS { Continue, Options, MainMenu }

    public BUTTONS CurrentButton; // Current selected main menu button
    public PAUSE_BUTTONS CurrentButton2; // Current selected pause menu button

    public int ButtonHooveredCount; // Count of hovered buttons

    // Property to check if any buttons are hovered
    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
    }
}
