using MANAGERS;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RB_Portal : MonoBehaviour
{
    [HideInInspector] public UnityEvent EventEnterInPortal;

    [Header("General")]
    [SerializeField] protected bool _goToNextSceneID = true;
    [SerializeField] protected string _nextSceneName = "";
    [SerializeField] protected int _nextSceneID = -1;
    [SerializeField] protected bool _isSwitchingOnPortalOpening = false;
    [SerializeField] protected bool _isSaving = true;

    [Header("Visual")]
    [SerializeField] protected Vector3 _openedSize;
    [SerializeField] protected Vector3 _closedSize;
    [SerializeField] protected float _openedLightIntensity;
    [SerializeField] protected float _closedLightIntensity;
    [SerializeField] protected bool _showRobert = true;

    [Header("Component")]
    [SerializeField] protected RB_CollisionDetection _collisionDetection;
    [SerializeField] protected Transform _portalTransform;
    [SerializeField] protected GameObject _robert;
    [SerializeField] protected Light _portalLight;

    protected bool _isOpened = false; //for entering the portal
    protected bool _isDoingEndCutscene = false;

    protected virtual void Start()
    {
        _robert.SetActive(_showRobert);
    }

    public virtual void OpenPortal()
    {
        _isOpened = true;
        StartCoroutine(OpenPortalAnim());
    }

    public virtual void ClosePortal()
    {
        _isOpened = false;
        RB_AudioManager.Instance.PlaySFX("rift_closing", RB_PlayerController.Instance.transform.position, false, 0, 1);
        StartCoroutine(ClosePortalAnim());
    }

    protected void SwitchScene()
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

    protected virtual void EnterPortal(Transform entity = null)
    {
        EventEnterInPortal?.Invoke();

        if (entity == null) entity = RB_PlayerController.Instance.transform;
        StartCoroutine(EnterPortalAnimation(entity, true));
    }

    protected virtual IEnumerator OpenPortalAnim(float duration = 0.5f)
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

    protected virtual IEnumerator ClosePortalAnim(float duration = 0.5f)
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

    protected virtual IEnumerator EnterPortalAnimation(Transform enteringObject, bool closePortal = true)
    {
        if (closePortal) _isDoingEndCutscene = true;

        SpriteRenderer entitySpriteRenderer = null;
        Rigidbody objectRB = enteringObject.GetComponent<Rigidbody>();
        if (enteringObject == RB_PlayerController.Instance.transform)
        {
            RB_PlayerController.Instance.enabled = false;
            RB_PlayerMovement.Instance.enabled = false;
            RB_PlayerMovement.Instance.SetVelocity(Vector3.zero);
            entitySpriteRenderer = RB_PlayerController.Instance.PlayerSpriteRenderer;
        }
        if (enteringObject.TryGetComponent<Rigidbody>(out Rigidbody objectRigidbody))
        {
            objectRigidbody.velocity = Vector3.zero;
            objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        enteringObject.rotation = Quaternion.LookRotation((transform.position - enteringObject.position));

        float moveDuration = 0.5f;
        float timer = 0;
        Vector3 enterStartPosition = enteringObject.position;
        Vector3 destination = transform.position;
        destination += Vector3.back * 0.15f;

        RB_Camera.Instance.VirtualCam.Follow = transform;

        if (entitySpriteRenderer.TryGetComponent<RB_PlayerAnim>(out RB_PlayerAnim playerAnim))
        {
            playerAnim.ForceWalking = true; //force walking animation
            playerAnim.LockRotation = false;
        }

        while (timer < moveDuration) //move to portal
        {
            objectRB.MovePosition(Vector3.Lerp(enterStartPosition, destination, timer / moveDuration));
            timer += Time.deltaTime;
            yield return null;
        }
        objectRB.MovePosition(destination);
        objectRB.MoveRotation(Quaternion.LookRotation(Vector3.back));
        timer = 0;

        if (playerAnim)
        {
            playerAnim.ForceWalking = false; //stoping walking animation
        }

        float disappearDuration = 1;
        RB_AppearingAI appearingScript = enteringObject.AddComponent<RB_AppearingAI>(); //adding disolve component
        appearingScript.TargetDissolveAmount = 1;
        appearingScript.StartDissolveAmount = 0;
        appearingScript.TimeForAppearing = disappearDuration;
        while (timer < disappearDuration) //disolve
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
        while (timer < switchSceneDuration) //wait before switching scene
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        yield return null;
    }
}
