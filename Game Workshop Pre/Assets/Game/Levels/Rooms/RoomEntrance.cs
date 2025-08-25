using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntrance : MonoBehaviour
{
    private ClosedRoom parentRoom;
    void Start()
    {
        parentRoom = GetComponentInParent<ClosedRoom>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            parentRoom.EnterRoom();
        }
    }
}
