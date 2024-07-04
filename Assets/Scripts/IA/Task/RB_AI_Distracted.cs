using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_Distracted : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    public RB_AI_Distracted(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;


        return _state;
    }
}
