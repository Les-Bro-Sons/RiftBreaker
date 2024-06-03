using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_PlayerInRoom : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    
    public RB_AI_PlayerInRoom(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (RB_RoomManager.Instance.GetPlayerCurrentRoom() == RB_RoomManager.Instance.GetEntityRoom(_btParent.AiHealth.Team, _btParent.gameObject))
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, 1000, 1 << 7); //PLACHOLDER
            if (colliders.Length > 0)
            {
                _btParent.Root.SetData("target", colliders[0].transform);
            }
            _state = BTNodeState.SUCCESS;
        }
        


        //placeholder from here
        /*Collider[] colliders = Physics.OverlapSphere(_transform.position, _btParent.FovRange * 2, 1 << 7);
        if (colliders.Length > 0)
        {
            _btParent.Root.SetData("target", colliders[0].transform);
            _state = BTNodeState.SUCCESS;
        }*/

        return _state;
    }
}
