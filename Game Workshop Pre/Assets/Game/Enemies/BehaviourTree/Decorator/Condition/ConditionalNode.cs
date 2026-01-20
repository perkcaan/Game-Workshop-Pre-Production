using System.Collections.Generic;
using UnityEngine;

// A conditional (decorator) node decorates another node, and only evaluates it if the condition is met.
public abstract class ConditionalNode : DecoratorNode
{
    private bool _evaluateResult = false;

    public override BTNodeState Evaluate()
    {
        _isActive = true;
        _evaluateResult = EvaluateCondition();
        if (_evaluateResult)
        {
            return Child.Evaluate();
        } else
        {
            return BTNodeState.Failure;
        }
    }

    protected abstract bool EvaluateCondition();

}
