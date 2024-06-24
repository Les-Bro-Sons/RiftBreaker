using System.Collections;
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

    [Header("Visual")]
    [SerializeField] private Vector3 _openedSize;
    [SerializeField] private Vector3 _closedSize;
    [SerializeField] private float _openedLightIntensity;
    [SerializeField] private float _closedLightIntensity;
    [SerializeField] private bool _showRobert = true;

    [Header("Component")]
    [SerializeField] private RB_CollisionDetection _collisionDetection;
    [SerializeField] private Transform _portalTransform;
    [SerializeField] private GameObject _robert;
    [SerializeField] private Light _portalLight;

    private bool _isOpened = false; //for entering the portal

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ClosePortal();

        _collisionDetection.EventOnObjectEntered.AddListener(delegate { CheckForPlayerEntered(_collisionDetection.GetDetectedObjects()); });

        _robert.SetActive(_showRobert);

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
            if (RB_Tools.TryGetComponentInParent<RB_PlayerController>(obj, out RB_PlayerController playerController)) //check if collider is the player
            {
                if (_isOpened)
                {
                    if (_goToNextSceneID) //switch scene to next build index
                    {
                        RB_SaveManager.Instance.SaveToJson();
                        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1); //go to next scene ID
                    }
                    else
                    {
                        if (_nextSceneName != "")
                        {
                            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), _nextSceneName); //switch scene by name
                        }
                        else
                        {
                            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), _nextSceneID); //switch scene by ID
                        }
                    }
                }

                return;
            }
        }
    }

    public void OpenPortal()
    {
        _isOpened = true;
        StartCoroutine(OpenPortalAnim());
    }

    public void ClosePortal()
    {
        _isOpened = false;
        RB_AudioManager.Instance.PlaySFX("rift_closing", RB_PlayerController.Instance.transform.position, false, 0, 1);
        StartCoroutine(ClosePortalAnim());
    }

    private IEnumerator OpenPortalAnim(float duration = 0.5f)
    {
        float timer = 0;
        while (timer < duration)
        {
            _portalTransform.localScale = Vector3.Lerp(_closedSize, _openedSize, timer / duration);
            _portalLight.intensity = Mathf.Lerp(_closedLightIntensity, _openedLightIntensity, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        _portalTransform.localScale = _openedSize;

        yield return null;
    }

    private IEnumerator ClosePortalAnim(float duration = 0.5f)
    {
        float timer = 0;
        while (timer < duration)
        {
            _portalTransform.localScale = Vector3.Lerp(_openedSize, _closedSize, timer / duration);
            _portalLight.intensity = Mathf.Lerp(_openedLightIntensity, _closedLightIntensity, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        _portalTransform.localScale = _closedSize;

        yield return null;
    }
}
