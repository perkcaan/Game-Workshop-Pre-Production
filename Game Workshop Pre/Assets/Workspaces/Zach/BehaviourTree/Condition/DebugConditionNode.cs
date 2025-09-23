using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConditionNode : BehaviourTreeNode
{
    [SerializeField] private float _nearbyRadius = 2f;

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
        if (!Blackboard.TryGet("target", out Vector2 target) /*|| !blackboard.TryGet("nearbyRadius", out float nearbyRadius)*/)
        {
            return BTNodeState.Failure;
        }

        return (Vector2.Distance(Blackboard.Self.transform.position, target) <= _nearbyRadius) ? BTNodeState.Success : BTNodeState.Failure;
    }
}
