using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{

    #region header
  
    // Properties
    [SerializeField] private PlayerMovementProps _movementProps;

    [Header("Swipe Properties")]
    [SerializeField] private float _swipeForce = 5f;
    public float SwipeForce { get { return _swipeForce; } }
    [SerializeField] [Range(0f,2f)] private float _swipeMovementScaler = 0.1f;
    public float SwipeMovementScaler { get { return _swipeMovementScaler; } }
    [SerializeField] private float _swipeDuration = 0.5f;
    public float SwipeDuration { get { return _swipeDuration; } }
    [SerializeField] private float _swipeCooldown = 1f;
    public float SwipeCooldown { get { return _swipeCooldown; } }

    // Fields
    //weight
    private float _weight = 0f;
    public float Weight
    {
        get { return _weight; }
        set { SetWeight(value); }
    }

    //context & state
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    //TODO: Temporary. Decouple Trash ball!
    public float rotation
    {
        get { return _ctx.Rotation; }
        set { _ctx.Rotation = value; }
    }
    public Vector2 currentVelocity { get; set; } = Vector2.zero; // This doesnt work. Just keeps things happy. Need to decouple.
    //TODO: end of temporary

    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this, _movementProps);
        _ctx.Rigidbody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponent<Animator>();
        _ctx.SwipeHandler = GetComponentInChildren<SwipeHandler>();
        _state = new PlayerStateMachine(_ctx);
    }

    private void Start()
    {
        SetWeight(_weight);
    }

    private void Update()
    {
        _state.Update();
        UpdateCooldowns();
    }
    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateCooldowns()
    {
        if (_ctx.SwipeCooldownTimer > 0f)
        {
            _ctx.SwipeCooldownTimer = Mathf.Max(_ctx.SwipeCooldownTimer - Time.deltaTime, 0f); 
        }
    }

    private void OnDrawGizmos()
    {
        if (_state == null) return;
        _state.OnDrawGizmos();
    }


    //inputs
    private void OnMove(InputValue value)
    {
        _ctx.MovementInput = value.Get<Vector2>();
    }

    private void OnMouseMove(InputValue value)
    {
        _ctx.MouseInput = value.Get<Vector2>();
    }

    private void OnSweep(InputValue value)
    {
        _ctx.IsSweepPressed = value.isPressed;
        if (!_ctx.IsSweepPressed) return;

    }

    private void OnSwipe(InputValue value)
    {
        if (_ctx.CanSwipe && _ctx.SwipeCooldownTimer <= 0f)
        {
            _state.ChangeState(PlayerStateEnum.Swiping);
        }
    }



    //collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            SceneManager.LoadScene("RebuildScene");
        }
    }


    // master movement handler using variables modified by state.
    private void UpdateMovement()
    {
        Vector2 velocityDelta = _ctx.FrameVelocity - _ctx.Rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta * _movementProps.ForceMultiplier, _movementProps.MaxMovementForce);
        _ctx.Rigidbody.AddForce(clampedForce, ForceMode2D.Force);


        _ctx.Animator.SetFloat("Speed", _ctx.MoveSpeed);
        _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);

    }


    //weight system
    public void SetWeight(float weight)
    {
        PlayerMovementProps p = _movementProps; //For shorthand access

        _ctx.MaxWalkSpeed = p.BaseMaxWalkSpeed / (1 + weight * p.MaxWalkSpeedReduction);
        _ctx.MaxSweepSpeed = _ctx.MaxWalkSpeed * p.SweepMaxSpeedModifier;
        _ctx.MaxChargeSpeed = _ctx.MaxWalkSpeed * p.ChargeMaxSpeedModifier;

        _ctx.Acceleration = p.BaseAcceleration / (1 + weight * p.AccelerationReduction);
        _ctx.SweepAcceleration = _ctx.Acceleration * p.SweepAccelerationModifier;
        _ctx.ChargeAcceleration = _ctx.Acceleration * p.ChargeAccelationModifier;

        _ctx.RotationSpeed = p.BaseRotationSpeed / (1 + weight * p.RotationSpeedReduction);
        _ctx.SweepRotationSpeed = _ctx.RotationSpeed * p.SweepRotationModifier;
        _ctx.ChargeRotationSpeed = _ctx.RotationSpeed * p.ChargeRotationModifier;

        _weight = weight;
    }
}
