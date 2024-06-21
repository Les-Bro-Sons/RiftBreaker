using BehaviorTree;
using UnityEngine;

public class RB_AICheck_Bool : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private bool _isString = true;

    private string _checkingStringBool;
    private bool _checkingBool;

    public RB_AICheck_Bool(RB_AI_BTTree btParent, string checkingBool)
    {
        _isString = true;
        _btParent = btParent;
        _transform = _btParent.transform;
        _checkingStringBool = checkingBool;
    }

    public RB_AICheck_Bool(RB_AI_BTTree btParent, bool checkingBool)
    {
        _isString = false;
        _btParent = btParent;
        _transform = _btParent.transform;
        _checkingBool = checkingBool;
    }

    public override BTNodeState Evaluate()
    {
        if (_isString)
        {
            if ((_btParent.BoolDictionnary.ContainsKey(_checkingStringBool) && _btParent.BoolDictionnary[_checkingStringBool])) //check if a bool exist and if it's true
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
