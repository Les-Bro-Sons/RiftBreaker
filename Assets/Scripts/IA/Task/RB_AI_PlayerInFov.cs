using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RB_AI_PlayerInFov : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Rigidbody _rb;

    private Transform _transform;
    private static int _layerMaskPlayer = 1 << 7;
    //private Animator _animator;

    [Header("Header")]
    //private CanvasGroup _canvasUi;
    //private Image _imageSpotBar;

    private bool _hasFocusedUx = false;

    private bool _isLoadingSpotBar = false;
    private bool _isUnloadingSpotBar = false;

    private bool _lookAtSuspicious;

    private float _lastTimeCheckTarget = 0;
    private float _checkTargetDelay = 1;

    public RB_AI_PlayerInFov(RB_AI_BTTree BtParent, float range, bool lookAtSuspicious = true)
    {
        _btParent = BtParent;
        _rb = _btParent.AiRigidbody;
        _transform = _btParent.transform;
        _lookAtSuspicious = lookAtSuspicious;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;
        switch (RB_LevelManager.Instance.CurrentPhase)
        {
            case PHASES.Infiltration:
                if (_btParent.GetBool(BTBOOLVALUES.IsAttacking)) return _state = BTNodeState.SUCCESS;
                _state = InfiltrationCheck();
                break;
            case PHASES.Boss:
            case PHASES.Combat:
                _state = InfiltrationCheck();
                break;
            default:
                break;
        }
        return _state;
    }

    //////INFILTRATION//////
    public BTNodeState InfiltrationCheck()
    {
        if (_btParent.GetBool(BTBOOLVALUES.IsAttacking))
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
            Transform t = (Transform)_btParent.Root.GetData("target");
            if (Time.time > _lastTimeCheckTarget + _checkTargetDelay || (t != null && ((RB_Tools.TryGetComponentInParent<RB_Health>(t.gameObject, out RB_Health targetHealth) && targetHealth.Dead))))
            {
                //searching for a target
                _lastTimeCheckTarget = Time.time;
                List<Collider> colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange * 2, _layerMaskPlayer).ToList();
                foreach (Collider collider in colliders.ToList<Collider>())
                {
                    if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth))
                    {
                        if (enemyHealth.Dead || enemyHealth.Team == _btParent.AiHealth.Team)
                        {
                            colliders.Remove(collider);
                        }
                    }
                    else
                    {
                        colliders.Remove(collider);
                    }
                }
                if (colliders.Count > 0)
                {
                    RB_Tools.TryGetComponentInParent<RB_Health>(colliders[0].gameObject, out RB_Health enemyHealth);
                    _btParent.Root.SetData("target", enemyHealth.transform);
                    t = enemyHealth.transform;
                }
                else
                {
                    t = null;
                }
            }

            Transform target = (Transform)t;

            if (target == null)
            {
                //Debug.Log("target est toujours null après la recherche.");
                _state = BTNodeState.FAILURE;
                return _state;
            }
            //else
            //    Debug.Log("Cible actuelle: " + t.GetType().Name.ToString());

            if (_btParent.GetBool(BTBOOLVALUES.IsTargetSpotted))
            {
                if (Vector3.Distance(_transform.position, target.position) > _btParent.FovRange + 0.5f)
                {
                    UnloadSpotBar();
                    _btParent.LastTargetPos = target.position;
                    if (_btParent.ImageSpotBar.fillAmount <= 0)
                    {
                        _btParent.BoolDictionnary[BTBOOLVALUES.GoToLastTargetPos] = true;
                        //UnloadCanvas();
                        _btParent.BoolDictionnary[BTBOOLVALUES.IsTargetSpotted] = false;
                        _state = BTNodeState.FAILURE;
                    }
                    else
                    {
                        //LoadCanvas();
                        _state = BTNodeState.RUNNING;
                    }
                }
                else
                {
                    //UnloadCanvas();
                    return _state = BTNodeState.SUCCESS;
                }
            }
            else
            {
                if (SeesPlayer(target))
                {
                    // _animator.SetBool("Attacking", true);
                    // _animator.SetBool("Walking", false);

                    //_btParent.ImageSpotBar.fillAmount = 0.0f; //DECOMENTER
                    //_btParent.CanvasUi.alpha = 0.0f;

                    if (!_btParent.GetBool(BTBOOLVALUES.HasACorrectView))
                    {
                        LoadSpotBar();
                        //look at player
                        if (_lookAtSuspicious)
                        {
                            _rb.MoveRotation(Quaternion.LookRotation(target.position - _transform.position));
                        }
                    }

                    if (_btParent.ImageSpotBar.fillAmount >= 1)
                    {
                        _btParent.BoolDictionnary[BTBOOLVALUES.IsTargetSpotted] = true;
                        if (!_hasFocusedUx)
                        {
                            _btParent.UxFocus();
                            _hasFocusedUx = true;
                        }
                        _state = BTNodeState.SUCCESS;
                        return _state;
                    }
                    else
                    {
                        _state = BTNodeState.RUNNING;
                        return _state;
                    }
                    
                }
                else
                {
                    UnloadSpotBar();
                }
                /*else if (_btParent.ImageSpotBar.fillAmount > 0)
                {
                    _btParent.transform.forward = (target.transform.position - _btParent.transform.position).normalized;
                    _state = BTNodeState.RUNNING;
                    return _state;
                }*/
            }
            
        }
        _hasFocusedUx = false;
        _state = BTNodeState.FAILURE;
        return _state;
    }

    bool SeesPlayer(Transform target)
    {
        _btParent.IsPlayerInSight = false;
        Vector3 targetDir = target.position - _transform.position;
        float angle = Vector3.Angle(targetDir, _transform.forward);
        if (angle >= -_btParent.FovAngle / 2 && angle <= _btParent.FovAngle / 2)
        {
            RaycastHit hit;

            if (Physics.Raycast(_transform.position, targetDir, out hit, _btParent.FovRange, ~((1 << 6) | (1 << 10))))
            {
                _btParent.IsPlayerInSight = true;
                RB_Tools.TryGetComponentInParent<RB_Health>(hit.transform.gameObject, out RB_Health hitHealth);
                if (hitHealth && hitHealth.Team != _btParent.AiHealth.Team)
                {
                    _btParent.Root.SetData("target", hitHealth.transform);
                }
                else
                {
                    return false;
                }

                return true;

                // Dessine un rayon vert si le joueur est détecté.
                Debug.DrawLine(_transform.position, hit.point, Color.green);

                Debug.Log("In View");

                if (!_btParent.GetBool(BTBOOLVALUES.HasACorrectView))
                {
                    Debug.Log("loading");
                    LoadSpotBar();
                }
                else
                {
                    if (!_hasFocusedUx)
                    {
                        _btParent.UxFocus();
                        _hasFocusedUx = true;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    private void LoadSpotBar()
    {

        if (!_isLoadingSpotBar)
        {
            _isLoadingSpotBar = true;
            _isUnloadingSpotBar = false;

        }
        Transform target = (Transform)_btParent.Root.GetData("target");
        _btParent.ImageSpotBar.fillAmount += Time.deltaTime / Mathf.Lerp(_btParent.MinDistDurationToLoadSpotBar, _btParent.MaxDistDurationToLoadSpotBar, Vector3.Distance(target.position, _transform.position) / _btParent.FovRange);

        if (_btParent.ImageSpotBar.fillAmount >= 1.0f)
        {
            _btParent.ImageSpotBar.fillAmount = 1.0f;
            _isLoadingSpotBar = false;
            _btParent.BoolDictionnary[BTBOOLVALUES.HasACorrectView] = true;
        }
    }

    private void UnloadSpotBar()
    {

        if (!_isUnloadingSpotBar)
        {
            _isLoadingSpotBar = false;
            _isUnloadingSpotBar = true;
        }

        _btParent.ImageSpotBar.fillAmount -= Time.deltaTime / _btParent.DurationToUnloadSpotBar;

        if (_btParent.ImageSpotBar.fillAmount <= 0.0f)
        {
            _btParent.ImageSpotBar.fillAmount = 0.0f;
            _isUnloadingSpotBar = false;
            _btParent.BoolDictionnary[BTBOOLVALUES.HasACorrectView] = false;
        }
    }

}