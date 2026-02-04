using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StainTrash : Trash, ISweepable
{
    [SerializeField] float _sweepDurationToClean;
    private float _sweepTimer;

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime;
        _spriteRenderer.color = new Color(1f, 1f, 1f, _sweepDurationToClean - _sweepTimer + 0.2f);
        if (_sweepTimer > _sweepDurationToClean)
        {
            Destroy(gameObject);
        }
    }
    public override bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        ScoreBehavior.SendScore?.Invoke(_pointValue);
        Destroy(gameObject);
        return false;
    }

}
