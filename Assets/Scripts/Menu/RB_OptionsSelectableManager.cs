using UnityEngine;
using UnityEngine.UI;

public class RB_OptionsSelectableManager : MonoBehaviour {
    public static RB_OptionsSelectableManager Instance;

    public Selectable CurrentSelectable;
    [SerializeField] Selectable _audioButton;

    public int ButtonHooveredCount;

    public bool IsButtonsHoovered => ButtonHooveredCount > 0 ? true : false;

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    public void Default() { 
        CurrentSelectable = _audioButton;
    }
}
