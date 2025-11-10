using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : BehaviourTreeNode
{
    // Fields

    [SerializeField] float _waitTime;
    private float _currentTime = 0f;

    // Behaviour tree
    public override void CheckRequiredComponents(EnemyBase self)
    {

    }

    protected override void Initialize()
    {
        
    }

    public override BTNodeState Evaluate()
    {
        if (_currentTime >= _waitTime)
        {
            _currentTime = 0f;
            return BTNodeState.Success;
        }
        
        _currentTime += Time.deltaTime;
        return BTNodeState.Running;
    }




    public override void DrawDebug()
    {

    }

}
