using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbsorbedState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

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
        _ctx.CanDash = false;
        _ctx.PlayerHasControl = false;
        _ctx.IsSweepPressed = false;
        _ctx.Collider.enabled = false;
    }

    public override void Update()
    {
        if (_ctx.AbsorbedTrashBall == null)
        {
            _state.ChangeState(PlayerStateEnum.Idle);
            return;
        } 
        Transform trashBallTranform = _ctx.AbsorbedTrashBall.transform;
        _ctx.Player.transform.position = new Vector3(trashBallTranform.position.x, trashBallTranform.position.y, _ctx.Player.transform.position.z);
    }

    public override void ExitState()
    {
        _ctx.SweepHandler.EndSweep();
        _ctx.Animator.SetBool("Absorbed", false);
        _ctx.Animator.speed = 1;
        _ctx.PlayerHasControl = true;
        _ctx.Collider.enabled = true;
    }

}
