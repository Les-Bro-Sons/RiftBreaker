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
        // Get all available screen resolutions
        _allResolutions = Screen.resolutions;

        _resolutionDropdown.ClearOptions();

        // Get the current screen refresh rate
        _currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        // Filter available resolutions to match the current refresh rate
        for (int u = 0; u < _allResolutions.Length; u++) {
            if (_allResolutions[u].refreshRateRatio.value == _currentRefreshRate.value) {
                _filteredResolutions.Add(_allResolutions[u]);
            }
        }

        // Create the dropdown list with filtered resolutions
        List<string> dropdownOptions = new List<string>();
        for (int u = 0; u < _filteredResolutions.Count; ++u) { 
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

    public void SetResolution(int resolutionID) {
        _currentResolutionID = resolutionID;
    }

    // Apply the current resolution settings
    public void ApplyResolution() {
        Debug.Log(_currentResolutionID);
        Resolution resolution = _filteredResolutions[_currentResolutionID];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, _currentRefreshRate);
        PlayerPrefs.SetInt("ResolutionID", _currentResolutionID);
        PlayerPrefs.Save();
    }

    // Initialize the resolution settings
    public void StartResolution() {
        if (!PlayerPrefs.HasKey("ResolutionID")) {
            _currentResolutionID = _oldResolutionID;
        } else {
            _currentResolutionID = PlayerPrefs.GetInt("ResolutionID");
        }
        _resolutionDropdown.value = _currentResolutionID;
    }

    public void SetDisplayMode(int displayID) {
        _currentDisplayID = displayID;
    }

    // Apply the current display mode settings
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

    // Initialize the display mode settings
    public void StartDisplayMode() {
        if (!PlayerPrefs.HasKey("DisplayID")) {
            _currentDisplayID = 0;
        } else {
            _currentDisplayID = PlayerPrefs.GetInt("DisplayID");
        }
        _displayDropdown.value = _currentDisplayID;
    }

    // Set display mode and resolution mode to default
    public void Default() {
        // Reset display mode to default
        _currentDisplayID = 0;
        _displayDropdown.value = _currentDisplayID;
        ApplyDisplayMode();
        PlayerPrefs.SetInt("DisplayID", _currentDisplayID); // Save default display mode

        // Reset resolution to default
        _currentResolutionID = _oldResolutionID;
        _resolutionDropdown.value = _currentResolutionID;
        ApplyResolution();
        PlayerPrefs.SetInt("ResolutionID", _currentResolutionID); // Save default resolution

        PlayerPrefs.Save();
    }
}
