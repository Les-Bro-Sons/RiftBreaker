using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_ButtonSelectioner : MonoBehaviour {

    [SerializeField] List<Button> _mainButtons = new List<Button>();
    [SerializeField] List<Button> _quitButtons = new List<Button>();
    [SerializeField] List<Button> _optionsButtons = new List<Button>();

    public enum BUTTON_TYPE {Main, Quit, Options};

    public void SelectMainButton(int ID) { _mainButtons[ID].Select(); }
    public void SelectQuitButton(int ID) { _quitButtons[ID].Select(); }
    public void SelectOptionsButton(int ID) { _optionsButtons[ID].Select(); }
}
