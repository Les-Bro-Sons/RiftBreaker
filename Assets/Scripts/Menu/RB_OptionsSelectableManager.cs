using UnityEngine;
using UnityEngine.UI;

public class RB_OptionsSelectableManager : MonoBehaviour
{
    public static RB_OptionsSelectableManager Instance;  // Static instance of RB_OptionsSelectableManager

    public Selectable CurrentSelectable;  // Currently selected selectable
    [SerializeField] Selectable _audioButton;  // Serialized field for the audio button selectable

    public int SelectableHooveredCount;  // Counter for how many selectables are hovered over

    // Property to check if any selectables are currently hovered over
    public bool IsSelectableHoovered => SelectableHooveredCount > 0 ? true : false;

    private void Awake()
    {
        // Ensure there is only one instance of RB_OptionsSelectableManager
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Set the default selectable
    public void Default()
    {
        CurrentSelectable = _audioButton;
    }
}
