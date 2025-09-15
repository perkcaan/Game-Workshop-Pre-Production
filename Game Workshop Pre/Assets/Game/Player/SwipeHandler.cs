using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles the player's swipe ability
[RequireComponent(typeof(Collider2D))]
public class SwipeHandler : MonoBehaviour
{

    // Components
    private PlayerMovementController _parent;
    private Collider2D _hitbox;
    private float _rotation = 0f;
    private float _movementSpeed = 1f;

    // Unity methods
    private void Awake()
    {
        _parent = transform.parent.GetComponent<PlayerMovementController>();
        if (_parent == null)
        {
            Debug.LogWarning("Player Swipe Handler cannot find Player.");
            gameObject.SetActive(false);
            return;
        }
        _hitbox = GetComponent<Collider2D>();
        _hitbox.enabled = false;
    }

    public void DoSwipe(float rotation, float movementSpeed)
    {
        _hitbox.enabled = true;
        _movementSpeed = movementSpeed;
        UpdateHitbox(rotation);
    }

    public void UpdateHitbox(float rotation)
    {
        _rotation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        // Set the swipe box position and rotation relative to the player and their rotation
        transform.position = _parent.transform.position + ((Vector3)offset * 0.75f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);
    }

    public void EndSwipe()
    {
        _hitbox.enabled = false;
        _movementSpeed = 1f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 directionMaybe = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        ISwipeable swipeableObject = collision.gameObject.GetComponent<ISwipeable>();
        if (swipeableObject != null)
        {
            float force = _parent.SwipeForce + _movementSpeed * _parent.SwipeMovementScaler;
            swipeableObject.OnSwipe(directionMaybe.normalized, force);
        }
    }

}
