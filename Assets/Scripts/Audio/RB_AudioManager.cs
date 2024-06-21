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

		public List<AudioSource> AudioSources = new List<AudioSource>();
		public AudioSource SfxSource;

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


        public AudioSource PlaySFX(string nameClip, Vector3 desiredPosition, bool loop, float pitchVariation = 0, float volume = 1,MIXERNAME mixer = MIXERNAME.SFX)
        {

            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");
            GameObject _audioSourceObject = Instantiate(_prefabAudioSource, desiredPosition, quaternion.identity);
			AudioSource _audioSource = _audioSourceObject.GetComponent<AudioSource>();

            _audioSource.pitch += Random.Range(-pitchVariation, pitchVariation);
            _audioSource.volume = volume;
            _audioSource.spatialBlend = 1;
            _audioSource.loop = loop;
            _audioSource.enabled = true;

            // Assignez le groupe à l'AudioSource
            _audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];
			if (_sfxClip != null)
			{
                _audioSource.clip = _sfxClip;
                _audioSource.Play();
				//Destroy(_audioSource,_sfxClip.length);
				return _audioSource;
			}
			else
			{
				Debug.LogWarning("SFX clip not found: " + nameClip);
				Destroy(_audioSourceObject);
				return null;
            }
		}

        
		public AudioSource PlaySFX(string nameClip, Transform desiredParent, bool loop, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX)
		{
			AudioSource audioSource = PlaySFX(nameClip, desiredParent.position, loop, pitchVariation, volume, mixer);
			audioSource.transform.parent = desiredParent;
			return audioSource;
		}
		


        public void StopSFX() 
		{
			//SfxSource?.Stop();
			Debug.LogWarning("doesn't work");
		}
	}
}
