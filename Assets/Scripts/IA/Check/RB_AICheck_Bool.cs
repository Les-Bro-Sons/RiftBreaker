using BehaviorTree;
using UnityEngine;

public class RB_AICheck_Bool : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private string _checkingBool;

    public RB_AICheck_Bool(RB_AI_BTTree btParent, string checkingBool)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _checkingBool = checkingBool;
    }

    public override BTNodeState Evaluate()
    {
        if ((_btParent.BoolDictionnary.ContainsKey(_checkingBool) && _btParent.BoolDictionnary[_checkingBool])) //check if a bool exist and if it's true
        {
            return _state = BTNodeState.SUCCESS;
        }
        else
        {
            return _state = BTNodeState.FAILURE;
        }
    }
}
