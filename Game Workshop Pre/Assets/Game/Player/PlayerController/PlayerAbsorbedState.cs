using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbsorbedState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // original material
    private PhysicsMaterial2D _originalMaterial;

    // Fields
    public PlayerAbsorbedState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Absorbed", true);
        _ctx.CanSwipe = false;
        _ctx.PlayerHasControl = false;
        _ctx.IsSweepPressed = false;
        _originalMaterial = _ctx.Rigidbody.sharedMaterial;
        _ctx.Rigidbody.sharedMaterial = _ctx.Props.TumbleMaterial;
        _ctx.CircleCollider.enabled = false;
    }

    public override void Update()
    {
        Transform trashBallTranform = _ctx.Player.absorbedTrashBall.transform;
        _ctx.Player.transform.position = new Vector3(trashBallTranform.position.x, trashBallTranform.position.y, _ctx.Player.transform.position.z);
    }

    public override void ExitState()
    {
        _ctx.SweepHandler.EndSweep();
        _ctx.Animator.SetBool("Absorbed", false);
        _ctx.Animator.speed = 1;
        _ctx.PlayerHasControl = true;
        _ctx.Rigidbody.sharedMaterial = _originalMaterial;
        _ctx.CircleCollider.enabled = true;
    }

}
