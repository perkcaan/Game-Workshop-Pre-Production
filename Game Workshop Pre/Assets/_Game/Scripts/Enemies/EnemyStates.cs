using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;


// Enum of possible enemy states
public enum EnemyStateEnum
{
    Default,
    Absorbed,
    Stunned,
    Pinball
}

// Base class for all enemy states
public abstract class EnemyState : BaseState<EnemyStateEnum>
{
    protected EnemyBase _self;
    protected EnemyStateMachine _state;
    public abstract bool HasBehaviour { get; }

    public EnemyState(EnemyBase self, EnemyStateMachine state)
    {
        _self = self;
        _state = state;
    }

    public override void EnterState()
    {
        if (!HasBehaviour) _self.CancelBehaviour();
    }

    public virtual void ModifySweep(ref EnemySweepData data) { }
    public virtual void ModifySwipe(ref EnemySwipeData data) { }
    public virtual void ModifyPoke(ref EnemyPokeData data) { }
    public virtual void ModifyAbsorb(ref EnemyAbsorbData data) { }
}


// State machine that handles switching between states
public class EnemyStateMachine: BaseStateMachine<EnemyStateEnum>
{
    private EnemyState _currentEnemyState => _currentState as EnemyState; 
    public bool HasBehaviour => _currentEnemyState.HasBehaviour;
    public void ModifySweep(ref EnemySweepData data)
    {
        _currentEnemyState.ModifySweep(ref data);
    }

    public void ModifySwipe(ref EnemySwipeData data)
    {
        _currentEnemyState.ModifySwipe(ref data);
    }

    public void ModifyPoke(ref EnemyPokeData data)
    {
        _currentEnemyState.ModifyPoke(ref data);
    }

    public void ModifyAbsorb(ref EnemyAbsorbData data)
    {
        _currentEnemyState.ModifyAbsorb(ref data);
    }


    public EnemyStateMachine(EnemyBase self)
    {
        Dictionary<EnemyStateEnum, BaseState<EnemyStateEnum>> states = new Dictionary<EnemyStateEnum, BaseState<EnemyStateEnum>>()
        {
            { EnemyStateEnum.Default, new EnemyDefaultState(self, this) },
            { EnemyStateEnum.Absorbed, new EnemyAbsorbedState(self, this) },
            { EnemyStateEnum.Stunned, new EnemyStunnedState(self, this) },
            { EnemyStateEnum.Pinball, new EnemyPinballState(self, this) }
        };

        Setup(states, EnemyStateEnum.Default);
    }
}


public class EnemyDefaultState : EnemyState
{
    public override bool HasBehaviour { get { return true; } }
    public EnemyDefaultState(EnemyBase self, EnemyStateMachine state) : base(self, state) { }

}

public class EnemyAbsorbedState : EnemyState
{
    public override bool HasBehaviour { get { return false; } }
    public EnemyAbsorbedState(EnemyBase self, EnemyStateMachine state) : base(self, state) { }
}

public class EnemyStunnedState : EnemyState
{
    private float _stunTime;
    public override bool HasBehaviour { get { return false; } }
    public EnemyStunnedState(EnemyBase self, EnemyStateMachine state) : base(self, state) { }

    public override void EnterState()
    {
        base.EnterState();
        _stunTime = _self.StunTime;
        //add stun particle effect
    }
    public override void Update()
    {
        if (_stunTime <= 0f)
        {
            _state.ChangeState(EnemyStateEnum.Default);
            return;
        }
        _stunTime -= Time.deltaTime;
    }

    public override void ExitState()
    {
        //remove stun particle effect
    }

    public override void ModifySwipe(ref EnemySwipeData data)
    {
        data.IsVulnerable = true;
    }

    public override void ModifyPoke(ref EnemyPokeData data)
    {
        data.IsVulnerable = true;
    }

    public override void ModifyAbsorb(ref EnemyAbsorbData data)
    {
        data.CanAbsorb = true;
    }
}

public class EnemyPinballState : EnemyState
{
    private float _pinballTime;
    public override bool HasBehaviour { get { return false; } }
    
    private PhysicsMaterial2D _originalMaterial;
    public EnemyPinballState(EnemyBase self, EnemyStateMachine state) : base(self, state) { }
    
    public override void EnterState()
    {
        base.EnterState();
        _pinballTime = _self.PinballProps.Time;
        _originalMaterial = _self.Rigidbody.sharedMaterial;
        _self.Rigidbody.sharedMaterial = _self.PinballProps.Material;
        _self.Animator.SetTrigger("DoPinball");
    }

    public override void Update()
    {
        if (_pinballTime <= 0f)
        {
            _state.ChangeState(EnemyStateEnum.Default);
            return;
        }
        _pinballTime -= Time.deltaTime;
    }

    public override void ExitState()
    {
        _self.Rigidbody.sharedMaterial = _originalMaterial;
        _self.Animator.SetTrigger("ReturnToIdle");
    }

    public override void ModifySwipe(ref EnemySwipeData data)
    {
        data.IsVulnerable = false;
        data.SwipeMultiplier *= _self.PinballProps.SwipeMultiplier;
        data.KnockbackMultiplier = 0f;
    }

    public override void ModifyPoke(ref EnemyPokeData data)
    {
        data.IsVulnerable = false;
        data.PokeMultiplier *= _self.PinballProps.PokeMultiplier;
        data.KnockbackMultiplier = 0f;
    }

    public override void ModifySweep(ref EnemySweepData data)
    {
        data.CanSweep = true;
    }

    public override void ModifyAbsorb(ref EnemyAbsorbData data)
    {
        data.CanAbsorb = true;
    }
}

[Serializable]
public class EnemyPinballProperties
{
    [SerializeField] private float _time = 5f;
    public float Time { get { return _time; } }
    [SerializeField] private PhysicsMaterial2D _material;
    public PhysicsMaterial2D Material { get { return _material; } }
    [SerializeField] private float _swipeMultiplier = 1f;
    public float SwipeMultiplier { get { return _swipeMultiplier; } }
    [SerializeField] private float _pokeMultiplier = 1f;
    public float PokeMultiplier { get { return _pokeMultiplier; } }
}
