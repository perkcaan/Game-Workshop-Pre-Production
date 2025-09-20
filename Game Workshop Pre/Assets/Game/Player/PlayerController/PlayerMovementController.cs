using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour, ISwipeable
{

    #region header
  
    // Properties
    [SerializeField] private PlayerMovementProps _movementProps;


    [Header("Mouse as Stick Properties")]
    [SerializeField] private float _mouseStickSensitivity = 10f;

    [Header("Sweep Properties")]
    [SerializeField] private float _sweepForce = 5f;
    public float SweepForce { get { return _sweepForce; } }
    [SerializeField] [Range(0f,2f)] private float _sweepMovementScaler = 0.1f;
    public float SweepMovementScaler { get { return _sweepMovementScaler; } }


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

    private Vector2 _targetStickPos = Vector2.right.normalized;

    //context & state
    private PlayerContext _ctx;
    private PlayerStateMachine _state;


    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this, _movementProps);
        _ctx.Rigidbody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponent<Animator>();
        _ctx.SwipeHandler = GetComponentInChildren<SwipeHandler>();
        _ctx.SweepHandler = GetComponentInChildren<BroomSweepHandler>();
        _state = new PlayerStateMachine(_ctx);
    }

    private void Start()
    {
        SetWeight(_weight);
        Cursor.lockState = CursorLockMode.Locked;
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
    private void OnMoveInput(InputValue value)
    {
        _ctx.MovementInput = value.Get<Vector2>();
    }

    private void OnSweepInput(InputValue value)
    {
        _ctx.IsSweepPressed = value.isPressed;
        if (!_ctx.IsSweepPressed)
        {
            _targetStickPos = Vector2.zero;
            return;
        }

    }

    private void OnMouseDeltaInput(InputValue value)
    {
        Vector2 mouseDelta = value.Get<Vector2>();
        if (mouseDelta != Vector2.zero)
        {
            _targetStickPos += mouseDelta;
            _targetStickPos = _targetStickPos.normalized;
        }
        _ctx.StickInput = Vector2.Lerp(_ctx.StickInput, _targetStickPos, 1f - Mathf.Exp(-_mouseStickSensitivity * Time.deltaTime)).normalized;
    }

    private void OnSwipeInput(InputValue value)
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
        PlayerMovementProps p = _movementProps;
        if (!_ctx.PlayerHasControl)
        {
            _ctx.Animator.SetFloat("Speed", _ctx.Rigidbody.velocity.magnitude);
            _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);
            return;
        }
        Vector2 velocityDelta = _ctx.FrameVelocity - _ctx.Rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta * _movementProps.ForceMultiplier, _movementProps.MaxMovementForce);
        _ctx.Rigidbody.AddForce(clampedForce, ForceMode2D.Force);


        _ctx.Animator.SetFloat("Speed", _ctx.MoveSpeed);
        _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);


        p._footstepCooldown -= Time.deltaTime;

        if (_ctx.MoveSpeed > 0.1f && p._footstepCooldown <= 0f)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Clean Step",transform.position);
            p._footstepCooldown = 0.3f;

            if(_ctx.MoveSpeed > _ctx.MaxWalkSpeed)
            {
                p._footstepCooldown = 0.15f;
            }
        }
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


    // Swipe puts you into tumble state
    public void OnSwipe(Vector2 direction, float force)
    {
        _state.ChangeState(PlayerStateEnum.Tumble);
        _ctx.Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

}
