using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StainTrash : Trash, ISweepable
{
    [SerializeField] float _sweepDurationToClean;
    private float _sweepTimer;
    private SpriteRenderer _sprite;

    [SerializeField] int _pointValue;
    public static Action<int> SendScore;

    void Start()
    {
        if (_pointValue <= 0) _pointValue = 1;
    }

    public void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime;
        _sprite.color = new Color(1f, 1f, 1f, _sweepDurationToClean - _sweepTimer + 0.2f);
        if (_sweepTimer > _sweepDurationToClean)
        {
            Destroy(gameObject);
        }
    }
    public override void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        SendScore?.Invoke(_pointValue);
        Destroy(gameObject);
    }

}
