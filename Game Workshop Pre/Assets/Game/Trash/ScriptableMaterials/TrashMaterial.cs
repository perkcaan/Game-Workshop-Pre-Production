using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMaterial", menuName = "ScriptableObjects/TrashMaterial", order = 1)]
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

    public virtual void whenBallRolls() { }
    public virtual void whenBallSwiped() { }
    public virtual void whenBallHitsWall() { }
}

[CreateAssetMenu(fileName = "TrashMaterial", menuName = "ScriptableObjects/TrashMaterial", order = 1)]
public class TrailTrashMaterial : TrashMaterial
{
    public override void whenBallRolls()
    {
        
    }
}

[CreateAssetMenu(fileName = "TrashMaterial", menuName = "ScriptableObjects/TrashMaterial", order = 1)]
public class BombTrashMaterial : TrashMaterial
{
    public override void whenBallRolls()
    {
        
    }
}


