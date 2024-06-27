using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used in animation event
public class RB_ButtonSelectioner : MonoBehaviour
{

    public static RB_ButtonSelectioner Instance; // Singleton instance

    void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Lists of buttons for different categories
    public List<Button> mainButtons = new List<Button>();
    [SerializeField] List<Button> _quitButtons = new List<Button>();
    [SerializeField] List<Button> _optionsButtons = new List<Button>();

    // Enum to specify button types
    public enum BUTTON_TYPE { Main, Quit, Options };

    // Methods to select buttons of specific types by index
    public void SelectMainButton(int ID)
    {
        mainButtons[ID].Select(); // Select a main button by index
    }

    public void SelectQuitButton(int ID)
    {
        _quitButtons[ID].Select(); // Select a quit button by index
    }

    public void SelectOptionsButton(int ID)
    {
        _optionsButtons[ID].Select(); // Select an options button by index
    }

    // Method to disable interaction with main buttons
    public void BlockInteraction()
    {
        for (int u = 0; u < mainButtons.Count; u++)
        {
            mainButtons[u].enabled = false; // Disable each main button
        }
    }
}