using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public PlayerMovementController player;
    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}
