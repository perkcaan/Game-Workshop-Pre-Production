using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

// A decorator node is a node that has some control or effect on its singular child
public abstract class DecoratorNode : BehaviourTreeNode
{
    // error here need to fix
    protected BehaviourTreeNode Child { 
        get { 
            if (Children.Count > 0) { return Children[0]; }
            else return null; } 
        set { Children[0] = value; } }
    public override int MaxChildren => 1;

}
