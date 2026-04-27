using System;
using UnityEngine;

[BehaviourNode(4, "Decorator/Conditions"), Serializable]
public class IsStomachFullNode : ConditionalNode
{
    protected override bool EvaluateCondition()
    {
        if (Blackboard.TryGet("stomachSize", out float stomachSize))
        {
            return stomachSize >= 30f;
        }

        return false;
    }
}