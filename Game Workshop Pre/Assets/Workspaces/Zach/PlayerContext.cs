using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The main point of this is to keep PlayerMovementController protected while allowing its states to have free access to its information
// This holds all the information that ONLY the player controller and its states should have knowledge about.
// You don't need to put something in here if only a single state knows about it.
public class PlayerContext
{
    // Player reference
    public ZPlayerMovementController Player { get; }

    // Fields
    //input
    public Vector2 MovementInput { get; set; } = Vector2.zero;
    public Vector2 MouseInput { get; set; } = Vector2.zero;
    public bool IsSweepPressed { get; set; }

    //weight adjusted stats
    public float MaxWalkSpeed { get; set; } = 0f;
    public float Acceleration { get; set; } = 0f;
    public float RotationSpeed { get; set; } = 0f;

    // Components
    public Rigidbody2D RigidBody { get; set; }
    public Animator Animator { get; set; }

    public PlayerContext(ZPlayerMovementController player)
    {
        Player = player;
    }


}
