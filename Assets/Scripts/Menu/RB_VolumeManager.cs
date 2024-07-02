using UnityEngine;
using UnityEngine.Audio;

public class RB_VolumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer; // Reference to the AudioMixer to control audio levels

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



    // Set the general volume based on the slider value
    public void SetGeneralVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for general volume

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("GeneralVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the music volume based on the slider value
    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for music volume

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Set the SFX volume based on the slider value
    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // Set AudioMixer parameter for SFX volume

        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
