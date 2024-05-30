using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_EnemyTouchDetection : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    private bool _setTarget;

    public RB_AICheck_EnemyTouchDetection(RB_AI_BTTree BtParent, bool setTarget)
    {
        _btParent = BtParent;
        _setTarget = setTarget;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;
        List<RB_Health> charactersCollided = _btParent.GetCollisions();

        foreach(RB_Health character in charactersCollided)
        {
            if (character.Team != _btParent.AiHealth.Team)
            {
                _state = BTNodeState.SUCCESS;
                if (_setTarget)
                {
                    _btParent.Root.SetData("target", character.transform);
                    _btParent.BoolDictionnary["HasAlreadySeen"] = true;
                }
                break;
            }
        }

        return _state;
    }
}
