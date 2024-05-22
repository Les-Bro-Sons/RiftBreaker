using UnityEngine;
using UnityEngine.Events;

public class RB_Enemy : MonoBehaviour
{
    public UnityEvent EventDead;
    [Header("Spawn")]
    [SerializeField] private bool _isAttachedToAPhase = true; // if false, everything under this in "Spawn" is useless
    [SerializeField] private PHASES _spawnInPhase = PHASES.Infiltration;

    protected virtual void Awake()
    {
        GetComponent<RB_Health>().EventDeath.AddListener(Death);
        GetComponent<RB_Health>().EventTakeDamage.AddListener(TakeDamage);
    }

    protected virtual void Start()
    {
        if (_isAttachedToAPhase && _spawnInPhase != RB_LevelManager.Instance.CurrentPhase)
        {
            RB_LevelManager.Instance.SaveEnemyToPhase(_spawnInPhase, gameObject);
            return;
        }
        Spawned();
    }

    public virtual void Spawned() //when the enemy is spawned
    {

    }

    protected virtual void TakeDamage()
    {

    }

    protected virtual void Death()
    {
        EventDead?.Invoke();
        Destroy(gameObject);
    }
}
