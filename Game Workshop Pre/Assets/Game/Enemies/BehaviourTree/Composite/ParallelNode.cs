using UnityEditor.Animations;
using UnityEngine;


// A simplified version of the ParallelSequence node.
// This node runs its children in parallel, without worrying about their state. 
// Good for having multiple actions running in parallel, hence the name

[BehaviourNode(2, "Composite")]
public class ParallelNode : CompositeNode
{
    public override BTNodeState Evaluate()
    {
        _isActive = true;
        foreach (BehaviourTreeNode child in Children)
        {
            child.Evaluate();
        }
        return BTNodeState.Success;
    }

}
