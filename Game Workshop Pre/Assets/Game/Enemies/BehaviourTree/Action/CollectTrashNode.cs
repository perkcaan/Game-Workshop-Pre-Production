using System;
using TMPro;
using UnityEngine;

[BehaviourNode(1,"Actions"), Serializable]

public class CollectTrashNode : BehaviourTreeNode
{
    [SerializeField]private Vector2 trashPos = Vector2.zero;
    private bool atTrash = false;
    [SerializeField] private float atTrashDistance = 0.5f;
    
    
    public override BTNodeState Evaluate()
    {
        if (!Blackboard.TryGet("targetPosition", out Vector2? targetPosition) || !targetPosition.HasValue)
        {
           
            return BTNodeState.Failure;
        }
        _isActive = true;
        trashPos = targetPosition.Value;

        if (atTrash) 
        {
            atTrash = false;
            if (Blackboard.TryGetNotNull("target", out ITargetable target))
            {
               
                Self.Pather.FacePoint(trashPos);
                return BTNodeState.Success;
            }
            return BTNodeState.Failure;
        }

        StartCollectingTrash();
        return BTNodeState.Running;
    }

    public void StartCollectingTrash()
    {
        
            Self.Pather.GoToPoint(trashPos, atTrashDistance, ArrivedAtTrash);
       
        
        
    }
    public void ArrivedAtTrash()
    {
        atTrash = true;
        Self.Pather.Stop();
        Reset();
        
        
        
    }

    protected override void Reset()
    {
        atTrash = false;
        trashPos = Vector2.zero;
        
    }

    protected override void DrawDebug()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(trashPos, atTrashDistance);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
