using UnityEngine;

// Chases target until close enough
[BehaviourNode(2, "Actions")]
public class ChaseTargetNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _arrivedAtTargetDistance = 1f;

    private Vector2 _storedTargetPos = Vector2.zero;
    private bool _hasArrived = false;


    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        if (!Blackboard.TryGet("targetPosition", out Vector2? targetPosition) || !targetPosition.HasValue)
        {
            return BTNodeState.Failure;
        }
        _isActive = true;
        _storedTargetPos = targetPosition.Value;

        if (_hasArrived) // When arrived, check if the target is here or not
        {
            _hasArrived = false;
            if (Blackboard.TryGetNotNull("target", out ITargetable target)) {
                Self.Pather.FacePoint(_storedTargetPos);
                return BTNodeState.Success;
            }
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
        _hasArrived = true;
        Self.Pather.Stop();
    }

    protected override void Reset()
    {
        _hasArrived = false;
    }



    protected override void DrawDebug()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_storedTargetPos, _arrivedAtTargetDistance);
    }

}
