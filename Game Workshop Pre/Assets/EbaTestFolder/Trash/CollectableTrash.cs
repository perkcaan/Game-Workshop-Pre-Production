using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public float trashSize;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out TrashBallController trashBallController))
        {
            trashBallController.AddCollectableTrash(this);
            DestroySelf();
        }
    }

    void DestroySelf() {
        Destroy(gameObject);
    }
}
