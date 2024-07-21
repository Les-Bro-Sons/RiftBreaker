using BehaviorTree;
using MANAGERS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Splines;
using UnityEngine.UI;

public class RB_AI_BTTree : RB_BTTree
{
    [Serializable]
    public struct Attack
    {
        public float Delay;
        
        public float Damage;
        public float Knockback;
        
        public float Range;
    }

    private List<PHASES> _infiltrationPhases = new();
    private List<PHASES> _combatPhases = new();

    public Dictionary<BTBOOLVALUES, bool> BoolDictionnary = new Dictionary<BTBOOLVALUES, bool>();
    private List<RB_Health> _characterCollisions = new();

    [Header("Main Parameters")]
    public ENEMYCLASS AiType = ENEMYCLASS.Light;
    public float MovementSpeed = 4f;
    public float MovementSpeedAggro = 8f;
    public float MovementSpeedFlee = 6f;
    public float FleeDistance = 2.5f;
    public float AttackSpeed = 0.2f;
    public float BoostMultiplier = 1f;
    public ParticleSystem BoostParticle;
    public bool IsDecorative = false;
    [HideInInspector] public int CurrentAttackIndex = 0;

    [Header("Static Mode Parameters")]
    public bool IsStatic = false;
    public Vector3 StaticLookDirection = Vector3.forward;
    public bool StaticPositionOnStart = true;
    public Vector3 StaticPosition;
    [HideInInspector] public bool IsOnStaticPoint = false;

    [Header("Spline Parameters")]
    public float WaitBeforeToMoveToNextWaypoint = 0.25f; // in seconds
    public int PatrolSplineIndex = 0;
    public bool HasAnInterval = false;
    public int StartWaitingWaypointInterval = 0;
    [HideInInspector] public int CurrentWaypointIndex = 0;
    [HideInInspector] public SplineContainer SplineContainer;

    [Header("Spot Parameters")]
    public bool InRange = false;
    [Range(1f, 50f)] public float FovRange = 10f;
    [Range(1f, 50f)] public float SpottedFovRange = 10f;
    public float FovAngle = 75f;
    public float MinDistDurationToLoadSpotBar = 0.25f;
    public float MaxDistDurationToLoadSpotBar = 1f;
    public float DurationToUnloadSpotBar = 1f;
    [HideInInspector] public Vector3 LastTargetPos;
    [HideInInspector] public bool IsPlayerInSight = false; //used only by other scripts

    [Header("Spot UI")]
    public CanvasGroup CanvasUi;
    public Image ImageSpotBar;
    public float DurationAlphaCanvas = 0.5f;
    [HideInInspector] public float LastSpotValue = 0;
    [SerializeField] private GameObject _prefabUxDetectedReadyMark;

    [Header("Components")]
    [HideInInspector] public RB_AiMovement AiMovement;
    [HideInInspector] public RB_Health AiHealth;
    [HideInInspector] public Rigidbody AiRigidbody;
    public Animator AiAnimator;
    [HideInInspector] public NavMeshAgent AiNavMeshAgent;
    public RB_EnemyAnimation AiEnemyAnimation;

    [Header("Infiltration")]
    public float InfSlashRange;
    public float InfSlashDamage;
    public float InfSlashKnockback;
    public float InfSlashDelay;
    public float InfSlashCollisionSize = 3;
    public float InfSpottedMoveSpeed = 11;
    public float InfSpottingMoveSpeed = 3;
    [Range(0, 1)] public float InfThresholdWalkingToSusPerson = 0.15f;
    [Range(0, 1)] public float InfThresholdRunningToSusPerson = 0.75f;
    public GameObject InfSlashParticles;
    [HideInInspector] public UnityEvent EventOnSpotted;

    [Header("Distractions")]
    [HideInInspector] public List<RB_Distraction> Distractions = new();
    [HideInInspector] public List<RB_Distraction> AlreadySeenDistractions = new();
    [HideInInspector] public RB_Distraction CurrentDistraction;
    public float MoveToDistractionSpeed = 6;
    public float DistractionDistanceNeeded = 1;
    public float DistractedSpotSpeedMultiplier = 2;


