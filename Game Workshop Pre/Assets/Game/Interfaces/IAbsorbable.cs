using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public interface IAbsorbable
{
    public bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb);
    public void OnTrashBallRelease(TrashBall trashBall);
    public void OnTrashBallDestroy();

    public int Size { get; }
    public TrashMaterial TrashMat { get; }
    public int TrashMatWeight { get; }
}
