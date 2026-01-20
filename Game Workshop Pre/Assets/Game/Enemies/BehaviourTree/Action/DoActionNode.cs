using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ink.Parsed;
using UnityEngine;

[BehaviourNode(3, "Actions")]
// Performs an action of a given index.
// Do not let this node lose control when it is running.
public class DoActionNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private int _indexOfAction;
    [SerializeField] private bool _actionStarted = false;
    [SerializeField] private BTNodeState _endState = BTNodeState.Running;

    public override BTNodeState Evaluate()
    {
        if (_endState != BTNodeState.Running)
        {
            BTNodeState endState = _endState;
            _endState = BTNodeState.Running;
            return endState;
        }
        _isActive = true;

        if (!_actionStarted)
        {
            _actionStarted = true;
            Self.PerformAction(_indexOfAction, EndAction);
        }

        return BTNodeState.Running;
    }

    private void EndAction(bool result)
    {
        _actionStarted = false;
        if (result)
        {
            _endState = BTNodeState.Success;
        } else
        {
            _endState = BTNodeState.Failure;
        }
    }

    protected override void Reset()
    {
        _actionStarted = false;
        _endState = BTNodeState.Running;
    }


}
