using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMaterial", menuName = "ScriptableObjects/TrashMaterial", order = 1)]
public class TrashMaterial : ScriptableObject
{
    public Color color;
    public PhysicsMaterial2D material;
    public float mass;
    public float drag;
    public float decayMultiplier;
    public float damageMultiplier;
    public float absorbMultiplier;
}
