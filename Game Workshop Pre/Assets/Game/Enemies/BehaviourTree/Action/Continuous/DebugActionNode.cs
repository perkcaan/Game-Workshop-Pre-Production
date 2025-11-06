using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Build;
using UnityEngine;

public class DebugActionNode : BehaviourTreeNode
{

    [SerializeField] private string _debugText;


    // Example test for requiring a component
    // private Collider2D _colliderToUse;
    // public override void CheckRequiredComponents(EnemyBase self)
    // {
    //     if (_colliderToUse == null)
    //     {
    //         if (self.TryGetComponent<Collider2D>(out Collider2D component))
    //         {
    //             _colliderToUse = component;
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Collider2D component is required to use SimpleWalkNode. Please add it and set it up properly.");
    //         }
    //     }
    // }

    public override void CheckRequiredComponents(EnemyBase self)
    {
        
    }

    protected override void Initialize()
    {
        // Nothing to initalize
    }

    public override BTNodeState Evaluate()
    {
        Debug.Log(_debugText);


        return BTNodeState.Running;
    }
}
