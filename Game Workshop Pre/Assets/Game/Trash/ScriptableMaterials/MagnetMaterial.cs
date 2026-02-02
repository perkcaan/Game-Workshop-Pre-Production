using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagnetTrashMaterial", menuName = "ScriptableObjects/TrashMaterial/MagnetTrashMaterial", order = 1)]
public class MagnetTrashMaterial : TrashMaterial
{
    public override void materialEnd(TrashBall trashBall)
    {
        trashBall.MagnetCollider.radius = 0.55f;
    }

    public override void whenBallRolls(TrashBall trashBall, TrashMaterialAmount amount)
    {
        float maxMagnetRadius = 3 / ((float)amount + 1);
        trashBall.MagnetCollider.radius = Mathf.Clamp((trashBall.Rigidbody.velocity.magnitude - 2) / 2, 0.55f, maxMagnetRadius);
    }
}