using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public interface IAbsorbable
{
    public bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb);
    public void OnTrashBallRelease(TrashBall trashBall, Vector2 unitVectorAngle);
    public void OnTrashBallDestroy();
    public void TrashBallUpdate(TrashBall trashBall) { }

    public int Size { get; }
    public TrashMaterial TrashMat { get; }
    public int TrashMatWeight { get; }
}
