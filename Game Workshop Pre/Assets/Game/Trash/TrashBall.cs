using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : Trash
{
    [SerializeField] float _scaleMultiplier;

    protected override bool MergePriority { get { return true; } }

    public void Initialize(float size, Vector2 velocity)
    {
        Size = size;
        _rigidBody.velocity = velocity;
    }

    protected override void OnSizeChanged()
    {
        float newSize = _scaleMultiplier * Mathf.Sqrt(Size);
        transform.localScale = Vector3.one * newSize;
    }

    // collision
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        IAbsorbable absorbableObject = other.gameObject.GetComponent<IAbsorbable>();
        if (absorbableObject != null)
        {
            absorbableObject.OnAbsorbedByTrashBall(this);
        }
    }

}
