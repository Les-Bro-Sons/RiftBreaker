using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RB_VolumeManager : MonoBehaviour{
    [SerializeField] AudioMixer _audioMixer;
    public Slider GeneralSlider;
    public TextMeshProUGUI GeneralVolume;
    public Slider MusicSlider;
    public TextMeshProUGUI MusicVolume;
    public Slider SFXSlider;
    public TextMeshProUGUI SFXVolume;
    public Button ResetButton;

    public static RB_VolumeManager Instance;

    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start() {
        //StartVolumes();
        //SetGeneralVolume();
        //SetMusicVolume();
        //SetSFXVolume();
    }

    // Initialize volume sliders with saved values or defaults
    public void StartVolumes() { 
         if (!PlayerPrefs.HasKey("GeneralVolume")){
            GeneralSlider.value = 1f;
        }
        else {
            GeneralSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume")){
            MusicSlider.value = 1f;
        }
        else {
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }    
        if (!PlayerPrefs.HasKey("SFXVolume")){
            SFXSlider.value = 1f;
        }
        else {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }    
    }

    // Reset all sliders to default values and save settings
    public void Default() {
        GeneralSlider.value = 1; MusicSlider.value = 1; SFXSlider.value = 1;
        PlayerPrefs.SetFloat("GeneralVolume", GeneralSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.Save();
    }

    // Set the general volume based on the music slider value
    public void SetGeneralVolume(float volume) {
        //float volume = GeneralSlider.value;
        print(volume);
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Save the current slider values to PlayerPrefs
        PlayerPrefs.SetFloat("GeneralVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the music volume based on the music slider value
    public void SetMusicVolume(float volume) { 
        //float volume = MusicSlider.value;
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume)*20);

        // Save the current slider values to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the SFX volume based on the music slider value
    public void SetSFXVolume(float volume) {
        //float volume = SFXSlider.value;
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);

        // Save the current slider values to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }




}
