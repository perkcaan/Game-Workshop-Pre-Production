using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BehaviourTree/Condition/Debug")]
public class DebugConditionNode : BehaviourTreeNode
{
    public override BTNodeState Evaluate(Blackboard blackboard)
    {
        if (!blackboard.TryGet("target", out Vector2 target) || !blackboard.TryGet("nearbyRadius", out float nearbyRadius))
        {
            return BTNodeState.Failure;
        }

        return (Vector2.Distance(blackboard.btt.transform.position, target) <= nearbyRadius) ? BTNodeState.Success : BTNodeState.Failure;
    }
}
