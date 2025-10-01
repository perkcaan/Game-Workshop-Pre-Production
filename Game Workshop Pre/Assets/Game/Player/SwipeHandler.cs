using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles the player's swipe ability
[RequireComponent(typeof(Collider2D))]
public class SwipeHandler : MonoBehaviour
{

    // Components
    [SerializeField] private DottedParticleLine _dottedLine;
    private PlayerMovementController _parent;
    private Collider2D _hitbox;

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
        
    }
    // Swipe
    public void DoSwipe(float rotation, float swipeForce)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Swipe/Swipe", _parent.transform.position);
        _hitbox.enabled = true;
        _swipeForce = swipeForce;
        
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

    // Collision trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 direction = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        ISwipeable swipeableObject = other.gameObject.GetComponent<ISwipeable>();
        if (swipeableObject != null)
        {
            SpawnSwipeFX(_rotation);
            swipeableObject.OnSwipe(direction.normalized, _swipeForce);
        }
    }

    private void SpawnSwipeFX(float rotation)
    {
        
        _swipeEffectInstance = Instantiate(_swipeEffect, transform.position, Quaternion.Euler(0, 0, rotation + 90f));
        _swipeEffectInstance.transform.rotation = transform.rotation;
        
    }

}
