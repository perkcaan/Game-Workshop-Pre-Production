using System.Collections.Generic;
using UnityEngine;

// This is the item that is stored inside the players inventory
[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{   
    [Header("Display Info")]
    public Sprite displayIcon;
    public string displayName;
    [TextArea(3, 5)]
    public string discriptionText;
    public List<ItemEffect> effects = new List<ItemEffect>();

    public void EquipItem(PlayerMovementController player)
    {
        foreach (var effect in effects)
        {
            effect.player = player;
            effect.ApplyEffect();
        }
    }
    public void UnequipItem()
    {
        foreach (var effect in effects)
        {
            effect.RemoveEffect();
        }
    }
}
