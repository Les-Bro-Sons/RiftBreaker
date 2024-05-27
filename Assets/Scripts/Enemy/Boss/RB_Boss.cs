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
    public float CooldownAttack1 = 0f;
    public float CooldownAttack2 = 0f;
    public float CooldownAttack3 = 0f;

    [HideInInspector] public RB_Health Health;
} 
