using UnityEngine;


// Interface with this to react to the player sweep poke.
public interface IPokeable
{
    public void OnPoke(Vector2 direction, float force, Collider2D collider);
}