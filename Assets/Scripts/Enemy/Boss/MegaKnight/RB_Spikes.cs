using MANAGERS;
using System.Threading;
using UnityEngine;

public class RB_Spikes : MonoBehaviour
{
    private new Transform transform;

    public RB_Mega_knight MegaKnight;
    private RB_CollisionDetection _collisionDetection;
    

    [Header("Main Properties")]
    [SerializeField] private float _lifetime = 1;
    private float _lifetimeTimer = 0;
    public float GoingUpDelay = 0;
    [SerializeField] private float _goingUpDuration = 0.2f;
    [SerializeField] private float _goingDownDuration = 0.2f;
    [SerializeField] private float _activeHeight = 1f; //height the spike will go up to
    [SerializeField] private float _inactiveHeight = -1f; //height the spike will go down to
    [SerializeField] private bool _canDamageWhenMoving = false;
    private bool _hasEnemyEnteredDuringMovement = false;
    private bool _isGoingUp = false;
    private bool _isGoingDown = false;
    private float _movingTimer = 0f;
    private float _delayTimer = 0f;

    [Header("Properties (useless if MegaKnight spawn them)")]
    [SerializeField] private float _damage;
    [SerializeField] private float _knockback;


    private void Awake()
    {
        transform = GetComponent<Transform>();
        _collisionDetection = GetComponent<RB_CollisionDetection>();

        transform.position = new Vector3(transform.position.x, _inactiveHeight, transform.position.z);
        _movingTimer = 0;
        _delayTimer = 0;
        _isGoingUp = true;

        _collisionDetection.EventOnEntityEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedEntity()[_collisionDetection.GetDetectedEntity().Count - 1]); });

        //Destroy(gameObject, _lifetime);
    }

    private void Start()
    {
        RB_AudioManager.Instance.PlaySFX("Spike_Sound", transform.position, false, 0, 1f);
    }
    private void Update()
    {
        if (_isGoingUp)
        {
            if (_movingTimer >= _goingUpDuration)
            {
                transform.position = new Vector3(transform.position.x, _activeHeight, transform.position.z);
                _isGoingUp = false;
                if (_hasEnemyEnteredDuringMovement) CheckForEnemies();
            }
            else if (_delayTimer < GoingUpDelay)
            {
                _delayTimer += Time.deltaTime;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(_inactiveHeight, _activeHeight, _movingTimer / _goingUpDuration), transform.position.z);
                _movingTimer += Time.deltaTime;
            }
        }
        else if (_isGoingDown)
        {
            if (_movingTimer >= _goingDownDuration)
            {
                transform.position = new Vector3(transform.position.x, _inactiveHeight, transform.position.z);
                Destroy(gameObject);
                this.enabled = false;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(_activeHeight, _inactiveHeight, _movingTimer / _goingDownDuration), transform.position.z);
                _movingTimer += Time.deltaTime;
            }
        }

        if (!_isGoingDown && _lifetime <= _lifetimeTimer)
        {
            _movingTimer = 0;
            _isGoingUp = false;
            _isGoingDown = true;
        }
        _lifetimeTimer += Time.deltaTime;
    }

    private void CheckForEnemies()
    {
        foreach (GameObject enemy in _collisionDetection.GetDetectedEntity())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
            {
                if (MegaKnight)
                {
                    MegaKnight.ApplySpikeDamage(enemyHealth);
                }
            }
        }
    }

    private void EnemyEntered(GameObject enemy)
    {
        if (_isGoingDown || _isGoingUp)
        {
            _hasEnemyEnteredDuringMovement = true;
            return;
        }

        if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
        {
            if (MegaKnight)
            {
                MegaKnight.ApplySpikeDamage(enemyHealth);
            }
        }
    }
}
