using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


[BehaviourNode(6, "Actions")]
public class RandomWanderNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private float _wanderRadius = 5f; // allowed distance from its start
    [SerializeField] private float _arriveWaitTime = 4f; // time before choosing new direction
    [SerializeField] private float _arrivalProximity = 0.1f;
    [SerializeField] private LayerMask _invalidLayers = (1 << 12) | (1 << 14); // 12 and 14 are intended to be Lava and Wall;

    private Vector2 _homePosition;
    private Vector2? _targetDestination;
    private bool _hasArrived = false;
    private float _waitTimer = 0f;

    // Behaviour tree
    protected override void Initialize() {
        _homePosition = Self.transform.position;
        _hasArrived = true;
    }

    public override BTNodeState Evaluate()
    {
        _isActive = true;
        if (_hasArrived) {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer > 0f)
            {
                return BTNodeState.Success;
            }
            _hasArrived = false;
            PickNewPoint();
        }

        return BTNodeState.Running;
    }
    private void PickNewPoint()
    {
        // chooses random point within a radius from the start position. Try up to 10 times.
        int tries = 0;
        while (tries < 10)
        {
            tries++;
            Vector2 randomOffset = Random.insideUnitCircle * _wanderRadius;
            Vector2 attemptedPoint = _homePosition + randomOffset;
            if (Physics2D.OverlapCircle(attemptedPoint, _arrivalProximity, _invalidLayers) != null) {
                continue;
            }
            _targetDestination = attemptedPoint; 
            Self.Pather.GoToPoint(_targetDestination.Value, _arrivalProximity, ArrivedAtPoint);
            return;
        }
        // Failed. Reset and try again next time.
        Debug.LogWarning("Enemy: " + Self.name + "repeatedly failed to pick a point. Make sure they have room to wander.");
        _hasArrived = true;
    }

    private void ArrivedAtPoint()
    {
        Self.Pather.Stop();
        _hasArrived = true;
        _targetDestination = null;
        _waitTimer = _arriveWaitTime;
    }

    protected override void Reset()
    {
        _hasArrived = true;
        _targetDestination = null;
        _waitTimer = 0f;
    }

    protected override void DrawDebug()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_homePosition, _wanderRadius);
        if (_targetDestination.HasValue)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_targetDestination.Value, 0.2f);
        }
    }

}
