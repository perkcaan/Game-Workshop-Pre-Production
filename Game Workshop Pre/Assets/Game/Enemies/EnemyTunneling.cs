using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTunneling : MonoBehaviour
{
    [SerializeField] private float _speedModifier;
    [SerializeField] private LayerMask _targetMask;

    private EnemyBase _enemy;
    private List<Collider2D> _collidersInMask = new List<Collider2D>();

    private void Awake()
    {
        _enemy = GetComponentInParent<EnemyBase>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & _targetMask) != 0)
        {
            _collidersInMask.Add(collider);
        }

        UpdateSpeed();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & _targetMask) != 0)
        {
            _collidersInMask.Remove(collider);
        }
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (_collidersInMask.Count > 0)
        {
            _enemy.SpeedModifier = _speedModifier;
        } else
        {
            _enemy.SpeedModifier = 1f;
        }
    }
}