using UnityEngine;


// Interface with this to react to the player and enemy swipes.
public interface ISwipeable
{
    public void OnSwipe(Vector2 direction, float force);

}