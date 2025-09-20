using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
