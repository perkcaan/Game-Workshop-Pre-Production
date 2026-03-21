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
}


// State machine that handles switching between states
public class EnemyStateMachine: BaseStateMachine<EnemyStateEnum>
{
    private EnemyState _currentEnemyState => _currentState as EnemyState; 
    public bool HasBehaviour => _currentEnemyState.HasBehaviour;
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
        _pinballTime = _self.PinballTime;
        _originalMaterial = _self.Rigidbody.sharedMaterial;
        _self.Rigidbody.sharedMaterial = _self.PinballMaterial;
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
}

