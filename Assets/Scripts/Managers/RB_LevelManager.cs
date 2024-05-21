using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_LevelManager : MonoBehaviour
{
    public PHASES Phase;
    public SCENENAMES CurrentScene;
    public static RB_LevelManager Instance;

    public static event Action OnGameStarted;
    public static event Action OnGamePhase1Ended;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }
    }

    private void Start()
    {
        OnGameStarted?.Invoke();
    }
    public void SwitchPhase()
    {
        if (CurrentScene != SCENENAMES.Boss1 && CurrentScene != SCENENAMES.Boss2 && CurrentScene != SCENENAMES.Boss3)
        {
            Phase = PHASES.Infiltration;
        }
        else
        {
            Phase = PHASES.Boss;
        }
    }
    
    public void SpawnCombatEnemies()
    {
        if (Phase == PHASES.Combat)
        {
            //Spawn combat ennemies
        }
    }

    public void PlayerLost() 
    {
        
    }

    public void PlayerWon()
    {

    }

    public void Rewinding()
    {

    }
    public void StopRewinding() 
    {

    }
}
