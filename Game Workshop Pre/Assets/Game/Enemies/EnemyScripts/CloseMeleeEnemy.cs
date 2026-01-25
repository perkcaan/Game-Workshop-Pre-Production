using System;
using System.Collections;
using UnityEngine;

public class CloseMeleeEnemy : EnemyBase
{
    [SerializeField] private float _attackDashForce = 20f;
    [SerializeField] private SimpleAttackProperties _attackProperties;
    [SerializeField] private HeatAttackHitbox _attackHitbox;

    public IEnumerator MeleeAttack(Action<bool> onComplete)
    {
        yield return SimpleAttack(_attackProperties,
        attackStart: () =>
        {
            _animator.SetTrigger("StartAttack");   
        },
        attack: () =>
        {
            // Enable attack hitbox
            _attackHitbox.UpdateRotation(transform, _facingRotation);
            _attackHitbox.Enable();

            // Dash in attacking direction
            float radians = _facingRotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            Rigidbody.AddForce(direction * _attackDashForce, ForceMode2D.Impulse);

            _animator.SetTrigger("DoAttack");
        },
        attackEnd: () =>
        {
            _attackHitbox.Disable();
            _animator.SetTrigger("ReturnToIdle");
        });
        onComplete?.Invoke(true);
    }
    
    protected override void OnStart() { }

    protected override void OnUpdate() { }
}
