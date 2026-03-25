using System.Collections.Generic;
using UnityEngine;

// A negation node is a decorator node that negates the output of what it decorates.
// So Success becomes Failure and Failure becomes Success.
[BehaviourNode(0, "Decorator")]
public class NegationNode : DecoratorNode
{
    public override BTNodeState Evaluate()
    {
        _isActive = true;
        switch (Child.Evaluate())
        {
            case BTNodeState.Success:
                return BTNodeState.Failure;
            case BTNodeState.Failure:
                return BTNodeState.Success;
        }
        return BTNodeState.Running;
    }

}
