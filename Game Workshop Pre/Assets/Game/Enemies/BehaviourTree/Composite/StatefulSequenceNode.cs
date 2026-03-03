// A sequence node is a composite note that runs its children in order until one fails and 
// it will lock (not continue) on Running, meaning children MUST return Success in order to continue.
// This node will start where it left off last frame if it ended due to its child Running
[BehaviourNode(4, "Composite")]
public class StatefulSequenceNode : CompositeNode
{
    private int _currentNodeIndex = 0;
    public override BTNodeState Evaluate()
    {
        _isActive = true;

        for (int i = _currentNodeIndex; i < Children.Count; i++)
        {
            BehaviourTreeNode child = Children[i];
            switch (child.Evaluate())
            {
                case BTNodeState.Running:
                    _currentNodeIndex = i;
                    return BTNodeState.Running;
                case BTNodeState.Failure:
                    _currentNodeIndex = 0;
                    return BTNodeState.Failure;
            }

        }

        _currentNodeIndex = 0;
        return BTNodeState.Success;
    }


}
