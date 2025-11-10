using UnityEngine;
using UnityEngine.Rendering;

// A sequence node is a composite note that runs its children in order until one fails.
// This node runs its children in parallel, meaning it will not lock on Running. 
// Good for having multiple actions running in parallel, hence the name
public class ParallelSequenceNode : BehaviourTreeNode
{
    [SerializeReference, SerializeReferenceDropdown] public BehaviourTreeNode[] children;

    public override void CheckRequiredComponents(EnemyBase self)
    {
        foreach (BehaviourTreeNode child in children)
        {
            if (child != null) child.CheckRequiredComponents(self);
        }
    }

    protected override void Initialize()
    {
        foreach (BehaviourTreeNode child in children)
        {
            child.Initialize(Blackboard, Self);
        }
    }

    public override BTNodeState Evaluate()
    {
        int successCount = 0;
        foreach (BehaviourTreeNode child in children)
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
        if (successCount >= children.Length) return BTNodeState.Success;
        return BTNodeState.Running;
    }

    public override void DrawDebug()
    {
        foreach (BehaviourTreeNode child in children)
        {
            if (child != null) child.DrawDebug();
        }
    }




}
