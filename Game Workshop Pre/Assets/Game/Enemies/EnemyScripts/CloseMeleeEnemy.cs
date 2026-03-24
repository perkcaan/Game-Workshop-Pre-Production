using System;
using System.Collections;
using UnityEngine;

public class CloseMeleeEnemy : EnemyBase
{
    [Header("Close Melee Enemy")]
    [SerializeField] private float _attackDashForce = 20f;
    [SerializeField] private SimpleAttackProperties _attackProperties;
    [SerializeField] private HeatAttackHitbox _attackHitbox;
    private bool _isInVulnerableState = false;
    

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
            AudioManager.Instance.PlayOnInstance(gameObject, "attack");
            // Dash in attacking direction
            float radians = _facingRotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            Rigidbody.AddForce(direction * _attackDashForce, ForceMode2D.Impulse);
            _animator.SetTrigger("DoAttack");
        },
        attackEnd: () =>
        {
            _attackHitbox.Disable();
            _isInVulnerableState = true;
            _animator.SetTrigger("ReturnToIdle");
        });
        _isInVulnerableState = false;
        onComplete?.Invoke(true);
    }
    
    protected override void OnStart() { }

    protected override void OnUpdate() { }
    protected override void ForceDisableHitboxes()
    {
        _attackHitbox.Disable();
    }

    protected override void ModifySwipe(ref EnemySwipeData data)
    {
        if (_isInVulnerableState) data.IsVulnerable = true;
    }

    protected override void ModifyPoke(ref EnemyPokeData data)
    {
        if (_isInVulnerableState) data.IsVulnerable = true;
    }

    protected override void ModifyAbsorb(ref EnemyAbsorbData data)
    {
        if (_isInVulnerableState) data.CanAbsorb = true;
    }

}
