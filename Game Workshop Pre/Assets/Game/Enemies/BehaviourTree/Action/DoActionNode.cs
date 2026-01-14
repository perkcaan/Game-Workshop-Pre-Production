using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoActionNode : BehaviourTreeNode
{
    // Fields
    [SerializeField] private int _indexOfAction;

    // Behaviour tree
    protected override void CheckRequiredComponents() { }

    protected override void Initialize()
    {
        Blackboard.Set<bool>("isInAction", false);
    }

    public override BTNodeState Evaluate()
    {
        if (Blackboard.TryGet("isInAction", out bool isInAction)) { }
        if (!isInAction)
        {
            StartAction();
            return BTNodeState.Success;
        }
        return BTNodeState.Running;
    }

    private void StartAction()
    {
        Blackboard.Set<bool>("isInAction", true);
        Self.PerformAction(_indexOfAction);
    }




    protected override void DrawDebug()
    {

    }

}
