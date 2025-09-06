using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{

    #region header

    // Properties
    [Header("Base Movement")]
    [SerializeField] private float _baseMaxWalkSpeed;
    [SerializeField] private float _baseAcceleration;
    [SerializeField] private float _baseRotationSpeed;
    [Header("Sweeping Movement")]
    [SerializeField] private float _sweepMaxSpeedModifier;
    [SerializeField] private float _sweepAccelerationModifier;
    [SerializeField] private float _sweepRotationModifier;
    [SerializeField] private float _sweepingMaxAngleBeforeTurnSlowdown;
    [SerializeField] private float _sweepingSlowdownTurnSpeed;

    [Header("Weighted Movement")]
    [SerializeField] private float _maxWalkSpeedReduction;
    [SerializeField] private float _accelerationReduction;
    [SerializeField] private float _rotationSpeedReduction;

    // Fields
    //weight
    private float _weight = 0f;
    public float Weight
    {
        get { return _weight; }
        set { SetWeight(value); }
    }

    //context & state
    PlayerContext _ctx;
    PlayerStateMachine _state;

    //TODO: Temporary. Decouple Trash ball!
    TrashBallController _trashBallController;
    public float rotation {
        get { return _ctx.Rotation; }
        set { _ctx.Rotation = value; } }
    public Vector2 currentVelocity { get; set; } = Vector2.zero; // This doesnt work. Just keeps things happy. Need to decouple.

    //TODO: end of temporary

    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this);
        _ctx.RigidBody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponent<Animator>();
        _state = new PlayerStateMachine(_ctx);
        _trashBallController = GetComponentInChildren<TrashBallController>();
    }

    private void Start()
    {
        SetWeight(_weight);
    }

    private void Update()
    {
        _state.Update();
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
        //Debug.Log("Swipe");
    }


    //collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrashBall"))
        {
            //Debug.Log("Reattached to ball");
            _trashBallController.isAttached = true;
            _trashBallController.trashBall.transform.parent = transform;
            _trashBallController.trashRb.bodyType = RigidbodyType2D.Kinematic;
            _trashBallController.SyncBallScale();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            SceneManager.LoadScene("RebuildScene");
        }
    }


    //weight system
    public void SetWeight(float weight)
    {
        _ctx.MaxWalkSpeed = _baseMaxWalkSpeed / (1 + weight * _maxWalkSpeedReduction);
        _ctx.MaxSweepSpeed = _ctx.MaxWalkSpeed * _sweepMaxSpeedModifier;
        _ctx.Acceleration = _baseAcceleration / (1 + weight * _accelerationReduction);
        _ctx.SweepAcceleration = _ctx.Acceleration * _sweepAccelerationModifier;
        _ctx.RotationSpeed = _baseRotationSpeed / (1 + weight * _rotationSpeedReduction);
        _ctx.SweepRotationSpeed = _ctx.RotationSpeed * _sweepRotationModifier;
        _weight = weight;

        //these arent really weight dependent
        _ctx.SweepSlowdownAngle = _sweepingMaxAngleBeforeTurnSlowdown;
        _ctx.SweepSlowdownSpeed = _sweepingSlowdownTurnSpeed;
    }
}
