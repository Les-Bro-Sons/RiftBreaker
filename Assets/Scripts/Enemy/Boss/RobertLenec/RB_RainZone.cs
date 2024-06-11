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

    [Header("Properties (useless if MegaKnight spawn them)")]
    [SerializeField] private float _damage;
    [SerializeField] private float _knockback;

    [HideInInspector] public float DamageCooldown;
    private float _damageTimer = 0;

    public TEAMS Team = TEAMS.Ai;

    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        //_collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedObjects()[_collisionDetection.GetDetectedObjects().Count - 1]); });
        _appearTimer = 0;
        _isAppearing = true;
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        if (_isAppearing)
        {
            if (_appearTimer >= _appearingDuration)
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1);
                _isAppearing = false;
            }
            else
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, Mathf.Lerp(0, 1, _appearTimer / _appearingDuration));
                _appearTimer += Time.deltaTime;
            }
        }
        else if (_isDisappearing)
        {
            if (_appearTimer >= _disappearingDuration)
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0);
                _isDisappearing = false;
            }
            else
            {
                _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, Mathf.Lerp(1, 0, _appearTimer / _disappearingDuration));
                _appearTimer += Time.deltaTime;
            }
        }
        if (!_isAppearing && _damageTimer >= DamageCooldown) CheckForEnemies();

        if (!_isDisappearing && _lifetime <= _lifetimeTimer)
        {
            _appearTimer = 0;
            _isAppearing = false;
            _isDisappearing = true;
        }
        _damageTimer += Time.deltaTime;
        _lifetimeTimer += Time.deltaTime;
    }

    private void CheckForEnemies()
    {
        List<RB_Health> enemies = new();
        foreach (GameObject enemy in _collisionDetection.GetDetectedObjects())
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
