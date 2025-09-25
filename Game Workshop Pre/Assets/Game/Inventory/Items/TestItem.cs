using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item, IConsumable
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Use()
    {
        Debug.Log("Test Item Used");
    }

    public void Equip()
    {

    }
        

}
