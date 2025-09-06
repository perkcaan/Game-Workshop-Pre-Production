using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// States the player can be in
public enum PlayerStateEnum
{
    Idle,
    Sweeping
} 

public class PlayerStateMachine : BaseStateMachine<PlayerStateEnum>
{
    private PlayerContext _ctx;

    public PlayerStateMachine(PlayerContext context)
    {
        Dictionary<PlayerStateEnum, BaseState<PlayerStateEnum>> states = new Dictionary<PlayerStateEnum, BaseState<PlayerStateEnum>>()
    {
        { PlayerStateEnum.Idle, new PlayerIdleState(context, this) },
        { PlayerStateEnum.Sweeping, new PlayerSweepingState(context, this) }
    };
        _ctx = context;
        Setup(states, PlayerStateEnum.Idle);
    }
}
