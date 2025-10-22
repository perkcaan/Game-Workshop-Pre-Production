using UnityEngine;


// Interface with this to react to the player's broom sweep.
public interface ISweepable
{
    public void OnSweep(Vector2 position, Vector2 direction, float force);

}