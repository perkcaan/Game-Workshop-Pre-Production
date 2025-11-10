using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HasLastKnownPositionNode : BehaviourTreeNode
{
    public override void CheckRequiredComponents(EnemyBase self)
    {
        // No required components
    }

    protected override void Initialize()
    {
        // Nothing to initalize
    }

    public override BTNodeState Evaluate()
    {
        if (Blackboard.TryGet("targetPosition", out Vector2? targetPosition))
        {
            if (!targetPosition.HasValue) return BTNodeState.Failure;
            return BTNodeState.Success;
        }
        else
        {
            return BTNodeState.Failure;
        }

    }


    
    

    public override void DrawDebug()
    {
    }
}
