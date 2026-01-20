using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SerializationTest : MonoBehaviour
{
    [SerializeReference] private BehaviourTreeNode _behaviour;


    [ContextMenu("Behaviour Details")]
    private void PrintDetails()
    {
        PrintDetailsR(_behaviour);
    }

    private void PrintDetailsR(BehaviourTreeNode currentNode)
    {
        if (currentNode == null) return;

        Debug.Log(currentNode.DisplayName + " | " + currentNode.MaxChildren);
        foreach (BehaviourTreeNode node in currentNode.Children)
        {
            PrintDetailsR(node);
        }

    }
}
