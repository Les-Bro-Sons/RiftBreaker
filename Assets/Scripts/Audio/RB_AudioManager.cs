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

		private Coroutine _musicSwitchCoroutines;

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
            AudioClip _musicClip = Resources.Load<AudioClip>($"{ROOT_PATH}/Music/{nameClip}");
			if (_musicSource.clip == _musicClip) return;
			


			if (_musicClip != null)
			{
				if (_musicSource.loop != true)
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
                Debug.LogWarning("Music clip not found: " + nameClip);
			}
		}

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


        public AudioSource PlaySFX(string nameClip, Vector3 desiredPosition, bool loop = false, float pitchVariation = 0, float volume = 1,MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
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

            // Assignez le groupe à l'AudioSource
            _audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(mixer.ToString())[0];
			if (_sfxClip != null)
			{
                _audioSource.clip = _sfxClip;
                _audioSource.Play();
                //Destroy(_audioSource,_sfxClip.length);
                AudioSources.Add(_audioSource);
                return _audioSource;
			}
			else
			{
				Debug.LogWarning("SFX clip not found: " + nameClip);
				Destroy(_audioSourceObject);
				return null;
            }
		}

        
		public AudioSource PlaySFX(string nameClip, Transform desiredParent, bool loop = false, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
		{
			AudioSource audioSource = PlaySFX(nameClip, desiredParent.position, loop, pitchVariation, volume, mixer, pitchOffset);
			audioSource.transform.parent = desiredParent;
			return audioSource;
		}

        public AudioSource PlaySFX(string nameClip, bool localised = false, bool loop = false, float pitchVariation = 0, float volume = 1, MIXERNAME mixer = MIXERNAME.SFX, float pitchOffset = 0)
        {
            AudioSource audioSource = PlaySFX(nameClip, Vector3.zero, loop, pitchVariation, volume, mixer, pitchOffset);
			audioSource.spatialize = false;
			audioSource.spatialBlend = 0;
            return audioSource;
        }

		public void StopSFXByClip(string nameClip)
		{
            AudioClip _sfxClip = Resources.Load<AudioClip>($"{ROOT_PATH}/SFX/{nameClip}");

            if (_sfxClip == null)
                return;

            foreach (AudioSource audioSource in AudioSources)
            {
                if (audioSource.clip == _sfxClip)
                {
                    audioSource.Stop();
                }
            }
        }

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
					clipPlaying += 1;
				}
			}

			return clipPlaying;
		}

        public void StopSFX() 
		{
			//SfxSource?.Stop();
			Debug.LogWarning("doesn't work");
		}
	}
}