    [Header("Faible")]
    public float SlashRange;
    public float SlashDamage;
    public float SlashKnockback;
    public float SlashDelay;
    public float SlashCollisionSize = 3;
    public GameObject SlashParticles;

    [Header("Moyen")]
    public GameObject ArrowPrefab;
    public float BowRange;
    public float BowDamage;
    public float BowKnockback;
    public float BowDelay;
    public float ArrowSpeed;
    public float ArrowDistance;

    [Header("Fort")]
    public GameObject HeavyArrowPrefab;
    public float HeavyBowRange;
    public float HeavyBowDamage;
    public float HeavyBowKnockback;
    public float HeavyBowDelay;
    public float HeavyBowProjectileNumber = 3;
    public float HeavyBowDelayBetweenProjectile = 0.2f;
    public float HeavyArrowSpeed;
    public float HeavyArrowDistance;

    public float HeavySlashRange;
    public float HeavySlashDamage;
    public float HeavySlashKnockback;
    public float HeavySlashFirstDelay;
    public float HeavySlashComboDelay;
    public float HeavySlashCollisionSize = 3;
    public int CurrentHeavySlashCombo = 0;
    public int MaxHeavySlashCombo = 5;
    public GameObject HeavySlashParticles;

    [Header("Tower")]
    public float ExplosionDamage = 30;
    public float ExplosionKnockback = 15;
    public float ExplosionRadius = 3;
    public float ExplosionDelay = 1;
    public float ExplosionStartRange = 1;
    public GameObject ExplosionParticles;

    [Header("Boss")]
    public float Attack1Cooldown = 1;
    public float Attack2Cooldown = 1;
    public float Attack3Cooldown = 1;
    public float WaitInIdleAfterAttack = 1;
    [HideInInspector] public float Attack1CurrentCooldown;
    [HideInInspector] public float Attack2CurrentCooldown;
    [HideInInspector] public float Attack3CurrentCooldown;
    [HideInInspector] public float CurrentCooldownBetweenAttacks;
    [HideInInspector] public float CurrentWaitInIdle;

    [Header("MegaKnight")]
    public float MegaSlashRange;
    public float MegaSlashDamage;
    public float MegaSlashKnockback;
    public float MegaSlashDelay;
    public float MegaSlashCollisionSize = 3;
    public GameObject MegaSlashParticles;

    public GameObject Spikes;
    public float SpikesLength = 5;
    public float SpikesSpaces = 0.75f;
    public float SpikeDamage = 10;
    public float SpikeKnockback = 10;
    public bool CanSpikeDamageMultipleTime = false;
    public float SpikeDelayIncrementation = 0.1f;

    public float JumpHeight = 5f;
    public AnimationCurve JumpAttackCurve;
    public float JumpDuration = 1;
    public float LandingRadius = 2;
    public float LandingDamage = 20;
    public float LandingKnockback = 10;
    public GameObject LandingParticles;
    [HideInInspector] public float CurrentJumpDuration = 0;
    [HideInInspector] public Vector3 JumpStartPos;
    [HideInInspector] public Vector3 JumpEndPos;

    [Header("Robert Le Nec")]
    public GameObject RedBall;
    public float RedBallOffset = 1f;

    public GameObject WoodenPieceRainZone;
    public float AreaDamageRadius = 1f;
    public float AreaDamageInterval = 1f;
    public float AreaDamageAmount = 1f;
    public float AreaDamageKnockback = 1f;
    public bool CanAreaDamageZoneDamageMultipleTime = false;
    public float CurrentCooldownBeforeTakeDamage;
    [HideInInspector] public List<RB_Health> AlreadyAreaDamageZoneDamaged = new();

