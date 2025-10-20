using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerMovementController : MonoBehaviour, ISwipeable, IAbsorbable, IHeatable
{

    #region header

    // Properties
    [SerializeField] private PlayerMovementProps _movementProps;

    [Header("Start Properties")]
    [SerializeField][Range(-180, 180)] private float _startAngle = 270f;

    [Header("Sweep Properties")]
    [SerializeField] private float _sweepForce = 5f;
    public float SweepForce { get { return _sweepForce; } }
    [SerializeField][Range(0f, 2f)] private float _sweepForceMovementScaler = 0.1f;
    public float SweepForceMovementScaler { get { return _sweepForceMovementScaler; } }


    [Header("Swipe Properties")]
    [SerializeField] private float _baseSwipeForce = 5f;
    public float BaseSwipeForce { get { return _baseSwipeForce; } }
    [SerializeField] private float _fullChargeSwipeForce = 10f;
    public float FullChargeSwipeForce { get { return _fullChargeSwipeForce; } }
    [SerializeField][Range(0f, 2f)] private float _swipeForceMovementScaler = 0.1f;
    public float SwipeForceMovementScaler { get { return _swipeForceMovementScaler; } }
    [SerializeField] private float _swipeTimeUntilHold = 1f;
    public float SwipeTimeUntilHold { get { return _swipeTimeUntilHold; } }
    [SerializeField] private float _swipeHoldChargeTime = 5f;
    public float SwipeHoldChargeTime { get { return _swipeHoldChargeTime; } }
    [SerializeField] private float _swipeDuration = 0.5f;
    public float SwipeDuration { get { return _swipeDuration; } }
    [SerializeField] private float _swipeCooldown = 1f;
    public float SwipeCooldown { get { return _swipeCooldown; } }

    [Header("Absorbed Properties")]
    [SerializeField] private float _absorbResistance;
    [SerializeField] private float _minTrashSizeToAbsorb;
    [SerializeField] private int _playerEscapeDamage;

    [Header("Swipe Visual Line")]
    [SerializeField] private float _swipeVisualLineDistance = 10f;
    public float SwipeVisualLineDistance { get { return _swipeVisualLineDistance; } }
    [SerializeField] private int _swipeVisualLineSegments = 20;
    public int SwipeVisualLineSegments { get { return _swipeVisualLineSegments; } }

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
        _ctx.Collider = GetComponent<Collider2D>();
        _ctx.Rotation = Mathf.DeltaAngle(0f, _startAngle);
        _state = new PlayerStateMachine(_ctx);
    }

    private void Start()
    {
        SetWeight(_weight);
        Cursor.lockState = CursorLockMode.Confined;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Music/Hellish Sample", transform.position);
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
                ParticleManager.Instance.Play("dashBack", transform.position,Quaternion.identity,transform);
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
        //DrawDashCooldownGizmo();
    }


    //collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            SceneManager.LoadScene("RebuildScene");
        }
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
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Dash", transform.position);
            _state.ChangeState(PlayerStateEnum.Dash);
        }
    }

    private void OnMouseMoveInput(InputValue value)
    {
        _ctx.MouseInput = value.Get<Vector2>();
    }

    private void OnMouseDeltaInput(InputValue value)
    {
        Vector2 mouseDelta = value.Get<Vector2>();
        if (mouseDelta.magnitude > 0.5f)
        {
            _targetStickPos += mouseDelta;
            _targetStickPos = _targetStickPos.normalized;
        }
        //_ctx.StickInput = Vector2.Lerp(_ctx.StickInput, _targetStickPos, 1f - Mathf.Exp(-_mouseStickSensitivity * Time.deltaTime)).normalized;
    }

    private void OnSwipeInput(InputValue value)
    {
        _ctx.IsSwipePressed = value.isPressed;
        if (_ctx.CanSwipe && _ctx.SwipeCooldownTimer <= 0f && value.isPressed)
        {
            _state.ChangeState(PlayerStateEnum.Swiping);
        }
    }


    private void OnEscapeTrashBallInput(InputValue value)
    {
        if (_ctx.AbsorbedTrashBall != null)
        {
            _ctx.Animator.speed += 0.3f;
            _ctx.AbsorbedTrashBall.TakeDamage(_playerEscapeDamage);
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
            _footstepCooldown = 0.13f;

            if (_ctx.MoveSpeed > _ctx.MaxWalkSpeed)
            {
                _footstepCooldown = 0.125f;
            }
        }
    }

    //weight system
    public void SetWeight(float weight)
    {
        PlayerMovementProps p = _movementProps; //For shorthand access

        _ctx.MaxWalkSpeed = p.BaseMaxWalkSpeed / (1 + weight * p.MaxWalkSpeedReduction);
        _ctx.MaxSweepWalkSpeed = _ctx.MaxWalkSpeed * p.SweepMaxSpeedModifier;
        _ctx.MaxSwipeWalkSpeed = _ctx.MaxWalkSpeed * p.SwipeMaxSpeedModifier;

        _ctx.Acceleration = p.BaseAcceleration / (1 + weight * p.AccelerationReduction);
        _ctx.SweepAcceleration = _ctx.Acceleration * p.SweepAccelerationModifier;
        _ctx.SwipeAcceleration = _ctx.Acceleration * p.SwipeAccelerationModifier;

        _weight = weight;
    }


    // Being swiped puts you into tumble state
    public void OnSwipe(Vector2 direction, float force)
    {
        _state.ChangeState(PlayerStateEnum.Tumble);
        _ctx.Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    // IAbsorbable

    public void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        if (forcedAbsorb || (absorbingPower > _absorbResistance && trashBall.Size > _minTrashSizeToAbsorb))
        {
            trashBall.absorbedObjects.Add(this);
            _ctx.AbsorbedTrashBall = trashBall;
            _state.ChangeState(PlayerStateEnum.Absorbed);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        _ctx.AbsorbedTrashBall = null;
        _state.ChangeState(PlayerStateEnum.Idle);
    }

    // IHeatable
    public void OnIgnite(HeatMechanic heat)
    {
        transform.position = new Vector3(-6.5f, 2f, transform.position.z); //Temporary. Need a death condition
        heat.Reset();
        LayerMask layerMask = new LayerMask();
        _ctx.Collider.excludeLayers = layerMask;
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

    public void OnTrashBallIgnite()
    {
        // Also temporary need a death function
        transform.position = new Vector3(-6.5f, 2f, transform.position.z);
    }
}
