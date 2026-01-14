using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWalkNode : BehaviourTreeNode
{
    // Fields
    [Tooltip("0 for right. 90 for up. 180 for left. 270 for down.")]
    [SerializeField] private float _angleToWalk;
    [SerializeField] private LayerMask _layersToFlipWhenWalkedInto = (1 << 12) | (1 << 14); // 12 and 14 are intended to be Lava and Wall;
    [SerializeField] private float _distToCheckForObstacle = 0.2f;

    private bool _turnedAround = false;

    // stored here for debug purposes
    Vector2 _storedDirection = Vector2.zero;
    Vector2 _storedOrigin = Vector2.zero;
    
    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize() { }

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
        Bounds b = Self.Collider.bounds;

        // Find the point on the collider bounds edge in that direction
        Vector2 extents = b.extents;
        Vector2 origin = (Vector2)b.center + Vector2.Scale(direction, extents);

        // Setup and raycast
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_layersToFlipWhenWalkedInto);
        filter.useTriggers = true;

        RaycastHit2D[] results = new RaycastHit2D[1]; // store hits
        int hitCount = Physics2D.Raycast(origin, direction, filter, results, _distToCheckForObstacle);
        // Check all raycast hits, ignore itself
        for (int i = 0; i < hitCount; i++)
        {
            if (results[i].collider != Self.Collider)
            {
                _turnedAround = !_turnedAround;
            }
        }

        _storedDirection = direction;
        _storedOrigin = origin;
    }

    private void WalkAtAngle(Vector2 direction)
    {
        if (!Blackboard.TryGet("moveSpeed", out float moveSpeed)) { }
        Vector2 frameVelocity = direction * moveSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Blackboard.Set<Vector2>("frameVelocity", frameVelocity);
        Blackboard.Set<float>("rotation", angle);
    }
    
    protected override void DrawDebug()
    {
        Debug.DrawRay(_storedOrigin, _storedDirection * _distToCheckForObstacle, Color.red);
        
    }
    

}
