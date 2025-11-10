using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConditionNode : BehaviourTreeNode
{
    [SerializeField] private bool _evaluateStatus;

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
        if (_evaluateStatus)
        {
            return BTNodeState.Success;
        }
        else
        {
            return BTNodeState.Failure;
        }
    }
}
