using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Patrol with no AI pathfinding.
public class SimplePatrolNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private List<Vector2> _patrolPoints;
    [SerializeField] private bool _reversePatrolInsteadOfWrap;
    [SerializeField] private float _arrivalProximity = 0.2f;
    private int _patrolIndex = 0;
    private bool _patrolReversed;

    // Behaviour tree
    public override void CheckRequiredComponents(EnemyBase self)
    {

    }

    protected override void Initialize()
    {

    }

    public override BTNodeState Evaluate()
    {
        DoPatrol();
        return BTNodeState.Running;
    }

    private void DoPatrol()
    {
        Vector2 targetDestination = _patrolPoints[_patrolIndex];
        if (Vector2.Distance(Self.transform.position, targetDestination) <= _arrivalProximity)
        {

            if (_reversePatrolInsteadOfWrap)
            {
                if (_patrolIndex == 0) _patrolReversed = false;
                if (_patrolIndex == _patrolPoints.Count - 1) _patrolReversed = true;
            }
            
            int change = 1;
            if (_patrolReversed) change = -1;

            _patrolIndex = (_patrolIndex + change + _patrolPoints.Count) % _patrolPoints.Count;
            targetDestination = _patrolPoints[_patrolIndex];
        }

        Vector2 targetDirection = (targetDestination - (Vector2)Self.transform.position).normalized;
        
        if (!Blackboard.TryGet("moveSpeed", out float moveSpeed)) { }
        Vector2 frameVelocity = targetDirection * moveSpeed;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        
        Blackboard.Set<Vector2>("frameVelocity", frameVelocity);
        Blackboard.Set<float>("rotation", angle);
    }



    public override void DrawDebug()
    {
        Gizmos.color = Color.black;
        foreach (Vector2 point in _patrolPoints)
        {
            Gizmos.DrawSphere(point, _arrivalProximity);
        }
    }

}
