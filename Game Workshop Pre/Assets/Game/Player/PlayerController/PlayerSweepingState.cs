using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerSweepingState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private float _zeroMoveTimer = 0f;

    public PlayerSweepingState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Sweeping", true);
        _ctx.CanSwipe = true;
        _ctx.CanDash = true;
        float sweepForce = _ctx.Player.SweepForce + _ctx.MoveSpeed * _ctx.Player.SweepMovementScaler;
        _ctx.SweepHandler.BeginSweep(_ctx.Rotation, sweepForce);
    }

    public override void Update()
    {
        HandleMovement();
        HandleRotation();
        float sweepForce = _ctx.Player.SweepForce + _ctx.MoveSpeed * _ctx.Player.SweepMovementScaler;
        _ctx.SweepHandler.UpdateHitbox(_ctx.Rotation, sweepForce);
        TryChangeState();
    }

    public override void ExitState()
    {
        _ctx.SweepHandler.EndSweep();
        _ctx.Animator.SetBool("Sweeping", false);
    }
    
    public override void OnDrawGizmos()
    {
        DrawRotationGizmo();
    }


    //movement
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxSweepSpeed, _ctx.SweepAcceleration * Time.deltaTime);
         
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
        float targetAngle = Mathf.Atan2(_ctx.StickInput.y, _ctx.StickInput.x) * Mathf.Rad2Deg;
        // Rotate slower based on speed
        // Disabled. I think this feels bad. -Zachs
        //float rotationSpeedReduction = Mathf.Max(_ctx.MoveSpeed / _ctx.MaxWalkSpeed, 1);
        //float newAngle = Mathf.LerpAngle(_ctx.Rotation, targetAngle, _ctx.SweepRotationSpeed / rotationSpeedReduction * Time.deltaTime);

        _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle);
        
    }

    //state
    private void TryChangeState()
    {
        if (!_ctx.IsSweepPressed) _state.ChangeState(PlayerStateEnum.Idle);
    }

    private void DrawRotationGizmo()
    {
        //gizmo settings
        float distance = 0.6f;
        float circleRadius = 0.2f;

        //draw circle at rotation
        Vector3 offset = new Vector3(Mathf.Cos(_ctx.Rotation * Mathf.Deg2Rad), Mathf.Sin(_ctx.Rotation * Mathf.Deg2Rad), 0f) * distance;
        Vector3 circlePos = _ctx.Player.transform.position + offset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(circlePos, circleRadius);
        
    }
}
