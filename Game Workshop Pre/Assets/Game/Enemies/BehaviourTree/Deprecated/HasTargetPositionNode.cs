using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[BehaviourNode(0, "_Deprecated")]
public class HasLastKnownPositionNode : BehaviourTreeNode
{
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

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


    
    

    protected override void DrawDebug()
    {
    }
}
