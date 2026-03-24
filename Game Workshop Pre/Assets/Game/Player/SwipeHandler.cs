using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;


// Handles the player's swipe ability
[RequireComponent(typeof(Collider2D))]
public class SwipeHandler : MonoBehaviour
{

    // Components
    [SerializeField] private DottedParticleLine _dottedLine;
    private PlayerMovementController _parent;
    private BoxCollider2D _hitbox;
    [SerializeField] private LayerMask _layers;
    [SerializeField] GameObject _swipeOrigin;

    // Fields
    private float _rotation = 0f;
    private float targetAngle;

    // Methods

    private void Awake()
    {
        _hitbox = GetComponent<BoxCollider2D>();
        _hitbox.enabled = false;
    }

    public void Initialize(PlayerMovementController player)
    {
        _parent = player;
    }

    // Collision trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // This trigger can potentially be used for things such as attack parries if they are added later.
    }

    // Swipe
    public void DoSwipe(float rotation, float swipeForce)
    {
        _hitbox.enabled = true;
        UpdateHitbox(rotation);
        Vector2 swipeDirection = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));
        bool wasSomethingHit = false;

        // Get collider bounds
        Vector2 center = (Vector2) _hitbox.transform.TransformPoint(_hitbox.offset);
        Vector2 size = Vector2.Scale(_hitbox.size, _hitbox.transform.lossyScale);
        float angle = _hitbox.transform.eulerAngles.z;

        // Do hit
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, _layers);

        // Sort hits by their HitParent
        Dictionary<GameObject, List<Collider2D>> hitObjects = new();
        foreach (Collider2D collider in hits)
        {
            if (!collider.TryGetComponent(out ISwipeable swipeable)) continue;
            if (swipeable.HitParent == null)
            {
                Debug.LogWarning("Please ensure a valid HitParent for Swipeable object: " + collider.name);
                continue;
            }

            if (!hitObjects.TryGetValue(swipeable.HitParent, out List<Collider2D> list))
            {
                list = new List<Collider2D>();
                hitObjects.Add(swipeable.HitParent, list);
            }
            list.Add(collider);

            wasSomethingHit = true;
        }

        // Find best collider on each object.
        List<(Collider2D collider, float distance)> bestColliders = new();
        foreach (KeyValuePair<GameObject, List<Collider2D>> kvp in hitObjects)
        {
            GameObject gameObject = kvp.Key;
            List<Collider2D> colliders = kvp.Value;
            
            Collider2D bestCollider = colliders[0];
            float closestDistance = float.MaxValue;

            foreach (Collider2D collider in colliders)
            {
                Vector2 origin = _swipeOrigin.transform.position;
                float dist = Vector2.Distance(origin, collider.ClosestPoint(origin));
                if (dist < closestDistance)
                {
                    bestCollider = collider;
                    closestDistance = dist;
                }

            }

            bestColliders.Add((bestCollider, closestDistance));
        }

        // Sort by distance and Swipe each of them in order
        bestColliders.Sort((a, b) => a.distance.CompareTo(b.distance));
        foreach ((Collider2D collider, float distance) entry in bestColliders)
        {
            Collider2D collider = entry.collider;
            if (!collider.TryGetComponent(out ISwipeable swipeable)) {
                Debug.LogWarning("Lost a Swipeable object somehow. This code must be faulty");
                continue;
            }

            Vector2 direction = (collider.transform.position - _parent.transform.position).normalized;
            Debug.Log($"Angle: {swipeDirection - direction}");
            // Add angle fall off later...
            //From 0>90 lerp 1>0.6
            // Including both directions
            swipeable.OnSwipe(direction, swipeForce, collider);
            //TODO: Add support for shields blocking the swipe
            //if swipe is blocked-> 
            //break here and play a sound and do knockback or whatever
        }


        // Play sound effect
        if (wasSomethingHit)
        {
            AudioManager.Instance.PlayInstance("Swipe");
        } else
        {
            AudioManager.Instance.PlayInstance("SwipeMiss");
        }
    }

    public void UpdateHitbox(float rotation)
    {
        _rotation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        // Set the swipe box position and rotation relative to the player and their rotation
        transform.position = _parent.transform.position + ((Vector3)offset * 0.75f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);  
    }

    public void UpdateLine(float rotation, float lineDist, int linePoints)
    {
        float radians = rotation * Mathf.Deg2Rad;
        Vector2 point = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * lineDist;
        _dottedLine.UpdateLine(point, linePoints);
    }

    public void HideLine()
    {
        _dottedLine.HideLine();
    }

    public void EndSwipe()
    {
        _hitbox.enabled = false;
    }


    // Gizmos
    public void OnDrawGizmosSelected()
    {
        if (_hitbox == null || !_hitbox.enabled) return;
        Transform t = _hitbox.transform;

        Vector3 center = t.TransformPoint(_hitbox.offset);
        Gizmos.matrix = Matrix4x4.TRS(
            center,
            t.rotation,
            t.lossyScale
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, _hitbox.size);
    }


}

