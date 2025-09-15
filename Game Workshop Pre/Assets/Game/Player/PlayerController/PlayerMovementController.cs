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


    [SerializeField] private bool _doStuff;
    private Vector2 collisionVelocity;
    public Vector2 Input { get { return _ctx.MovementInput; }}
    //TODO: end of temporary

    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this, _movementProps);
        _ctx.Rigidbody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponent<Animator>();
        _state = new PlayerStateMachine(_ctx);
        _trashBallController = GetComponentInChildren<TrashBallController>();
    }

    private void Start()
    {
        SetWeight(_weight);
    }

    private void FixedUpdate()
    {
        if (_doStuff)
        {
            UpdateMovement();
        }
        collisionVelocity = Vector2.zero;
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
        _ctx.Rigidbody.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
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

    [SerializeField] private float _maxForce = 10f;
    [SerializeField] private float _forceMultiplier = 10f;
    // master movement handler using variables modified by state.
    private void UpdateMovement()
    {

        _state.Update();
        Vector2 velocityDelta = _ctx.FrameVelocity - _ctx.Rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta * _forceMultiplier, _maxForce);
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
