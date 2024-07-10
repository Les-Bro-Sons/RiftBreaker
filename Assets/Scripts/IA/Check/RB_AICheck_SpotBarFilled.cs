using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_SpotBarFilled : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private string _operator;
    private float _number;

    public RB_AICheck_SpotBarFilled(RB_AI_BTTree btParent, string sign, float number)
    {
        _btParent = btParent;
        _operator = sign;
        _number = number;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        float spotValue = _btParent.ImageSpotBar.fillAmount;

        switch(_operator)
        {
            case ">=":
                if (spotValue >= _number) _state = BTNodeState.SUCCESS;
                break;
            case "<=":
                if (spotValue <= _number) _state = BTNodeState.SUCCESS;
                break;
            case ">":
                if (spotValue > _number) _state = BTNodeState.SUCCESS;
                break;
            case "<":
                if (spotValue < _number) _state = BTNodeState.SUCCESS;
                break;
            case "==":
                if (spotValue == _number) _state = BTNodeState.SUCCESS;
                break;
            default:
                _state = BTNodeState.FAILURE;
                break;
        }

        return _state;
    }
}