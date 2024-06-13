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

		
		[Header("Animation")]
		public Animator AnimatorSetting;
		public GameObject SettingsCanvas;


		public const string ROOT_PATH = "Audio";

		public const string MASTER_KEY = "masterVolume";
		public const string MUSIC_KEY = "musicVolume";
		public const string SFX_KEY = "sfxVolume";

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else


				DestroyImmediate(gameObject);

			//LoadVolume();
		}
		/*
����public void EatSFX()
����{
������AudioClip clip = _eatClips[Random.Range(0, _eatClips.Count)];

������_eatSource.PlayOneShot(clip);
����}
����*/

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
			
			GameObject _audioSource = Instantiate(_prefabAudioSource, desiredPosition, quaternion.identity);
			
			AudioSource sfxSource = _audioSource.GetComponent<AudioSource>();
			
			AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");
			
			sfxSource.pitch += Random.Range(-pitchVariation, pitchVariation);
			
			sfxSource.volume = volume;
			
			// Assignez le groupe à l'AudioSource
			sfxSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];
			
			if (_sfxClip != null)
			{
				sfxSource.clip = _sfxClip;
				sfxSource.Play();
				Destroy(_audioSource,_sfxClip.length);
				return sfxSource;
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
			Debug.LogWarning("doesn't work");
		}
		
		//private void LoadVolume() // Volume saved in AudioSettings.cs
		//{
		//	float masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
		//	float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
		//	float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

		//	_mixer.SetFloat(RB_AudioSettings.MIXER_MASTER, Mathf.Log10(masterVolume) * 20);
		//	_mixer.SetFloat(RB_AudioSettings.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
		//	_mixer.SetFloat(RB_AudioSettings.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
		//}


		//	public void ToggleSettingsButton()
		//	{
		//		RB_GameManager.Instance.ToggleSettingsButton();
		//	}

		//	public void EventDiscordLink(Animator animator)
		//	{
		//		RB_GameManager.Instance.EventOpenDiscord(animator);
		//	}
	}
}
