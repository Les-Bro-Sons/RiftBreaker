using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_LevelManager : MonoBehaviour
{
    public static RB_LevelManager Instance;

    public PHASES CurrentPhase;
    public SCENENAMES CurrentScene;

    [HideInInspector] public UnityEvent EventPlayerLost;
    [HideInInspector] public UnityEvent EventPlayerWon;

    public Dictionary<PHASES, List<GameObject>> _savedEnemiesInPhase;

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
        switch(CurrentPhase)
        {
            case PHASES.Infiltration:
                CurrentPhase = PHASES.Combat;
                break;
        }
        SpawnEnemiesInPhase(CurrentPhase);
    }

    public void SwitchPhase(PHASES phaseToSwitch)
    {
        CurrentPhase = phaseToSwitch;
        SpawnEnemiesInPhase(CurrentPhase);
    }

    public void SaveEnemyToPhase(PHASES phase, GameObject enemy)
    {
        _savedEnemiesInPhase[phase].Add(enemy);
        enemy.SetActive(false);
    }
    
    public void SpawnEnemiesInPhase(PHASES phase)
    {
        foreach (GameObject enemy in _savedEnemiesInPhase[phase])
        {
            enemy.SetActive(true);
            enemy.GetComponent<RB_Enemy>().Spawned();
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
