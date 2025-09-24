using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Name and Icon")]

    public string itemName; // Item name

    public Sprite Icon; // Icon is played in Inventory UI

    [SerializeField] private GameObject itemRadius;

    [SerializeField] protected Inventory Inventory_Ref; // References Inventory singleton


}
