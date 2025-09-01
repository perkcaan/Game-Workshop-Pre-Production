using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : PushableObject
{
    [SerializeField] float scaleMultiplier;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            weight += collectableTrash.weight;
            Destroy(collectableTrash.gameObject);
            SetSize();
        }
    }

    public void SetSize()
    {
        float newSize = scaleMultiplier * Mathf.Sqrt(weight);
        transform.localScale = Vector3.one * newSize;
    }
}
