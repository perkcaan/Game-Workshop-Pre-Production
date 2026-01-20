using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyPather : MonoBehaviour
{
    private EnemyBase _enemy;
    private bool _isPathing = false;
    private Vector2 _targetDestination;
    private Action _arriveAction;
    private float _arrivalProximity = 0.1f;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBase>();   
    }

    private void FixedUpdate()
    {
        // Only run if currently pathing.
        if (!_isPathing) return;

        if (Vector2.Distance(_targetDestination, transform.position) <= _arrivalProximity)
        {
            Arrive();
        } else
        {
            MoveToPoint();
        }

    }

    // Designed to be able to be called once, or multiple times in order to update the target frequently.
    public void GoToPoint(Vector2 targetDestination, float arrivalProximity, Action onArrive = null)
    {
        _isPathing = true;
        _targetDestination = targetDestination;
        _arriveAction = onArrive;
        _arrivalProximity = arrivalProximity;

    }

    public void Stop()
    {

        _isPathing = false;
        _targetDestination = Vector2.zero;
        _enemy.Animator.SetFloat("Speed", 0f);
        _arriveAction = null;
        _arrivalProximity = 0.1f;
    }

    private void Arrive()
    {
        _arriveAction?.Invoke();
    }

    private void MoveToPoint() //call function on complete
    {
        //TODO: handle facing angle better
        Animator animator = _enemy.Animator;
        Rigidbody2D rigidbody = _enemy.Rigidbody;

        Vector2 targetDirection = (_targetDestination - (Vector2) transform.position).normalized;
        Vector2 frameVelocity = targetDirection * _enemy.MoveSpeed;
        float facingAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Vector2 velocityDelta = frameVelocity - rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta, frameVelocity.magnitude);
        rigidbody.AddForce(clampedForce, ForceMode2D.Force);

        animator.SetFloat("Speed", frameVelocity.magnitude);
        
        _enemy.FacingRotation = facingAngle;
        animator.SetFloat("Rotation", facingAngle);
    }

    private void OnDrawGizmos()
    {
        //draw rays for steering
    }
}
