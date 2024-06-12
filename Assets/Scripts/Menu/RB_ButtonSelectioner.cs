using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used in animation event
public class RB_ButtonSelectioner : MonoBehaviour {

    public static RB_ButtonSelectioner Instance;

    void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public List<Button> mainButtons = new List<Button>();
    [SerializeField] List<Button> _quitButtons = new List<Button>();
    [SerializeField] List<Button> _optionsButtons = new List<Button>();

    public enum BUTTON_TYPE {Main, Quit, Options};

    public void SelectMainButton(int ID) { mainButtons[ID].Select(); }
    public void SelectQuitButton(int ID) { _quitButtons[ID].Select(); }
    public void SelectOptionsButton(int ID) { _optionsButtons[ID].Select(); }
    
    public void BlockInteraction() {
        Debug.Log(mainButtons.Count);
        for (int u = 0; u < mainButtons.Count; u++) {
            mainButtons[u].enabled = false;
            Debug.Log(mainButtons[u]);
        }
    }
}
