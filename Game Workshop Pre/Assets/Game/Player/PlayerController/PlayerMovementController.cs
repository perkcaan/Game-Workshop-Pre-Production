using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using System;
using FMODUnity;
using UnityEditor.Experimental.GraphView;

public class PlayerMovementController : MonoBehaviour, ISwipeable, IAbsorbable, IHeatable, ITargetable
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

    [Header("Sweep Poke Properties")]
    [SerializeField] private float _sweepAllowPokeTime = 0.1f;
    public float SweepAllowPokeTime { get { return _sweepAllowPokeTime; } }
    [SerializeField] private float _pokeForce = 5f;
    public float PokeForce { get { return _pokeForce; } }

    [SerializeField] private float _pokeDuration = 0.5f;
    public float PokeDuration { get { return _pokeDuration; } }
    [SerializeField] private float _pokeCooldown = 1f;
    public float PokeCooldown { get { return _pokeCooldown; } }
    
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

    [Header("Hook Properties")]
    [SerializeField] private float _hookPullForce = 5f;
    public float HookPullForce { get { return _hookPullForce; } }
    [SerializeField][Range(0f, 2f)] private float _hookThrowMovementScaler = 0.1f;
    public float HookThrowMovementScaler { get { return _hookThrowMovementScaler; } }
    [SerializeField] private float _hookDuration = 0.5f;
    public float HookDuration { get { return _hookDuration; } }
    [SerializeField] private float _hookCooldown = 1f;
    public float HookCooldown { get { return _hookCooldown; } }

    [Header("Absorbed Properties")]
    [SerializeField] private float _minTrashSizeToAbsorb;
    [SerializeField] private int _playerEscapeDamage;
    public int Size { get { return 0; } }
    [SerializeField] private TrashMaterial _trashMaterial;
    public TrashMaterial TrashMat { get { return _trashMaterial; } }
    [SerializeField] private int _trashMaterialWeight;
    public int TrashMatWeight { get { return _trashMaterialWeight; } }
    [SerializeField] protected float _minVelocityToAbsorb;

    [Header("Swipe Visual Line")]
    [SerializeField] private float _swipeVisualLineDistance = 10f;
    public float SwipeVisualLineDistance { get { return _swipeVisualLineDistance; } }
    [SerializeField] private int _swipeVisualLineSegments = 20;
    public int SwipeVisualLineSegments { get { return _swipeVisualLineSegments; } }

    [Header("Audio")]
    private float _footstepCooldown = 0f; // this is used for particles
    private FMOD.Studio.EventInstance _heatSound;


    public static Action<bool> playerDeath;
    

    [Header("Item Effected Properties")]
    public bool canSweep = false;
    public bool canPoke = false;
    public bool canSwipe = false;
    public bool canDash = false;
    public bool canHook = false;

    // Fields
    //weight
    private float _weight = 0f;
    public float Weight
    {
        get { return _weight; }
        set { SetWeight(value); }
    }

    //input
    private bool _isUsingVirtualMouse = false;
    private Vector2 _virtualMouseVector = Vector2.zero;
    private float _virtualMouseDistance = 2f;
    //context & state
    private PlayerContext _ctx;
    private PlayerStateMachine _state;
    private HeatMechanic _playerHeat;

    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this, _movementProps);
        _ctx.Rigidbody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponentInChildren<Animator>();
        _ctx.SwipeHandler = GetComponentInChildren<SwipeHandler>();
        _ctx.SweepHandler = GetComponentInChildren<BroomSweepHandler>();
        _ctx.HookHandler = GetComponentInChildren<HookHandler>();
        _ctx.Collider = GetComponent<Collider2D>();
        _ctx.Rotation = Mathf.DeltaAngle(0f, _startAngle);
        _playerHeat = GetComponent<HeatMechanic>();
        _state = new PlayerStateMachine(_ctx);
        //_heatSound = FMODUnity.RuntimeManager.CreateInstance("event:/Heat System/Heat Meter");
        

    }

    private void Start()
    {
        SetWeight(_weight);
        Cursor.lockState = CursorLockMode.Confined;
        AudioManager.Instance.PlayInstance("Heat");
        //_heatSound.start();
        _playerHeat = GetComponent<HeatMechanic>();
        
    }

    private void Update()
    {
        _state.Update();
        UpdateCooldowns();
        UpdateVirtualMouse();
    }
    private void FixedUpdate()
    {
        UpdateMovement();
        //_heatSound.setParameterByName("Heat", _playerHeat.Heat / 10);
        AudioManager.Instance.ModifyParameter(FindObjectOfType<AudioManager>().gameObject,"Heat", "Heat", _playerHeat.Heat / 10);

        
    }

    private void UpdateCooldowns()
    {

        // Swipe timer
        if (_ctx.SwipeCooldownTimer > 0f)
        {
            _ctx.SwipeCooldownTimer = Mathf.Max(_ctx.SwipeCooldownTimer - Time.deltaTime, 0f);
        }

        if (_ctx.HookCooldownTimer > 0f)
        {
            _ctx.HookCooldownTimer = Mathf.Max(_ctx.HookCooldownTimer - Time.deltaTime, 0f);
        }

        if (_ctx.PokeCooldownTimer > 0f)
        {
            _ctx.PokeCooldownTimer = Mathf.Max(_ctx.PokeCooldownTimer - Time.deltaTime, 0f);
        }

        // Dash timer
        if (_ctx.DashCooldownTimer > 0f)
        {
            _ctx.DashCooldownTimer = Mathf.Max(_ctx.DashCooldownTimer - Time.deltaTime, 0f);
            if (_ctx.DashCooldownTimer == 0f)
            {
                _ctx.DashesRemaining = _movementProps.DashRowCount;
                ParticleManager.Instance.Play("StarWave", transform.position, parent:transform, force:0.5f);
                AudioManager.Instance.Play("dashBack", transform);
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
        if (!canSweep) return;
        _ctx.IsSweepPressed = value.isPressed;
        if (!_ctx.IsSweepPressed)
        {
            return;
        }
    }

    private void OnDashInput(InputValue value)
    {
        if (!canDash) return;
        if (!value.isPressed) return;
        if (_ctx.CanDash && _ctx.DashesRemaining > 0 && _ctx.DashRowCooldownTimer <= 0f)
        {
            AudioManager.Instance.Play("Dash", transform);
            _state.ChangeState(PlayerStateEnum.Dash);
        }
    }

    private void OnMouseMoveInput(InputValue value)
    {
        _isUsingVirtualMouse = false;
        _ctx.MouseInput = value.Get<Vector2>();
    }

    private void OnControllerAimInput(InputValue value)
    {
        _isUsingVirtualMouse = true;
        Vector2 normalVector = value.Get<Vector2>().normalized;
        if (normalVector.magnitude < 0.1f)
        {
            float radians = _ctx.Rotation * Mathf.Deg2Rad;
            _virtualMouseVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        } else
        {
            _virtualMouseVector = normalVector;
        }
        UpdateVirtualMouse();
    }

    private void OnSwipeInput(InputValue value)
    {
        if (!canSwipe) return;
        _ctx.IsSwipePressed = value.isPressed;
        if (_ctx.CanSwipe && _ctx.SwipeCooldownTimer <= 0f && value.isPressed)
        {
            _state.ChangeState(PlayerStateEnum.Swiping);
        }
    }

    private void OnHookInput(InputValue value)
    {
        if (!canHook) return;
        _ctx.IsHookPressed = value.isPressed;
        if (_ctx.CanHook && _ctx.HookCooldownTimer <= 0f && value.isPressed)
        {
            _state.ChangeState(PlayerStateEnum.HookThrow);
        }
    }

    private void OnEscapeTrashBallInput(InputValue value)
    {
        if (!value.isPressed) return;
        if (_ctx.AbsorbedTrashBall != null)
        {
            _ctx.Animator.speed += 0.3f;
            _ctx.AbsorbedTrashBall.TakeDamage(_playerEscapeDamage);
        }
    }

    //updates virtual mouse position if in use
    private void UpdateVirtualMouse()
    {
        if (!_isUsingVirtualMouse) return;
        Vector2 offset = _virtualMouseVector * _virtualMouseDistance; //distance
        Vector2 worldPos = (Vector2) transform.position + offset;
        _ctx.MouseInput = Camera.main.WorldToScreenPoint(worldPos);
    }


    // master movement handler using variables modified by state.
    private void UpdateMovement()
    {
        Vector2 velocityDelta = _ctx.FrameVelocity - _ctx.Rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta, _ctx.FrameVelocity.magnitude);
        _ctx.Rigidbody.AddForce(clampedForce, ForceMode2D.Force);


        _ctx.Animator.SetFloat("Speed", _ctx.MovementInput.magnitude);
        _ctx.Animator.SetFloat("Rotation", _ctx.Rotation);

        

        if (_ctx.MoveSpeed > 0.1f)
        {
            _footstepCooldown -= Time.deltaTime;
            if (_footstepCooldown <= 0f)
            {
                ParticleManager.Instance.Play("PlayerStepDust", transform.position, parent:_ctx.Player.transform);
                AudioManager.Instance.Play("Steps", transform);
                _footstepCooldown = 0.3f;
            }
        }
        else if (_ctx.MoveSpeed < 0.01f)
        {
            _footstepCooldown = 0.3f;
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
    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        if (force >= _movementProps.EnterTumbleThreshold) _state.ChangeState(PlayerStateEnum.Tumble);
        _ctx.Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    // IAbsorbable

    public bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (forcedAbsorb || (ballVelocity.magnitude > _minVelocityToAbsorb && trashBall.Size >= _minTrashSizeToAbsorb))
        {
            _ctx.AbsorbedTrashBall = trashBall;
            _state.ChangeState(PlayerStateEnum.Absorbed);
            return true;
        }
        return false;
    }

    public void OnTrashBallRelease(TrashBall trashBall, Vector2 unitVectorAngle)
    {
        _ctx.AbsorbedTrashBall = null;
        _state.ChangeState(PlayerStateEnum.Idle);
    }


    public void OnTrashBallDestroy()
    {
        Death();
    }

    // IHeatable
    public void PrepareIgnite(HeatMechanic heat)
    {
        //TODO: Make a death state where the player can't do anything and is locked to it until respawn
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        // transform.position = new Vector3(-6.5f, 2f, transform.position.z); //Temporary. Need a death condition

        Death();
        heat.Reset();
    }

    // ITargetable

    public TargetType GetTargetType()
    {
        return TargetType.Player;
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

    private void Death()
    {
        CheckpointManager.Instance.GoToCheckpoint(transform);
        AudioManager.Instance.Play("playerDeath", transform);
        AudioManager.Instance.Stop(gameObject,"Sweep");
        playerDeath?.Invoke(true);
        
        //Debug.Log("Return to Checkpoint");
    }

    public void ApplyWebSlow(float slowAmount)
    {
        SetWeight(_weight + slowAmount);
    }

    public void RemoveWebSlow(float slowAmount)
    {
        SetWeight(_weight - slowAmount);
    }
}
