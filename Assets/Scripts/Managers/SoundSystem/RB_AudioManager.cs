using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_AudioManager : MonoBehaviour {
    public static RB_AudioManager Instance;

    [Header("~~~~~~~~ Audio Source ~~~~~~~~")]
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _SFXSource;


    private void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(this); }
    }

    public void PlaySFX(AudioClip clip) {

    }

    public void PlayRandomSFX(AudioClip[] clip) {

    }

    public void PlayMainMusic(AudioClip clip) { 

    }

    public void StopSFX() {

    }

}