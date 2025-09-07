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

    //TODO: end of temporary

    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this, _movementProps);
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
