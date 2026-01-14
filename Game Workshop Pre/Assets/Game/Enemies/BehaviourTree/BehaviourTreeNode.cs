using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
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

    #if UNITY_EDITOR
    //name of the node. For use within the editor.
    public string DisplayName = "Node";
    #endif


    [SerializeReference] public List<BehaviourTreeNode> Children = new List<BehaviourTreeNode>();
    
    //override if node has children
    public virtual int MaxChildren => 0; //-1 for virtually infinite possible children
    



    // Call Blackboard to get a reference to the enemy blackboard in child objects
    private EnemyBlackboard _blackboard;
    protected EnemyBlackboard Blackboard { get { return _blackboard; } }

    private EnemyBase _self;
    protected EnemyBase Self { get { return _self; } }

    protected bool _isActive = false;

    protected abstract void CheckRequiredComponents();
    protected abstract void Initialize();
    protected virtual void DrawDebug() { }

    public void Validate(EnemyBase self)
    {
        _self = self;
        CheckRequiredComponents(); 
    }

    public void Initialize(EnemyBlackboard blackboard, EnemyBase self)
    {
        _blackboard = blackboard;
        _self = self;
        Children.RemoveAll(child => child == null); 
        Initialize();
    }


    public void Reset()
    {
        _isActive = false;
    }

    public abstract BTNodeState Evaluate();
    
    public void DrawDebugIfActive()
    {
        if (_isActive) DrawDebug();
    }

}
