using FMOD;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private static Inventory Inventory_Instance; // One instance of Inventory

    public Item[] ItemSlots = new Item[4]; // Slots for possessed items. The first is reserved for Mops

    public int selectedSlot; // Points to the Item Slot the player wants to modify

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

        // ** Maintain item icons
    }

    public void AddItem(Item item)
    {
        // If the item is not a Mop, assign it to selected slot so long as it's not 0

        if (item.tag != "Mop" && selectedSlot != 0)
        {
            RemoveItem(selectedSlot);
            ItemSlots[selectedSlot] = item;
        }

        // If the item is not a Mop and the selected slot is 0, issue a warning
        else if (item.tag != "Mop" && selectedSlot == 0)
        {
            // ** Issue a warning noise. Non-mop cannot be equipped in Mop slot
        }

        // if the item is a Mop, no matter the selected slot, assign it to 0
        else if (item.tag == "Mop")
        {
            RemoveItem(0);
            ItemSlots[0] = item;
        }

    }

    public void RemoveItem(int slotNum)
    {
        // ** Instantiate the removed Item on the ground

        // Remove what was within the Item slot

        if (ItemSlots[slotNum] != null)
        {
            ItemSlots[slotNum] = null;
        }
    }
}
