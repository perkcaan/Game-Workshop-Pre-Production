using UnityEngine;


// Interface with this to react to the player and enemy swipes.
public interface ISwipeable : IHittable
{
    public void OnSwipe(Vector2 direction, float force, Collider2D collider, ref float knockbackMultiplier);
    //knockback multiplier can be used for the one Swiping to receive knockback
    //it is relative to the force given (0f = no knockback, 1f = equal to Force)

    // ISwipeable also must implement IHittable's HitParent
}