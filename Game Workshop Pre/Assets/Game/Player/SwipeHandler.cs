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
    [SerializeField] public ParticleSystem _swipeEffect;
    private ParticleSystem _swipeEffectInstance;

    // Fields
    private float _rotation = 0f;
    private float _swipeForce = 1f;

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

    private void Update()
    {
        UpdateHitbox(_parent.rotation);
    }
    // Swipe
    public void DoSwipe(float rotation, float swipeForce)
    {
        _hitbox.enabled = true;
        _swipeForce = swipeForce;
        SpawnSwipeFX(rotation);
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
    }

    // Collision trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 direction = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        ISwipeable swipeableObject = other.gameObject.GetComponent<ISwipeable>();
        if (swipeableObject != null)
        {
            swipeableObject.OnSwipe(direction.normalized, _swipeForce);
        }
    }

    private void SpawnSwipeFX(float rotation)
    {
        _swipeEffectInstance = Instantiate(_swipeEffect, transform.position, Quaternion.Euler(0, 0, rotation + 90f));
        _swipeEffectInstance.transform.rotation = transform.rotation;
    }

}
