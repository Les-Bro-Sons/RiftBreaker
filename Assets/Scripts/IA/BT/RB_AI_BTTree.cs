using BehaviorTree;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class RB_AI_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    private List<PHASES> _infiltrationPhases = new();
    private List<PHASES> _combatPhases = new();

    public Dictionary<string, bool> BoolDictionnary = new Dictionary<string, bool>();

    [Header("Main Parameters")]
    public ENEMYCLASS AiType = ENEMYCLASS.Light;
    public float MovementSpeed = 4f;
    public float RotationSpeed = 4f;
    public float MovementSpeedAttack = 8f;
    [Range (1f, 10f)] public float AttackRange = 2f;
    public float AttackSpeed = 2f;
    public float AttackDamage = 2f;
    public float WaitBeforeAttack = 0.5f;

    [Header("Spline Parameters")]
    public SplineContainer SplineContainer;
    public float WaitBeforeToMoveToNextWaypoint = 0.25f; // in seconds
    public int PatrolSplineIndex = 0;

    public bool HasAnInterval = false;
    public int StartWaitingWaypointInterval = 0;

    [Header("Spot Parameters")]
    public bool InRange = false;
    [Range(1f, 50f)] public float FovRange = 10f;
    public float FovAngle = 75f;
    [HideInInspector] public bool HasAlreadySeen = false;
    [HideInInspector] public bool IsAttacking = false;
    [HideInInspector] public Vector3 LastTargetPos;

    [Header("Spot UI")]
    public CanvasGroup CanvasUi;
    public Image ImageSpotBar;
    public float DurationAlphaCanvas = 0.5f;
    public float DurationToLoadSpotBar = 1f;
    public float DurationToUnloadSpotBar = 0.5f;
    [SerializeField] private GameObject _prefabUxDetectedReadyMark;

    [Header("Components")]
    [HideInInspector] public RB_AiMovement AiMovement;
    [HideInInspector] public RB_Health AiHealth;

    [Header("Faible")]
    [SerializeField] public float SlashRange;
    [SerializeField] public float SlashDamage;
    [SerializeField] public float SlashKnockback;
    [SerializeField] public GameObject SlashParticles;


    private void Awake()
    {
        /*
        if (CanvasUi.alpha > 0f)
            CanvasUi.alpha = 0;*/

        AiMovement = GetComponent<RB_AiMovement>();
        if (AiMovement == null)
            AiMovement.AddComponent<RB_AiMovement>();
        AiHealth = GetComponent<RB_Health>();
    }

    /*public void UxFocus()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }*/

    public void UxFocus()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    protected override RB_BTNode SetupTree()
    {
        _infiltrationPhases.Add(PHASES.Infiltration);
        _combatPhases.Add(PHASES.Combat);
        _combatPhases.Add(PHASES.Boss);

        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // Sequence CHECK PHASE INFILTRATION
            {
                new RB_AICheck_Phase(_infiltrationPhases),
                new RB_BTSelector(new List<RB_BTNode>  // Sequence INFILTRATION
                {
                    new RB_BTSelector(new List<RB_BTNode> // selector ai lost sight of target
                    {
                        new RB_BTSequence(new List<RB_BTNode> // sequence spot target again
                        {
                            new RB_AICheck_Bool(this, "GoToLastTargetPos"),
                            new RB_AI_PlayerInFov(this),
                            new RB_AI_SetBool(this, "GoToLastTargetPos", false)
                        }),

                        new RB_BTSequence(new List<RB_BTNode> // sequence go to last target pos
                        {
                            new RB_AICheck_Bool(this, "GoToLastTargetPos"),
                            new RB_AI_GoToLastTargetPos(this, 2, AiMovement.MovementMaxSpeed / 2f, AiMovement.MovementAcceleration / 2f),
                            new RB_AICheck_WaitForSeconds(this, 2, "GoToLastTargetPos", false) // ADD RANDOM PATROL (Feature)
                        }),
                        
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence spotted
                    {
                        new RB_AICheck_Bool(this, "HasAlreadySeen"),
                        new RB_AI_PlayerInFov(this),
                        new RB_AI_GoToTarget(this),
                        new RB_AI_Attack(this),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Check Spot
                    {
                        new RB_AI_PlayerInFov(this),
                    }),

                    new RB_AI_Task_DefaultPatrol(this),  // task default
                }),
                
            }),

            
            new RB_BTSequence(new List<RB_BTNode> // Sequence COMBAT
            {
                new RB_AICheck_Phase(_combatPhases),
                new RB_BTSelector(new List<RB_BTNode>
                {
                    new RB_BTSequence(new List<RB_BTNode> // Sequence Faible
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Light),
                        new RB_AI_PlayerInFov(this),
                        new RB_AI_GoToTarget(this),
                        new RB_AI_Attack(this),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Moyen
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Medium),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Fort
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Heavy),
                    }),
                }),
            }),
        });;

        return root;
    }

    public void SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab)
            Instantiate(prefab, position, rotation);
    }
}