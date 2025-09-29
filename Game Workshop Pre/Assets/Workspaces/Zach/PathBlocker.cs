using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBlocker : MonoBehaviour
{
    [SerializeField] private Collider2D _colliderToUse;
    [SerializeField] private float _margin;

    private List<PathfindingNode> _registeredNodes = new List<PathfindingNode>();
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;

    private void Start()
    {
        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
        OnMove();
    }

    private void Update()
    {
        if (transform.position != _lastPosition || transform.rotation != _lastRotation)
        {   
            OnMove();
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }
    }

    private void OnMove()
    {
        if (_colliderToUse == null || !PathfindingManager.Instance.IsPrepared) return;

        List<PathfindingNode> currentNodes = GetBlockingNodes();
        foreach (PathfindingNode node in _registeredNodes)
        {
            node.UpdateBlocking(-1);
        }
        foreach (PathfindingNode node in currentNodes)
        {
            node.UpdateBlocking(1);
        }
        _registeredNodes = currentNodes;
    }

    private void OnDisable()
    {
        foreach (PathfindingNode node in _registeredNodes)
        {
            node.UpdateBlocking(-1);
        }
    }

    // Need to make this work OnEnable and OnMove... Also check to make sure PathfindingManager is set up already
    private List<PathfindingNode> GetBlockingNodes()
    {
        Vector2[] offsets =
        {
            Vector2.zero,
            new Vector2(-_margin, 0),
            new Vector2(_margin, 0),
            new Vector2(0, -_margin),
            new Vector2(0, _margin),
            // 8 ways
            new Vector2(-_margin, -_margin),
            new Vector2(-_margin, _margin),
            new Vector2(_margin, -_margin),
            new Vector2(_margin, _margin)
        };
        List<PathfindingNode> nodesToCheck = new List<PathfindingNode>();
        Bounds bounds = _colliderToUse.bounds;
        Grid grid = PathfindingManager.Instance.LayoutGrid;
        Vector2Int minCell = (Vector2Int)grid.WorldToCell(bounds.min);
        Vector2Int maxCell = (Vector2Int)grid.WorldToCell(bounds.max);
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cellPos = new Vector2Int(x, y);
                if (PathfindingManager.Instance.Nodes.ContainsKey(cellPos))
                {
                    nodesToCheck.Add(PathfindingManager.Instance.Nodes[cellPos]);
                }
            }
        }
        List<PathfindingNode> nodes = new List<PathfindingNode>();
        foreach (PathfindingNode node in nodesToCheck)
        {
            if (_margin > 0f)
            {
                foreach (Vector2 offset in offsets)
                {
                    if (_colliderToUse.OverlapPoint(node.WorldPos + offset))
                    {
                        nodes.Add(node);
                        break;
                    }
                }
            }
            else
            {
                if (_colliderToUse.OverlapPoint(node.WorldPos))
                {
                    nodes.Add(node);
                }
            }
        }
        //Debug.Log(gameObject.name + " blocking: " + nodes.Count);
        return nodes;
    }
}
