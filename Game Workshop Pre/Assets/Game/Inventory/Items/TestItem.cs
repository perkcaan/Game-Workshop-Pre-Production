using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item, IConsumable
{

    void Start()
    {
        Inventory_Ref = FindObjectOfType<Inventory>();
    }

    public void Use()
    {
        Debug.Log("Test Item Used");
    }

    public void Equip()
    {
        Inventory_Ref.AddItem(this);
    }
        

}
