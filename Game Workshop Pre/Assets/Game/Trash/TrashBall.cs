using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : CollectableTrash
{
    [SerializeField] float scaleMultiplier;

    public void SetSize()
    {
        float newSize = scaleMultiplier * Mathf.Sqrt(weight);
        transform.localScale = Vector3.one * newSize;
    }
}
