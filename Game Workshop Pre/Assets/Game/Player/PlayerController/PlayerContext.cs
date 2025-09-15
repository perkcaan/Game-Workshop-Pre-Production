using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The main point of this is to keep PlayerMovementController protected while allowing its states to have free access to its information
// This holds all the information that ONLY the player controller and its states should have knowledge about.
// You don't need to put something in here if only a single state knows about it.
public class PlayerContext
{
    // Player reference
    public PlayerMovementController Player { get; }

    // movement props reference
    public PlayerMovementProps Props { get; }

    // Fields
    //input
    public Vector2 MovementInput { get; set; } = Vector2.zero;
    public Vector2 MouseInput { get; set; } = Vector2.zero;
    public bool IsSweepPressed { get; set; } = false;

    //swiping
    public bool CanSwipe { get; set; } = true;
    public float SwipeCooldownTimer { get; set; } = 0f;

    //weight adjusted stats
    public float MaxWalkSpeed { get; set; } = 0f;
    public float MaxSweepSpeed { get; set; } = 0f;
    public float MaxChargeSpeed { get; set; } = 0f;
    public float Acceleration { get; set; } = 0f;
    public float SweepAcceleration { get; set; } = 0f;
    public float ChargeAcceleration { get; set; } = 0f;
    public float RotationSpeed { get; set; } = 0f;
    public float SweepRotationSpeed { get; set; } = 0f;
    public float ChargeRotationSpeed { get; set; } = 0f;


    // current values
    public float Rotation { get; set; } = 0f;
    public float MoveSpeed { get; set; } = 0f;
    public Vector2 FrameVelocity { get; set; } = Vector2.zero;

    // Components
    public Rigidbody2D Rigidbody { get; set; }
    public Animator Animator { get; set; }
    public SwipeHandler SwipeHandler { get;  set;}

    public PlayerContext(PlayerMovementController player, PlayerMovementProps props)
    {
        Player = player;
        Props = props;
    }


}
