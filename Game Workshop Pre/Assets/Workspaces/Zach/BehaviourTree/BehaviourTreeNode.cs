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

// If this is confusing to you at all, I will explain it
//-Zach
public abstract class BehaviourTreeNode : ScriptableObject
{
    public abstract BTNodeState Evaluate(Blackboard blackboard);
}
