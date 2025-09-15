using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargingState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    public PlayerChargingState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Sweeping", true);
        _ctx.CanSwipe = false;

        // Change rotation to velocity direction
        SetRotationToVelocityDirection();
    }

    public override void Update()
    {
        HandleMovementAndRotation();
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

    private void SetRotationToVelocityDirection()
    {
        // Only update rotation if moving
        if (_ctx.Rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 velocityDir = _ctx.Rigidbody.velocity.normalized;

            float angle = Mathf.Atan2(velocityDir.y, velocityDir.x) * Mathf.Rad2Deg;

            _ctx.Rotation = Mathf.DeltaAngle(0f, angle);
            _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);
        }
        else
        {
            Debug.LogWarning("Velocity is small even though charging... Problem?");
        }
    }

    private void HandleMovementAndRotation()
    {
        Vector2 input = _ctx.MovementInput.normalized;

        //make relative axes and map inputs into them
        Vector2 forwardAxis = new Vector2(Mathf.Cos(_ctx.Rotation * Mathf.Deg2Rad), Mathf.Sin(_ctx.Rotation * Mathf.Deg2Rad));
        Vector2 sidewaysAxis = new Vector2(-forwardAxis.y, forwardAxis.x); //perpendicular

        float forwardInput = Vector2.Dot(input, forwardAxis);
        float sidewaysInput = Vector2.Dot(input, sidewaysAxis);

        //Acceleration
        if (forwardInput > 0.1f)
        {
            _ctx.MoveSpeed = Mathf.MoveTowards(_ctx.MoveSpeed, _ctx.MaxChargeSpeed, _ctx.ChargeAcceleration * Time.deltaTime);
        }

        //Deceleration
        if (forwardInput < -0.1f)
        {
            _ctx.MoveSpeed = Mathf.MoveTowards(_ctx.MoveSpeed, 0, _ctx.Props.ChargeDeceleration * Time.deltaTime);
        }

        //Rotation
        if (Mathf.Abs(sidewaysInput) > 0.1f)
        {
            float turnSpeed = _ctx.ChargeRotationSpeed / Mathf.Max(_ctx.MoveSpeed / _ctx.MaxWalkSpeed, 1);
            float angle =  _ctx.Rotation + sidewaysInput * turnSpeed * Time.deltaTime;
            _ctx.Rotation = Mathf.DeltaAngle(0f, angle);
        }

        _ctx.FrameVelocity = forwardAxis * _ctx.MoveSpeed;

    }

    //state
    private void TryChangeState()
    {
        float velocityThreshold = _ctx.Props.ExitChargeSpeedThreshold;
        if (_ctx.MoveSpeed < velocityThreshold) _state.ChangeState(PlayerStateEnum.Sweeping);
    }
    

    private void DrawRotationGizmo()
    {
        //gizmo settings
        float distance = 0.6f;
        float circleRadius = 0.2f;

        //draw circle at rotation
        Vector3 offset = new Vector3(Mathf.Cos(_ctx.Rotation * Mathf.Deg2Rad), Mathf.Sin(_ctx.Rotation * Mathf.Deg2Rad), 0f) * distance;
        Vector3 circlePos = _ctx.Player.transform.position + offset;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(circlePos, circleRadius);
    }

}
