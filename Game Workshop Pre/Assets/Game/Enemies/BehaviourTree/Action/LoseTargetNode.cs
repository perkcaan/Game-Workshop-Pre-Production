using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Resets target and targetPosition knowledge. Return success
[BehaviourNode(4, "Actions")]
public class LoseTargetNode : BehaviourTreeNode
{


    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    public override BTNodeState Evaluate()
    {
        _isActive = true;
        Blackboard.Set<ITargetable>("target", null);
        Blackboard.Set<Vector2?>("targetPosition", null);
        return BTNodeState.Success;
    }

}
