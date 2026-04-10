using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


// Handles the player's swipe ability
[RequireComponent(typeof(Collider2D))]
public class SwipeHandler : MonoBehaviour
{

    // Components
    [SerializeField] private DottedParticleLine _dottedLine;
    private PlayerMovementController _parent;
    private PlayerContext _ctx;
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

    public void Initialize(PlayerMovementController player, PlayerContext ctx)
    {
        _parent = player;
        _ctx = ctx;
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

        // Get collider bounds
        Vector2 center = (Vector2) _hitbox.transform.TransformPoint(_hitbox.offset);
        Vector2 size = Vector2.Scale(_hitbox.size, _hitbox.transform.lossyScale);
        float angle = _hitbox.transform.eulerAngles.z;

        // Do hit and process data
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, _layers);
        List<(Collider2D collider, float distance)> hitColliders = HitProcessor.ProcessHits<ISwipeable>(hits, _swipeOrigin.transform.position);

        bool wasSomethingHit = false;
        foreach ((Collider2D collider, float distance) entry in hitColliders)
        {
            Collider2D collider = entry.collider;
            if (!collider.TryGetComponent(out ISwipeable swipeable)) continue;

            Vector2 hitDirection = (collider.transform.position - _parent.transform.position).normalized;

            //Falloff angle is 0 (dead on) to 90 (hit on the side or behind)
            float falloffAngle = Mathf.Clamp(Mathf.Abs(Vector2.SignedAngle(swipeDirection, hitDirection)), 0, 90); 
            float falloffReduction = Mathf.Lerp(0, _parent.SwipeFalloffReduction, falloffAngle / 90f);
            float hitForce = swipeForce - swipeForce * falloffReduction;

            float knockbackMultiplier = 0f;
            swipeable.OnSwipe(hitDirection, hitForce, collider, ref knockbackMultiplier);
            wasSomethingHit = true;

            // Apply resulting knockback (if there is any)
            if (knockbackMultiplier > 0f)
            {
                float knockbackForce =  knockbackMultiplier * _ctx.Rigidbody.mass * hitForce / collider.attachedRigidbody.mass;
                _ctx.Rigidbody.AddForce(-hitDirection * knockbackForce * _parent.SwipeKnockbackMultiplier, ForceMode2D.Impulse);
            }

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

