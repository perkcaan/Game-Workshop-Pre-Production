using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BehaviourNode(8, "Actions")]
public class FireAtTargetNode : BehaviourTreeNode
{
    [SerializeField] private float _projectileSpeed = 10f;

    [SerializeField] private float _projectileLifetime = 3f;
    [SerializeField] private float _fireInterval = 1f;
    [SerializeField] private GameObject _projectilePrefab;

    private float _fireTimer = 0f;
    private Vector2 _targetPos;

    public override BTNodeState Evaluate()
    {
        if (!Blackboard.TryGet("targetPosition", out Vector2? targetPosition) || !targetPosition.HasValue)
        {
            return BTNodeState.Failure;
        }

        _isActive = true;
        _targetPos = targetPosition.Value;

        Self.Pather.Stop(); // stop movement

        _fireTimer += Time.deltaTime;

        if (_fireTimer >= _fireInterval)
        {
            FireProjectile();
            _fireTimer = 0f;
        }

        return BTNodeState.Running;
    }

    private void FireProjectile()
    {
        if (_projectilePrefab == null)
        {
            Debug.LogWarning("FireProjectile is working, but there is no projectile prefab assigned!");
            return;
        }

        Debug.Log("Firing");
        Vector2 firePosition = Self.transform.position;
        Vector2 direction = (_targetPos - firePosition).normalized;

        GameObject projectile = UnityEngine.Object.Instantiate(_projectilePrefab, firePosition, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * _projectileSpeed;
        }

        UnityEngine.Object.Destroy(projectile, _projectileLifetime);
    }

}