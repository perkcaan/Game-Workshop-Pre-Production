using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Service nodes are nodes that GATHER information and should be ran concurrently with other nodes. (Use a Parallel Node) 
// Having them run like an action node isn't recommended.
public abstract class ServiceNode : BehaviourTreeNode
{
    public override BTNodeState Evaluate()
    {
        _isActive = true;
        EvaluateService();
        return BTNodeState.Running;
    }

    protected abstract void EvaluateService();
}
