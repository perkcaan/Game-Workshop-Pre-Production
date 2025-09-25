using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PathfindingManager : StaticInstance<PathfindingManager>
{

    [SerializeField] private List<PathfindingNode> _serializedNodes = new List<PathfindingNode>();
    public Dictionary<Vector2Int, PathfindingNode> Nodes { get; private set; }
    [SerializeField] private Grid _layoutGrid;
    public Grid LayoutGrid
    {
        get { return _layoutGrid; }
        set { _layoutGrid = value; }
    }

    public bool IsPrepared { get; set; } = false;

    private void OnValidate()
    {
        Instance = this;
    }

    protected override void Awake()
    {
        base.Awake();
        Nodes = new Dictionary<Vector2Int, PathfindingNode>();
        foreach (PathfindingNode node in _serializedNodes)
        {
            Nodes[node.GridPos] = node;
        }
        IsPrepared = true;
    }

    public void SetNodes(List<PathfindingNode> nodes)
    {
        _serializedNodes = nodes;
    }


    private void OnDrawGizmosSelected()
    {
        if (Nodes == null)
        {
            foreach (PathfindingNode node in _serializedNodes)
            {
                GizmoDrawNode(node);
            }
        }
        else
        {
            foreach (KeyValuePair<Vector2Int, PathfindingNode> nodePair in Nodes)
            {
                PathfindingNode node = nodePair.Value;
                GizmoDrawNode(node);
            }
        }
    }

    private void GizmoDrawNode(PathfindingNode node)
    {
        if (node.IsBlocking)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawSphere(node.WorldPos, 0.1f);
    }
}
