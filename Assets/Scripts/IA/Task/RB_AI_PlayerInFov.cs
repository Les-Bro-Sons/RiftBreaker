using BehaviorTree;
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
                if (_btParent.GetBool("IsAttacking")) return _state = BTNodeState.SUCCESS;
                _state = InfiltrationCheck();
                break;
            case PHASES.Boss:
            case PHASES.Combat:
                _state = CombatCheck();
                break;
            default:
                break;
        }
        return _state;
    }


    ////COMBAT////
    private BTNodeState CombatCheck()
    {
        if (!_btParent.GetBool("IsTargetSpotted"))
        {
            object t = GetData("target");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange, _layerMaskPlayer);
                if (colliders.Length > 0) //A MODIFIER
                {
                    _btParent.Root.SetData("target", colliders[0].transform);
                    t = colliders[0].transform;

                    _btParent.BoolDictionnary["IsTargetSpotted"] = true;
                }
            }
            else
            {
                _btParent.BoolDictionnary["IsTargetSpotted"] = false;
            }

            Transform target = (Transform)t;

            if (target == null)
            {
                _state = BTNodeState.FAILURE;
                return _state;
            }
        }

        if (_btParent.GetBool("IsTargetSpotted"))
            _state = BTNodeState.SUCCESS;
        else
            _state = BTNodeState.FAILURE;

        return _state;
    }


    //////INFILTRATION//////
    public BTNodeState InfiltrationCheck()
    {
        if (_btParent.GetBool("IsAttacking"))
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
            object t = _btParent.Root.GetData("target");
            if (t == null)
            {
                //Debug.Log("Recherche de la cible...");
                Collider[] colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange, _layerMaskPlayer);
                if (colliders.Length > 0)
                {
                    //Debug.Log("Cible trouvée, assignation en cours...");
                    _btParent.Root.SetData("target", colliders[0].transform);
                    t = colliders[0].transform;
                }
                //else
                //    Debug.Log("Aucune cible trouvée dans la portée.");
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

            if (_btParent.GetBool("IsTargetSpotted"))
            {
                if (Vector3.Distance(_transform.position, target.position) > _btParent.FovRange)
                {
                    UnloadSpotBar();
                    _btParent.LastTargetPos = target.position;
                    if (_btParent.ImageSpotBar.fillAmount <= 0)
                    {
                        _btParent.BoolDictionnary["GoToLastTargetPos"] = true;
                        //UnloadCanvas();
                        _btParent.BoolDictionnary["IsTargetSpotted"] = false;
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
                    Debug.Log(target.name);

                    if (!_btParent.GetBool("HasACorrectView"))
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
                        _btParent.BoolDictionnary["IsTargetSpotted"] = true;
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
        Vector3 targetDir = target.position - _transform.position;
        float angle = Vector3.Angle(targetDir, _transform.forward);
        if (angle >= -_btParent.FovAngle / 2 && angle <= _btParent.FovAngle / 2)
        {
            RaycastHit hit;

            Debug.DrawLine(_transform.position, _transform.position + targetDir.normalized * _btParent.FovRange, Color.red);
            if (Physics.Raycast(_transform.position, targetDir, out hit, _btParent.FovRange, ~((1 << 6) | (1 << 10))))
            {
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

                if (!_btParent.GetBool("HasACorrectView"))
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

        _btParent.ImageSpotBar.fillAmount += Time.deltaTime / _btParent.DurationToLoadSpotBar;

        if (_btParent.ImageSpotBar.fillAmount >= 1.0f)
        {
            _btParent.ImageSpotBar.fillAmount = 1.0f;
            _isLoadingSpotBar = false;
            _btParent.BoolDictionnary["HasACorrectView"] = true;
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
            _btParent.BoolDictionnary["HasACorrectView"] = false;
        }
    }

}