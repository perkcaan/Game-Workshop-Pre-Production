using UnityEngine;

// A negation node is a decorator node that negates the output of what it decorates.
// So Success becomes Failure and Failure becomes Success.
public class NegationNode : BehaviourTreeNode
{
    [SerializeReference, SerializeReferenceDropdown] public BehaviourTreeNode child;

    public override void CheckRequiredComponents(EnemyBase self)
    {
        if (child != null) child.CheckRequiredComponents(self);
    }

    protected override void Initialize()
    {
        child.Initialize(Blackboard, Self);
    }

    public override BTNodeState Evaluate()
    {
        switch (child.Evaluate())
        {
            case BTNodeState.Success:
                return BTNodeState.Failure;
            case BTNodeState.Failure:
                return BTNodeState.Success;
        }
        return BTNodeState.Running;
    }

    public override void DrawDebug()
    {
        if (child != null) child.DrawDebug();
    }


    

}
