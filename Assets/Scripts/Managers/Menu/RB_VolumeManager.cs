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

    public void Default() { 
        _generalSlider.value = 1; _musicSlider.value = 1; _SFXSlider.value = 1;

        SaveSettings();
    }

    public void SetGeneralVolume() {
        float volume = _generalSlider.value;
        _audioMixer.SetFloat("master", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume() { 
        float volume = _musicSlider.value;
        _audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
    }

    public void SetSFXVolume() {
        float volume = _SFXSlider.value;
        _audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }

    public void SaveSettings() {
        PlayerPrefs.SetFloat("GeneralVolume", _generalSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _SFXSlider.value);
        PlayerPrefs.Save();
    }

    private void Update() {
        _generalVolume.text = ((int) (_generalSlider.value*100)).ToString() + " %";
        _musicVolume.text = ((int) (_musicSlider.value*100)).ToString() + " %";
        _SFXVolume.text = ((int) (_SFXSlider.value*100)).ToString() + " %";
        
    }


}
