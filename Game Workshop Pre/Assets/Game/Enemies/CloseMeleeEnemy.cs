using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CloseMeleeEnemy : EnemyBase, ICleanable
{
    [SerializeField] private float _attackDashForce = 20f;

    [SerializeField] private float _attackStartup = 0.3f;
    [SerializeField] private float _attackDuration = 0.5f;
    [SerializeField] private float _attackEndlag = 1f;
    [SerializeField] private EnemyHeatHitbox _attackHitbox;
    private Room _parentRoom;

   
    public int Size { get { return (int)_size; } }

    public IEnumerator MeleeAttack(Action<bool> onComplete)
    {

        _animator.SetTrigger("StartAttack");   
        yield return new WaitForSeconds(_attackStartup);

        // Enable attack hitbox
        _attackHitbox.UpdateRotation(transform, _facingRotation);
        _attackHitbox.Enable();

        // Dash in attacking direction
        float radians = _facingRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        Rigidbody.AddForce(direction * _attackDashForce, ForceMode2D.Impulse);

        _animator.SetTrigger("DoAttack");
        yield return new WaitForSeconds(_attackDuration);

        // Disable attack hitbox
        _attackHitbox.Disable();

        _animator.SetTrigger("ReturnToIdle");

        yield return new WaitForSeconds(_attackEndlag);
        onComplete?.Invoke(true);
    }

    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {

    }

    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }
}
