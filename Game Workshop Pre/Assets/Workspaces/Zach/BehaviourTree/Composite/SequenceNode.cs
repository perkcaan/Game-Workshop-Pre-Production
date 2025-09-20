using UnityEngine;

// A sequence node is a composite note that runs its children in order until one fails.
[CreateAssetMenu(menuName = "BehaviourTree/Composite/Sequence")]
public class SequenceNode : BehaviourTreeNode
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
                case BTNodeState.Failure:
                    return BTNodeState.Failure;
            }
        }
        return BTNodeState.Success;
    }

}
