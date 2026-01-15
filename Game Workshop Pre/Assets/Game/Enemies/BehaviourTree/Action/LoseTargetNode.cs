using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Resets target and targetPosition knowledge then waits for a bit. Return success when finished waiting.
[BehaviourNode(4, "Actions")]
public class LoseTargetNode : BehaviourTreeNode
{
    [SerializeField] private float _loseTargetConfusionTime = 5f;
    private float _currentTime = 0f;
    
    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    public override BTNodeState Evaluate()
    {
        _isActive = true;
        Debug.Log("where'd he go?");

        Blackboard.Set<ITargetable>("target", null);
        Blackboard.Set<Vector2?>("targetPosition", null);

        if (_currentTime >= _loseTargetConfusionTime)
        {
            Debug.Log("i lost him.");
            _currentTime = 0f;
            return BTNodeState.Success;
        }
        
        _currentTime += Time.deltaTime;
        return BTNodeState.Running;
    }

    protected override void Reset()
    {
        Debug.Log("Reset timer");
        _currentTime = 0f;
    }
}
