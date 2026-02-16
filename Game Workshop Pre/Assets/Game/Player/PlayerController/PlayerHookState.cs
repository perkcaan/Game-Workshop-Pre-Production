using System;
using System.Collections;
using UnityEngine;

public class PlayerHookState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private float _zeroMoveTimer = 0f;
    private float _chargeTimer = 0f;
    private bool _hasHookBeenThrown = false;
    private Coroutine _swipeCoroutine;

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
        //_ctx.Animator.SetBool("HoldingSwipe", true);
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        _hasHookBeenThrown = false;
        _chargeTimer = 0;
    }

    public override void Update()
    {
        HandleMovement();
        if (!_ctx.IsHookPressed && !_hasHookBeenThrown) DoSwipe();
        if (_hasHookBeenThrown)
        {
            //_ctx.SwipeHandler.UpdateHitbox(_ctx.Rotation);
        }
        else
        {
            Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
            direction.Normalize();
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle);
        }

        if (_chargeTimer > 0 && !_hasHookBeenThrown)
        {
            //_ctx.SwipeHandler.UpdateLine(_ctx.Rotation, _ctx.Player.SwipeVisualLineDistance, _ctx.Player.SwipeVisualLineSegments);   
        }
        _chargeTimer = _chargeTimer + Time.deltaTime;
    }

    public override void ExitState()
    {
        //if (_swipeCoroutine != null) _ctx.Player.StopCoroutine(_swipeCoroutine);
        //_ctx.SwipeHandler.EndSwipe();
        //_ctx.SwipeCooldownTimer = _ctx.Player.SwipeCooldown;
        //_ctx.Animator.SetBool("Swiping", false);
    }


    private IEnumerator SwipeDuration()
    {
        yield return new WaitForSeconds(_ctx.Player.SwipeDuration);
        LeaveSwipeState();
    }

    //swipe
    private void DoSwipe()
    {
        int rotation = Mathf.RoundToInt(_ctx.Rotation/45f)*45;
        Quaternion burstRotation = Quaternion.Euler(0, 0, rotation-90);
        ParticleManager.Instance.Play("PlayerSwipeDust", _ctx.Player.transform.position, burstRotation, parent:_ctx.Player.transform);

        //_ctx.Animator.SetBool("Swiping", true);
        //_ctx.Animator.SetBool("HoldingSwipe", false);
        _hasHookBeenThrown = true;
        //_ctx.SwipeHandler.HideLine();

        float chargeSwipeForce = Mathf.Lerp(_ctx.Player.BaseSwipeForce, _ctx.Player.FullChargeSwipeForce, _chargeTimer / _ctx.Player.SwipeHoldChargeTime);
        float swipePower = chargeSwipeForce + _ctx.MoveSpeed * _ctx.Player.SwipeForceMovementScaler;
        
        //_ctx.HookHandler.DoSwipe(_ctx.Rotation, swipePower);
        //_swipeCoroutine = _ctx.Player.StartCoroutine(SwipeDuration());
    }

    //movement
    // This is the exact same movement as PlayerIdleState
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxSwipeWalkSpeed, _ctx.Acceleration * Time.fixedDeltaTime);
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
                if ((velocity.magnitude > 0.5f) && (velocity.magnitude < _ctx.MaxSwipeWalkSpeed) && _ctx.Props.WillCancelSwipeSlide)
                {
                    Vector2 fullCancelForce = -velocity.normalized * _ctx.MaxSwipeWalkSpeed;
                    _ctx.FrameVelocity = Vector2.ClampMagnitude(fullCancelForce, (-velocity * _ctx.Rigidbody.mass / Time.fixedDeltaTime).magnitude);
                    return;
                }
            }
        }

        _ctx.FrameVelocity = _ctx.MoveSpeed * input.normalized;
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