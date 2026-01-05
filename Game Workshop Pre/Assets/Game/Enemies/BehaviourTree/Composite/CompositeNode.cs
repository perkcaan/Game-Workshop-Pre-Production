using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The base class for all composite nodes. Composite nodes have multiple children and direct flow between them.
public abstract class CompositeNode : BehaviourTreeNode
{
    public override int MaxChildren => -1; //-1 means virtually infinite

    public override void CheckRequiredComponents(EnemyBase self)
    {
        foreach (BehaviourTreeNode child in Children)
        {
            if (child != null) child.CheckRequiredComponents(self);
        }
    }

    protected override void Initialize()
    {
        foreach (BehaviourTreeNode child in Children)
        {
            child.Initialize(Blackboard, Self);
        }
    }

    public override void DrawDebug()
    {
        foreach (BehaviourTreeNode child in Children)
        {
            if (child != null) child.DrawDebug();
        }
    }


}
