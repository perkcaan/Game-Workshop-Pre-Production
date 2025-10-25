using FMOD;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] int maxItemSlots;
    [SerializeField] TextMeshProUGUI displayedItemText;
    [SerializeField] TextMeshProUGUI displayedItemTitle;
    [SerializeField] GameObject itemSlotsObject;
    [SerializeField] List<Item> equippedItems = new List<Item>();
    List<ItemSlot> itemSlots = new List<ItemSlot>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        foreach (ItemSlot slot in itemSlotsObject.GetComponentsInChildren<ItemSlot>())
        {
            itemSlots.Add(slot);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void DisplayItem(Item item)
    {
        displayedItemTitle.SetText(item.displayName);
        displayedItemText.SetText(item.discriptionText);
    }

    public void StoreItem(Item newItem)
    {
        ItemSlot emptySlot = null;
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.storedItem == null)
            {
                emptySlot = slot;
                slot.StoreItem(newItem);
                return;
            }
        }
    }
    public void RemoveItem(Item item)
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.storedItem.Equals(item))
            {
                slot.ClearItem();
                return;
            }
        }
    }

    public bool EquipItem(Item item)
    {
        if (equippedItems.Count > maxItemSlots)
        {
            displayedItemTitle.SetText("Max Item slots!");
            displayedItemText.SetText("unequip something first");
            return false;
        }
        equippedItems.Add(item);
        item.EquipItem();
        return true;
    }

    // return false for things that cant be unequipped like brooms
    public bool UnequipItem(Item item)
    {
        equippedItems.Remove(item);
        item.UnequipItem();
        return true;
    }

}
