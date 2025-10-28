using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTumbleState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // original material
    private PhysicsMaterial2D _originalMaterial;

    // Fields
    public PlayerTumbleState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("Tumbling", true);
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        _ctx.PlayerHasControl = false;
        _originalMaterial = _ctx.Rigidbody.sharedMaterial;
        _ctx.Rigidbody.sharedMaterial = _ctx.Props.TumbleMaterial;
        _ctx.FrameVelocity = Vector2.zero;
    }

    public override void Update()
    {
        TryChangeState();
    }

    public override void ExitState()
    {
        _ctx.SweepHandler.EndSweep();
        _ctx.Animator.SetBool("Tumbling", false);
        _ctx.PlayerHasControl = true;
        _ctx.Rigidbody.sharedMaterial = _originalMaterial;
    }




    //state
    private void TryChangeState()
    {
        float velocityThreshold = 0.1f;
        if (_ctx.Rigidbody.velocity.magnitude <= velocityThreshold)
        {
            _state.ChangeState(PlayerStateEnum.Idle);
        }
        
    }


}
