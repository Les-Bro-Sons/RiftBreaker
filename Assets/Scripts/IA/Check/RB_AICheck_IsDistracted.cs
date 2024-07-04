using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_AICheck_IsDistracted : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    public RB_AICheck_IsDistracted(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        _btParent.Distractions.RemoveAll(value => value == null);

        if (_btParent.Distractions.Count > 0 )
        {
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
