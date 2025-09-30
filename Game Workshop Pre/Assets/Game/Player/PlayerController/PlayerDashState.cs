using System;
using System.Collections;
using UnityEngine;

public class PlayerDashState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields

    // Constructor
    public PlayerDashState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Dashing", true);
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        _ctx.DashesRemaining -= 1;
        Vector2 velocityToUse = _ctx.FrameVelocity.normalized;
        _ctx.FrameVelocity = Vector2.zero;
        _ctx.Player.StartCoroutine(DashDuration());

        // Fallback to using rotation instead of current velocity if you're standing still
        if (velocityToUse == Vector2.zero)
        {
            float radians = _ctx.Rotation * Mathf.Deg2Rad;
            velocityToUse = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }
        _ctx.Rigidbody.AddForce(velocityToUse * _ctx.Props.DashForce, ForceMode2D.Impulse);
    }

    public override void Update()
    {

    }

    public override void ExitState()
    {
        _ctx.SwipeHandler.EndSwipe();
        _ctx.DashCooldownTimer = _ctx.Props.DashCooldown;
        _ctx.DashRowCooldownTimer = _ctx.Props.DashRowCooldown;
        _ctx.Animator.SetBool("Dashing", false);
    }


    private IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(_ctx.Props.DashDuration);
        LeaveDashState();
    }

    private void LeaveDashState()
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