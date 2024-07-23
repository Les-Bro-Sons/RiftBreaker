using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RB_LevelExit : MonoBehaviour
{
    public static RB_LevelExit Instance;

    [HideInInspector] public UnityEvent EventEnterInPortal;

    [Header("General")]
    [SerializeField] private bool _goToNextSceneID = true;
    [SerializeField] private string _nextSceneName = "";
    [SerializeField] private int _nextSceneID = -1;
    [SerializeField] private bool _isSwitchingOnPortalOpening = false;
    [SerializeField] private bool _isSaving = true;

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
    private bool _isDoingEndCutscene = false;

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

        _collisionDetection.EventOnEntityEntered.AddListener(delegate { CheckForPlayerEntered(_collisionDetection.GetDetectedEntity()); });

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

    private void EnterPortal()
    {
        EventEnterInPortal?.Invoke();

        StartCoroutine(EnterPortalAnimation(RB_PlayerController.Instance.transform, true));
    }

    private void SwitchScene()
    {
        if (_goToNextSceneID) //switch scene to next build index
        {
            if (_isSaving)
            {
                RB_SaveManager.Instance.SaveToJson();
            }
            RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, SceneManager.GetActiveScene().buildIndex + 1); //go to next scene ID
        }
        else
        {
            if (_nextSceneName != "")
            {
                RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, _nextSceneName); //switch scene by name
            }
            else
            {
                RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, _nextSceneID); //switch scene by ID
            }
        }
    }

    private IEnumerator EnterPortalAnimation(Transform enteringObject, bool closePortal = true)
    {
        if (closePortal) _isDoingEndCutscene = true;

        Rigidbody objectRB = enteringObject.GetComponent<Rigidbody>();
        if (enteringObject == RB_PlayerController.Instance.transform)
        {
            RB_PlayerController.Instance.enabled = false;
        }
        if (enteringObject.TryGetComponent<Rigidbody>(out Rigidbody objectRigidbody))
        {
            objectRigidbody.velocity = Vector3.zero;
        }

        float moveDuration = 0.5f;
        float timer = 0;
        Vector3 enterStartPosition = enteringObject.position;
        Vector3 destination = transform.position;
        destination += Vector3.back * 0.15f;

        RB_Camera.Instance.VirtualCam.Follow = transform;

        if (TryGetComponent<RB_PlayerAnim>(out RB_PlayerAnim playerAnim)) playerAnim.ForceWalking = true;

        while (timer < moveDuration)
        {
            objectRB.MovePosition(Vector3.Lerp(enterStartPosition, destination, timer / moveDuration));
            timer += Time.deltaTime;
            yield return null;
        }
        objectRB.MovePosition(destination);
        timer = 0;

        if (playerAnim) playerAnim.ForceWalking = false;

        float disappearDuration = 1;
        RB_AppearingAI appearingScript = enteringObject.AddComponent<RB_AppearingAI>();
        appearingScript.TargetDissolveAmount = 1;
        appearingScript.StartDissolveAmount = 0;
        appearingScript.TimeForAppearing = disappearDuration;
        while (timer < disappearDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        if (closePortal)
        {
            ClosePortal();
        }

        float switchSceneDuration = 0.75f;
        while (timer < switchSceneDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        if (closePortal)
        {
            SwitchScene();
        }

        yield return null;
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
