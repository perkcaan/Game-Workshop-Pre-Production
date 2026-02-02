// A sequence node is a composite note that runs its children in order until one fails and 
// it will lock (not continue) on Running, meaning children MUST return Success in order to continue. 
[BehaviourNode(1, "Composite")]
public class SequenceNode : CompositeNode
{
    public override BTNodeState Evaluate()
    {
        _isActive = true;

        foreach (BehaviourTreeNode child in Children)
        {
            switch (child.Evaluate())
            {
                case BTNodeState.Running:
                    return BTNodeState.Running;
                case BTNodeState.Failure:
                    return BTNodeState.Failure;
            }
        }
        return BTNodeState.Success;
    }


}
