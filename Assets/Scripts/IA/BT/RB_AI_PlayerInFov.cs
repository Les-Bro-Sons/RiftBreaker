using BehaviorTree;
using UnityEngine;

public class RB_AI_PlayerInFov : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    private Transform _transform;
    private static int _layerMaskPlayer = 1 << 7;
    //private Animator _animator;

    [Header("Header")]
    //private CanvasGroup _canvasUi;
    //private Image _imageSpotBar;
    private float _currentValueFillSpotBar = 0.0f;
    private float _currentValueAlphaCanvas = 0.0f;

    private bool _isLoadingCanvas = false;
    private bool _isUnloadingCanvas = false;

    private bool _hasACorrectView = false;
    private bool _hasFocusedUx = false;

    private bool _isLoadingSpotBar = false;
    private bool _isUnloadingSpotBar = false;

    public RB_AI_PlayerInFov(RB_AI_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;
        switch (RB_LevelManager.Instance.CurrentPhase)
        {
            case PHASES.Infiltration:
                if (_btParent.IsAttacking) return _state = BTNodeState.SUCCESS;

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
        if (!_btParent.HasAlreadySeen)
        {
            object t = GetData("target");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange, _layerMaskPlayer);
                if (colliders.Length > 0)
                {
                    Parent.Parent.SetData("target", colliders[0].transform);
                    t = colliders[0].transform;

                    _btParent.HasAlreadySeen = true;
                }
            }
            else
            {
                _btParent.HasAlreadySeen = false;
            }

            Transform target = (Transform)t;

            if (target == null)
            {
                _state = BTNodeState.FAILURE;
                return _state;
            }
        }

        if (_btParent.HasAlreadySeen)
            _state = BTNodeState.SUCCESS;
        else
            _state = BTNodeState.FAILURE;

        return _state;
    }


    //////INFILTRATION//////
    public BTNodeState InfiltrationCheck()
    {
        if (_btParent.IsAttacking)
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
            object t = GetData("target");
            if (t == null)
            {
                //Debug.Log("Recherche de la cible...");
                Collider[] colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange, _layerMaskPlayer);
                if (colliders.Length > 0)
                {
                    //Debug.Log("Cible trouvée, assignation en cours...");
                    Parent.Parent.SetData("target", colliders[0].transform);
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
            if (SeesPlayer(target))
            {
                // _animator.SetBool("Attacking", true);
                // _animator.SetBool("Walking", false);

                _btParent.ImageSpotBar.fillAmount = 0.0f;
                _btParent.CanvasUi.alpha = 0.0f;
                _state = BTNodeState.SUCCESS;
                return _state;
            }
            else if (_btParent.ImageSpotBar.fillAmount > 0)
            {
                _state = BTNodeState.RUNNING;
                return _state;
            }

            _hasFocusedUx = false;
            _state = BTNodeState.FAILURE;
            return _state;
        }
    }

    bool SeesPlayer(Transform target)
    {
        Vector3 targetDir = target.position - _transform.position;
        float angle = Vector3.Angle(targetDir, _transform.forward);
        if (angle >= -_btParent.FovAngle / 2 && angle <= _btParent.FovAngle / 2)
        {
            RaycastHit hit;

            Debug.DrawLine(_transform.position, _transform.position + targetDir.normalized * _btParent.FovRange, Color.red);
            if (Physics.Raycast(_transform.position, targetDir, out hit, _btParent.FovRange, _layerMaskPlayer))
            {
                if (hit.transform == target.parent)
                {
                    // Dessine un rayon vert si le joueur est détecté.
                    Debug.DrawLine(_transform.position, hit.point, Color.green);

                    if (!_hasACorrectView)
                    {
                        LoadCanvas();

                        if (_currentValueAlphaCanvas >= 1)
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
                else
                {
                    // Dessine un rayon jaune si un autre objet est touché.
                    Debug.DrawLine(_transform.position, hit.point, Color.yellow);
                }
            }
            else
            {
                UnloadSpotBar();

                if (_currentValueFillSpotBar <= 0)
                    UnloadCanvas();
            }
        }
        else
        {
            UnloadSpotBar();

            if (_currentValueFillSpotBar <= 0)
                UnloadCanvas();
        }

        return false;
    }

    private void LoadCanvas()
    {
        if (!_isLoadingCanvas)
        {
            _isLoadingCanvas = true;
            _isUnloadingCanvas = false;
        }

        _btParent.CanvasUi.alpha += Time.deltaTime / _btParent.DurationAlphaCanvas;

        if (_btParent.CanvasUi.alpha >= 1.0f)
        {
            _btParent.CanvasUi.alpha = 1.0f;
            _isLoadingCanvas = false;
        }

        _currentValueAlphaCanvas = _btParent.CanvasUi.alpha;
    }

    private void UnloadCanvas()
    {
        if (!_isUnloadingCanvas)
        {
            _isLoadingCanvas = false;
            _isUnloadingCanvas = true;
        }

        _btParent.CanvasUi.alpha -= Time.deltaTime / _btParent.DurationAlphaCanvas;

        if (_btParent.CanvasUi.alpha <= 0.0f)
        {
            _btParent.CanvasUi.alpha = 0.0f;
            _isUnloadingCanvas = false;
        }

        _currentValueAlphaCanvas = _btParent.CanvasUi.alpha;
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
            _hasACorrectView = true;
        }

        _currentValueFillSpotBar = _btParent.ImageSpotBar.fillAmount;
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
            _hasACorrectView = false;
        }

        _currentValueFillSpotBar = _btParent.ImageSpotBar.fillAmount;
    }

}