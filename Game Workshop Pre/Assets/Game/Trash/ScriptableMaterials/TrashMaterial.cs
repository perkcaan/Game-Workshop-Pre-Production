using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMaterial", menuName = "ScriptableObjects/TrashMaterial/Generic", order = 1)]
public class TrashMaterial : ScriptableObject
{
    public Color color;
    public float bounciness;
    public float mass;
    public float drag;
    public float decayMultiplier;
    public float damageMultiplier;
    public float swipeForceMultiplier;
    public float knockbackMultiplier;
    public float sizeToAbsorbChange;

    public virtual void materialEnd(TrashBall trashBall) { }
    public virtual void whenBallRolls(TrashBall trashBall, TrashMaterialAmount amount) { }
    public virtual void whenBallSwiped(TrashBall trashBall, TrashMaterialAmount amount) { }
    public virtual void whenBallHitsWall(TrashBall trashBall, TrashMaterialAmount amount) { }
    public virtual void whenBallIgnite(TrashBall trashBall, TrashMaterialAmount amount) { }
    public virtual void whenAbsorbTrash(TrashBall trashBall, TrashMaterialAmount amount) { }
}

public enum TrashMaterialAmount
{
    Dominant,
    Primary,
    Secondary,
}
