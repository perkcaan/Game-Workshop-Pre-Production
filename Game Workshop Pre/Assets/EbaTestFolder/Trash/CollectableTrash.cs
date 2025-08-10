using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public float trashSize;


    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.TryGetComponent(out TrashBallController trashBallController) && trashBallController.isAttached)
        {
            trashBallController.AddCollectableTrash(this);
            DestroySelf();
        }
        // Ball collects trash independently
        else if (collision.gameObject.TryGetComponent(out BallCollisionHandler ballHandler) && !ballHandler.trashBallController.isAttached)
        {
            // You can implement a method in BallCollisionHandler to handle trash collection,
            // or forward it to the TrashBallController if needed.
            ballHandler.trashBallController.AddCollectableTrash(this);
            DestroySelf();



        }

        void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
