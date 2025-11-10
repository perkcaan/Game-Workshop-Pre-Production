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
    public virtual void whenBallIgnite() { }
    public virtual void whenAbsorbTrash() { }
}


[CreateAssetMenu(fileName = "BombTrashMaterial", menuName = "ScriptableObjects/BombTrashMaterial", order = 1)]
public class BombTrashMaterial : TrashMaterial
{
    private int health = 3;
    public override void whenBallSwiped() {
        health--;
        if(health < 0)
        {
            // explode
        }
     }

    public override void whenAbsorbTrash()
    {
        // maybe heal on absorbing trash? im not sure
    }
}


