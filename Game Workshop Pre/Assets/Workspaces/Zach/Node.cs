using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathfindingNode
{
    public Vector2 WorldPos;
    public Vector2Int GridPos;
    private int _blockingValue;
    public bool IsBlocking
    {
        get { return _blockingValue > 0; }
    }

    public PathfindingNode(Vector2Int gridPos, Vector3 worldPos)
    {
        GridPos = gridPos;
        WorldPos = worldPos;
    }

    public void UpdateBlocking(int value)
    {
        _blockingValue = Mathf.Max(_blockingValue + value, 0);
    }
}

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;

    public float gCostScore;
    public float heuristicScore;

    public float CostFunction()
    {
        return gCostScore + heuristicScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}
