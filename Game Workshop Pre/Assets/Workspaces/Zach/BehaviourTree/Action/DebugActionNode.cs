using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviourTree/Action/Debug")]
public class DebugActionNode : BehaviourTreeNode
{
    public override BTNodeState Evaluate(Blackboard blackboard)
    {
        if (blackboard.TryGet("name", out string name))
        {
            Debug.Log("Name: " + name);
        }


        return BTNodeState.Running;
    }
}
