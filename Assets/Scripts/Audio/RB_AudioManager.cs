using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace MANAGERS
{
    public class RB_AudioManager : MonoBehaviour
    {
        public static RB_AudioManager Instance;  // Singleton instance
        public GameObject _prefabAudioSource;   // Prefab for audio sources

        [Header("Audio")]
        [SerializeField] private string _nameMusicGame = "SheathingSound"; // Name of the game music

        [Header("Mixer")]
        [SerializeField] private AudioMixer _mixer;          // Audio mixer
        [SerializeField] private AudioSource _musicSource;   // Audio source for music
        public AudioSource MusicSource { get { return _musicSource; } }

        public const string ROOT_PATH = "Audio";  // Root path for audio files

        // Mixer keys for volumes
        public const string MASTER_KEY = "masterVolume";
        public const string MUSIC_KEY = "musicVolume";
        public const string SFX_KEY = "sfxVolume";

        public List<AudioSource> AudioSources = new List<AudioSource>(); // List of audio sources
        public AudioSource SfxSource;  // Audio source for sound effects

        private Coroutine _musicSwitchCoroutines;  // Coroutine for music switching

        private void Awake()
        {
            // Ensure singleton instance
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }


        /// <summary>
        /// Plays the specified music track.
        /// </summary>
        /// <param name="nameClip">The name of the music clip to play.</param>
        public void PlayMusic(string nameClip)
        {
            AudioClip _musicClip = Resources.Load<AudioClip>($"{ROOT_PATH}/Music/{nameClip}");
            if (_musicSource.clip == _musicClip) return;  // Return if the music is already playing

            _musicSource.spatialBlend = 0;  // Set spatial blend to 0 for 2D sound

            if (_musicClip != null)
            {
                if (!_musicSource.loop)
                    _musicSource.loop = true;

                if (_musicSource.isPlaying)
                {
                    if (_musicSwitchCoroutines != null) StopCoroutine(_musicSwitchCoroutines);
                    _musicSwitchCoroutines = StartCoroutine(ReplaceMusic(_musicClip));
                }
                else
                {
                    if (_musicSwitchCoroutines != null) StopCoroutine(_musicSwitchCoroutines);
                    _musicSwitchCoroutines = StartCoroutine(ReplaceMusic(_musicClip, 1, false));
                }
            }
            else
            {
                if (_musicSwitchCoroutines != null) StopCoroutine(_musicSwitchCoroutines);
                _musicSwitchCoroutines = StartCoroutine(ReplaceMusic(_musicClip, 1, false));
                Debug.LogWarning("Music clip not found: " + nameClip);  // Log a warning if the clip is not found
            }
        }

        /// <summary>
        /// Coroutine to replace the current music with a new track, optionally fading out and in.
        /// </summary>
        /// <param name="clip">The new music clip.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <param name="fadeOut">Whether to fade out the current track.</param>
        /// <returns>IEnumerator for the coroutine.</returns>
        private IEnumerator ReplaceMusic(AudioClip clip, float duration = 1, bool fadeOut = true)
        {
            float timer = 0;
            float fadeOutDuration = (duration / 2);
            float fadeInDuration = (duration / 2);

            if (fadeOut)
            {
                while (timer < fadeOutDuration)
                {
                    _musicSource.volume = Mathf.Lerp(0, 1, timer / fadeOutDuration);
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            _musicSource.volume = 0;
            timer = 0;
            _musicSource.clip = clip;
            if (!_musicSource.isPlaying) _musicSource.Play();

            while (timer < fadeInDuration)
            {
                _musicSource.volume = Mathf.Lerp(1, 0, timer / fadeInDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            _musicSource.volume = 1;

            yield return null;
        }

        /// <summary>
        /// Plays a sound effect at a specific position.
        /// </summary>
        /// <param name="nameClip">The name of the sound effect clip.</param>
        /// <param name="desiredPosition">The position to play the sound effect.</param>
        /// <param name="loop">Whether the sound effect should loop.</param>
        /// <param name="pitchVariation">The amount of pitch variation.</param>
        /// <param name="volume">The volume of the sound effect.</param>
        /// <param name="mixer">The audio mixer group for the sound effect.</param>
        /// <param name="pitchOffset">The pitch offset for the sound effect.</param>
        /// <returns>The audio source playing the sound effect.</returns>
        public AudioSource PlaySFX(string nameClip, Vector3 desiredPosition, bool loop = false, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
        {
            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");
            GameObject _audioSourceObject = Instantiate(_prefabAudioSource, desiredPosition, quaternion.identity);
            AudioSource _audioSource = _audioSourceObject.GetComponent<AudioSource>();

            _audioSource.pitch += Random.Range(-pitchVariation, pitchVariation);
            _audioSource.pitch = Mathf.Clamp(_audioSource.pitch + pitchOffset, 0, int.MaxValue);
            _audioSource.volume = volume;
            _audioSource.spatialBlend = 1;
            _audioSource.loop = loop;
            _audioSource.enabled = true;

            // Assign the mixer group to the audio source
            _audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];
            if (_sfxClip != null)
            {
                _audioSource.clip = _sfxClip;
                _audioSource.Play();
                AudioSources.Add(_audioSource);  // Add to the list of audio sources
                return _audioSource;
            }
            else
            {
                Debug.LogWarning("SFX clip not found: " + nameClip);  // Log a warning if the clip is not found
                Destroy(_audioSourceObject);
                return null;
            }
        }

        /// <summary>
        /// Plays a sound effect at the position of a specified transform.
        /// </summary>
        /// <param name="nameClip">The name of the sound effect clip.</param>
        /// <param name="desiredParent">The transform at whose position the sound effect should play.</param>
        /// <param name="loop">Whether the sound effect should loop.</param>
        /// <param name="pitchVariation">The amount of pitch variation.</param>
        /// <param name="volume">The volume of the sound effect.</param>
        /// <param name="mixer">The audio mixer group for the sound effect.</param>
        /// <param name="pitchOffset">The pitch offset for the sound effect.</param>
        /// <returns>The audio source playing the sound effect.</returns>
        public AudioSource PlaySFX(string nameClip, Transform desiredParent, bool loop = false, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
        {
            AudioSource audioSource = PlaySFX(nameClip, desiredParent.position, loop, pitchVariation, volume, mixer, pitchOffset);
            audioSource.transform.parent = desiredParent;  // Parent the audio source to the desired transform
            return audioSource;
        }

        /// <summary>
        /// Plays a sound effect, optionally setting it to be non-localized.
        /// </summary>
        /// <param name="nameClip">The name of the sound effect clip.</param>
        /// <param name="localised">Whether the sound effect should be localized.</param>
        /// <param name="loop">Whether the sound effect should loop.</param>
        /// <param name="pitchVariation">The amount of pitch variation.</param>
        /// <param name="volume">The volume of the sound effect.</param>
        /// <param name="mixer">The audio mixer group for the sound effect.</param>
        /// <param name="pitchOffset">The pitch offset for the sound effect.</param>
        /// <returns>The audio source playing the sound effect.</returns>
        public AudioSource PlaySFX(string nameClip, bool localised = false, bool loop = false, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
        {
            AudioSource audioSource = PlaySFX(nameClip, Vector3.zero, loop, pitchVariation, volume, mixer, pitchOffset);
            audioSource.spatialize = false;
            audioSource.spatialBlend = 0;  // Set spatial blend to 0 for 2D sound
            return audioSource;
        }

        /// <summary>
        /// Stops the sound effects that match the specified clip name.
        /// </summary>
        /// <param name="nameClip">The name of the sound effect clip to stop.</param>
        public void StopSFXByClip(string nameClip)
        {
            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");

            if (_sfxClip == null)
                return;

            foreach (AudioSource audioSource in AudioSources)
            {
                if (audioSource.clip.name == _sfxClip.name)
                {
                    audioSource.Stop();  // Stop the audio source if it matches the clip name
                }
            }
        }

        /// <summary>
        /// Gets the count of audio sources playing the specified clip.
        /// </summary>
        /// <param name="nameClip">The name of the clip.</param>
        /// <returns>The number of audio sources playing the clip.</returns>
        public int ClipPlayingCount(string nameClip)
        {
            int clipPlaying = 0;
            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");

            if (_sfxClip == null)
                return 0;

            foreach (AudioSource audioSource in AudioSources)
            {
                if (audioSource.clip == _sfxClip)
                {
                    clipPlaying += 1;  // Count the number of audio sources playing the clip
                }
            }

            return clipPlaying;
        }

        /// <summary>
        /// Stops the current music.
        /// </summary>
        public void StopMusic()
        {
            _musicSource.Stop();  // Stop the music source
        }


    }
}
