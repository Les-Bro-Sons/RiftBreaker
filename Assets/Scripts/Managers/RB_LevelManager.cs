using System.Collections;
using System.Collections.Generic;
using MANAGERS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RB_LevelManager : MonoBehaviour
{
    public static RB_LevelManager Instance;

    public PHASES CurrentPhase;
    [HideInInspector] public PHASES LastPhase;
    public SCENENAMES CurrentScene;

    [HideInInspector] public UnityEvent EventPlayerLost;
    [HideInInspector] public UnityEvent EventPlayerWon;

    public Dictionary<PHASES, List<GameObject>> _savedEnemiesInPhase = new();

    [Header("HUD SKILLS")]
    [SerializeField] private string _phaseInfiltration = $"SkillsPhase{PHASES.Infiltration}";
    [SerializeField] private string _phaseInfiltrationWithoutWnim = $"SkillsPhase{PHASES.Infiltration}WithoutAnim";
    [SerializeField] private string _phaseCombat = $"SkillsPhase{PHASES.Combat}";
    [SerializeField] private string _phaseBoss = $"SkillsPhase{PHASES.Boss}";




    public GameObject ChargeSpecialAttackParticlePrefab;


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
        if(CurrentPhase == PHASES.Boss)
        {
            switch (CurrentScene)
            {
                case SCENENAMES.Boss1:
                    RB_HUDManager.Instance.BossHealthBar.Rb_health = RB_Mega_knight.Instance.GetComponent<RB_Health>();
                    break;
                case SCENENAMES.Boss2:
                    RB_HUDManager.Instance.BossHealthBar.Rb_health = RB_RobertLenec.Instance.GetComponent<RB_Health>();
                    break;
                case SCENENAMES.Boss3:
                    /*RB_HUDManager.Instance.BossHealthBar.Rb_health = */
                    break;
            }
            RB_HUDManager.Instance.PlayAnimation(_phaseBoss);
        }
    }

    public void SwitchPhase()
    {
        LastPhase = CurrentPhase;
        
        switch(CurrentPhase)
        {
            case PHASES.Infiltration:
                RB_HUDManager.Instance.PlayAnimation(_phaseCombat);
                CurrentPhase = PHASES.Combat;
                break;
            case PHASES.Boss:


                break;
        }



        SpawnEnemiesInPhase(CurrentPhase);
        RB_UxVolumePhase.Instance.ActionUxSwitchPhase();
    }

    public void SwitchPhase(PHASES phaseToSwitch)
    {
        LastPhase = CurrentPhase;

        RB_HUDManager.Instance.PlayAnimation($"SkillsPhase{phaseToSwitch}");

        CurrentPhase = phaseToSwitch;
        SpawnEnemiesInPhase(CurrentPhase);

        RB_UxVolumePhase.Instance.ActionUxSwitchPhase();
    }

    public void SaveEnemyToPhase(PHASES phase, GameObject enemy)
    {
        _savedEnemiesInPhase[phase].Add(enemy);
        enemy.SetActive(false);
    }
    
    public void SpawnEnemiesInPhase(PHASES phase)
    {
        if (!_savedEnemiesInPhase.ContainsKey(phase)) return;
        foreach (GameObject enemy in _savedEnemiesInPhase[phase])
        {
            if (enemy && enemy.TryGetComponent<RB_Enemy>(out RB_Enemy rbEnemy))
            {
                enemy.SetActive(true);
                rbEnemy.Spawned();
            }
        }
        _savedEnemiesInPhase[phase].Clear();
    }

    public void PlayerLost() 
    {
        StartCoroutine(PlayerLostUX());
        EventPlayerLost?.Invoke();
        RB_AudioManager.Instance.PlaySFX("scream-no", RB_PlayerController.Instance.transform.position, 0, 1);
    }

    public IEnumerator PlayerLostUX()
    {
        RB_Camera.Instance.Zoom(0.5f, 1);
        yield return new WaitForSeconds(3);
        RB_Camera.Instance.Zoom(1f, 0.3f);
        FullLevelRewind();
        yield return null;
    } 

    public void PlayerWon()
    {
        EventPlayerWon?.Invoke();
    }

    public void FullLevelRewind()
    {
        RB_TimeManager.Instance.StartRewinding(true, true);
    }
}