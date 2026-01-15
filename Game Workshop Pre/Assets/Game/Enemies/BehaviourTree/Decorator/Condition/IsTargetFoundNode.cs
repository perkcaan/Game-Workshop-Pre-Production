using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BehaviourNode(1, "Decorator/Conditions")]
public class IsTargetFoundNode : ConditionalNode
{
    [SerializeField] private bool _hasKnownLocationMode = false; 
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    protected override bool EvaluateCondition()
    {
        if (Blackboard.TryGetNotNull("target", out ITargetable target)) return true;

        if (_hasKnownLocationMode)
        {
            if (Blackboard.TryGet("targetPosition", out Vector2? targetPosition))
            {
                if (targetPosition.HasValue) return true;
            }
        }
        
        return false;
    }
}
