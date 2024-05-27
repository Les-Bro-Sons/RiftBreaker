using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RB_AICombat_PlayerInFov : RB_BTNode
{
    private RB_AICombat_BTTree _btParent;

    private Transform _transform;
    private static int _layerMaskPlayer = 1 << 7;
    //private Animator _animator;

    [Header("Header")]
    //private CanvasGroup _canvasUi;
    //private Image _imageSpotBar;
    private float _currentValueAlphaCanvas = 0.0f;

    private bool _isLoadingCanvas = false;
    private bool _isUnloadingCanvas = false;
    
    public RB_AICombat_PlayerInFov(RB_AICombat_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
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
}