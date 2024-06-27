using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_VolumeSliders : MonoBehaviour
{
    [SerializeField] Slider _generalSlider; // Slider for general volume control
    [SerializeField] TextMeshProUGUI _generalVolume; // Text displaying general volume percentage
    [SerializeField] Slider _musicSlider; // Slider for music volume control
    [SerializeField] TextMeshProUGUI _musicVolume; // Text displaying music volume percentage
    [SerializeField] Slider _SFXSlider; // Slider for sound effects (SFX) volume control
    [SerializeField] TextMeshProUGUI _SFXVolume; // Text displaying SFX volume percentage
    [SerializeField] Button _resetButton; // Button to reset volumes to default

    void Start()
    {
        // Add listeners to sliders to update volume settings when changed
        _generalSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetGeneralVolume(_generalSlider.value); });
        _SFXSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetSFXVolume(_SFXSlider.value); });
        _musicSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetMusicVolume(_musicSlider.value); });

        // Add listener to reset button to reset volumes to default
        _resetButton.onClick.AddListener(Default);

        // Initialize volume sliders based on saved player preferences
        StartVolumes();
    }

    // Method to initialize volume sliders based on player preferences
    public void StartVolumes()
    {
        // Set general volume slider value based on player preferences or default to 1 if not set
        if (!PlayerPrefs.HasKey("GeneralVolume"))
        {
            _generalSlider.value = 1f;
        }
        else
        {
            _generalSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        }

        // Set music volume slider value based on player preferences or default to 1 if not set
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            _musicSlider.value = 1f;
        }
        else
        {
            _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        // Set SFX volume slider value based on player preferences or default to 1 if not set
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            _SFXSlider.value = 1f;
        }
        else
        {
            _SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
    }

    // Method to reset volume sliders to default values
    public void Default()
    {
        _generalSlider.value = 1;
        _musicSlider.value = 1;
        _SFXSlider.value = 1;

        // Save default volume values to player preferences
        PlayerPrefs.SetFloat("GeneralVolume", _generalSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _SFXSlider.value);
        PlayerPrefs.Save(); // Save preferences immediately
    }

    // Update method to display current volume percentages on the UI
    private void Update()
    {
        _generalVolume.text = (Mathf.RoundToInt(_generalSlider.value * 100)).ToString() + " %";
        _musicVolume.text = (Mathf.RoundToInt(_musicSlider.value * 100)).ToString() + " %";
        _SFXVolume.text = (Mathf.RoundToInt(_SFXSlider.value * 100)).ToString() + " %";
    }
}
