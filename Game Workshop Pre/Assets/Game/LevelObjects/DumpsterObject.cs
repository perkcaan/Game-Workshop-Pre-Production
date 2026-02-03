using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpsterObject : MonoBehaviour
{

    // Trigger-based interaction radius
    [SerializeField] private Collider2D InteractRadius;

    [SerializeField] private Item StoredItem;

    public bool isOpened;

    //*** Interaction Outline Shader


    void Start()
    {
        isOpened = false;
    }

    // Update is called once per frame
    public void InPlayerVicinity()
    {
        Debug.Log("In vicinity of " + this.gameObject);
    }

    public void DumpsterOpened()
    {
        // Spawn Item
    }
}
