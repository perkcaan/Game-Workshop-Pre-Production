using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCloseTrigger : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Room _parentRoom;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        _parentRoom = GetComponentInParent<Room>();
        if (_parentRoom == null)
        {
            Debug.LogWarning("RoomCloseTrigger at location " + (Vector2) transform.position + " does not belong to a room. Please fix.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out PlayerMovementController player))
        {
            Debug.Log("Player entered room: " + _parentRoom.name);
            // This needs to call a function on the parent room if its NOT focused and NOT cleared
            // Parent room then has references to every gate that connects with it.
            // Parent room SHUTS its gates (as long as player is in it. If player leaves then it has to open its gates)
        }
    }
}
