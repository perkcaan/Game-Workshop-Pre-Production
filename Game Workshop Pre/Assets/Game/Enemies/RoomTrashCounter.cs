using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrashCounter : MonoBehaviour
{
    int trashCount = 0;
    bool playerOccupied = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            playerOccupied = false;
        }
        else if (collision.name.Contains("Trash"))
        {
            trashCount -= 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            playerOccupied = true;
        }
        else if (collision.name.Contains("Trash"))
        {
            trashCount += 1;
        }
    }

    private void Update()
    {
        if (playerOccupied)
        {
            print(trashCount);
        }
    }
}
