using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbsorbable
{
    public void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb);
    public void OnTrashBallExplode(TrashBall trashBall);
}
