using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStatEffect", menuName = "ScriptableObjects/Item Effects/PlayerStats", order = 1)]
public abstract class PlayerStatEffect : ItemEffect
{
    public enum StatType
    {
        Speed,
    }
    public StatType statToModify;
    public float boostAmount;
        
    public override void ApplyEffect()
    {
        
    }
    public override void RemoveEffect()
    {
        
    }
}
