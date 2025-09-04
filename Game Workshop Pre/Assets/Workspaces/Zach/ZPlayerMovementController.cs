using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ZPlayerMovementController : MonoBehaviour
{

    #region header

    // Properties
    [Header("Base Movement")]
    [SerializeField] private float _baseMaxWalkSpeed;
    [SerializeField] private float _baseAcceleration;
    [SerializeField] private float _baseRotationSpeed;
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


    #endregion

    // Methods
    private void Awake()
    {
        _ctx = new PlayerContext(this);
        _ctx.RigidBody = GetComponent<Rigidbody2D>();
        _ctx.Animator = GetComponent<Animator>();
        _state = new PlayerStateMachine(_ctx);
    }

    private void Start()
    {
        SetWeight(_weight);
    }

    private void Update()
    {
        _state.Update();
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

        //Debug.Log("Sweep");
    }

    private void OnSwipe(InputValue value)
    {
        //Debug.Log("Swipe");
    }


    //weight system
    public void SetWeight(float weight)
    {
        _ctx.MaxWalkSpeed = _baseMaxWalkSpeed / (1 + weight * _maxWalkSpeedReduction);
        _ctx.Acceleration = _baseAcceleration / (1 + weight * _accelerationReduction);
        _ctx.RotationSpeed = _baseRotationSpeed / (1 + weight * _rotationSpeedReduction);
        _ctx.Animator.SetBool("Sweeping", weight > 0);
        _weight = weight;
    }
}
