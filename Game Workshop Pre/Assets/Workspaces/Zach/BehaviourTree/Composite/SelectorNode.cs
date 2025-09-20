using UnityEngine;

// A selector node is a composite note that runs its children in order until one succeeds.
[CreateAssetMenu(menuName = "BehaviourTree/Composite/Selector")]
public class SelectorNode : BehaviourTreeNode
{
    public BehaviourTreeNode[] children;

    public override BTNodeState Evaluate(Blackboard blackboard)
    {
        foreach (BehaviourTreeNode child in children)
        {
            switch (child.Evaluate(blackboard))
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
