using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : CollectableTrash
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            trashSize += collectableTrash.trashSize;
            Destroy(collectableTrash.gameObject);
        }
    }
}
