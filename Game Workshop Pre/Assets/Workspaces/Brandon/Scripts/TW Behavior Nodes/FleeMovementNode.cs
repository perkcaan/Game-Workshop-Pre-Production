using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeMovementNode : BehaviourTreeNode
{
     protected override void Initialize()
    {

    }

    public override BTNodeState Evaluate()
    {
        return BTNodeState.Running;
    }

}
