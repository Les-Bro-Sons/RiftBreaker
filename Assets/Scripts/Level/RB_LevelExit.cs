using System.Collections.Generic;
using MANAGERS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_LevelExit : MonoBehaviour
{
    public static RB_LevelExit Instance;

    [Header("General")]
    [SerializeField] private bool _goToNextSceneID = true;
    [SerializeField] private string _nextSceneName = "";
    [SerializeField] private int _nextSceneID = -1;

    [Header("Phase")]
    [SerializeField] private bool _availableOnPhase = false;
    [SerializeField] private PHASES _phaseNeeded = PHASES.Combat;

    [Header("Kill")]
    [SerializeField] private bool _availableOnKill = false;
    [SerializeField] private List<RB_Health> _enemyToKill = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (RB_Health enemyHealth in _enemyToKill)
        {
            enemyHealth.EventDeath.AddListener(CheckEnemyList);
        }
    }

    private void CheckEnemyList()
    {
        AreAllEnemyInListDead();
    }

    private bool AreAllEnemyInListDead()
    {
        foreach (RB_Health enemyHealth in _enemyToKill)
        {
            if (!enemyHealth.Dead)
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent<RB_PlayerController>(other.gameObject, out RB_PlayerController playerController)) //check if collider is the player
        {
            if (_availableOnKill && !AreAllEnemyInListDead()) //check if the enemy condition is met
            {
                return;
            }
            if (_availableOnPhase && _phaseNeeded != RB_LevelManager.Instance.CurrentPhase) //check if the phase condition is met
            {
                return;
            }

            print("PlayerWon");
            RB_AudioManager.Instance.PlaySFX("rift_closing", RB_PlayerController.Instance.transform.position, 0,1);
            if (_goToNextSceneID) //switch scene to next build index
            {
                RB_SceneTransitionManager.Instance.NewScene(SceneManager.GetActiveScene().buildIndex + 1); 
            }
            else
            {
                if (_nextSceneName != "")
                {
                    RB_SceneTransitionManager.Instance.NewScene(_nextSceneName); //switch scene by name
                }
                else
                {
                    RB_SceneTransitionManager.Instance.NewScene(_nextSceneID); //switch scene by ID
                }
            }
            
        }
    }
}
