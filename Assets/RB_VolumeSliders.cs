using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RB_VolumeSliders : MonoBehaviour{
    [SerializeField] Slider _generalSlider;
    [SerializeField] TextMeshProUGUI _generalVolume;
    [SerializeField] Slider _musicSlider;
    [SerializeField] TextMeshProUGUI _musicVolume;
    [SerializeField] Slider _SFXSlider;
    [SerializeField] TextMeshProUGUI _SFXVolume;
    [SerializeField] Button _resetButton;



    void Start() {
        _generalSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetGeneralVolume(_generalSlider.value); });
        _SFXSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetSFXVolume(_SFXSlider.value); });
        _musicSlider.onValueChanged.AddListener(delegate { RB_VolumeManager.Instance.SetMusicVolume(_musicSlider.value); });
        _resetButton.onClick.AddListener(Default);
        StartVolumes();
    }

    public void StartVolumes()
    {
        if (!PlayerPrefs.HasKey("GeneralVolume"))
        {
            _generalSlider.value = 1f;
        }
        else
        {
            _generalSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            _musicSlider.value = 1f;
        }
        else
        {
            _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            _SFXSlider.value = 1f;
        }
        else
        {
            _SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
    }

    public void Default()
    {
        _generalSlider.value = 1; _musicSlider.value = 1; _SFXSlider.value = 1;
        PlayerPrefs.SetFloat("GeneralVolume", _generalSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _SFXSlider.value);
        PlayerPrefs.Save();
    }

    // Update the text displaying the current volume percentage for each slider
    private void Update()
    {
        _generalVolume.text = (Mathf.RoundToInt(_generalSlider.value * 100)).ToString() + " %";
        _musicVolume.text = (Mathf.RoundToInt(_musicSlider.value * 100)).ToString() + " %";
        _SFXVolume.text = (Mathf.RoundToInt(_SFXSlider.value * 100)).ToString() + " %";
    }
}
