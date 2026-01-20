using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Chases target until close enough
[BehaviourNode(2, "Actions")]
public class ChaseTargetNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _arrivedAtTargetDistance = 1f;

    private Vector2 _storedTargetPos = Vector2.zero;


    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        if (!Blackboard.TryGet("targetPosition", out Vector2? targetPosition) || !targetPosition.HasValue)
        {
            return BTNodeState.Failure;
        }
        _isActive = true;
        _storedTargetPos = targetPosition.Value;

        if (Vector2.Distance(Self.transform.position, _storedTargetPos) <= _arrivedAtTargetDistance) // When arrive, check if the target is here or not
        {
            if (Blackboard.TryGetNotNull("target", out ITargetable target)) return BTNodeState.Success;
            return BTNodeState.Failure;
        }

        ChaseTarget();
        return BTNodeState.Running;
    }

    private void ChaseTarget()
    {
        Self.Pather.GoToPoint(_storedTargetPos, _arrivedAtTargetDistance, ArrivedAtTarget);
    }

    private void ArrivedAtTarget()
    {
        Self.Pather.Stop();
    }




    protected override void DrawDebug()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_storedTargetPos, _arrivedAtTargetDistance);
    }

}
