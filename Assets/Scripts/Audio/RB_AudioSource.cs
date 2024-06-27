using MANAGERS;
using UnityEngine;

public class RB_AudioSource : MonoBehaviour
{
    private AudioSource _audioSource;  // Reference to the AudioSource component
    private float _basePitch;  // Store the base pitch of the audio source

    [SerializeField] private bool _isMusic;  // Flag to indicate if this is a music audio source

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();  // Get the AudioSource component attached to this GameObject
        _basePitch = _audioSource.pitch;  // Store the initial pitch of the AudioSource

        // Destroy the GameObject if it's not music and doesn't have an audio clip assigned
        if (!_isMusic && _audioSource.clip == null)
            Destroy(gameObject);
    }

    private void Update()
    {
        // Adjust pitch based on whether the game is rewinding and the time scale
        if (RB_TimeManager.Instance && RB_TimeManager.Instance.IsRewinding)
        {
            _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, -_basePitch * Time.timeScale, 4 * Time.deltaTime);
        }
        else
        {
            _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, _basePitch * Time.timeScale, 4 * Time.deltaTime);
        }

        // Check if the audio is not playing
        if (!_audioSource.isPlaying)
        {
            // Destroy the GameObject if it's not music, doesn't loop, and is not playing
            if (!_isMusic && !_audioSource.loop)
            {
                Destroy(gameObject);
            }
            // If the pitch is negative, set the time to the end of the clip and play it
            else if (_audioSource.pitch < 0)
            {
                _audioSource.time = _audioSource.clip.length - 0.01f;
                _audioSource.Play();
            }
        }
    }

    private void OnDestroy()
    {
        // Remove the audio source from the AudioManager's list when this GameObject is destroyed
        RB_AudioManager.Instance.AudioSources.Remove(_audioSource);
    }
}
