using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsumable
{
    void Use(); // Customizable function to determine Item's abilities

    void Equip(); // Equips Item to the Inventory
}
