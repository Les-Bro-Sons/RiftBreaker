using UnityEngine;
using UnityEngine.UI;

public class RB_QuitButtonManager : MonoBehaviour
{
    public static RB_QuitButtonManager Instance; // Static instance of RB_QuitButtonManager

    public Button CurrentButton; // Public field to store the current button

    public int ButtonHooveredCount; // Counter for how many buttons are hovered

    // Property to check if any buttons are currently hovered
    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake()
    {
        // Ensure there is only one instance of RB_QuitButtonManager
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
