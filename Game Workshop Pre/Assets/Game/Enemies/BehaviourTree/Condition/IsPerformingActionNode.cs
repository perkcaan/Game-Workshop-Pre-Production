using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IsPerformingActionNode : BehaviourTreeNode
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
        if (Blackboard.TryGet("isInAction", out bool isInAction)) { }

        if (isInAction)
        {
            return BTNodeState.Success;
        } else
        {
            return BTNodeState.Failure;
        }
    }


    
    

    public override void DrawDebug()
    {
    }
}
