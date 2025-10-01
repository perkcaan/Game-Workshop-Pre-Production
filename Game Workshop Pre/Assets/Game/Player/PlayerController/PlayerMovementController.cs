using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerMovementController : MonoBehaviour, ISwipeable
{

    #region header

    // Properties
    [SerializeField] private PlayerMovementProps _movementProps;
    [SerializeField] ParticleSystem _dashfx;
    ParticleSystem _dashfxInstance;

    [Header("Start Properties")]
    [SerializeField][Range(-180, 180)] private float _startAngle = 270f;

    [Header("Mouse as Stick Properties")]
    [SerializeField] private float _mouseStickSensitivity = 10f;

    [Header("Sweep Properties")]
    [SerializeField] private float _sweepForce = 5f;
    public float SweepForce { get { return _sweepForce; } }
    [SerializeField][Range(0f, 2f)] private float _sweepMovementScaler = 0.1f;
    public float SweepMovementScaler { get { return _sweepMovementScaler; } }


    [Header("Swipe Properties")]
    [SerializeField] private float _swipeForce = 5f;
    public float SwipeForce { get { return _swipeForce; } }
    [SerializeField][Range(0f, 2f)] private float _swipeMovementScaler = 0.1f;
    public float SwipeMovementScaler { get { return _swipeMovementScaler; } }
    [SerializeField] private float _swipeDuration = 0.5f;
    public float SwipeDuration { get { return _swipeDuration; } }
    [SerializeField] private float _swipeCooldown = 1f;
    public float SwipeCooldown { get { return _swipeCooldown; } }

    [Header("Audio")]
    [SerializeField] private float _footstepCooldown = 0f;


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
        _ctx.Rotation = Mathf.DeltaAngle(0f, _startAngle);
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
        Transform transform = this.transform;
        //_dashfxInstance = Instantiate(_dashfx, transform.position, Quaternion.Euler(0, 0, 0));
        _dashfx.gameObject.transform.SetParent(this.transform);
    }
    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateCooldowns()
    {

        // Swipe timer
        if (_ctx.SwipeCooldownTimer > 0f)
        {
            _ctx.SwipeCooldownTimer = Mathf.Max(_ctx.SwipeCooldownTimer - Time.deltaTime, 0f);
        }

        // Dash timer
        if (_ctx.DashCooldownTimer > 0f)
        {
            
            _ctx.DashCooldownTimer = Mathf.Max(_ctx.DashCooldownTimer - Time.deltaTime, 0f);
            if (_ctx.DashCooldownTimer == 0f)
            {
                _ctx.DashesRemaining = _movementProps.DashRowCount;
                SpawnDashFX();
            }
        }

        // Dash in a row timer
        if (_ctx.DashRowCooldownTimer > 0f)
        {
            _ctx.DashRowCooldownTimer = Mathf.Max(_ctx.DashRowCooldownTimer - Time.deltaTime, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (_state == null) return;
        _state.OnDrawGizmos();
        DrawDashCooldownGizmo();
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

    private void OnDashInput(InputValue value)
    {
        if (!value.isPressed) return;
        if (_ctx.CanDash && _ctx.DashesRemaining > 0 && _ctx.DashRowCooldownTimer <= 0f)
        {
            _state.ChangeState(PlayerStateEnum.Dash);
        }
    }

    private void OnMouseDeltaInput(InputValue value)
    {
        Vector2 mouseDelta = value.Get<Vector2>();
        if (mouseDelta.magnitude > 0.5f)
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
        Vector2 velocityDelta = _ctx.FrameVelocity - _ctx.Rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta, _ctx.FrameVelocity.magnitude);
        _ctx.Rigidbody.AddForce(clampedForce, ForceMode2D.Force);


        _ctx.Animator.SetFloat("Speed", _ctx.FrameVelocity.magnitude);
        _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);


        _footstepCooldown -= Time.deltaTime;

        if (_ctx.MoveSpeed > 0.1f && _footstepCooldown <= 0f)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Clean Step", transform.position);
            _footstepCooldown = 0.3f;

            if (_ctx.MoveSpeed > _ctx.MaxWalkSpeed)
            {
                _footstepCooldown = 0.15f;
            }
        }
    }

    //weight system
    public void SetWeight(float weight)
    {
        PlayerMovementProps p = _movementProps; //For shorthand access

        _ctx.MaxWalkSpeed = p.BaseMaxWalkSpeed / (1 + weight * p.MaxWalkSpeedReduction);
        _ctx.MaxSweepSpeed = _ctx.MaxWalkSpeed * p.SweepMaxSpeedModifier;

        _ctx.Acceleration = p.BaseAcceleration / (1 + weight * p.AccelerationReduction);
        _ctx.SweepAcceleration = _ctx.Acceleration * p.SweepAccelerationModifier;

        _weight = weight;
    }


    // Being swiped puts you into tumble state
    public void OnSwipe(Vector2 direction, float force)
    {
        _state.ChangeState(PlayerStateEnum.Tumble);
        _ctx.Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void SpawnDashFX()
    {
        
        _dashfxInstance = Instantiate(_dashfx, transform.position, Quaternion.Euler(0, 0, 0));
        //_dashfxInstance.transform.position = transform.position;
        _dashfxInstance.Play();
        _dashfxInstance.gameObject.transform.SetParent(this.transform);
    }




    // Gizmo to display the dash cooldowns
    private void DrawDashCooldownGizmo()
    {
        if (_ctx.DashesRemaining > 0)
        {
            if (_ctx.DashRowCooldownTimer > 0f)
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawSphere(_ctx.Player.transform.position + new Vector3(0, 1, 0), _ctx.DashesRemaining / 10f);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_ctx.Player.transform.position + new Vector3(0, 1, 0), 0.1f);
        }
    }
    
}
