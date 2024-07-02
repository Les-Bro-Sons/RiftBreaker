using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_VibrationSlider : MonoBehaviour {
    [SerializeField] Slider _vibrationSlider;
    [SerializeField] TextMeshProUGUI _vibartionLevel;

    void Start() {
        _vibrationSlider.onValueChanged.AddListener(delegate { RB_VibrationManager.Instance.SetVibration((int)_vibrationSlider.value); });
        StartVibration();
    }

    void Update() { 
       _vibartionLevel.text = RB_VibrationManager.Instance.ActualName;
    }

    public void StartVibration() {
        // Set vibration slider value based on player preferences or default to 1 if not set
        if (!PlayerPrefs.HasKey("Vibration")) {
            _vibrationSlider.value = 4f;
            Debug.Log("gyat");
        }
        else {
            _vibrationSlider.value = PlayerPrefs.GetFloat("Vibration");
        }

    }

    public void Default() {
        _vibrationSlider.value = 4f;

        // Save default volume values to player preferences
        PlayerPrefs.SetFloat("Vibration", _vibrationSlider.value);
        PlayerPrefs.Save(); // Save preferences immediately
    }
}
