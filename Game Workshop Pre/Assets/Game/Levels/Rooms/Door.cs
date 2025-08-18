using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private ClosedRoom parentRoom;
    void Start()
    {
        parentRoom = GetComponentInParent<ClosedRoom>();
        parentRoom.doors.Add(this);
        OpenDoor();
    }

    public void OpenDoor()
    {
        // door open animation here
        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
        // door close animation here
    }
}
