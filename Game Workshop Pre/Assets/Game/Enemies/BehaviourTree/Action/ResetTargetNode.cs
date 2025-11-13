using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTargetNode : BehaviourTreeNode
{
    // Fields
    
    // just reset target and targetPosition. Return success

    // Behaviour tree
    public override void CheckRequiredComponents(EnemyBase self)
    {

    }

    protected override void Initialize()
    {

    }

    public override BTNodeState Evaluate()
    {
        Blackboard.Set<ITargetable>("target", null);
        Blackboard.Set<Vector2?>("targetPosition", null);
        return BTNodeState.Success;
    }




    public override void DrawDebug()
    {

    }

}
