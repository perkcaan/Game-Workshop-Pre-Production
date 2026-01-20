using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Patrol with no AI pathfinding.
// Returns Running most of the time. 
// Returns Success once when arrived at a patrol point.
[BehaviourNode(1, "Actions")]
public class PatrolNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private List<Vector2> _patrolPoints;
    [SerializeField] private bool _reversePatrolInsteadOfWrap;
    [SerializeField] private float _arrivalProximity = 0.2f;
    private int _patrolIndex = 0;
    private bool _patrolReversed;
    private bool _arrivedThisTick = false;

    // Behaviour tree
    public override BTNodeState Evaluate()
    {
        if (_patrolPoints == null || _patrolPoints.Count <= 0) return BTNodeState.Failure;
        _isActive = true;

        if (_arrivedThisTick)
        {
            _arrivedThisTick = false;
            return BTNodeState.Success;
        }
        DoPatrol();
        return BTNodeState.Running;
    }

    private void DoPatrol()
    {
        Vector2 targetDestination = _patrolPoints[_patrolIndex];
        Self.Pather.GoToPoint(targetDestination, _arrivalProximity, ArrivedAtPoint);
    }

    private void ArrivedAtPoint()
    {
        Self.Pather.Stop();
        if (_reversePatrolInsteadOfWrap)
        {
            if (_patrolIndex == 0) _patrolReversed = false;
            if (_patrolIndex == _patrolPoints.Count - 1) _patrolReversed = true;
        }
        
        int change = 1;
        if (_patrolReversed) change = -1;

        _patrolIndex = (_patrolIndex + change + _patrolPoints.Count) % _patrolPoints.Count;
        _arrivedThisTick = true;
    }



    protected override void DrawDebug()
    {
        foreach (Vector2 point in _patrolPoints)
        {
            if (_patrolPoints[_patrolIndex] == point) {
                Gizmos.color = Color.red;
            } else
            {
                Gizmos.color = Color.black;
            } 
            Gizmos.DrawSphere(point, _arrivalProximity);
        }
    }

}