    public GameObject Clone;
    public List<Transform> Waypoints;
    public Transform TpPoint;
    public int NumberOfArrow = 1;
    public float CloneAttackInterval = 0.5f;
    public float CloneAttackDelay = 1f;
    public float CloneLifeTime = 1f;
    public float CooldownForReaparition = 1f;
    public float MinCooldownForAttack = 10f;
    public float MaxCooldownForAttack = 30f;
    [HideInInspector] public Vector3 LastPosition;
    [HideInInspector] public float CurrentCooldownBeforeReactivate;

    [Header("Yog")]
    public RB_Tentacles YogTentacle;
    public RB_CollisionDetection YogTentacleCollision;
    public bool YogTentacleHitFullRange = true;
    public float YogTentacleHitWidth = 1f;
    public float YogTentacleHitRange = 1f;
    public float YogTentacleHitKnockback = 1f;
    public float YogTentacleHitDamage = 1f;
    public float YogTentacleHitDelay = 1f;
    public float YogTentacleHitDuration = 1f;
    public float YogTentacleRemoveDuration = 0.25f;
    public AnimationCurve YogTentacleHitCurve;
    public AnimationCurve YogTentacleRemoveCurve;
    public float YogTentacleDelayBeforeHit = 0.2f;
    public int YogTentaclenumberTotalOfAttack = 5;
    public GameObject YogTentacleHitParticles;
    public GameObject YogTentacleHitAnimation;
    [HideInInspector] public int YogTentacleAttackDone;
    [HideInInspector] public float YogTentacleHitDelayTimer = 0;
    public float YogTentacleHitFirstDelay = 3;
    [HideInInspector] public float YogTentacleHitFirstDelayTimer = 0;

    public GameObject YogExplosionZone;
    public GameObject YogExplosionParticles;
    public float YogAreaDamageRadius = 1f;
    public float YogAreaDamageInterval = 1f;
    public float YogAreaDamageAmount = 1f;
    public float YogAreaDamageKnockback = 1f;
    public float YogAreaExpandingTime = 1f;
    public bool YogCanAreaDamageZoneDamageMultipleTime = false;
    public AnimationCurve YogAreaExpandCurve;
    [HideInInspector] public List<RB_Health> YogAlreadyAreaDamageZoneDamaged = new();
    [HideInInspector] public List<GameObject> YogExplosion = new List<GameObject>();

    public float YogExplosionDamage = 1f;
    public float YogExplosionKnockback = 1f;
    public bool YogCanExplosionDamageMultipleTime = false;
    public List<RB_Health> YogAlreadyExplosionDamaged = new();

    [HideInInspector] public List<GameObject> MinionSpawned = new ();
    public int NumberOfMinionSpawn = 3;
    public float TimeForRescalingMinion = 1f;
    public List<GameObject> SpawnableMinion = new();
    public Transform YogCenterOfRoom;

    private void Awake()
    {
        AiMovement = GetComponent<RB_AiMovement>();
        AiHealth = GetComponent<RB_Health>();
        AiRigidbody = GetComponent<Rigidbody>();
        AiAnimator = GetComponentInChildren<Animator>();
        if (AiAnimator) AiAnimator.SetFloat("EnemyID", (int)AiType);
    }

    protected override void Update()
    {
        base.Update();
        SpotCanvasAlpha();
        ApplyBoostParticles();
        if (AiAnimator) AiAnimator.SetFloat("EnemyID", (int)AiType);
        else Debug.LogWarning("NO AiAnimator in " + gameObject.name);
    }

    private void ApplyBoostParticles()
    {
        if (!BoostParticle) return;

        if (BoostMultiplier > 1 && !BoostParticle.isPlaying) 
        {
            BoostParticle.Play();
        }
        else if (BoostParticle.isPlaying)
        {
            BoostParticle.Stop();
        }
    }

