using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeReference, SerializeReferenceDropdown] private BehaviourTreeNode _behaviour;

    protected EnemyBlackboard blackboard;

    private void OnValidate()
    {
        _behaviour.CheckRequiredComponents(this);
    }

    private void Start()
    {
        blackboard = new EnemyBlackboard(this);
        PrepareBlackboard();
        _behaviour.Initialize(blackboard);
    }

    private void Update()
    {
        if (_behaviour != null)
        {
            _behaviour.Evaluate();
        }
    }

    private void PrepareBlackboard()
    {
        Vector3 target = Vector3.zero;
        float nearbyRadius = 5f;
        string name = gameObject.name;

        blackboard.Set<Vector2>("target", target);
        blackboard.Set<float>("nearbyRadius", nearbyRadius);
        blackboard.Set<string>("name", name);
    }
}
