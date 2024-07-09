using BehaviorTree;
using MANAGERS;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RB_AI_BTRobertLenec : RB_AI_BTTree
{
    [Header("Movement")]
    [SerializeField] public float MinMovementDistance = 3f;
    [SerializeField] public float MaxMovementDistance = 10f;
    [SerializeField] public float DelayMovement = 1f;
    public float CurrentCooldownBetweenMovement;

    [Header("Single Shot (attack1)")]
    public GameObject RedBall;
    [SerializeField] public float _minMovementBeforeAttackDistance = 3f;
    [SerializeField] public float _maxMovementBeforeAttackDistance = 10f;
    [SerializeField] public float _dashBeforeAttackSpeed = 10f;
    [SerializeField] public float _redBallOffset = 1f;

    [Header("WoodenPiece Rain Zone (attack2)")]
    public GameObject WoodenPieceRainZone;
    [SerializeField] public float _areaDamageRadius = 1f;
    [SerializeField] public float _areaDamageInterval = 1f;
    [SerializeField] public float _areaDamageAmount = 1f;
    [SerializeField] public float _areaDamageKnockback = 1f;
    [SerializeField] public bool _canAreaDamageZoneDamageMultipleTime = false;
    protected float _currentCooldownBeforeTakeDamage;
    private List<RB_Health> _alreadyAreaDamageZoneDamaged = new();

    [Header("Clone Attack (attack3)")]
    public GameObject Clone;
    [SerializeField] public List<Transform> _waypoints;
    [SerializeField] public int _numberOfArrow;
    [SerializeField] public float _cloneAttackInterval = 0.5f;
    [SerializeField] public float _cloneAttackDelay = 1f;
    [SerializeField] public float _cloneLifeTime = 1f;
    [SerializeField] public float _cooldownForReaparition = 1f;
    [SerializeField] public float _minCooldownForAttack = 10f;
    [SerializeField] public float _maxCooldownForAttack = 30f;
    private List<GameObject> _clones = new List<GameObject>();
    private Vector3 _lastPosition;
    protected float _currentCooldownBeforeReactivate;

    protected override RB_BTNode SetupTree()
    {
        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> //sequence enemy in room
            {
                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                new RB_BTSelector(new List<RB_BTNode>
                {

                }),
            }),

            new RB_BTSequence(new List<RB_BTNode> //sequence no enemy in room
            {
                new RB_AI_ReverseState(new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, false)),
            }),
        });

        return root;
    }
}
