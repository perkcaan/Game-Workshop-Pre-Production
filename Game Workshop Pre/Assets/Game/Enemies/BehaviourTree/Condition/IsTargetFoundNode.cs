using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTargetFoundNode : BehaviourTreeNode
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
        if (Blackboard.TryGetNotNull("target", out ITargetable target))
        {
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
