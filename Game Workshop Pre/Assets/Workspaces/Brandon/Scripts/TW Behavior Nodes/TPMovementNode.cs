using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMovement : BehaviourTreeNode
{

    [Tooltip("Game Objects that act as location for Wizard to teleport to.")]
    [SerializeField] GameObject[] TPPoints;

    public override void CheckRequiredComponents(EnemyBase self)
    {

    }

    protected override void Initialize()
    {

    }

    public override BTNodeState Evaluate()
    {
        return BTNodeState.Running;
    }

}
