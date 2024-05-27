using BehaviorTree;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class RB_AICombat_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    public enum AI_Type
    {
        FAIBLE,
        MOYEN,
        FORT,
    }

    [Header("Main Parameters")]
    public AI_Type AiType = AI_Type.FAIBLE;

    public float MovementSpeed = 4f;
    public float RotationSpeed = 4f;
    public float MovementSpeedAttack = 8f;
    [Range (1f, 10f)] public float AttackRange = 2f;
    public float AttackSpeed = 2f;
    public float AttackDamage = 2f;


    [Header("Spline Parameters")]
    public SplineContainer SplineContainer;
    public float WaitBeforeToMoveToNextWaypoint = 0.25f; // in seconds
    public int PatrolSplineIndex = 0;

    public bool HasAnInterval = false;
    public int StartWaitingWaypointInterval = 0;

    [Header("FOV Parameters")]
    public bool InRange = false;
    [Range(1f, 50f)] public float FovRange = 10f;

/*    [Header("UI")]
    public CanvasGroup CanvasUi;
    public Image ImageSpotBar;
    public float DurationAlphaCanvas = 0.5f;
    [SerializeField] private GameObject _prefabUxDetectedReadyMark;*/

    [Header("Components")]
    [HideInInspector] public RB_AiMovement AiMovement;
    [HideInInspector] public bool HasAlreadySeen = false;


    private void Awake()
    {
        /*
        if (CanvasUi.alpha > 0f)
            CanvasUi.alpha = 0;*/

        AiMovement = GetComponent<RB_AiMovement>();
        if (AiMovement == null)
            AiMovement.AddComponent<RB_AiMovement>();
    }

    /*public void UxFocus()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }*/

    protected override RB_BTNode SetupTree()
    {
        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // Sequence Attack
            {
                new RB_AICombat_PlayerInFov(this),
                new RB_AICombat_GoToTarget(this),
                new RB_AICombat_Attack(this),
            }),

            new RB_AICombat_Task_DefaultPatrol(this),  // task default
        });

        return root;
    }
}