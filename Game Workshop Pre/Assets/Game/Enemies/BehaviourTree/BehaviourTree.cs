using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


// Wrapper class for the entire behaviour tree.
[Serializable]
public class BehaviourTree
{
    [SerializeReference] BehaviourTreeNode _root;
    [NonSerialized] private List<BehaviourTreeNode> _allNodes = new List<BehaviourTreeNode>();

    [SerializeField] private bool _enableBehaviourDebug;
    private EnemyBase _self;
    private EnemyBlackboard _blackboard;
    public EnemyBlackboard Blackboard { get { return _blackboard; } }

    public void Validate(EnemyBase self)
    {
        _self = self;
        RebuildNodeCache();
        foreach (BehaviourTreeNode node in _allNodes)
        {
            if (node == null) continue;
            node.Validate(_self);
        }
    }

    public void Initialize(EnemyBase self)
    {
        _self = self;
        _blackboard = PrepareBlackboard();
        RebuildNodeCache();
        PrepareBlackboard();
        foreach (BehaviourTreeNode node in _allNodes)
        {
            if (node == null) continue;
            node.Initialize(_blackboard, _self);
        }
    }

    public void Tick()
    {
        if (_root == null) return;

        foreach (BehaviourTreeNode node in _allNodes)
        {
            node.Reset();
        }
        _root.Evaluate();
    }

    public void DrawDebug()
    {
        if (_root == null) return;
        if (!_enableBehaviourDebug) return;

        foreach (BehaviourTreeNode node in _allNodes)
        {
            node.DrawDebugIfActive();
        }
    }


    // Updates AllNodes.
    private void RebuildNodeCache()
    {
        _allNodes = new List<BehaviourTreeNode>();

        if (_root == null) return;

        Stack<BehaviourTreeNode> nodesToVisit = new Stack<BehaviourTreeNode>();
        nodesToVisit.Push(_root);

        while (nodesToVisit.Count > 0)
        {
            BehaviourTreeNode currentNode = nodesToVisit.Pop();
            _allNodes.Add(currentNode);
            foreach (BehaviourTreeNode child in currentNode.Children)
            {
                if (child == null) continue;
                nodesToVisit.Push(child);
            }
        }
    }

    private EnemyBlackboard PrepareBlackboard()
    {
        EnemyBlackboard blackboard = new EnemyBlackboard(_self);

        /* add variables as needed, like such:
        _blackboard.Set<string>("name", gameObject.name);
        _blackboard.Set<Vector2>("frameVelocity", Vector2.zero);
        _blackboard.Set<float>("moveSpeed", _moveSpeed);
        */
        return blackboard;
    }
}
