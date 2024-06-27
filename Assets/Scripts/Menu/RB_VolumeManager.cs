using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RB_VolumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer; // Reference to the AudioMixer to control audio levels

    // References to UI elements for volume control
    public Slider GeneralSlider; // Slider for general volume
    public TextMeshProUGUI GeneralVolume; // Text displaying general volume percentage
    public Slider MusicSlider; // Slider for music volume
    public TextMeshProUGUI MusicVolume; // Text displaying music volume percentage
    public Slider SFXSlider; // Slider for sound effects (SFX) volume
    public TextMeshProUGUI SFXVolume; // Text displaying SFX volume percentage
    public Button ResetButton; // Button to reset volumes to default

    public static RB_VolumeManager Instance; // Singleton instance

    private void Awake()
    {
        // Singleton pattern: ensure only one instance of RB_VolumeManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        // StartVolumes(); // Uncomment to initialize volume sliders with saved values or defaults
        // SetGeneralVolume(); // Uncomment to set initial general volume
        // SetMusicVolume(); // Uncomment to set initial music volume
        // SetSFXVolume(); // Uncomment to set initial SFX volume
    }

    // Initialize volume sliders with saved values or defaults
    public void StartVolumes()
    {
        if (!PlayerPrefs.HasKey("GeneralVolume"))
        {
            GeneralSlider.value = 1f;
        }
        else
        {
            GeneralSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicSlider.value = 1f;
        }
        else
        {
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXSlider.value = 1f;
        }
        else
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
    }

    // Reset all sliders to default values and save settings
    public void Default()
    {
        GeneralSlider.value = 1;
        MusicSlider.value = 1;
        SFXSlider.value = 1;

        // Save default volume values to player preferences
        PlayerPrefs.SetFloat("GeneralVolume", GeneralSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.Save();
    }

    // Set the general volume based on the slider value
    public void SetGeneralVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for general volume

        // Update UI text to display current volume percentage
        if (GeneralVolume != null)
        {
            GeneralVolume.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("GeneralVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the music volume based on the slider value
    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for music volume

        // Update UI text to display current volume percentage
        if (MusicVolume != null)
        {
            MusicVolume.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the SFX volume based on the slider value
    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for SFX volume

        // Update UI text to display current volume percentage
        if (SFXVolume != null)
        {
            SFXVolume.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
