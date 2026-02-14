using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStatEffect", menuName = "ScriptableObjects/Item Effects/PlayerStats", order = 1)]
public class PlayerStatEffect : ItemEffect
{
    public enum StatType
    {
        canSweep,
        canSwipe,
    }
    public StatType statToModify;
    public float boostAmount;
    public override void ApplyEffect()
    {
        switch (statToModify)
        {
            case StatType.canSweep:
                player.canSweep = true;
                break;
            case StatType.canSwipe:
                player.canSwipe = true;
                break;
        }
    }
    public override void RemoveEffect()
    {
        // items cant be unequipped, so this is just for future use if unequipping comes back
    }
}
