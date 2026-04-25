using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*** Enemies must be able to find and target trash before
// full implementation


public class SlugEnemy : EnemyBase
{
    [Header("Slug Properties")]
    [SerializeField] private float _eatRadius = 3f;
    [SerializeField] private float _stomachLimit = 30f;

    private EnemyBlackboard _blackboard; 
    public EnemyBlackboard Blackboard => _blackboard;

    private GameObject _currentTarget;
    private float _currentTargetSize;

    protected override void OnStart()
    {

        _blackboard = new EnemyBlackboard(this);
        _blackboard.Set("stomachSize", 0f);
    }

    protected override void OnUpdate()
    {
        if (Blackboard.TryGet("stomachSize", out float size))
        {
            Debug.Log("Slug stomach size: " + size);
        }
    }

    public void EatTrashAction(GameObject target, float size)
    {
        if (target == null) return;

        // Add size to blackboard
        if (Blackboard.TryGet("stomachSize", out float currentSize))
        {
            Blackboard.Set("stomachSize", currentSize + size);
        }

        Destroy(target);
    }

    public bool IsStuffed()
    {
        if (Blackboard.TryGet("stomachSize", out float size))
        {
            return size >= _stomachLimit;
        }
        return false;
    }
}