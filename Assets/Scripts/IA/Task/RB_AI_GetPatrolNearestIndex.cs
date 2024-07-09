using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class RB_AI_GetPatrolNearestIndex : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private RB_AiMovement _btMovement;

    private List<Vector3> _points = new();

    public RB_AI_GetPatrolNearestIndex(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _btMovement = btParent.AiMovement;

        if (_btParent.PatrolSplineIndex < _btParent.SplineContainer.Splines.Count)
            foreach (BezierKnot points in _btParent.SplineContainer.Splines[_btParent.PatrolSplineIndex].Knots)
                _points.Add(points.Position);
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        int? nearestPointIndex = null;
        float currentPathLength = _btMovement.GetPathLength(_btMovement.GetPath(_points[_btParent.CurrentWaypointIndex]));

        for (int i = 0; i < _points.Count; ++i)
        {
            NavMeshPath path = _btMovement.GetPath(_points[i]);
            if (path == null) continue;

            float pathLength = _btMovement.GetPathLength(path);
            if (pathLength < currentPathLength)
            {
                currentPathLength = pathLength;
                nearestPointIndex = i;
            }
        }

        if (nearestPointIndex != null)
        {
            _btParent.CurrentWaypointIndex = nearestPointIndex.Value;
            _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
