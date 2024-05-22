using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RB_AIInf_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    [Header("Main Parameters")]
    public float MovementSpeed = 4f;
    public float AttackRange = 1f;
    public float AttackSpeed = 2f;

    [Header("Spline Parameters")]
    public SplineContainer SplineContainer;
    public float WaitBeforeToMoveToNextWaypoint = 0.25f; // in seconds
    public int PatrolSplineIndex = 0;

    public bool HasAnInterval = false;
    public int StartWaitingWaypointInterval = 0;


    [Header("FOV Parameters")]
    public bool InFov = false;
    public float FovRange = 10f;
    public float FovAngle = 75f;


    protected override RB_BTNode SetupTree()
    {
        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // Sequence Attack
            {
                new RB_AI_PlayerInFov(this),
                //new TaskAttack(transform),
            }),

            new RB_AI_Task_DefaultPatrol(this),  // task attack
        });

        return root;
    }
}