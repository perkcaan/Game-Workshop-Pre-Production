using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeatHitbox : MonoBehaviour
{
    [SerializeField] private float _heatApplied = 10f;
    [SerializeField] private float _knockbackApplied = 10f;

    private Collider2D _collider;
    private float _rotation = 0f;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void Enable()
    {
        _collider.enabled = true;
    }

    public void UpdateRotation(Transform parent, float rotation)
    {
        float rotationRad = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad));

        // Set the hitbox position and rotation relative to rotation
        transform.position = parent.position + ((Vector3)offset * 0.75f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);
        _rotation = rotation;
    }

    public void Disable()
    {
        _collider.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out HeatMechanic heat))
        {
            heat.ModifyHeat(_heatApplied, true);
        }

        if (collider.gameObject.TryGetComponent(out ISwipeable swipeable))
        {
            float rotationRad = _rotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad)).normalized;

            swipeable.OnSwipe(direction, _knockbackApplied);
        }
    }
}
