using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BehaviourNode(0, "Decorator/Conditions")]
// Returns the success or failure based on a debug boolean value. Use for testing Behaviour Tree logic.
public class DebugConditionalNode : ConditionalNode
{
    [SerializeField] private bool _booleanValue = false;
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    protected override bool EvaluateCondition()
    {
        return _booleanValue;
    }
}
