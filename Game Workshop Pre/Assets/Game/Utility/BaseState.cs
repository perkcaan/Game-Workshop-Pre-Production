using System;


// This class should be able to work as a state for whatever state machine you want.
// If you need help implementing or using ask me! -Zach
public abstract class BaseState<StateEnum> where StateEnum : Enum
{
    public BaseState()
    {

    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
    public virtual void OnDrawGizmos() { }
}