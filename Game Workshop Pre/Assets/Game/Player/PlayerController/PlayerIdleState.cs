using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private float _zeroMoveTimer = 0f;


    public PlayerIdleState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
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
        TryChangeState();
    }

    public override void ExitState()
    {
        
    }

    //movement
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxWalkSpeed, _ctx.Acceleration * Time.deltaTime);

            // dont know whether to use move towards or lerp...
        }
        else
        {
            if (_zeroMoveTimer < 0.05)
            {
                _zeroMoveTimer += Time.deltaTime;
            }
            if (_zeroMoveTimer >= 0.05)
            {
                _ctx.MoveSpeed = 0f;
            }
        }

        _ctx.FrameVelocity = _ctx.MoveSpeed * input.normalized;
    }

    private void HandleRotation()
    {
        Vector2 input = _ctx.MovementInput;
        if (input.sqrMagnitude <= 0.01f) return;
        float targetAngle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.LerpAngle(_ctx.Rotation, targetAngle, _ctx.RotationSpeed * Time.deltaTime);
        
        _ctx.Rotation = Mathf.DeltaAngle(0f, newAngle);
    }


    //state
    private void TryChangeState()
    {
        if (_ctx.IsSweepPressed) _state.ChangeState(PlayerStateEnum.Sweeping);
    }

}
