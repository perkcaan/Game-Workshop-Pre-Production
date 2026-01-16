using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoActionNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private int _indexOfAction;
    [SerializeField] private bool _actionStarted = false;

    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    //TODO: Need to make it so you can't just leave while this is running... Will probably need to restructure the behaviour tree quiiiiite a bit
    // go back to using the Parallel node... its important for Service nodes!
    // do something closer to whats on discord/Programming
    public override BTNodeState Evaluate()
    {
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
        Debug.Log("Action ended with result: " + result);
    }

    protected override void Reset()
    {
        
    }


}