    #region Spot Visual Functions
    private void SpotCanvasAlpha() //handle the alpha of the spot canvas when needed
    {
        if (!ImageSpotBar) return;

        float SpotValue = ImageSpotBar.fillAmount;
        if (LastSpotValue != SpotValue && SpotValue > 0 && SpotValue < 1)
        {
            if (CanvasUi.alpha < 1)
            {
                ShowSpotCanvas();
            }
        }
        else
        {
            if (CanvasUi.alpha > 0)
            {
                HideSpotCanvas();
            }
        }
        LastSpotValue = SpotValue;
    }

    private void ShowSpotCanvas()
    {
        CanvasUi.alpha = Mathf.Clamp(CanvasUi.alpha + Time.deltaTime / DurationAlphaCanvas, 0, 1);
    }

    private void HideSpotCanvas()
    {
        CanvasUi.alpha = Mathf.Clamp(CanvasUi.alpha - Time.deltaTime / DurationAlphaCanvas, 0, 1);
    }

    public void OnSpotted()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        RB_AudioManager.Instance.PlaySFX("Detection", transform.position, false, 0,1);
        EventOnSpotted?.Invoke();
    }

    public void OnDistracted()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    #endregion

    protected override RB_BTNode SetupTree()
    {
        _infiltrationPhases.Add(PHASES.Infiltration);
        _combatPhases.Add(PHASES.Combat);
        _combatPhases.Add(PHASES.Boss);
        this.SplineContainer = RB_SplineManager.Splines;

        if (StaticPositionOnStart) StaticPosition = transform.position;

        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // sequence decoration Infiltration
            {
                #region Sequence Phase Infiltration Decorative
                new RB_AICheck_Phase(_infiltrationPhases),
                new RB_AICheck_Bool(this, IsDecorative),
                new RB_AI_BecomeDecoration(this),
                #endregion
            }),

            new RB_BTSequence(new List<RB_BTNode> // Sequence CHECK PHASE INFILTRATION
            {
                #region Sequence Phase Infiltration
                new RB_AICheck_Phase(_infiltrationPhases),
                new RB_AICheck_Bool(this, !IsDecorative),
                
                new RB_BTSelector(new List<RB_BTNode>  // Sequence INFILTRATION
                {
                    #region Touch detection Sequence
                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AICheck_EnemyTouchDetection(this, true),
                        new RB_AI_DoFailure(),
                    }),
                    #endregion

                    #region Static AI Sequence
                    new RB_BTSequence(new List<RB_BTNode> //static sequence
                    {
                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.IsTargetSpotted)),
                        new RB_AI_StaticWatchOut(this),
                        new RB_AI_ToState(new RB_AI_PlayerInFov(this, FovRange), BTNodeState.SUCCESS),
                    }),
                    #endregion

                    #region Walking to suspicious person during spotting Sequence
                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.IsTargetSpotted)),
                        new RB_AICheck_Bool(this, BTBOOLVALUES.IsTargetInSight),
                        new RB_AICheck_SpotBarFilled(this, ">=", InfThresholdWalkingToSusPerson),
                        new RB_AI_ToState(new RB_AI_GoToTarget(this, InfSpottingMoveSpeed, 1), BTNodeState.FAILURE),
                    }),
                    #endregion

                    #region AI completely lost sight of target Sequence
                    new RB_BTSelector(new List<RB_BTNode>
                    {
                        new RB_BTSequence(new List<RB_BTNode> // sequence spot target again
                        {
                            new RB_AICheck_Bool(this, BTBOOLVALUES.GoToLastTargetPos),
                            new RB_AI_PlayerInFov(this, FovRange, DistractedSpotSpeedMultiplier),
                            new RB_AI_SetBool(this, BTBOOLVALUES.GoToLastTargetPos, false)
                        }),

                        new RB_BTSequence(new List<RB_BTNode> // sequence go to last target pos
                        {
                            new RB_AICheck_Bool(this, BTBOOLVALUES.GoToLastTargetPos),
                            new RB_AI_GoToLastTargetPos(this, 2, AiMovement.MovementMaxSpeed, AiMovement.MovementAcceleration),
                            new RB_AICheck_WaitForSeconds(this, 2, BTBOOLVALUES.GoToLastTargetPos, false) // ADD RANDOM PATROL (Feature)
                        }),
                        
                    }),
                    #endregion

                    #region Target Spotted Sequence
                    new RB_BTSequence(new List<RB_BTNode> // Sequence spotted
                    {
                        new RB_AICheck_Bool(this, BTBOOLVALUES.IsTargetSpotted),
                        new RB_AI_PlayerInFov(this, SpottedFovRange),
                        new RB_AI_GoToTarget(this, InfSpottedMoveSpeed, InfSlashRange),
                        new RB_AI_Attack(this, -1),
                    }),
                    #endregion

                    #region Spot Sequence
                    new RB_BTSequence(new List<RB_BTNode> // Sequence Check Spot
                    {
                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.IsTargetSpotted)),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //distracted spot
                            {
                                new RB_AICheck_IsDistracted(this),
                                new RB_AI_PlayerInFov(this, FovRange, DistractedSpotSpeedMultiplier),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //not distracted spot
                            {
                                new RB_AI_ReverseState(new RB_AICheck_IsDistracted(this)),
                                new RB_AI_PlayerInFov(this, FovRange),
                            }),
                        }),
                    }),
                    #endregion

                    #region Go to suspicious person position Sequence
                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.GoToLastTargetPos)),
                        new RB_AICheck_SpotBarFilled(this, ">=", InfThresholdRunningToSusPerson),
                        new RB_AI_SetBool(this, BTBOOLVALUES.GoToLastTargetPos, true),
                    }),
                    #endregion

                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AICheck_LookForDistractions(this),
                    }),

                    #region Distracted Sequence
                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AICheck_IsDistracted(this),
                        //new RB_AI_ReverseState(this, new RB_AI_PlayerInFov(this, FovRange)),
                        new RB_AI_GetHighestPriorityDistraction(this, TARGETMODE.Closest),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            #region Broken Pot / DEFAULT Sequence
                            new RB_BTSequence(new List<RB_BTNode> //broken pot / DEFAULT sequence
                            {
                                new RB_AI_GoToDistraction(this),
                                new RB_AI_LookAtDistraction(this),
                                new RB_AICheck_WaitForSeconds(this, 1),
                                new RB_AI_CompleteDistraction(this),
                            }),
                            #endregion
                        }),
                        new RB_AI_GetPatrolNearestIndex(this),
                    }),
                    #endregion

                    new RB_AI_Task_DefaultPatrol(this),  // task default
                }),
                #endregion
            }),
            
            new RB_BTSequence(new List<RB_BTNode> // Sequence COMBAT
            {
                #region Sequence Phase Combat
                new RB_AICheck_Phase(_combatPhases),
                new RB_BTSelector(new List<RB_BTNode>
                {
                    new RB_BTSequence(new List<RB_BTNode> // Sequence Faible
                    {
                        #region Sequence Enemy Type Light
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Light),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest),
                                new RB_AI_GoToTarget(this, MovementSpeedAggro, SlashRange),
                                new RB_AI_Attack(this, 0), //slash
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AICheck_Bool(this, !IsDecorative),
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Moyen
                    {
                        #region Sequence Enemy Type Medium
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Medium),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //spot sequence
                            {
                                new RB_AICheck_Bool(this, BTBOOLVALUES.PlayerSpottedInCombat),
                                new RB_AICheck_IsTargetAlive(this),
                                new RB_BTSelector(new List<RB_BTNode>
                                {
                                    new RB_BTSequence(new List<RB_BTNode> //flee sequence
                                    {
                                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.IsAttacking)),
                                        new RB_AI_ReverseState(new RB_AI_FleeFromTarget(this, FleeDistance, MovementSpeedFlee)),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode> //bow sequence
                                    {
                                        new RB_AI_GoToTarget(this, MovementSpeedAggro, BowRange),
                                        new RB_AI_Attack(this, 0), //bow
                                    }),
                                }),
                                
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AI_SetBool(this, BTBOOLVALUES.PlayerSpottedInCombat, false),
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest),
                                new RB_AI_SetBool(this, BTBOOLVALUES.PlayerSpottedInCombat, true),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AICheck_Bool(this, !IsDecorative),
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Fort
                    {
                        #region Sequence Enemy Type Heavy
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Heavy),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest),
                                new RB_BTSelector(new List<RB_BTNode>
                                {
                                    new RB_BTSequence(new List<RB_BTNode> //flee sequence
                                    {
                                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.IsAttacking)),
                                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.HeavyAttackSlash)), //when bow attack
                                        new RB_AI_ReverseState(new RB_AI_FleeFromTarget(this, FleeDistance, MovementSpeedFlee)),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode> //3 projectile sequence
                                    {
                                        new RB_AI_ReverseState(new RB_AICheck_Bool(this, BTBOOLVALUES.HeavyAttackSlash)), // to switch attacks
                                        new RB_AI_GoToTarget(this, MovementSpeedAggro, HeavyBowRange),
                                        new RB_AI_Attack(this, 0),
                                    }),
                                    
                                    new RB_BTSequence(new List<RB_BTNode> //heavy slash sequence
                                    {
                                        new RB_AICheck_Bool(this, BTBOOLVALUES.HeavyAttackSlash), // to switch attacks
                                        new RB_AI_GoToTarget(this, MovementSpeedAggro, HeavySlashRange),
                                        new RB_AI_Attack(this, 1),
                                    }),
                                }),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AICheck_Bool(this, !IsDecorative),
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Pawn
                    {
                        #region Sequence Enemy Type Pawn
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Pawn),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_AI_GoToTarget(this, MovementSpeedAggro, SlashRange),
                                new RB_AI_Attack(this, 0), //slash
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //follow leader sequence
                            {
                                new RB_AI_FollowLeader(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Tower
                    {
                        #region Sequence Enemy Type Tower
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Tower),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_AI_GoToTarget(this, MovementSpeedAggro, ExplosionStartRange),
                                new RB_AI_Attack(this, 0), //explode
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //follow leader sequence
                            {
                                new RB_AI_FollowLeader(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Megaknight
                    {
                        #region Enemy Type Megaknight
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Megaknight),
                        new RB_AI_CooldownProgress(this),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Attack sequence
                            {
                                new RB_AICheck_Bool(this, BTBOOLVALUES.IsAttacking),
                                new RB_AI_Attack(this, "CurrentAttackIndex"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //wait in idle sequence
                            {
                                new RB_AICheck_Comparison(this, "CurrentWaitInIdle", 0, ">"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_AI_ToState(new RB_AI_GoToTarget(this, MovementSpeedAggro, MegaSlashRange), BTNodeState.SUCCESS),
                                new RB_BTSequence(new List<RB_BTNode>
                                {
                                    new RB_AICheck_Comparison(this, "CurrentCooldownBetweenAttacks", 0, "<="),
                                    new RB_BTSelector(new List<RB_BTNode> //Attack sequence
                                    {
                                        new RB_BTSequence(new List<RB_BTNode> //Attack 1
                                        {
                                            new RB_AICheck_Comparison(this, "Attack1CurrentCooldown", 0, "<="),
                                            new RB_AICheck_IsTargetClose(this, MegaSlashRange),
                                            new RB_AI_Attack(this, 0),
                                        }),

                                        new RB_BTSequence(new List<RB_BTNode> //Attack 2
                                        {
                                            new RB_AICheck_Comparison(this, "Attack2CurrentCooldown", 0, "<="),
                                            new RB_AICheck_IsTargetClose(this, 7),
                                            new RB_AICheck_IsTargetFar(this, MegaSlashRange),
                                            new RB_AI_Attack(this, 1),
                                        }),

                                        new RB_BTSequence(new List<RB_BTNode> //Attack 3
                                        {
                                            new RB_AICheck_Comparison(this, "Attack3CurrentCooldown", 0, "<="),
                                            new RB_AICheck_IsTargetFar(this, 7),
                                            new RB_AI_Attack(this, 2),
                                        }),

                                        new RB_BTSequence(new List<RB_BTNode>
                                        {
                                            new RB_AICheck_Random(0.4f, 1),
                                            new RB_AI_Attack(this, new Dictionary<int, float>{{1, 50}, {2, 50} }),
                                        }),
                                    }),
                                }),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AI_ReverseState(new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, false)),
                                new RB_AI_StaticWatchOut(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Robert Le Nec
                    {
                        #region Enemy Type Robert Le Nec
                        new RB_AICheck_Class(AiType, ENEMYCLASS.RobertLeNec),
                        new RB_AI_CooldownProgress(this),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Attack sequence
                            {
                                new RB_AICheck_Bool(this, BTBOOLVALUES.IsAttacking),
                                new RB_AI_Attack(this, "CurrentAttackIndex"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //wait in idle sequence
                            {
                                new RB_AICheck_Comparison(this, "CurrentWaitInIdle", 0, ">"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_BTSelector(new List<RB_BTNode>
                                {
                                    new RB_BTSequence(new List<RB_BTNode> //flee sequence
                                    {
                                        new RB_AICheck_IsTargetClose(this, 5),
                                        new RB_AI_ToState(new RB_AI_FleeFromTarget(this, 5), BTNodeState.FAILURE),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode> //Attack sequence
                                    {
                                        new RB_AICheck_Comparison(this, "CurrentCooldownBetweenAttacks", 0, "<="),
                                        new RB_BTSelector(new List<RB_BTNode> //Attack switch
                                        {
                                            new RB_BTSequence(new List<RB_BTNode> //Attack 1
                                            {
                                                new RB_AICheck_Comparison(this, "Attack1CurrentCooldown", 0, "<="),
                                                new RB_AICheck_IsTargetFar(this, 5),
                                                new RB_AI_Attack(this, 0),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode> //Attack 2
                                            {
                                                new RB_AICheck_Comparison(this, "Attack2CurrentCooldown", 0, "<="),
                                                new RB_AICheck_IsTargetFar(this, 2),
                                                new RB_AICheck_IsTargetClose(this, 5),
                                                new RB_AI_Attack(this, 1),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode> //Attack 3
                                            {
                                                new RB_AICheck_Comparison(this, "Attack3CurrentCooldown", 0, "<="),
                                                new RB_AI_Attack(this, 2),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode>
                                            {
                                                new RB_AICheck_Random(0.5f, 1),
                                                new RB_AI_Attack(this, new Dictionary<int, float>{{0, 50}, {1, 50}, {2, 50}}),
                                            }),
                                        }),
                                    }),
                                }),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AI_ReverseState(new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, false)),
                                new RB_AI_StaticWatchOut(this),
                            }),
                        }),
                        #endregion
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Yog
                    {
                        #region Enemy Type Yog
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Yog),
                        new RB_AI_CooldownProgress(this),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Attack sequence
                            {
                                new RB_AICheck_Bool(this, BTBOOLVALUES.IsAttacking),
                                new RB_AI_Attack(this, "CurrentAttackIndex"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AICheck_Comparison(this, "YogTentacleAttackDone", 0, ">"),
                                new RB_AI_Attack(this, 0),
                            }),

                            new RB_BTSequence(new List<RB_BTNode> //wait in idle sequence
                            {
                                new RB_AICheck_Comparison(this, "CurrentWaitInIdle", 0, ">"),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_BTSelector(new List<RB_BTNode>
                                {
                                    new RB_BTSequence(new List<RB_BTNode>
                                    {
                                        new RB_AICheck_IsTargetClose(this, 3),
                                        new RB_AICheck_Comparison(this, "Attack2CurrentCooldown", 0, ">="),
                                        new RB_AI_ToState(new RB_AI_FleeFromTarget(this, 3, MovementSpeed), BTNodeState.FAILURE),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode>
                                    {
                                        new RB_AICheck_IsTargetFar(this, 5),
                                        new RB_AI_ToState(new RB_AI_GoToTarget(this, MovementSpeed, 1), BTNodeState.FAILURE),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode> //Attack sequence
                                    {
                                        new RB_AICheck_Comparison(this, "CurrentCooldownBetweenAttacks", 0, "<="),
                                        new RB_BTSelector(new List<RB_BTNode> //Attack switch
                                        {
                                            new RB_BTSequence(new List<RB_BTNode> //Attack 3
                                            {
                                                new RB_AICheck_Comparison(this, "Attack3CurrentCooldown", 0, "<="),
                                                new RB_AI_Attack(this, 2),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode> //Attack 1
                                            {
                                                new RB_AICheck_Comparison(this, "Attack1CurrentCooldown", 0, "<="),
                                                new RB_AICheck_IsTargetFar(this, YogAreaDamageRadius/2f),
                                                new RB_AI_Attack(this, 0),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode> //Attack 2
                                            {
                                                new RB_AICheck_Comparison(this, "Attack2CurrentCooldown", 0, "<="),
                                                new RB_AICheck_IsTargetClose(this, YogAreaDamageRadius/2f),
                                                new RB_AI_Attack(this, 1),
                                            }),

                                            new RB_BTSequence(new List<RB_BTNode>
                                            {
                                                new RB_AICheck_Random(0.2f, 1),
                                                new RB_AI_Attack(this, new Dictionary<int, float>{{0, 50}, {1, 50}, {2, 25} }),
                                            }),
                                        }),
                                    }),
                                }),
                            })
                        }),
                        #endregion
                    }),
                }),
                #endregion
            }),
        });

        return root;
    }

    #region BT Tools
    public bool AddDistraction(RB_Distraction distraction, bool removeDistractionOfSameType = false)
    {
        if (!Distractions.Contains(distraction) && !AlreadySeenDistractions.Contains(distraction))
        {
            if (removeDistractionOfSameType)
            {
                foreach (RB_Distraction otherDistraction in Distractions.ToList())
                {
                    if (otherDistraction.DistractionType != distraction.DistractionType) continue;
                    Distractions.Remove(otherDistraction);
                    otherDistraction.OnDistractionCompleted();
                }
            }

            Distractions.Add(distraction);
            OnDistracted();
            return true;
        }
        return false;
    }


    public void OnCompleteDistraction(RB_Distraction distraction)
    {
        if (Distractions.Contains(distraction))
        {
            AlreadySeenDistractions.Add(distraction);
            Distractions.Remove(distraction);
        }
    }

    public void Boost(float boost)
    {
        BoostMultiplier = boost;
        AiMovement.MoveSpeedBoost = boost;
    }

    public float ApplyDamage(RB_Health enemy, float damage, bool applyBoost = true)
    {
        float dealDamage = damage * BoostMultiplier;
        enemy.TakeDamage(damage * BoostMultiplier);
        return dealDamage;
    }

    public float ApplyDamage(float damage, bool applyBoost = true)
    {
        float dealDamage = damage * BoostMultiplier;
        return dealDamage;
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab)
            return Instantiate(prefab, position, rotation);
        else
        {
            return null;
        }
    }

    public bool GetBool(BTBOOLVALUES value)
    {
        return (BoolDictionnary.ContainsKey(value) && BoolDictionnary[value]);
    }
    #endregion

    #region Collision Detection
    private void OnCollisionEnter(Collision collision)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(collision.gameObject, out RB_Health health))
        {
            _characterCollisions.Add(health);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(collision.gameObject, out RB_Health health) && _characterCollisions.Contains(health))
        {
            _characterCollisions.Remove(health);
        }
    }

    public List<RB_Health> GetCollisions()
    {
        return _characterCollisions;
    }
    #endregion
}