using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbsorbable
{
    public void OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb);
    public void OnTrashBallExplode(TrashBall trashBall);
    public void OnTrashBallIgnite();
}
