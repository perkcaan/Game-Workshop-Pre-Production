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
            _parentRoom.TriggerRoomClose();
        }
    }
}
