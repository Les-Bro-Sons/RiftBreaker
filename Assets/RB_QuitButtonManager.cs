using UnityEngine;
using UnityEngine.UI;

public class RB_QuitButtonManager : MonoBehaviour {
    public static RB_QuitButtonManager Instance;

    public Button CurrentButton;

    public int ButtonHooveredCount;

    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake(){
    if (Instance == null) { Instance = this; }
    }
}
