using System;
using System.Collections;
using UnityEngine;

public class PlayerHookState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    private float _zeroMoveTimer = 0f;
    private bool _hasHookBeenThrown = false;

    // Constructor
    public PlayerHookState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.CanHook = false;
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        _hasHookBeenThrown = false;
        
        // _ctx.Animator.SetBool("HookAim", true);
    }

    public override void Update()
    {
        HandleMovement();

        if (!_hasHookBeenThrown)
        {
            Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(_ctx.MouseInput);
            Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle);

            if (!_ctx.IsHookPressed) 
            {
                ThrowHook();
            }
        }
        else
        {
            if (!_ctx.HookHandler.IsActive)
            {
                LeaveHookState();
            }
        }
    }

    public override void ExitState()
    {
        if (_ctx.HookHandler.IsActive)
        {
            _ctx.HookHandler.StartRetract();
        }
        
        _ctx.HookCooldownTimer = _ctx.Player.HookCooldown;
        // _ctx.Animator.SetBool("HookAim", false);
    }

    private void ThrowHook()
    {
        _hasHookBeenThrown = true;
        float force = _ctx.Player.HookPullForce;
        _ctx.HookHandler.ThrowHook(_ctx.Rotation, force, _ctx.Player.HookDuration);
    }

    //movement
    // This is the exact same movement as PlayerIdleState
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxHookWalkSpeed, _ctx.Acceleration * Time.fixedDeltaTime);
        }
        else
        {
            if (_zeroMoveTimer < 0.02)
            {
                _zeroMoveTimer += Time.deltaTime;
            } else
            {
                _ctx.MoveSpeed = 0f;
                // Cancels sliding with an opposing force
                Vector2 velocity = _ctx.Rigidbody.velocity;
                if ((velocity.magnitude > 0.5f) && (velocity.magnitude < _ctx.MaxHookWalkSpeed) && _ctx.Props.WillCancelSwipeSlide)
                {
                    Vector2 fullCancelForce = -velocity.normalized * _ctx.MaxHookWalkSpeed;
                    _ctx.FrameVelocity = Vector2.ClampMagnitude(fullCancelForce, (-velocity * _ctx.Rigidbody.mass / Time.fixedDeltaTime).magnitude);
                    return;
                }
            }
        }

        _ctx.FrameVelocity = _ctx.MoveSpeed * input.normalized;
    }

    private void LeaveHookState()
    {
        if (_ctx.PlayerHasControl)
        {
            if (_ctx.IsSweepPressed)
            {
                _state.ChangeState(PlayerStateEnum.Sweeping);
            }
            else
            {
                _state.ChangeState(PlayerStateEnum.Idle);
            }
        }
    }
}