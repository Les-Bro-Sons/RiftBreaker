using MANAGERS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class RB_YogCinematic : MonoBehaviour
{
    private float _bloodTimer = 0;
    private bool _isBleeding = true;

    [SerializeField] private float _bloodSpawnTime = 1.25f;
    [SerializeField] private float _fastBloodSpawnTime = 0.5f;
    [SerializeField] private float _bloodOffsetAmount = 1.25f;

    [SerializeField] private GameObject _yogBlood;
    [SerializeField] private GameObject _yogDeathParticle;
    [SerializeField] private Transform _yog;
    private List<GameObject> _yogBloodParticles;


    private void Start()
    {
        RB_AudioManager.Instance.PlayMusic("End_Cinematic_Music");
    }
    private void Update()
    {
        _bloodTimer += Time.deltaTime;
        if (_bloodTimer > _bloodSpawnTime && _isBleeding)
        {
            _bloodTimer = 0;
            Vector3 offset = new Vector3(Random.Range(-_bloodOffsetAmount, _bloodOffsetAmount), Random.Range(-_bloodOffsetAmount, _bloodOffsetAmount));
            Instantiate(_yogBlood, _yog.transform.position + offset, Quaternion.identity);
            RB_AudioManager.Instance.PlaySFX("DamageSound", false, false, 0f, 1f);
        }
        
    }

    public void StopBleeding()
    {
        _isBleeding = false;
    }

    public void StartBleeding()
    {
        _isBleeding = true;
    }

    public void FastBleeding()
    {
        _bloodSpawnTime = _fastBloodSpawnTime;
    }

    public void YogDeath()
    {
        StopBleeding();
        Instantiate(_yogDeathParticle, _yog.position, Quaternion.identity);
        RB_AudioManager.Instance.PlaySFX("Explosion_Sound", false, false, 0f, 1f);
    }

    public void GoToNextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SwordSound()
    {
        RB_AudioManager.Instance.PlaySFX("Sword_End_Cinematic_Sound", false, false, 0f, 1f);
    }
}
