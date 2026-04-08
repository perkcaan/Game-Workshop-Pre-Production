using UnityEngine;


// Interface used with ISwipeable and IPokeable
public interface IHittable
{
    public GameObject HitParent { get; }
}