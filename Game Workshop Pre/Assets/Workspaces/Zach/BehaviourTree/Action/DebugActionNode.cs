using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActionNode : BehaviourTreeNode
{

    // Example test for requiring a component
    public override void CheckRequiredComponents(EnemyBase self)
    {
        if (!self.TryGetComponent<CircleCollider2D>(out CircleCollider2D component))
        {
            Debug.LogWarning("CircleCollider2D component is required to use DebugActionNode. Please add it and set it up properly.");
        }
    }

    protected override void Initialize()
    {
        // Nothing to initalize
    }

    public override BTNodeState Evaluate()
    {
        if (Blackboard.TryGet("name", out string name))
        {
            Debug.Log("Name: " + name);
        }


        return BTNodeState.Running;
    }
}
