using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles the player's sweep ability
[RequireComponent(typeof(Collider2D))]
public class BroomSweepHandler : MonoBehaviour
{

    // Components
    private PlayerMovementController _parent;
    private Collider2D _hitbox;
    private PlayerContext _ctx;

    // Fields
    private float _rotation = 0f;
    private float _sweepForce = 1f;

    // Unity methods
    private void Awake()
    {
        _parent = transform.parent.GetComponent<PlayerMovementController>();
        if (_parent == null)
        {
            Debug.LogWarning("Broom Sweep Handler cannot find Player.");
            gameObject.SetActive(false);
            return;
        }
        _hitbox = GetComponent<Collider2D>();
        _hitbox.enabled = false;
        
        
    }

    // Sweep
    public void BeginSweep(float rotation, float sweepForce)
    {
        _hitbox.enabled = true;
        
        UpdateHitbox(rotation, sweepForce);
    }

    public void UpdateHitbox(float rotation, float sweepForce)
    {
        _sweepForce = sweepForce;
        _rotation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));
        

        // Set the sweep box position and rotation relative to the player and their rotation
        transform.position = _parent.transform.position + ((Vector3)offset * 0.75f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);
    }

    public void EndSweep()
    {
        _hitbox.enabled = false;
    }


    // Collision trigger
    private void OnTriggerStay2D(Collider2D other)
    {
        Vector2 direction = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        ISweepable sweepableObject = other.gameObject.GetComponent<ISweepable>();
        if (sweepableObject != null)
        {
            sweepableObject.OnSweep(direction.normalized, _sweepForce);
        }
        
    }


}
