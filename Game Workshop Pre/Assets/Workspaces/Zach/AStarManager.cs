using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openList = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gCostScore = float.MaxValue;
        }

        start.gCostScore = 0;
        start.heuristicScore = Vector2.Distance(start.transform.position, end.transform.position);
        openList.Add(start);

        while (openList.Count > 0)
        {
            int lowestF = default;
            for (int i = 1; i < openList.Count; i++)
            {
                lowestF = i;
            }

            Node currentNode = openList[lowestF];
            openList.Remove(currentNode);


            // When end node is found
            if (currentNode == end)
            {
                List<Node> path = new List<Node>();
                path.Insert(0, end);
                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gCostScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);
                if (heldGScore < connectedNode.gCostScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gCostScore = heldGScore;
                    connectedNode.heuristicScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);
                    if (!openList.Contains(connectedNode))
                    {
                        openList.Add(connectedNode);
                    }
                }
            }

        }

        return null;
    }
}
