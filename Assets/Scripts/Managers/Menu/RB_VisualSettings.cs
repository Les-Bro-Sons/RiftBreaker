using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RB_VisualSettings : MonoBehaviour {
    [SerializeField] TMP_Dropdown _resolutionDropdown;
    [SerializeField] TMP_Dropdown _displayDropdown;


    Resolution[] _allResolutions;
    List<Resolution> _filteredResolutions = new List<Resolution>();
    RefreshRate _currentRefreshRate;

    int _oldResolutionID;
    int _currentResolutionID;
    int _currentDisplayID;

    void Start() {
        //obtiens tout les résolution disponible sur l'écran
        _allResolutions = Screen.resolutions;

        _resolutionDropdown.ClearOptions();
        
        //obtiens le taux de rafraichissment actuel de l'écran
        _currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        //Filtre les résolutions disponible pour garder le même taux de refraichissment
        for(int u = 0; u < _allResolutions.Length; u++) {
            if (_allResolutions[u].refreshRateRatio.value == _currentRefreshRate.value) {
                _filteredResolutions.Add(_allResolutions[u]);
            }
        }

        //Création du drop-down avec les résolution filtrée
        List<string> dropdownOptions = new List<string>();
        for( int u = 0; u < _filteredResolutions.Count; ++u) { 
            string resolutionOption = _filteredResolutions[u].width + " x " + _filteredResolutions[u].height;
            dropdownOptions.Add(resolutionOption);
            if (_filteredResolutions[u].width == Screen.width && _filteredResolutions[u].height == Screen.height) {
                _oldResolutionID = u;
            }
        }

        _resolutionDropdown.AddOptions(dropdownOptions);
        _resolutionDropdown.RefreshShownValue();

        StartResolution();
        ApplyResolution();
        StartDisplayMode();
        ApplyDisplayMode();
    }

    public void SetResolution(int  resolutionID) {
        _currentResolutionID = resolutionID;
    }

    public void ApplyResolution() {
        Resolution resolution = _filteredResolutions[_currentResolutionID];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, _currentRefreshRate);
        PlayerPrefs.SetInt("ResolutionID", _currentResolutionID);
        PlayerPrefs.Save();
    }

    public void StartResolution() {
        if (!PlayerPrefs.HasKey("ResolutionID")){
            _currentResolutionID = _oldResolutionID;
        }
        else {
            _currentResolutionID = PlayerPrefs.GetInt("ResolutionID");
        }
        _resolutionDropdown.value = _currentResolutionID;
    }

    public void SetDisplayMode(int displayID) {
        _currentDisplayID = displayID;
    }

    public void ApplyDisplayMode() {
        switch (_currentDisplayID) {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
        PlayerPrefs.SetInt("DisplayID", _currentDisplayID);
        PlayerPrefs.Save();
    }


    public void StartDisplayMode(){
        if (!PlayerPrefs.HasKey("DisplayID")){
            _currentDisplayID = 0;
        }
        else {
            _currentDisplayID = PlayerPrefs.GetInt("DisplayID");
        }
        _displayDropdown.value = _currentDisplayID;
    }

    public void Default() {
        _currentDisplayID = 0;
        _displayDropdown.value = _currentDisplayID;

        ApplyDisplayMode();

        _currentResolutionID = _oldResolutionID;
        _resolutionDropdown.value = _currentResolutionID;
        ApplyResolution();
    }
}
