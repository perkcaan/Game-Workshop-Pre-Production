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
    private float _chargeTimer = 0f;
    private bool _hasSwipeBeenActivated = false;
    private Coroutine _swipeCoroutine;

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
        _ctx.Animator.SetBool("HoldingSwipe", true);
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        _hasSwipeBeenActivated = false;
        _chargeTimer = -_ctx.Player.SwipeTimeUntilHold;
    }

    public override void Update()
    {
        HandleMovement();
        if (!_ctx.IsSwipePressed && !_hasSwipeBeenActivated) DoSwipe();
        if (_hasSwipeBeenActivated)
        {
            _ctx.SwipeHandler.UpdateHitbox(_ctx.Rotation);
        }
        else
        {
            Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle);
        }
        if (_chargeTimer > 0 && !_hasSwipeBeenActivated)
        {
            _ctx.SwipeHandler.UpdateLine(_ctx.Rotation, _ctx.Player.SwipeVisualLineDistance, _ctx.Player.SwipeVisualLineSegments);   
        }
        _chargeTimer = Mathf.Min(_chargeTimer + Time.deltaTime, _ctx.Player.SwipeHoldChargeTime);
    }

    public override void ExitState()
    {
        if (_swipeCoroutine != null) _ctx.Player.StopCoroutine(_swipeCoroutine);
        _ctx.SwipeHandler.EndSwipe();
        _ctx.SwipeCooldownTimer = _ctx.Player.SwipeCooldown;
        _ctx.Animator.SetBool("Swiping", false);
    }


    private IEnumerator SwipeDuration()
    {
        yield return new WaitForSeconds(_ctx.Player.SwipeDuration);
        LeaveSwipeState();
    }

    //swipe
    private void DoSwipe()
    {
        _ctx.Animator.SetBool("Swiping", true);
        _ctx.Animator.SetBool("HoldingSwipe", false);
        _hasSwipeBeenActivated = true;
        _ctx.SwipeHandler.HideLine();

        float chargeTime = Mathf.Max(_chargeTimer, 0f);
        float chargeSwipeForce = Mathf.Lerp(_ctx.Player.BaseSwipeForce, _ctx.Player.FullChargeSwipeForce, chargeTime / _ctx.Player.SwipeHoldChargeTime);
        float swipePower = chargeSwipeForce + _ctx.MoveSpeed * _ctx.Player.SwipeForceMovementScaler;
        _ctx.SwipeHandler.DoSwipe(_ctx.Rotation, swipePower);
        _swipeCoroutine = _ctx.Player.StartCoroutine(SwipeDuration());
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

        _ctx.FrameVelocity = _ctx.MaxSwipeWalkSpeed * input.normalized;
    }

    private void LeaveSwipeState()
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