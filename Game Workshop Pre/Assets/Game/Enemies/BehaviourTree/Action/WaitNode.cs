using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Waits some amount of time, then returns Success
[BehaviourNode(4, "Actions")]
public class WaitNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] float _waitTime;
    private float _currentTime = 0f;

    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        _isActive = true;

        if (_currentTime >= _waitTime)
        {
            _currentTime = 0f;
            return BTNodeState.Success;
        }
        
        _currentTime += Time.deltaTime;
        return BTNodeState.Running;
    }

    protected override void Reset()
    {
        _currentTime = 0f;
    }

}
