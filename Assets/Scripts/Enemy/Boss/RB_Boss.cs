using UnityEngine;


public enum BOSSSTATES
{
    Idle,
    Moving,
    Attack1,
    Attack2,
    Attack3,
}
public class RB_Boss : RB_Enemy
{
    [Header("Main Parameters")]
    public float AttackRange = 1f;
    public float AttackSpeed = 2f;
    public float DistanceFromPlayer = 0f;
    public float DetectionRadius = 100f;
    public float CooldownAttack1 = 0f;
    public float CooldownAttack2 = 0f;
    public float CooldownAttack3 = 0f;
    public float CooldownBetweenAttacks = 0f;
    public float WaitInIdle = 1;
    protected float _currentCooldownAttack1;
    protected float _currentCooldownAttack2;
    protected float _currentCooldownAttack3;
    protected float _currentCooldownBetweenAttacks;
    protected float _currentWaitInIdle;

    public Rigidbody BossRB;

    [Header("PlayerInfos")]
    public Transform PlayerPosition;
    public LayerMask PlayerLayer;


    [HideInInspector] public RB_Health Health;

    protected virtual void Start()
    {
        Health = GetComponent<RB_Health>();
        BossRB = GetComponent<Rigidbody>();
        GetTarget();
    }

    protected virtual void Update()
    {
        
    }

    protected bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, PlayerPosition.position) <= DetectionRadius;
    }

    

    
    
} 
