using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrailTrashMaterial", menuName = "ScriptableObjects/TrashMaterial/TrailTrashMaterial", order = 1)]
public class TrailTrashMaterial : TrashMaterial
{
    public TrashBallTrail trailPrefab;
    public float _sizeMultiplier;
    public bool _burningRequired;
    public bool _swipeRequired;
    public override void whenBallRolls(TrashBall trashBall, TrashMaterialAmount amount)
    {
        if (_burningRequired) if (!trashBall.isBurning) return;
        if (_swipeRequired) if (!trashBall.isSwiped) return;

        TrashBallTrail trail = Instantiate(trailPrefab);
        trail.gameObject.transform.position = trashBall.transform.position;
        trail.gameObject.transform.localScale = Vector2.one * Mathf.Pow(trashBall.Size, 1f / 3f) * _sizeMultiplier;
    }
}