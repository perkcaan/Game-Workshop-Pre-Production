using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AOEEnemy : EnemyBase
{
    [SerializeField] private float _attackDuration = 10f;
    [SerializeField] private float _attackCooldown = 0f;
    [SerializeField] private EnemyHeatHitbox _heatArea;

    public void PerformAttack()
    {
        Debug.Log("attacking");
        _blackboard.TryGet<float>("rotation", out float rotation);
        _heatArea.UpdateRotation(transform, rotation);
        _heatArea.Enable();
        _animator.SetBool("Attacking", true);


        StartCoroutine(AttackDuration());
    }

    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(_attackDuration);
        _heatArea.Disable();
        _animator.SetBool("Attacking", false);
        yield return new WaitForSeconds(_attackCooldown);
        _blackboard.Set<bool>("isInAction", false);

    }


    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {

    }
}