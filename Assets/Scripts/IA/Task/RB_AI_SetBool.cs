using BehaviorTree;
using UnityEngine;

public class RB_AI_SetBool : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private BTBOOLVALUES _boolID;
    private bool _boolValue;

    public RB_AI_SetBool(RB_AI_BTTree btParent, BTBOOLVALUES boolName, bool boolValue)
    {
        _btParent = btParent;
        _transform = _btParent.transform;

        _boolID = boolName;
        _boolValue = boolValue;
    }

    public override BTNodeState Evaluate()
    {
        _btParent.BoolDictionnary[_boolID] = _boolValue;

        return _state = BTNodeState.SUCCESS;
    }
}
