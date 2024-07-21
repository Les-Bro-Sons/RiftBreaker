using MANAGERS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_YogCinematic : MonoBehaviour
{
    private float _bloodTimer = 0;   // Timer for blood particle spawning
    private bool _isBleeding = true;   // Flag indicating if Yog is bleeding

    [SerializeField] private float _bloodSpawnTime = 1.25f;   // Time interval between blood spawns
    [SerializeField] private float _fastBloodSpawnTime = 0.5f;   // Reduced time interval for faster blood spawns
    [SerializeField] private float _bloodOffsetAmount = 1.25f;   // Maximum offset distance for blood spawn positions

    [SerializeField] private GameObject _yogBlood;   // Prefab for Yog's blood particle effect
    [SerializeField] private GameObject _yogDeathParticle;   // Particle effect for Yog's death
    [SerializeField] private Transform _yog;   // Transform of Yog object
    private List<GameObject> _yogBloodParticles;   // List to store spawned blood particle objects

    private void Start()
    {
        RB_AudioManager.Instance.PlayMusic("End_Cinematic_Music");   // Play end cinematic music
    }

    private void Update()
    {
        _bloodTimer += Time.deltaTime;
        if (_bloodTimer > _bloodSpawnTime && _isBleeding)
        {
            _bloodTimer = 0;
            Vector3 offset = new Vector3(Random.Range(-_bloodOffsetAmount, _bloodOffsetAmount), Random.Range(-_bloodOffsetAmount, _bloodOffsetAmount));
            Instantiate(_yogBlood, _yog.transform.position + offset, Quaternion.identity);   // Spawn blood particles with random offset
            RB_AudioManager.Instance.PlaySFX("DamageSound", false, false, 0f, 1f);   // Play damage sound effect
        }
    }

    /// <summary>
    /// Stops Yog from bleeding.
    /// </summary>
    public void StopBleeding()
    {
        _isBleeding = false;
    }

    /// <summary>
    /// Starts Yog's bleeding.
    /// </summary>
    public void StartBleeding()
    {
        _isBleeding = true;
    }

    /// <summary>
    /// Increases blood particle spawn rate for faster bleeding effect.
    /// </summary>
    public void FastBleeding()
    {
        _bloodSpawnTime = _fastBloodSpawnTime;
    }

    /// <summary>
    /// Initiates Yog's death sequence, stops bleeding, spawns death particle, and plays explosion sound.
    /// </summary>
    public void YogDeath()
    {
        StopBleeding();
        Instantiate(_yogDeathParticle, _yog.position, Quaternion.identity);   // Spawn death particle effect at Yog's position
        RB_AudioManager.Instance.PlaySFX("Explosion_Sound", false, false, 0f, 1f);   // Play explosion sound effect
    }

    /// <summary>
    /// Initiates transition to the next scene. (The end's Credits)
    /// </summary>
    public void GoToNextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, SceneManager.GetActiveScene().buildIndex + 1);   // Transition to the next scene
    }

    /// <summary>
    /// Plays the sound effect of a sword at the end of the cinematic.
    /// </summary>
    public void SwordSound()
    {
        RB_AudioManager.Instance.PlaySFX("Sword_End_Cinematic_Sound", false, false, 0f, 1f);   // Play sword sound effect
    }
}
