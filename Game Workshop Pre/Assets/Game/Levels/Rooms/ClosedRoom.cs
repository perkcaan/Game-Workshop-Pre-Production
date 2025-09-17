using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoom : MonoBehaviour
{
    public List<CollectableTrash> trashList;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            Debug.Log("Populate: " + trashList.Count);
            trashList.Add(trash);
        }

        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Debug.Log("Heyyyyy");
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
