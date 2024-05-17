using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_LevelManager : MonoBehaviour
{
    public PHASES Phase;
    public static RB_LevelManager Instance;

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
    public void SwitchPhase()
    {
        
    }
    
    public void SpawnCombatEnemies()
    {
        
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
