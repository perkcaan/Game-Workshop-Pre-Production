using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BehaviourNode(1, "Decorator/Conditions")]
public class IsTargetFoundNode : ConditionalNode
{
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    protected override bool EvaluateCondition()
    {
        if (Blackboard.TryGetNotNull("target", out ITargetable target)) return true;
        return false;
    }
}
