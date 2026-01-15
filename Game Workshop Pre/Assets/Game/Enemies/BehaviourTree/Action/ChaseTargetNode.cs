using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Chases target until close enough
[BehaviourNode(3, "Actions")]
public class ChaseTargetNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _arrivedAtTargetDistance = 1f;

    private Vector2 _storedTargetPos = Vector2.zero;
    private bool _arrivedThisTick = false;


    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

    public override BTNodeState Evaluate()
    {
        _isActive = true;
        if (Blackboard.TryGet("targetPosition", out Vector2? targetPosition))
        {
            if (!targetPosition.HasValue) return BTNodeState.Failure;
        }
        _storedTargetPos = targetPosition.Value;

        if (_arrivedThisTick) // When arrive, check if the target is here or not
        { // NEED TO FIX SO THAT IT DOESNT ATTACK IF IT REFINDS PLAYER WHEN IT ARRIVED THIS TICK BUT PLAYER ISNT AT ARRIVAL POINT
            _arrivedThisTick = false;
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
        _arrivedThisTick = true;
    }




    protected override void DrawDebug()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_storedTargetPos, _arrivedAtTargetDistance);
    }

}
