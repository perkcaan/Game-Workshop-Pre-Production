using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoom : MonoBehaviour
{
    public List<Trash> trashList;

    private void OnEnable()
    {
        var district = GetComponentInParent<District>();
        if (district != null)
        {
            district.RegisterRoom(this);
        }
    }

    private void OnDisable()
    {
        var district = GetComponentInParent<District>();
        if (district != null)
        {
            district.UnregisterRoom(this);
        }
    }

    void Awake()
    {
        // Collect all trash already inside the room hierarchy
        //trashList = new List<CollectableTrash>(GetComponentsInChildren<CollectableTrash>(true));
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Trash trash))
        {
            if (trashList.Contains(trash)) return;
            trashList.Add(trash);
        }

        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            if (trashList.Count > 0)
            {
                PlayerState.Instance.EnterRoom(this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Trash trash))
        {
            trashList.Remove(trash);
        }
    }
}
