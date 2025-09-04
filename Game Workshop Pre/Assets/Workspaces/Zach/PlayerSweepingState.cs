using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSweepingState : BaseState<PlayerStateEnum>
{
    // Player Context
    private PlayerContext _ctx;

    public PlayerSweepingState(PlayerContext context)
    {
        _ctx = context;
    }
}
