using MANAGERS;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RB_RainZone : MonoBehaviour
{

    public RB_RobertLenec Sylvashot;
    private RB_CollisionDetection _collisionDetection;
    

    [Header("Main Properties")]
    [SerializeField] private float _lifetime = 1;
    [SerializeField] private float _lifetimeTimer = 0;
    [SerializeField] private float _appearingDuration = 0.2f;
    [SerializeField] private float _disappearingDuration = 0.2f;
    [SerializeField] private bool _isAppearing = false;
    [SerializeField] private bool _isDisappearing = false;
    [SerializeField] private SpriteRenderer _sprite;
    private float _appearTimer = 0;
    private float _appearAlpha = 0;


    [HideInInspector] public float DamageCooldown;
    private float _damageTimer = 0;

    public TEAMS Team = TEAMS.Ai;

    [SerializeField] private ParticleSystem _particles;

    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        //_collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedObjects()[_collisionDetection.GetDetectedObjects().Count - 1]); });
        _appearTimer = 0;
        _isAppearing = true;
        _appearAlpha = _sprite.color.a;
    }

    private void Start()
    {
        RB_AudioManager.Instance.PlaySFX("Rain_Sound", transform.position, true, 0f, 1f);
        ParticleSystem.ShapeModule partShape = _particles.shape;
        partShape.radius = transform.lossyScale.x / 2f; //set the radius of particles
    }

    private void Update()
    {
        if (_isAppearing)
        {
            if (_appearTimer >= _appearingDuration) //end of appearing
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, _appearAlpha);
                _isAppearing = false;
            }
            else //is appearing
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, Mathf.Lerp(0, _appearAlpha, _appearTimer / _appearingDuration));
                _appearTimer += Time.deltaTime;
            }
        }
        else if (_isDisappearing)
        {
            if (_appearTimer >= _disappearingDuration) //end of disappearing
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0);
                _isDisappearing = false;
            }
            else //is disappearing
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, Mathf.Lerp(_appearAlpha, 0, _appearTimer / _disappearingDuration));
                _appearTimer += Time.deltaTime;
            }
        }
        if (!_isAppearing && _damageTimer >= DamageCooldown) CheckForEnemies(); //collision check

        if (!_isDisappearing && _lifetime <= _lifetimeTimer) //activate disappearing
        {
            _appearTimer = 0;
            _isAppearing = false;
            _isDisappearing = true;
        }
        if (_lifetimeTimer >= _lifetime) //lifetime check
        {
            _particles.transform.parent = null;
            _particles.Stop();
            _particles.transform.localScale = Vector3.one;
            Destroy(_particles, _particles.main.duration);
            RB_AudioManager.Instance.StopSFXByClip("Rain_Sound");
            Destroy(gameObject);
            return;
        }
        _damageTimer += Time.deltaTime;
        _lifetimeTimer += Time.deltaTime;
    }

    private void CheckForEnemies()
    {
        List<RB_Health> enemies = new();
        foreach (GameObject enemy in _collisionDetection.GetDetectedEntity())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth) && enemyHealth.Team != Team)
            {
                enemies.Add(enemyHealth);
            }
        }

        if (Sylvashot && enemies.Count > 0)
        {
            _damageTimer = 0;
            Sylvashot.ApplyRainZoneDamage(enemies);
        }
    }

    private void EnemyEntered(GameObject enemy)
    {
        List<RB_Health> enemies = new();
        if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
        {
            if (Sylvashot)
            {
                enemies.Add(enemyHealth);
                Sylvashot.ApplyRainZoneDamage(enemies);
            }
        }

        
    }


}
