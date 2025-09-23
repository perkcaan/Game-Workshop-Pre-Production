using UnityEngine;

// A selector node is a composite note that runs its children in order until one succeeds.
public class SelectorNode : BehaviourTreeNode
{
    [SerializeReference, SerializeReferenceDropdown] public BehaviourTreeNode[] children;

    public override void CheckRequiredComponents(EnemyBase self)
    {
        foreach (BehaviourTreeNode child in children)
        {
            child.CheckRequiredComponents(self);
        }
    }

    protected override void Initialize()
    {
        foreach (BehaviourTreeNode child in children)
        {
            child.Initialize(Blackboard);
        }
    }

    public override BTNodeState Evaluate()
    {
        foreach (BehaviourTreeNode child in children)
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
