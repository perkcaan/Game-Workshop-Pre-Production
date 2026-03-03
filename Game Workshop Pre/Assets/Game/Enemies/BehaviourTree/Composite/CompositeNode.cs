using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// The base class for all composite nodes. Composite nodes have multiple children and direct flow between them.
public abstract class CompositeNode : BehaviourTreeNode
{
    public override int MaxChildren => -1; //-1 means virtually infinite

}
