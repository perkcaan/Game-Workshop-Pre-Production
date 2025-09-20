using System;
using System.Collections;
using UnityEngine;

public class PlayerSwipingState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private float _zeroMoveTimer = 0f;

    // Constructor
    public PlayerSwipingState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Swiping", true);
        _ctx.CanSwipe = false;
        GetSwipeRotation();
        float swipeForce = _ctx.Player.SwipeForce + _ctx.MoveSpeed * _ctx.Player.SwipeMovementScaler;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Swipe/Swipe",_ctx.Player.transform.position);
        _ctx.SwipeHandler.DoSwipe(_ctx.Rotation, swipeForce);
        _ctx.Player.StartCoroutine(SwipeDuration());
    }

    public override void Update()
    {
        HandleMovement();
        _ctx.SwipeHandler.UpdateHitbox(_ctx.Rotation);
    }

    public override void ExitState()
    {
        _ctx.SwipeHandler.EndSwipe();
        _ctx.SwipeCooldownTimer = _ctx.Player.SwipeCooldown;
        _ctx.Animator.SetBool("Swiping", false);
    }


    private IEnumerator SwipeDuration()
    {
        yield return new WaitForSeconds(_ctx.Player.SwipeDuration);
        LeaveSwipeState();
    }

    //rotation
    private void GetSwipeRotation()
    {
        float targetAngle = Mathf.Atan2(_ctx.StickInput.y, _ctx.StickInput.x) * Mathf.Rad2Deg;

        _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle); 
    }

    //movement
    // This is the exact same movement and PlayerIdleState
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxWalkSpeed, _ctx.Acceleration * Time.deltaTime);
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

    private void LeaveSwipeState()
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