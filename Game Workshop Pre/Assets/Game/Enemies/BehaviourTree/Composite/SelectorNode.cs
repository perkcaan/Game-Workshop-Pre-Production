

// A selector node is a composite note that runs its children in order until one succeeds and 
// it will lock (not continue) on Running, meaning children MUST return Failure in order to continue. 
[BehaviourNode(0, "Composite")]
public class SelectorNode : CompositeNode
{


    public override BTNodeState Evaluate()
    {
        foreach (BehaviourTreeNode child in Children)
        {
            switch (child.Evaluate())
            {
                case BTNodeState.Running:
                    return BTNodeState.Running;
                case BTNodeState.Success:
                    return BTNodeState.Success;
            }
        }
        return BTNodeState.Failure;
    }


}