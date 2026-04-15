using UnityEngine;


// Interface with this to react to the player sweep poke.
public interface IPokeable : IHittable
{
    public void OnPoke(Vector2 direction, float force, Collider2D collider, ref float knockbackMultiplier);
    //knockback multiplier can be used for the one Poking to receive knockback
    //it is relative to the force given (0f = no knockback, 1f = equal to Force)

    // IPokeable also must implement IHittable's HitParent
}