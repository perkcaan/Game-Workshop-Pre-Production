using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles the player's swipe ability
public class HookHandler : MonoBehaviour
{

    // Components
    private PlayerMovementController _parent;
    private Collider2D _hitbox;
    public bool connecting;

    // Fields
    private float _rotation = 0f;
    private float _hookForce = 1f;
    private float _movingForce = 0f;
    // Unity methods
    private void Awake()
    {
        _parent = transform.parent.GetComponent<PlayerMovementController>();

        if (_parent == null)
        {
            Debug.LogWarning("Player Hook Handler cannot find Player.");
            gameObject.SetActive(false);
            return;
        }

        _hitbox = GetComponent<Collider2D>();
        _hitbox.enabled = false;
    }

    private void Update()
    {
        
    }
    // Swipe
    public void DoThrow(float rotation, float hookForce)
    {
        _hitbox.enabled = true;
        _hookForce = hookForce;
        transform.position = _parent.transform.position;

        UpdateHitbox(rotation);
    }

    public void UpdateHitbox(float rotation)
    {
        _rotation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        transform.position = _parent.transform.position + ((Vector3)offset * 0.75f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);  
    }

    public void EndSwipe()
    {
        _hitbox.enabled = false;
        connecting = false;
    }

    // Collision trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 direction = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));
        Vector3 contactPoint = other.ClosestPoint(transform.position);
        
        ISwipeable swipeableObject = other.gameObject.GetComponent<ISwipeable>();
        if (swipeableObject != null)
        {
            connecting = true;
            swipeableObject.OnSwipe(-direction.normalized, _hookForce, other);
        }
    }

}
