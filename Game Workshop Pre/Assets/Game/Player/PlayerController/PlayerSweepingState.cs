using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerSweepingState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;
    private float _dustParticleCooldown = 0f;
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
        float sweepForce = _ctx.Player.SweepForce + _ctx.MoveSpeed * _ctx.Player.SweepForceMovementScaler;
        _ctx.SweepHandler.BeginSweep(_ctx.Rotation, sweepForce);
    }

    public override void Update()
    {
        _dustParticleCooldown -= Time.deltaTime;
        if (_dustParticleCooldown <= 0f)
        {
            ParticleManager.Instance.Play("PlayerSweepDust", _ctx.Player.transform.position, Quaternion.Euler(0, 0, _ctx.Rotation), parent:_ctx.Player.transform);
            _dustParticleCooldown = 0.3f;
        }
        HandleMovement();
        HandleRotation();
        float sweepForce = _ctx.Player.SweepForce + _ctx.MoveSpeed * _ctx.Player.SweepForceMovementScaler;
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
    // This is the exact same movement as PlayerIdleState
    private void HandleMovement()
    {
        Vector2 input = _ctx.MovementInput;

        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveSpeed = Mathf.Lerp(_ctx.MoveSpeed, _ctx.MaxSweepWalkSpeed, _ctx.Acceleration * Time.fixedDeltaTime);
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
                if ((velocity.magnitude > 0.5f) && (velocity.magnitude < _ctx.MaxSweepWalkSpeed) && _ctx.Props.WillCancelSweepSlide)
                {
                    Vector2 fullCancelForce = -velocity.normalized * _ctx.MaxSweepWalkSpeed;
                    _ctx.FrameVelocity = Vector2.ClampMagnitude(fullCancelForce, (-velocity * _ctx.Rigidbody.mass / Time.fixedDeltaTime).magnitude);
                    return;
                }
            }
        }

        _ctx.FrameVelocity = _ctx.MoveSpeed * input.normalized;
    }

    private void HandleRotation()
    {
        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
        direction.Normalize();
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Rotate slower based on speed - Disabled because I didnt like it. -Zach
        float rotationSpeedReduction = 1f; //Mathf.Max(_ctx.MoveSpeed / _ctx.MaxWalkSpeed, 1);

        float newAngle = Mathf.LerpAngle(_ctx.Rotation, targetAngle, _ctx.Props.RotationSpeed / rotationSpeedReduction * Time.deltaTime);
        _ctx.Rotation = Mathf.DeltaAngle(0f, newAngle);
        
        
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
