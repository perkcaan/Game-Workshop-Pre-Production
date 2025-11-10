using System;
using UnityEngine;

public enum BTNodeState { Running, Success, Failure }


// The base node for a behaviour tree. 
// A behaviour tree is comprised of three types of nodes:
// Composite nodes - Control flow between multiple child nodes
// Decorator nodes - Modify behaviour of a single child node
// Leaf nodes - Actual behaviour

// Leaf nodes can be further separated into Condition nodes and Action nodes:
// Condition nodes evaluate true based on some condition
// Action nodes perform an action

// If this is confusing you at all, I will explain it
//-Zach
[Serializable]
public abstract class BehaviourTreeNode
{
    // Call Blackboard to get a reference to the enemy blackboard in child objects
    private EnemyBlackboard _blackboard;
    protected EnemyBlackboard Blackboard { get { return _blackboard; } }

    private EnemyBase _self;
    protected EnemyBase Self { get { return _self; } }

    public abstract void CheckRequiredComponents(EnemyBase self);
    protected abstract void Initialize();
    public abstract BTNodeState Evaluate();
    public virtual void DrawDebug() { }

    public void Initialize(EnemyBlackboard blackboard, EnemyBase self)
    {
        _blackboard = blackboard;
        _self = self;
        Initialize();
    }
}
