using System;
using UnityEngine;


[Serializable]
public class PlayerMovementProps
{
    // Properties
    [Header("Physics Influence")]
    [SerializeField] private float _maxMovementForce = 20f;
    public float MaxMovementForce { get { return _maxMovementForce; } }
    [SerializeField] private float _forceMultiplier = 10f;
    public float ForceMultiplier { get { return _forceMultiplier; } }

    [Header("Base Movement")]
    [SerializeField] private float _baseMaxWalkSpeed;
    public float BaseMaxWalkSpeed { get { return _baseMaxWalkSpeed; } }
    [SerializeField] private float _baseAcceleration;
    public float BaseAcceleration { get { return _baseAcceleration; } }
    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed { get { return _rotationSpeed; } }



    [Header("Sweeping Movement")]
    [SerializeField] private float _sweepMaxSpeedModifier;
    public float SweepMaxSpeedModifier { get { return _sweepMaxSpeedModifier; } }
    [SerializeField] private float _sweepAccelerationModifier;
    public float SweepAccelerationModifier { get { return _sweepAccelerationModifier; } }


    [Header("Tumble")]
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
