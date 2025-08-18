using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoom : MonoBehaviour
{
    public List<Door> doors;
    public List<CollectableTrash> trashList;

    public void CloseRoom()
    {
        if (trashList.Count > 0)
        {
            foreach (Door door in doors)
            {
                door.CloseDoor();
            }
        }
    }

    public void OpenRoom()
    {
        if (trashList.Count <= 0)
        {
            foreach (Door door in doors)
            {
                door.OpenDoor();
            }
        }
    }
}
