using BehaviorTree;
using UnityEngine;
using static RB_AI_BTTree;

public class RB_AICheck_Bool : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private bool _isEnum = true;

    private BTBOOLVALUES _checkingEnumBool;
    private bool _checkingBool;

    public RB_AICheck_Bool(RB_AI_BTTree btParent, BTBOOLVALUES checkingBool)
    {
        _isEnum = true;
        _btParent = btParent;
        _transform = _btParent.transform;
        _checkingEnumBool = checkingBool;
    }

    public RB_AICheck_Bool(RB_AI_BTTree btParent, bool checkingBool)
    {
        _isEnum = false;
        _btParent = btParent;
        _transform = _btParent.transform;
        _checkingBool = checkingBool;
    }

    public override BTNodeState Evaluate()
    {
        if (_isEnum)
        {
            if ((_btParent.GetBool(_checkingEnumBool))) //check if a bool exist and if it's true
            {
                return _state = BTNodeState.SUCCESS;
            }
            else
            {
                return _state = BTNodeState.FAILURE;
            }
        }
        else
        {
            if (_checkingBool)
            {
                return _state = BTNodeState.SUCCESS;
            }
            else
            {
                return _state = BTNodeState.FAILURE;
            }
        }
    }
}
