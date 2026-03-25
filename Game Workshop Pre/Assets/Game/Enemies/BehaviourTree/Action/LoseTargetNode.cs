using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Waits for a bit then resets target and targetPosition. Return success when finished waiting.
// Currently, this is basically the exact same as the wait node . Might change later.
[BehaviourNode(5, "Actions")]
public class LoseTargetNode : BehaviourTreeNode
{
    [SerializeField] private float _loseTargetConfusionTime = 2f;
    private float _currentTime = 0f;
    
    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        _isActive = true;

        if (_currentTime >= _loseTargetConfusionTime)
        {
            Blackboard.Set<ITargetable>("target", null);
            Blackboard.Set<Vector2?>("targetPosition", null);
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
