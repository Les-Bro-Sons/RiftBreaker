using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MainMenuButtonManager : MonoBehaviour {
    public static RB_MainMenuButtonManager Instance;

    public enum BUTTONS { Play, Options, Credits, Quit}

    public BUTTONS CurrentButton;

    public int ButtonHooveredCount;

    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake(){
        if (Instance == null) { Instance = this; }
    }
}
