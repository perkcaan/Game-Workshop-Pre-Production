using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRadius : MonoBehaviour
{
    private Item parentItem;

    private void Start()
    {
        parentItem = GetComponentInParent<Item>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log(parentItem.name);
        }
    }
}
