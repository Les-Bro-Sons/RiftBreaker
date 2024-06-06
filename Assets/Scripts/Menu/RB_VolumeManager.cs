using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RB_VolumeManager : MonoBehaviour{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _generalSlider;
    [SerializeField] TextMeshProUGUI _generalVolume;
    [SerializeField] Slider _musicSlider;
    [SerializeField] TextMeshProUGUI _musicVolume;
    [SerializeField] Slider _SFXSlider;
    [SerializeField] TextMeshProUGUI _SFXVolume;

    private void Start() {
        StartVolumes();
        SetGeneralVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    // Initialize volume sliders with saved values or defaults
    public void StartVolumes() { 
         if (!PlayerPrefs.HasKey("GeneralVolume")){
            _generalSlider.value = 1f;
        }
        else {
            _generalSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume")){
            _musicSlider.value = 1f;
        }
        else {
            _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }    
        if (!PlayerPrefs.HasKey("SFXVolume")){
            _SFXSlider.value = 1f;
        }
        else {
            _SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }    
    }

    // Reset all sliders to default values and save settings
    public void Default() { 
        _generalSlider.value = 1; _musicSlider.value = 1; _SFXSlider.value = 1;
        SaveSettings();
    }

    // Set the general volume based on the music slider value
    public void SetGeneralVolume() {
        float volume = _generalSlider.value;
        _audioMixer.SetFloat("master", Mathf.Log10(volume) * 20);
    }

    // Set the music volume based on the music slider value
    public void SetMusicVolume() { 
        float volume = _musicSlider.value;
        _audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
    }

    // Set the SFX volume based on the music slider value
    public void SetSFXVolume() {
        float volume = _SFXSlider.value;
        _audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }

    // Save the current slider values to PlayerPrefs
    public void SaveSettings() {
        PlayerPrefs.SetFloat("GeneralVolume", _generalSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _SFXSlider.value);
        PlayerPrefs.Save();
    }

    // Update the text displaying the current volume percentage for each slider
    private void Update() {
        _generalVolume.text = (Mathf.RoundToInt(_generalSlider.value*100)).ToString() + " %";
        _musicVolume.text = (Mathf.RoundToInt (_musicSlider.value*100)).ToString() + " %";
        _SFXVolume.text = (Mathf.RoundToInt(_SFXSlider.value*100)).ToString() + " %";
        
    }
}
