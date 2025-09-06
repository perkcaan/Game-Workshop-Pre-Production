using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSweepingState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private Vector2 _sweepVelocity = Vector2.zero;
    private float _zeroMoveTimer = 0f;
    private float? _lastMovingAngle = null;

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
        _sweepVelocity = _ctx.MoveVelocity * _ctx.MovementInput.normalized;
        _lastMovingAngle = null;
    }

    public override void Update()
    {
        HandleMovement();
        HandleRotation();
        TryChangeState();
    }

    public override void ExitState()
    {
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

        //Option 2
        if (input.sqrMagnitude > 0.01f)
        {
            _zeroMoveTimer = 0f;
            _ctx.MoveVelocity = Mathf.MoveTowards(_ctx.MoveVelocity, _ctx.MaxSweepSpeed, _ctx.SweepAcceleration * Time.deltaTime);
            //angle based threshold slowdown
            //TODO: idea not sure about... Maybe something better
            Vector2 rotationVector = new Vector2(Mathf.Cos(_ctx.Rotation * Mathf.Deg2Rad), Mathf.Sin(_ctx.Rotation * Mathf.Deg2Rad));
            float angle = Vector2.SignedAngle(rotationVector, input.normalized);

            if (_lastMovingAngle.HasValue)
            {
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(_lastMovingAngle.Value, angle));
                if (angleDiff > _ctx.SweepSlowdownAngle)
                {
                    _ctx.MoveVelocity = Mathf.Min(_ctx.MoveVelocity, _ctx.SweepSlowdownSpeed);
                }

            }
            _lastMovingAngle = angle;
            //end idea not sure about
        }
        else
        {
            if (_zeroMoveTimer < 0.05)
            {
                _zeroMoveTimer += Time.deltaTime;
            }
            if (_zeroMoveTimer >= 0.05)
            {
                _ctx.MoveVelocity = 0f;
                _lastMovingAngle = null;
            }
        }


        _ctx.Animator.SetFloat("Speed", _ctx.MoveVelocity);
        _ctx.RigidBody.velocity = _ctx.MoveVelocity * input.normalized;
        //
    }

    private void HandleRotation()
    {
        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
        direction.Normalize();
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Rotate slower based on speed
        float speedRotationReduction = Mathf.Max(_ctx.MoveVelocity / _ctx.MaxWalkSpeed, 1);  

        float newAngle = Mathf.LerpAngle(_ctx.Rotation, targetAngle, _ctx.SweepRotationSpeed / speedRotationReduction * Time.deltaTime);
        _ctx.Rotation = Mathf.DeltaAngle(0f, newAngle);
        _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);
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
