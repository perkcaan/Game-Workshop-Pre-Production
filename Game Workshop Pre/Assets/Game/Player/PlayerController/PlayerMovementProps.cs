using System;
using UnityEngine;


[Serializable]
public class PlayerMovementProps
{
    // Properties
    [Header("Base Movement")]
    [SerializeField] private float _baseMaxWalkSpeed;
    public float BaseMaxWalkSpeed { get { return _baseMaxWalkSpeed; } }
    [SerializeField] private float _baseAcceleration;
    public float BaseAcceleration { get { return _baseAcceleration; } }
    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed { get { return _rotationSpeed; } }
    [SerializeField] private bool _willCancelSlide;
    public bool WillCancelSlide { get { return _willCancelSlide; } }

    [Header("Sweeping Movement")]
    [SerializeField] private float _sweepMaxSpeedModifier;
    public float SweepMaxSpeedModifier { get { return _sweepMaxSpeedModifier; } }
    [SerializeField] private float _sweepAccelerationModifier;
    public float SweepAccelerationModifier { get { return _sweepAccelerationModifier; } }

    [SerializeField] private bool _willCancelSweepSlide;
    public bool WillCancelSweepSlide { get { return _willCancelSweepSlide; } }


    [Header("Swipe Movement")]
    [SerializeField] private float _swipeMaxSpeedModifier;
    public float SwipeMaxSpeedModifier { get { return _swipeMaxSpeedModifier; } }
    [SerializeField] private float _swipeAccelerationModifier;
    public float SwipeAccelerationModifier { get { return _swipeAccelerationModifier; } }

    [SerializeField] private bool _willCancelSwipeSlide;
    public bool WillCancelSwipeSlide { get { return _willCancelSwipeSlide; } }


    [Header("Dashing")]
    [SerializeField] private int _dashRowCount;
    public int DashRowCount { get { return _dashRowCount; } }
    [SerializeField] private float _dashForce;
    public float DashForce { get { return _dashForce; } }
    [SerializeField] private float _dashDuration;
    public float DashDuration { get { return _dashDuration; } }
    [SerializeField] private float _dashRowCooldown;
    public float DashRowCooldown { get { return _dashRowCooldown; } }
    [SerializeField] private float _dashCooldown;
    public float DashCooldown { get { return _dashCooldown; } }


    [Header("Tumble")]
    [SerializeField] private float _enterTumbleThreshold;
    public float EnterTumbleThreshold { get { return _enterTumbleThreshold; } }
    [SerializeField] private PhysicsMaterial2D _tumbleMaterial;
    public PhysicsMaterial2D TumbleMaterial { get { return _tumbleMaterial; } }
    

    [Header("Weighted Movement")]
    [SerializeField] private float _maxWalkSpeedReduction;
    public float MaxWalkSpeedReduction { get { return _maxWalkSpeedReduction; } }
    [SerializeField] private float _accelerationReduction;
    public float AccelerationReduction { get { return _accelerationReduction; } }
    [SerializeField] private float _rotationSpeedReduction;
    public float RotationSpeedReduction { get { return _rotationSpeedReduction; }}
}
