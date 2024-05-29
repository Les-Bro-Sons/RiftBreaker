using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class RB_LevelManager : MonoBehaviour
{
    public static RB_LevelManager Instance;

    public PHASES CurrentPhase;
    [HideInInspector] public PHASES LastPhase;
    public SCENENAMES CurrentScene;

    [HideInInspector] public UnityEvent EventPlayerLost;
    [HideInInspector] public UnityEvent EventPlayerWon;

    public Dictionary<PHASES, List<GameObject>> _savedEnemiesInPhase = new();


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

    public void SwitchPhase()
    {
        LastPhase = CurrentPhase;
        
        switch(CurrentPhase)
        {
            case PHASES.Infiltration:
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
        EventPlayerLost?.Invoke();
    }

    public void PlayerWon()
    {
        EventPlayerWon?.Invoke();
    }
}