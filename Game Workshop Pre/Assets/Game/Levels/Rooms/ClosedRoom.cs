using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoom : MonoBehaviour
{
    public List<CollectableTrash> trashList;

    public void EnterRoom()
    {
        if (trashList.Count > 0)
        {
            PlayerState.Instance.EnterRoom();
        }
    }

    public void ExitRoom()
    {
        if (trashList.Count <= 0)
        {
            PlayerState.Instance.ExitRoom();
        }
    }
}
