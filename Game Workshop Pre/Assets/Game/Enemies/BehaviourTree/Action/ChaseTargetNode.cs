using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Chases target until close enough
public class ChaseTargetNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _arrivedAtTargetDistance = 0.5f;

    // Behaviour tree
    public override void CheckRequiredComponents(EnemyBase self)
    {

    }

    protected override void Initialize()
    {

    }

    public override BTNodeState Evaluate()
    {
        if (Blackboard.TryGet("targetPosition", out Vector2? targetPosition))
        {
            if (!targetPosition.HasValue) return BTNodeState.Failure;
        }
        Vector2 targetPos = targetPosition.Value;

        if (Vector2.Distance(Self.transform.position, targetPos) <= _arrivedAtTargetDistance)
        {
            return BTNodeState.Success;
        }


        Vector2 targetDirection = (targetPos - (Vector2)Self.transform.position).normalized;
        
        if (!Blackboard.TryGet("moveSpeed", out float moveSpeed)) { }
        Vector2 frameVelocity = targetDirection * moveSpeed;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        
        Blackboard.Set<Vector2>("frameVelocity", frameVelocity);
        Blackboard.Set<float>("rotation", angle);
        
        return BTNodeState.Running;
    }




    public override void DrawDebug()
    {

    }

}
