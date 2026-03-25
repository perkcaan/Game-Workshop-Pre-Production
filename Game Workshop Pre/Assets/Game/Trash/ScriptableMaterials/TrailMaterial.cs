using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrailTrashMaterial", menuName = "ScriptableObjects/TrashMaterial/TrailTrashMaterial", order = 1)]
public class TrailTrashMaterial : TrashMaterial
{
    public TrashBallTrail trailPrefab;
    public float _sizeMultiplier;

    public override void whenBallRolls(TrashBall trashBall, TrashMaterialAmount amount)
    {
        Debug.Log("paper");
        TrashBallTrail fire = Instantiate(trailPrefab);
        fire.gameObject.transform.position = trashBall.transform.position;
        fire.gameObject.transform.localScale = Vector2.one * Mathf.Pow(trashBall.Size, 1f / 3f) * _sizeMultiplier;
    }
}