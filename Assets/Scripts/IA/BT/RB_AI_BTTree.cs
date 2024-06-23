using BehaviorTree;
using System.Collections.Generic;
using MANAGERS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.Events;

public class RB_AI_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    private List<PHASES> _infiltrationPhases = new();
    private List<PHASES> _combatPhases = new();

    public Dictionary<string, bool> BoolDictionnary = new Dictionary<string, bool>();
    private List<RB_Health> _characterCollisions = new();

    [Header("Main Parameters")]
    public ENEMYCLASS AiType = ENEMYCLASS.Light;
    public float MovementSpeed = 4f;
    public float MovementSpeedAggro = 8f;
    public float MovementSpeedFlee = 6f;
    public float AttackSpeed = 0.2f;
    public float BoostMultiplier = 1f;
    public ParticleSystem BoostParticle;
    public bool IsDecorative = false;

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
    public float DurationToLoadSpotBar = 0.5f;
    public float DurationToUnloadSpotBar = 1f;
    [HideInInspector] public Vector3 LastTargetPos;
    public bool IsPlayerInSight = false; //used only by other scripts

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

    [Header("Infiltration")]
    [SerializeField] public float InfSlashRange;
    [SerializeField] public float InfSlashDamage;
    [SerializeField] public float InfSlashKnockback;
    [SerializeField] public float InfSlashDelay;
    [SerializeField] public float InfSlashCollisionSize = 3;
    [SerializeField] public float InfSpottedMoveSpeed = 11;
    [SerializeField] public GameObject InfSlashParticles;
    [HideInInspector] public UnityEvent EventOnSpotted;

    [Header("Faible")]
    [SerializeField] public float SlashRange;
    [SerializeField] public float SlashDamage;
    [SerializeField] public float SlashKnockback;
    [SerializeField] public float SlashDelay;
    [SerializeField] public float SlashCollisionSize = 3;
    [SerializeField] public GameObject SlashParticles;

    [Header("Moyen")]
    [SerializeField] public GameObject ArrowPrefab;
    [SerializeField] public float BowRange;
    [SerializeField] public float BowDamage;
    [SerializeField] public float BowKnockback;
    [SerializeField] public float BowDelay;
    [SerializeField] public float ArrowSpeed;
    [SerializeField] public float ArrowDistance;

    [Header("Fort")]
    [SerializeField] public GameObject HeavyArrowPrefab;
    [SerializeField] public float HeavyBowRange;
    [SerializeField] public float HeavyBowDamage;
    [SerializeField] public float HeavyBowKnockback;
    [SerializeField] public float HeavyBowDelay;
    [SerializeField] public float HeavyBowProjectileNumber = 3;
    [SerializeField] public float HeavyBowDelayBetweenProjectile = 0.2f;
    [SerializeField] public float HeavyArrowSpeed;
    [SerializeField] public float HeavyArrowDistance;

    [SerializeField] public float HeavySlashRange;
    [SerializeField] public float HeavySlashDamage;
    [SerializeField] public float HeavySlashKnockback;
    [SerializeField] public float HeavySlashFirstDelay;
    [SerializeField] public float HeavySlashComboDelay;
    [SerializeField] public float HeavySlashCollisionSize = 3;
    public int CurrentHeavySlashCombo = 0;
    [SerializeField] public int MaxHeavySlashCombo = 5;
    [SerializeField] public GameObject HeavySlashParticles;

    [Header("Tower")]
    [SerializeField] public float ExplosionDamage = 30;
    [SerializeField] public float ExplosionKnockback = 15;
    [SerializeField] public float ExplosionRadius = 3;
    [SerializeField] public float ExplosionDelay = 1;
    [SerializeField] public float ExplosionStartRange = 1;
    [SerializeField] public GameObject ExplosionParticles;

    private void Awake()
    {
        /*
        if (CanvasUi.alpha > 0f)
            CanvasUi.alpha = 0;*/

        AiMovement = GetComponent<RB_AiMovement>();
        if (AiMovement == null)
            AiMovement.AddComponent<RB_AiMovement>();
        AiHealth = GetComponent<RB_Health>();
        AiRigidbody = GetComponent<Rigidbody>();
        AiAnimator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        SpotCanvasAlpha();
        ApplyBoostParticles();
    }

    private void ApplyBoostParticles()
    {
        if (BoostMultiplier > 1 && !BoostParticle.isPlaying) 
        {
            BoostParticle.Play();
        }
        else if (BoostParticle.isPlaying)
        {
            BoostParticle.Stop();
        }
    }

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

    public void UxFocus()
    {
        GameObject spawnSpriteUxDetected = Instantiate(_prefabUxDetectedReadyMark, transform);
        spawnSpriteUxDetected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        RB_AudioManager.Instance.PlaySFX("hey", transform.position, false, 0,1);
        EventOnSpotted?.Invoke();
    }

    protected override RB_BTNode SetupTree()
    {
        if (AiAnimator) AiAnimator.SetFloat("EnemyID", (int)AiType);
        else Debug.LogWarning("NO AiAnimator in " + gameObject.name);
        
        _infiltrationPhases.Add(PHASES.Infiltration);
        _combatPhases.Add(PHASES.Combat);
        _combatPhases.Add(PHASES.Boss);
        this.SplineContainer = RB_SplineManager.Splines;

        if (StaticPositionOnStart) StaticPosition = transform.position;

        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // sequence decoration Infiltration
            {
                new RB_AICheck_Phase(_infiltrationPhases),
                new RB_AICheck_Bool(this, IsDecorative),
                new RB_AI_BecomeDecoration(this),
            }),

            new RB_BTSequence(new List<RB_BTNode> // Sequence CHECK PHASE INFILTRATION
            {
                new RB_AICheck_Phase(_infiltrationPhases),
                new RB_AICheck_Bool(this, !IsDecorative),
                new RB_BTSelector(new List<RB_BTNode>  // Sequence INFILTRATION
                {
                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AICheck_EnemyTouchDetection(this, true),
                        new RB_AI_DoFailure(),
                    }),

                    new RB_BTSequence(new List<RB_BTNode>
                    {
                        new RB_AI_ReverseState(this, new RB_AICheck_Bool(this, "IsTargetSpotted")),
                        new RB_AI_StaticWatchOut(this),
                        new RB_AI_ToState(new RB_AI_PlayerInFov(this, FovRange), BTNodeState.SUCCESS),
                    }),

                    new RB_BTSelector(new List<RB_BTNode> // selector ai completely lost sight of target
                    {
                        new RB_BTSequence(new List<RB_BTNode> // sequence spot target again
                        {
                            new RB_AICheck_Bool(this, "GoToLastTargetPos"),
                            new RB_AI_PlayerInFov(this, FovRange),
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
                        new RB_AICheck_Bool(this, "IsTargetSpotted"),
                        new RB_AI_PlayerInFov(this, SpottedFovRange),
                        new RB_AI_GoToTarget(this, InfSpottedMoveSpeed, InfSlashRange),
                        new RB_AI_Attack(this, -1),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Check Spot
                    {
                        new RB_AI_PlayerInFov(this, FovRange),
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
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Moyen
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Medium),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //spot sequence
                            {
                                new RB_AICheck_Bool(this, "PlayerSpottedInCombat"),
                                new RB_BTSelector(new List<RB_BTNode>
                                {
                                    new RB_BTSequence(new List<RB_BTNode> //flee sequence
                                    {
                                        new RB_AI_ReverseState(this, new RB_AICheck_Bool(this, "IsAttacking")),
                                        new RB_AICheck_IsTargetClose(this, 5),
                                        new RB_AI_FleeFromTarget(this, 5, MovementSpeedFlee),
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
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest),
                                new RB_AI_SetBool(this, "PlayerSpottedInCombat", true),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> // Sequence Fort
                    {
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
                                        new RB_AI_ReverseState(this, new RB_AICheck_Bool(this, "IsAttacking")),
                                        new RB_AI_ReverseState(this, new RB_AICheck_Bool(this, "HeavyAttackSlash")), //when bow attack
                                        new RB_AICheck_IsTargetClose(this, HeavyBowRange/1.5f),
                                        new RB_AI_FleeFromTarget(this, HeavyBowRange/1.5f, MovementSpeedFlee),
                                    }),

                                    new RB_BTSequence(new List<RB_BTNode> //3 projectile sequence
                                    {
                                        new RB_AI_ReverseState(this, new RB_AICheck_Bool(this, "HeavyAttackSlash")), // to switch attacks
                                        new RB_AI_GoToTarget(this, MovementSpeedAggro, HeavyBowRange),
                                        new RB_AI_Attack(this, 0),
                                    }),
                                    
                                    new RB_BTSequence(new List<RB_BTNode> //heavy slash sequence
                                    {
                                        new RB_AICheck_Bool(this, "HeavyAttackSlash"), // to switch attacks
                                        new RB_AI_GoToTarget(this, MovementSpeedAggro, HeavySlashRange),
                                        new RB_AI_Attack(this, 1),
                                    }),
                                }),
                            }),

                            new RB_BTSequence(new List<RB_BTNode>
                            {
                                new RB_AI_Task_DefaultPatrol(this),
                            }),
                        }),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Pawn
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Pawn),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_AI_GoToTarget(this, MovementSpeedAggro, SlashRange),
                                new RB_AI_Attack(this, 0), //slash
                            }),
                        }),
                    }),

                    new RB_BTSequence(new List<RB_BTNode> //sequence Tower
                    {
                        new RB_AICheck_Class(AiType, ENEMYCLASS.Tower),
                        new RB_BTSelector(new List<RB_BTNode>
                        {
                            new RB_BTSequence(new List<RB_BTNode> //Spot sequence
                            {
                                new RB_AICheck_EnemyInRoom(this, TARGETMODE.Closest, true),
                                new RB_AI_GoToTarget(this, MovementSpeedAggro, ExplosionStartRange),
                                new RB_AI_Attack(this, 0), //explode
                            }),
                        }),
                    }),
                }),
            }),
        });;

        return root;
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

    public bool GetBool(string name)
    {
        return (BoolDictionnary.ContainsKey(name) && BoolDictionnary[name]);
    }
}