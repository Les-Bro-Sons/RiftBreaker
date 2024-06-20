using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AudioSource : MonoBehaviour
{
    private AudioSource _audioSource;

    private float _basePitch;

    [SerializeField] private bool _isMusic;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _basePitch = _audioSource.pitch;

        if (!_isMusic && _audioSource.clip == null)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (RB_TimeManager.Instance && RB_TimeManager.Instance.IsRewinding) //scale the pitch on rewind and timescale
        {
            _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, -_basePitch * Time.timeScale, 4 * Time.deltaTime);
        }
        else
        {
            _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, _basePitch * Time.timeScale, 4 * Time.deltaTime);
        }
       
        
        if (!_audioSource.isPlaying)
        {
            if (!_isMusic && !_audioSource.loop)
            {
                Destroy(gameObject);
            }
            else if (_audioSource.pitch < 0)
            {
                _audioSource.time = _audioSource.clip.length - 0.01f;
                _audioSource.Play();
            }
        }
    }

    private void OnDestroy()
    {
        RB_AudioManager.Instance.AudioSources.Remove(_audioSource);
    }
}
