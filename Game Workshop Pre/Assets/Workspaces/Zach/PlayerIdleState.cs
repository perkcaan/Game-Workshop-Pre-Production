using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BaseState<PlayerStateEnum>
{
    // Player Context
    private PlayerContext _ctx;
    // Fields
    //movement
    private float _currentVelocity = 0f;
    private float _rotation = 0f;
    private float _zeroMoveTimer = 0f;

    // Components
    public PlayerIdleState(PlayerContext context)
    {
        _ctx = context;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _zeroMoveTimer = 0f;
    }

    public override void Update()
    {
        HandleMovement();
        HandleRotation();
    }


    //movement

    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _currentVelocity = Mathf.Lerp(_currentVelocity, _ctx.MaxWalkSpeed, _ctx.Acceleration * Time.deltaTime);
        }
        else
        {
            if (_zeroMoveTimer < 0.05)
            {
                _zeroMoveTimer += Time.deltaTime;
            }
            if (_zeroMoveTimer >= 0.05)
            {
                _currentVelocity = 0f;
            }
        }

        _ctx.Animator.SetFloat("Speed", _currentVelocity);
        _ctx.RigidBody.velocity = _currentVelocity * input.normalized;
    }
    
    private void HandleRotation()
    {
        Vector2 input = _ctx.MovementInput;
        if (input.sqrMagnitude <= 0.01f) return;
        float targetAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        _rotation = Mathf.LerpAngle(_rotation, targetAngle, _ctx.RotationSpeed * Time.deltaTime);

        _rotation %= 360;
        if (_rotation <= -180) _rotation += 360;
        else if (_rotation > 180) _rotation -= 360;

        _ctx.Animator.SetFloat("Rotation", _rotation);
    }
}
