using UnityEngine;

// A sequence node is a composite note that runs its children in order until one fails.
// This node runs its children in parallel, meaning it will not lock on Running. 
// Good for having multiple actions running in parallel, hence the name


[BehaviourNode(2, "Composite")]
public class ParallelSequenceNode : CompositeNode
{
    public override BTNodeState Evaluate()
    {
        int successCount = 0;
        foreach (BehaviourTreeNode child in Children)
        {
            switch (child.Evaluate())
            {
                case BTNodeState.Success:
                    successCount++;
                    break;
                case BTNodeState.Failure:
                    return BTNodeState.Failure;
            }
        }
        if (successCount >= Children.Count) return BTNodeState.Success;
        return BTNodeState.Running;
    }

}
