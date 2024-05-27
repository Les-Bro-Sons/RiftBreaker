using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class RB_AIInf_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    [Header("Main Parameters")]
    public float MovementSpeed = 4f;
    public float RotationSpeed = 4f;
    public float MovementSpeedAttack = 8f;
    [Range (1f, 10f)] public float AttackRange = 2f;
    public float AttackSpeed = 2f;
    public float AttackDamage = 2f;
    public float WaitBeforeAttack = 0.5f;
     public bool IsAttacking = false;

    [Header("Spline Parameters")]
    public SplineContainer SplineContainer;
    public float WaitBeforeToMoveToNextWaypoint = 0.25f; // in seconds
    public int PatrolSplineIndex = 0;

    public bool HasAnInterval = false;
    public int StartWaitingWaypointInterval = 0;

    [Header("FOV Parameters")]
    public bool InFov = false;
    public float FovRange = 10f;
    public float FovAngle = 75f;

    [Header("UI")]
    public CanvasGroup CanvasUi;
    public Image ImageSpotBar;
    public float DurationAlphaCanvas = 0.5f;
    public float DurationToLoadSpotBar = 1f;
    public float DurationToUnloadSpotBar = 0.5f;
    [SerializeField] private GameObject _prefabUxDetectedReadyMark;

    [Header("Components")]
    public RB_AiMovement AiMovement;


    private void Awake()
    {
        if (CanvasUi.alpha > 0f)
            CanvasUi.alpha = 0;
        AiMovement = GetComponent<RB_AiMovement>();
    }



    public void UxFocus()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    protected override RB_BTNode SetupTree()
    {
        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // Sequence Attack
            {
                new RB_AIInf_PlayerInFov(this),
                new RB_AIInf_GoToTarget(this),
                new RB_AIInf_Attack(this),
            }),

            new RB_AI_Task_DefaultPatrol(this),  // task default
        });

        return root;
    }
}