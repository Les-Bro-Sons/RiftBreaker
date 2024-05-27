using System.Collections;
using System.Collections.Generic;
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
    public float MovementSpeed = 4f;
    public float AttackRange = 1f;
    public float AttackSpeed = 2f;
    public float DistanceFromPlayer = 0f;
    public float DetectionRadius = 100f;
    public float CooldownAttack1 = 0f;
    public float CooldownAttack2 = 0f;
    public float CooldownAttack3 = 0f;

    private Rigidbody BossRB;

    [Header("PlayerInfos")]
    public Transform PlayerPosition;
    public LayerMask PlayerLayer;


    [HideInInspector] public RB_Health Health;

    private void Start()
    {
        BossRB = GetComponent<Rigidbody>();
    }
    private bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, PlayerPosition.position) <= DetectionRadius;
    }

    
} 
