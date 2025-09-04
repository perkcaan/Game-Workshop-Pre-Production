using System;
using System.Collections.Generic;


// This class should be able to work as a state machine for whatever you want.
// If you need help implementing or using ask me! -Zach
public abstract class BaseStateMachine<StateEnum> where StateEnum : Enum
{
    protected Dictionary<StateEnum, BaseState<StateEnum>> _states = new Dictionary<StateEnum, BaseState<StateEnum>>();
    protected BaseState<StateEnum> _currentState;

    protected void Setup(Dictionary<StateEnum, BaseState<StateEnum>> states, StateEnum startState)
    {
        _states = states;
        _currentState = _states[startState];
        _currentState.EnterState();
    }

    public void ChangeState(StateEnum state)
    {
        _currentState.ExitState();
        _currentState = _states[state];
        _currentState.EnterState();
    }

    public void Update()
    {
        _currentState.Update();
    }

}