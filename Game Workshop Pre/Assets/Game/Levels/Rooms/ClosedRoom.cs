using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoom : MonoBehaviour
{
 //   public ClosedRoom room = new ClosedRoom();
    public List<CollectableTrash> trashList = new List<CollectableTrash>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            trashList.Add(trash);
        }

        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            if (trashList.Count > 0)
            {
                PlayerState.Instance.EnterRoom();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            trashList.Remove(trash);
            if (trashList.Count <= 0)
            {
                PlayerState.Instance.ExitRoom();
            }
        }
    }
}
