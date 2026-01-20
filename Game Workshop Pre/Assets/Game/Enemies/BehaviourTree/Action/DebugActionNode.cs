using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Returns NodeState and prints text. Use for testing behaviour tree logic.
[BehaviourNode(0, "Actions")]
public class DebugActionNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private string _debugText;
    [SerializeField] private BTNodeState _nodeState;

    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        _isActive = true;
        if (_debugText != "") Debug.Log(_debugText);
        return _nodeState;
    }

}
