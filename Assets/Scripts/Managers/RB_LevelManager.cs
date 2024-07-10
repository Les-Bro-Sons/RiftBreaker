using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RB_LevelManager : MonoBehaviour
{
    public static RB_LevelManager Instance;

    public PHASES CurrentPhase;
    [HideInInspector] public PHASES LastPhase;
    public SCENENAMES CurrentScene;

    [HideInInspector] public UnityEvent EventPlayerLost;
    [HideInInspector] public UnityEvent EventPlayerWon;
    [HideInInspector] public UnityEvent EventSwitchPhase;

    public Dictionary<PHASES, List<GameObject>> _savedEnemiesInPhase = new();

    [Header("HUD SKILLS")]
    [SerializeField] private string _phaseInfiltration = $"SkillsPhase{PHASES.Infiltration}";
    [SerializeField] private string _phaseInfiltrationWithoutWnim = $"SkillsPhase{PHASES.Infiltration}WithoutAnim";
    [SerializeField] private string _phaseCombat = $"SkillsPhase{PHASES.Combat}";
    [SerializeField] private string _phaseBoss = $"SkillsPhase{PHASES.Boss}";


    private RB_Health _health;

    public GameObject ChargeSpecialAttackParticlePrefab;

    //Dialogues
    [Header("Dialogues")]
    [SerializeField] private RB_Dialogue _robertTalkLevelBeginning;

    public Vector3 BeginningPos = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        _savedEnemiesInPhase[PHASES.Infiltration] = new List<GameObject>();
        _savedEnemiesInPhase[PHASES.Boss] = new List<GameObject>();
        _savedEnemiesInPhase[PHASES.Combat] = new List<GameObject>();
    }

    private void Start()
    {
        RB_PlayerController.Instance.GetComponent<RB_Health>().EventDeath.AddListener(PlayerLost);
        RB_HUDManager.Instance.PlayAnimation(_phaseInfiltrationWithoutWnim);
        if(_robertTalkLevelBeginning != null)
            _robertTalkLevelBeginning.StartDialogue((int)CurrentScene);
        if (CurrentPhase == PHASES.Boss)
        {
            switch (CurrentScene)
            {
                case SCENENAMES.Boss1:
                    RB_HUDManager.Instance.BossHealthBar.Rb_health = RB_Mega_knight.Instance.GetComponent<RB_Health>();
                    RB_Mega_knight.Instance.EventPlayMKMusic.AddListener(PlayMKBossMusic);
                    break;
                case SCENENAMES.Boss2:
                    RB_HUDManager.Instance.BossHealthBar.Rb_health = RB_RobertLenec.Instance.GetComponent<RB_Health>();
                    RB_RobertLenec.Instance.EventPlayRobertMusic.AddListener(PlayRobertBossMusic);
                    break;
                case SCENENAMES.Boss3:
                    RB_HUDManager.Instance.BossHealthBar.Rb_health = RB_Yog.Instance.GetComponent<RB_Health>();
                    RB_Yog.Instance.EventPlayYogMusic.AddListener(PlayYogBossMusic);
                    break;
            }
            RB_HUDManager.Instance.PlayAnimation(_phaseBoss);
        }
        if (CurrentPhase == PHASES.Infiltration && CurrentScene != SCENENAMES.FirstCinematic && CurrentScene != SCENENAMES.EndCinematic && CurrentScene != SCENENAMES.Tuto)
        {
            RB_AudioManager.Instance.PlayMusic("Infiltration_Music");
        }

        BeginningPos = RB_PlayerAction.Instance.transform.position;

    }

    public void SwitchPhase()
    {
        LastPhase = CurrentPhase;
        
        switch(CurrentPhase)
        {
            case PHASES.Infiltration:
                RB_HUDManager.Instance.PlayAnimation(_phaseCombat);
                if (CurrentScene != SCENENAMES.Tuto)
                {
                    RB_AudioManager.Instance.PlayMusic("Combat_Music");
                }
                CurrentPhase = PHASES.Combat;
                break;
            case PHASES.Boss:


                break;
        }

        SpawnEnemiesInPhase(CurrentPhase);
        DespawnEnemiesIfNotInPhase(CurrentPhase);
        RB_UxVolumePhase.Instance.ActionUxSwitchPhase();

        EventSwitchPhase?.Invoke();
    }

    public void SwitchPhase(PHASES phaseToSwitch)
    {
        LastPhase = CurrentPhase;

        RB_HUDManager.Instance.PlayAnimation($"SkillsPhase{phaseToSwitch}");

        CurrentPhase = phaseToSwitch;
        SpawnEnemiesInPhase(CurrentPhase);
        DespawnEnemiesIfNotInPhase(CurrentPhase);

        RB_UxVolumePhase.Instance.ActionUxSwitchPhase();

        EventSwitchPhase?.Invoke();

        if (CurrentPhase == PHASES.Infiltration) 
        {
            RB_AudioManager.Instance.PlayMusic("Infiltration_Music");
        }
        
        if (CurrentPhase == PHASES.Combat) 
        {
            RB_AudioManager.Instance.PlayMusic("Combat_Music");
        }
    }

    public void SaveEnemyToPhase(PHASES phase, GameObject enemy)
    {
        _savedEnemiesInPhase[phase].Add(enemy);
        enemy.SetActive(false);
    }
    
    public void SpawnEnemiesInPhase(PHASES phase)
    {
        //if (!_savedEnemiesInPhase.ContainsKey(phase)) return;
        foreach (GameObject enemy in _savedEnemiesInPhase[phase])
        {
            if (enemy && enemy.TryGetComponent<RB_Enemy>(out RB_Enemy rbEnemy))
            {
                enemy.SetActive(true);
                rbEnemy.Spawned();
            }
        }
        //_savedEnemiesInPhase[phase].Clear();
    }

    public void DespawnEnemiesIfNotInPhase(PHASES phase)
    {
        switch(phase)
        {
            case PHASES.Infiltration:
                DespawnEnemiesInPhase(PHASES.Combat);
                DespawnEnemiesInPhase(PHASES.Boss);
                break;
            case PHASES.Combat:
                DespawnEnemiesInPhase(PHASES.Infiltration);
                DespawnEnemiesInPhase(PHASES.Boss);
                break;
            case PHASES.Boss:
                DespawnEnemiesInPhase(PHASES.Infiltration);
                DespawnEnemiesInPhase(PHASES.Combat);
                break;
        }
    }

    public void DespawnEnemiesInPhase(PHASES phase)
    {
        foreach (GameObject enemy in _savedEnemiesInPhase[phase])
        {
            if (enemy && enemy.TryGetComponent<RB_Enemy>(out RB_Enemy rbEnemy))
            {
                if (enemy.TryGetComponent<RB_TimeBodyRecorder>(out RB_TimeBodyRecorder timeBody)) timeBody.GoToFirstPointInTime();
                enemy.SetActive(false);
            }
        }
    }

    public void PlayerLost() 
    {
        StartCoroutine(PlayerLostUX());
        EventPlayerLost?.Invoke();
    }

    public IEnumerator PlayerLostUX()
    {
        RB_Camera.Instance.Zoom(0.5f, 1);
        yield return new WaitForSeconds(1.5f);
        RB_Camera.Instance.Zoom(1f, 0.3f);
        FullLevelRewind();
        yield return new WaitForSeconds(3f);
        RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, SceneManager.GetActiveScene().buildIndex);
        yield return null;
    } 

    public void PlayerWon()
    {
        EventPlayerWon?.Invoke();
    }

    public void FullLevelRewind()
    {
        RB_TimeManager.Instance.StartRewinding(REWINDENTITYTYPE.All ,true, true);
    }

    public void PlayMKBossMusic()
    {
        RB_AudioManager.Instance.PlayMusic("MK_Boss_Music");

    }

    public void PlayRobertBossMusic()
    {
        RB_AudioManager.Instance.PlayMusic("Robert_Boss_Music");

    }

    public void PlayYogBossMusic()
    {
        RB_AudioManager.Instance.PlayMusic("Yog_Boss_Music");
    }

}