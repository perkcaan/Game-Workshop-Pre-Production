using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private static Inventory Inventory_Instance; // One instance of Inventory

    public Item[] ItemSlots = new Item[4]; // Slots for possessed items. The first is reserved for Mops

    public int selected_slot;

    // * Put ref to UI Inventory Icons

    private void Awake()
    {

        if (Inventory_Instance != null && Inventory_Instance != this)            
        {
                
            Destroy(gameObject); // Prevent duplicates
                
            return;
            
        }

        Inventory_Instance = this;    
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (ItemSlots[0].tag != "Mop")
        {
            ItemSlots[0] = null;
        }
    }

    public void AddItem(Item item)
    {
        if (item.tag != "Mop")
        {
            if (ItemSlots[selected_slot] != null)
            {
                RemoveItem(ItemSlots[selected_slot]);
            }
            ItemSlots[selected_slot] = item;
            RemoveItem(ItemSlots[selected_slot]);
        }
        else
        {
            RemoveItem(ItemSlots[0]);
            ItemSlots[0] = item;
        }
    }

    public void RemoveItem(Item item)
    {
        // Instantiate the removed item on the ground
    }
}
