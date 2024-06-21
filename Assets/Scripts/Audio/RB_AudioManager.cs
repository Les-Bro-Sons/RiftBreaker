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
		public static RB_AudioManager Instance;
		public GameObject _prefabAudioSource;
		
		[Header("Audio")]
		[SerializeField] private string _nameMusicGame = "SheathingSound";

		[Header("Mixer")]
		[SerializeField] private AudioMixer _mixer;
		[SerializeField] private AudioSource _musicSource; public AudioSource MusicSource { get { return _musicSource; } }
		
		public const string ROOT_PATH = "Audio";

		public const string MASTER_KEY = "masterVolume";
		public const string MUSIC_KEY = "musicVolume";
		public const string SFX_KEY = "sfxVolume";

		public AudioSource SfxSource;
		public List<AudioSource> AudioSources = new List<AudioSource>();

		private void Awake()
		{
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

		private void Start()
		{
			PlayMusic("zik_hugoval_final");
		}

		public void PlayMusic(string nameClip)
		{
			if (_musicSource.isPlaying)
			{
				_musicSource.Stop();
			}
			AudioClip _musicClip = Resources.Load<AudioClip>($"{ROOT_PATH}/Music/{nameClip}");
			if (_musicClip != null)
			{
				_musicSource.clip = _musicClip;
				if (_musicSource.loop != true)
					_musicSource.loop = true;

				_musicSource.Play();
			}
			else
			{
				Debug.LogWarning("Music clip not found: " + nameClip);
			}
		}


		public AudioSource PlaySFX(string nameClip,Vector3 desiredPosition, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX) {

            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");
            GameObject _audioSource = Instantiate(_prefabAudioSource, desiredPosition, quaternion.identity);
            SfxSource = _audioSource.GetComponent<AudioSource>();
			RB_AudioSource _audioScript = _audioSource.GetComponent<RB_AudioSource>();

            SfxSource.pitch += Random.Range(-pitchVariation, pitchVariation);
            SfxSource.volume = volume;
            SfxSource.spatialBlend = 1;
            SfxSource.loop = false;
            SfxSource.enabled = true;

            // Assignez le groupe à l'AudioSource
            SfxSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];
			
			if (_sfxClip != null)
			{
                SfxSource.clip = _sfxClip;
                SfxSource.Play();
				//Destroy(_audioSource,_sfxClip.length);
				return SfxSource;
			}
			else
			{
				Debug.LogWarning("SFX clip not found: " + nameClip);
				Destroy(_audioSource);
				return null;
            }
		}

        public AudioSource PlaySFXOnLoop(string nameClip, Vector3 desiredPosition, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX)
        {

            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");
            GameObject _audioSource = Instantiate(_prefabAudioSource, desiredPosition, quaternion.identity);
            SfxSource = _audioSource.GetComponent<AudioSource>();
            RB_AudioSource _audioScript = _audioSource.GetComponent<RB_AudioSource>();

            SfxSource.pitch += Random.Range(-pitchVariation, pitchVariation);
            SfxSource.volume = volume;
            SfxSource.spatialBlend = 1;
            SfxSource.loop = true;
            SfxSource.enabled = true;

            // Assignez le groupe à l'AudioSource
            SfxSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];

            if (_sfxClip != null)
            {
                SfxSource.clip = _sfxClip;
                SfxSource.Play();
                //Destroy(_audioSource,_sfxClip.length);
                return SfxSource;
            }
            else
            {
                Debug.LogWarning("SFX clip not found: " + nameClip);
                Destroy(_audioSource);
                return null;
            }
        }
        public void StopSFX() 
		{
			SfxSource?.Stop();
		}
	}
}
