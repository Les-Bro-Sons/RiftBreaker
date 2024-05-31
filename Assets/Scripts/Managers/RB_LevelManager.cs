using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private string _phaseCombat = $"SkillsPhase{PHASES.Combat}";


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
    }

    private void Start()
    {
        RB_PlayerController.Instance.GetComponent<RB_Health>().EventDeath.AddListener(PlayerLost);
        RB_HUDManager.Instance.PlayAnimation(_phaseInfiltration);
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
        }

        SpawnEnemiesInPhase(CurrentPhase);
        RB_UxVolumePhase.Instance.ActionUxSwitchPhase();
    }

    public void SwitchPhase(PHASES phaseToSwitch)
    {
        LastPhase = CurrentPhase;
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