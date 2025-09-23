using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWalkNode : BehaviourTreeNode
{
    // Fields
    [Tooltip("0 for right. 90 for up. 180 for left. 270 for down.")]
    [SerializeField] private float _angleToWalk;
    [SerializeField] private Collider2D _colliderToUse;
    [SerializeField] private LayerMask _layersToFlipWhenWalkedInto;
    [SerializeField] private float _distToCheckForObstacle = 2f;

    private bool _turnedAround = false;


    // Behaviour tree
    public override void CheckRequiredComponents(EnemyBase self)
    {
        if (_colliderToUse == null)
        {
            if (self.TryGetComponent<Collider2D>(out Collider2D component))
            {
                _colliderToUse = component;
            }
            else
            {
                Debug.LogWarning("Collider2D component is required to use SimpleWalkNode. Please add it and set it up properly.");
            }
        }

    }

    protected override void Initialize()
    {
        // Nothing to initialize
    }

    //Serialized vector. Walk in that direction.normalized
    //Raycast each frame. When walk into something in Layer, turn around.
    // When turned around, flip vector
    public override BTNodeState Evaluate()
    {
        float radians = _angleToWalk * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        if (_turnedAround) direction *= -1;

        TurnOnObstacle(direction);
        WalkAtAngle(direction);
        return BTNodeState.Running;
    }

    // Simple Walk
    private void TurnOnObstacle(Vector2 direction)
    {
        Bounds b = _colliderToUse.bounds;

        // Find the point on the collider bounds edge in that direction
        Vector2 extents = b.extents;
        Vector2 origin = (Vector2)b.center + Vector2.Scale(direction, extents);

        // Setup and raycast
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_layersToFlipWhenWalkedInto);
        filter.useTriggers = false;

        RaycastHit2D[] results = new RaycastHit2D[1]; // store hits
        int hitCount = Physics2D.Raycast(origin, direction, filter, results, _distToCheckForObstacle);
        // Check all raycast hits, ignore itself
        for (int i = 0; i < hitCount; i++)
        {
            if (results[i].collider != _colliderToUse)
            {
                _turnedAround = !_turnedAround;
            }
        }

        //Debug.DrawRay(origin, direction * _distToCheckForObstacle, Color.red);

    }

    private void WalkAtAngle(Vector2 direction)
    {
        //TODO: Physics. maybe call gizmos a different way? See if we can check if gizmos are enabled -> call it from evaluate?
    }
    

}
