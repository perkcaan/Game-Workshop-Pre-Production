using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// States the player can be in
public enum PlayerStateEnum
{
    Idle,
    Sweeping,
    Charging,
    Swiping,
    Tumble,
    Absorbed
} 

public class PlayerStateMachine : BaseStateMachine<PlayerStateEnum>
{
    private PlayerContext _ctx;

    public PlayerStateMachine(PlayerContext context)
    {
        Dictionary<PlayerStateEnum, BaseState<PlayerStateEnum>> states = new Dictionary<PlayerStateEnum, BaseState<PlayerStateEnum>>()
        {
            { PlayerStateEnum.Idle, new PlayerIdleState(context, this) },
            { PlayerStateEnum.Sweeping, new PlayerSweepingState(context, this) },
            { PlayerStateEnum.Charging, new PlayerChargingState(context, this) },
            { PlayerStateEnum.Swiping, new PlayerSwipingState(context, this) },
            { PlayerStateEnum.Tumble, new PlayerTumbleState(context, this) },
            { PlayerStateEnum.Absorbed, new PlayerAbsorbedState(context, this) }
        };
        _ctx = context;
        Setup(states, PlayerStateEnum.Idle);
    }
}
