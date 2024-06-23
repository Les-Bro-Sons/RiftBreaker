using UnityEngine;

public class RB_OptionsButtonManager : MonoBehaviour {
    public static RB_OptionsButtonManager Instance;

    public enum OPTIONBUTTONS { Audio, Visual, Options, Exit }

    public OPTIONBUTTONS CurrentButton;

    public int ButtonHooveredCount;

    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void Default() { 
        CurrentButton = OPTIONBUTTONS.Audio;
    }
}
