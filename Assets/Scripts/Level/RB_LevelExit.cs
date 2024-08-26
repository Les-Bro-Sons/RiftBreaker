using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RB_LevelExit : RB_Portal
{
    public static RB_LevelExit Instance;

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
    }

    protected override void Start()
    {
        base.Start();

        ClosePortal();

        _collisionDetection.EventOnEntityEntered.AddListener(delegate { CheckForPlayerEntered(_collisionDetection.GetDetectedEntity()); });

        if (_availableOnKill)
        {
            foreach (RB_Health enemyHealth in _enemyToKill)
            {
                enemyHealth.EventDeath.AddListener(UpdatePortal);
            }
        }
        if (_availableOnPhase)
        {
            RB_LevelManager.Instance.EventSwitchPhase.AddListener(UpdatePortal);
        }
    }

    public void UpdatePortal()
    {
        if (CheckIfOpened() && _isOpened == false)
        {
            OpenPortal();
            if (_isSwitchingOnPortalOpening) EnterPortal();
        }
        else if (_isOpened == true)
        {
            ClosePortal();
        }
    }

    public bool CheckIfOpened()
    {
        if (_availableOnKill && !AreAllEnemyInListDead()) return false; //check if the enemy condition is met

        if (_availableOnPhase && _phaseNeeded != RB_LevelManager.Instance.CurrentPhase) return false; //check if the phase condition is met

        return true;
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

    private void CheckForPlayerEntered(List<GameObject> objects)
    {
        foreach(GameObject obj in objects)
        {
            if (!_isDoingEndCutscene && RB_Tools.TryGetComponentInParent<RB_PlayerController>(obj, out RB_PlayerController playerController)) //check if collider is the player
            {
                if (_isOpened)
                {
                    EnterPortal();
                }

                return;
            }
        }
    }

    protected override IEnumerator EnterPortalAnimation(Transform enteringObject, bool closePortal = true)
    {
        yield return base.EnterPortalAnimation(enteringObject, closePortal);

        if (closePortal)
        {
            SwitchScene();
        }

        yield return null;
    }
}
