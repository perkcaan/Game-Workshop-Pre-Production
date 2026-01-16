using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CloseMeleeEnemy : EnemyBase
{
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private EnemyHeatHitbox _attackHitbox;

    public void PerformAttack()
    {
        /* need to fix
        _blackboard.TryGet<float>("rotation", out float rotation);
        _attackHitbox.UpdateRotation(transform, rotation);
        _attackHitbox.Enable();
        _animator.SetBool("Attacking", true);


        StartCoroutine(AttackDuration());
        */
    }

    public IEnumerator MeleeAttack(Action<bool> onComplete)
    {
        Debug.Log("Startup...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Melee attack!");
        yield return new WaitForSeconds(1f);
        Debug.Log("Endlag finished");
        onComplete?.Invoke(true);
    }

    private IEnumerator AttackDuration()
    {
        yield return new WaitForEndOfFrame(); /* need to fix
        yield return new WaitForSeconds(_attackDuration);
        _attackHitbox.Disable();
        _animator.SetBool("Attacking", false);
        yield return new WaitForSeconds(_attackCooldown);
        _blackboard.Set<bool>("isInAction", false);
        */
    }


    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {

    }
}
