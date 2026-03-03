using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[BehaviourNode(1, "_Deprecated")]
public class IsPerformingActionNode : BehaviourTreeNode
{
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

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

}
