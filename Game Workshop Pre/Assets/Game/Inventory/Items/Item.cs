using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Name and Icon")]

    public string name;

    public Sprite Icon; // Icon displayed in Inventory UI

    [SerializeField] private GameObject itemRadius;

    public TextMeshProUGUI itemDescUI;

    [SerializeField] private Inventory Inventory_Ref; // References Inventory singleton


}
