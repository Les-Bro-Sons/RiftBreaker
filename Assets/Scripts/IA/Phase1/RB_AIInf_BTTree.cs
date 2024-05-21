using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class RB_AIInf_BTTree : RB_BTTree // phase Inf => Phase Infiltration
{
    public Transform[] Waypoints;

    public static float Speed = 4f;
    public static float AttackRange = 1f;
    public static float AttackSpeed = 2f;

    public static bool InFov = false;
    public static float FovRange = 10f;
    public static float FovAngle = 75f;

    protected override RB_BTNode SetupTree()
    {
        RB_BTNode root = new RB_BTSelector(new List<RB_BTNode>
        {
            new RB_BTSequence(new List<RB_BTNode> // Sequence Attack
            {
                new RB_AI_PlayerInFov(transform),
                //new TaskAttack(transform),
                new RB_AI_Task_DefaultPatrol(transform, Waypoints),  // task par defaut
            }),
            
        });

        return root;
    }
}