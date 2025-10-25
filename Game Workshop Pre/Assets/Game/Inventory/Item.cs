using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    [Header("Name and Icon")]
    public Sprite displayIcon;
    public string displayName;
    public string discriptionText;

    public void EquipItem()
    {

    }
    public void UnequipItem()
    {

    }
}
