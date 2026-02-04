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
        _ctx.CanSwipe = true;
        _ctx.CanDash = true;
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
        if (_ctx.IsSweepPressed) _state.ChangeState(PlayerStateEnum.Sweeping);
    }

}
