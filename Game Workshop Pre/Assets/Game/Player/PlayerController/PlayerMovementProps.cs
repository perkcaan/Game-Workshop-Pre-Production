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
    [SerializeField] private float _baseRotationSpeed;
    public float BaseRotationSpeed { get { return _baseRotationSpeed; } }



    [Header("Sweeping Movement")]
    [SerializeField] private float _sweepMaxSpeedModifier;
    public float SweepMaxSpeedModifier { get { return _sweepMaxSpeedModifier; } }
    [SerializeField] private float _sweepAccelerationModifier;
    public float SweepAccelerationModifier { get { return _sweepAccelerationModifier; } }
    [SerializeField] private float _sweepRotationModifier;
    public float SweepRotationModifier { get { return _sweepRotationModifier; } }
    [SerializeField] private float _sweepSlowdownAngle;
    public float SweepSlowdownAngle { get { return _sweepSlowdownAngle; } }
    [SerializeField] private float _sweepSlowdownSpeed;
    public float SweepSlowdownSpeed { get { return _sweepSlowdownSpeed; } }



    [Header("Charging Movement")]
    [SerializeField] private float _chargeAngleThreshold;
    public float ChargeAngleThreshold { get { return _chargeAngleThreshold; } }
    [SerializeField] private float _chargeSpeedThreshold;
    public float ChargeSpeedThreshold { get { return _chargeSpeedThreshold; } }
    [SerializeField] private float _chargeMaxSpeedModifier;
    public float ChargeMaxSpeedModifier { get { return _chargeMaxSpeedModifier; } }
    [SerializeField] private float _chargeAccelationModifier;
    public float ChargeAccelationModifier { get { return _chargeAccelationModifier; } }

    [SerializeField] private float _chargeRotationModifier;
    public float ChargeRotationModifier { get { return _chargeRotationModifier; } }
    [SerializeField] private float _chargeDeceleration;
    public float ChargeDeceleration { get { return _chargeDeceleration; } }



    [Header("Weighted Movement")]
    [SerializeField] private float _maxWalkSpeedReduction;
    public float MaxWalkSpeedReduction { get { return _maxWalkSpeedReduction; } }
    [SerializeField] private float _accelerationReduction;
    public float AccelerationReduction { get { return _accelerationReduction; } }
    [SerializeField] private float _rotationSpeedReduction;
    public float RotationSpeedReduction { get { return _rotationSpeedReduction; }}
}
