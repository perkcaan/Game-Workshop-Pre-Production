using System;
using System.Collections;
using UnityEngine;

public class PlayerSweepPokeState : BaseState<PlayerStateEnum>
{
    // Context & State
    private PlayerContext _ctx;
    private PlayerStateMachine _state;

    // Fields
    //movement
    private Coroutine _pokeCoroutine;

    // Constructor
    public PlayerSweepPokeState(PlayerContext context, PlayerStateMachine state)
    {
        _ctx = context;
        _state = state;
    }

    // Methods
    //state
    public override void EnterState()
    {
        _ctx.Animator.SetBool("SweepPoke", true);
        _ctx.CanHook = false;
        _ctx.CanSwipe = false;
        _ctx.CanDash = false;
        DoPoke();
    }

    public override void Update()
    {
        HandleMovement();
    }

            //movement
    // This is the exact same movement as PlayerIdleState
    private void HandleMovement()
    {
        _ctx.FrameVelocity = Vector2.zero;
    }

    public override void ExitState()
    {
        if (_pokeCoroutine != null) _ctx.Player.StopCoroutine(_pokeCoroutine);
        _ctx.SweepHandler.EndPoke();
        _ctx.PokeCooldownTimer = _ctx.Player.PokeCooldown;
        _ctx.Animator.SetBool("SweepPoke", false);
    }


    private IEnumerator PokeDuration()
    {
        yield return new WaitForSeconds(_ctx.Player.PokeDuration);
        LeavePokeState();
    }

    //poke
    private void DoPoke()
    {
        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(_ctx.MouseInput);
        Vector2 direction = mouseWorldPoint - (Vector2)_ctx.Player.transform.position;
        direction.Normalize();
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _ctx.Rotation = Mathf.DeltaAngle(0f, targetAngle);
        _ctx.SweepHandler.DoPoke(_ctx.Rotation, _ctx.Player.PokeForce);
        _pokeCoroutine = _ctx.Player.StartCoroutine(PokeDuration());
    }

    private void LeavePokeState()
    {
        if (_ctx.PlayerHasControl)
        {
            if (_ctx.IsSweepPressed)
            {
                _state.ChangeState(PlayerStateEnum.Sweeping);
            }
            else
            {
                _state.ChangeState(PlayerStateEnum.Idle);
            }
        }
    }
}