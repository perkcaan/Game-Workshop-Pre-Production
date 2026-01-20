// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


//Note: Need to be reimplemented with the new enemy system

// // Modified patrol to have random movement 
// public class RandomPatrolNode : BehaviourTreeNode
// {
//     // Fields
//     [SerializeField] private float _wanderRadius = 5f; // allowed distance from its start
//     [SerializeField] private float _newDirectionInterval = 4f; // time before choosing new direction
//     [SerializeField] private float _arrivalProximity = 0.2f;

//     private Vector2 _startPosition;
//     private Vector2 _targetDestination;
//     private float _directionTimer;

//     // Behaviour tree
//     public override void CheckRequiredComponents(EnemyBase self)
//     {

//     }

//     protected override void Initialize()
//     {
//         _startPosition = Self.transform.position;
//         PickNewDestination();
//     }

//     public override BTNodeState Evaluate()
//     {
//         // countdown
//         _directionTimer -= Time.deltaTime;
//         if ( _directionTimer <= 0f || Vector2.Distance(Self.transform.position, _targetDestination) <= _arrivalProximity) 
//             {
//                 PickNewDestination();
//             }

//         MoveTowardsTarget();
//         return BTNodeState.Running;
//     }

//     private void PickNewDestination()
//     {
//         // chooses random point within a radius from the start position
//         Vector2 randomOffset = Random.insideUnitCircle * _wanderRadius;
//         _targetDestination = _startPosition + randomOffset;
//         _directionTimer = _newDirectionInterval;
//     }

//     private void MoveTowardsTarget()
//     {
//         Vector2 currentPosition = Self.transform.position;
//         Vector2 targetDirection = (_targetDestination - currentPosition).normalized;

//         if (!Blackboard.TryGet("moveSpeed", out float moveSpeed))
//             moveSpeed = 1f;

//         Vector2 frameVelocity = targetDirection * moveSpeed;
//         float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

//         Blackboard.Set("frameVelocity", frameVelocity);
//         Blackboard.Set("rotation", angle);
//     }


//     public override void DrawDebug()
//     {
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireSphere(_startPosition, _wanderRadius);
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawSphere(_targetDestination, 0.2f);
//     }

// }
