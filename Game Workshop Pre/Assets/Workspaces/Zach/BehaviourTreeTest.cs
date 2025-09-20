using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTest : MonoBehaviour
{
    [SerializeField] private BehaviourTreeNode _rootNode;

    Blackboard blackboard = new Blackboard();

    private void Start()
    {
        PrepareBlackboard();
        // I think change blackboard to only runtime values. 
        // Serialized values can be in distinctive classes.
        // Each Node SO should have a serializable class associated with it
        // Use type serialization like in DOS, then connect node -> serializable class
        // Build SO tree from in script?
    }

    private void Update()
    {
        if (_rootNode != null)
        {
            _rootNode.Evaluate(blackboard);
        }
    }

    private void PrepareBlackboard()
    {
        blackboard.btt = this;
        Vector3 target = Vector3.zero;
        float nearbyRadius = 5f;
        string name = gameObject.name;

        blackboard.Set<Vector2>("target", target);
        blackboard.Set<float>("nearbyRadius", nearbyRadius);
        blackboard.Set<string>("name", name);
    }


}
