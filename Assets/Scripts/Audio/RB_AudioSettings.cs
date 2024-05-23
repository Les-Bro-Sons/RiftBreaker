using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MANAGERS
{
	public class RB_AudioSettings : MonoBehaviour
	{
		public static RB_AudioSettings Instance;

		[SerializeField] private AudioMixer _mixer;
		[SerializeField] private Slider _masterSlider;
		[SerializeField] private Slider _musicSlider;
		[SerializeField] private Slider _sfxSlider;

		public const string MIXER_MASTER = "MasterVolume";
		public const string MIXER_MUSIC = "MusicVolume";
		public const string MIXER_SFX = "SFXVolume";

		private float _defaultMasterVolume = 1f;    // slider 100%
		private float _defaultMusicVolume = 0.8f;   // slider 80%
		private float _defaultSFXVolume = 0.8f;     // slider 80%

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
				DestroyImmediate(gameObject);

			_masterSlider.onValueChanged.AddListener(SetMasterVolume);
			_musicSlider.onValueChanged.AddListener(SetMusicVolume);
			_sfxSlider.onValueChanged.AddListener(SetSFXVolume);
		}

		private void Start()
		{
			InitAudio();
		}

		private void OnDisable()
		{
			PlayerPrefs.SetFloat(RB_AudioManager.MASTER_KEY, _masterSlider.value);
			PlayerPrefs.SetFloat(RB_AudioManager.MUSIC_KEY, _musicSlider.value);
			PlayerPrefs.SetFloat(RB_AudioManager.SFX_KEY, _sfxSlider.value);
		}

		public void InitAudio()
		{
			//print(_masterSlider);
			_masterSlider.value = PlayerPrefs.GetFloat(RB_AudioManager.MASTER_KEY, 1f);
			_musicSlider.value = PlayerPrefs.GetFloat(RB_AudioManager.MUSIC_KEY, 1f);
			_sfxSlider.value = PlayerPrefs.GetFloat(RB_AudioManager.SFX_KEY, 1f);
		}

		private void SetMasterVolume(float value)
		{
			float volume = Mathf.Lerp(-80f, 0f, value);
			_mixer.SetFloat(MIXER_MASTER, volume);
			PlayerPrefs.SetFloat(RB_AudioManager.MASTER_KEY, _masterSlider.value);
		}

		private void SetMusicVolume(float value)
		{
			float volume = Mathf.Lerp(-80f, 0f, value);
			_mixer.SetFloat(MIXER_MUSIC, volume);
			PlayerPrefs.SetFloat(RB_AudioManager.MUSIC_KEY, _musicSlider.value);
		}

		private void SetSFXVolume(float value)
		{
			float volume = Mathf.Lerp(-80f, 0f, value);
			_mixer.SetFloat(MIXER_SFX, volume);
			PlayerPrefs.SetFloat(RB_AudioManager.SFX_KEY, _sfxSlider.value);
		}

		//Besoin d'un GameManager

		//public void ResetVolumes()
		//{
		//	if (!RB_GameManager.Instance.isAnyButtonClicked)
		//	{
		//		RB_GameManager.Instance.isAnyButtonClicked = true;
		//		StopAllCoroutines();

		//		StartCoroutine(ResetSlidersSmoothly());
		//	}
		//}

		private IEnumerator ResetSlidersSmoothly()
		{
			float elapsedTime = 0f;
			float duration = 0.3f;
			float startMasterVolume = _masterSlider.value;
			float startMusicVolume = _musicSlider.value;
			float startSFXVolume = _sfxSlider.value;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = Mathf.Clamp01(elapsedTime / duration);

				_masterSlider.value = Mathf.Lerp(startMasterVolume, _defaultMasterVolume, t);
				_musicSlider.value = Mathf.Lerp(startMusicVolume, _defaultMusicVolume, t);
				_sfxSlider.value = Mathf.Lerp(startSFXVolume, _defaultSFXVolume, t);

				SetMasterVolume(_masterSlider.value);
				SetMusicVolume(_musicSlider.value);
				SetSFXVolume(_sfxSlider.value);

				yield return null;
			}

			_masterSlider.value = _defaultMasterVolume;
			_musicSlider.value = _defaultMusicVolume;
			_sfxSlider.value = _defaultSFXVolume;

			SetMasterVolume(_defaultMasterVolume);
			SetMusicVolume(_defaultMusicVolume);
			SetSFXVolume(_defaultSFXVolume);

		//	RB_GameManager.Instance.isAnyButtonClicked = !RB_GameManager.Instance.isAnyButtonClicked;
		}
	}
}