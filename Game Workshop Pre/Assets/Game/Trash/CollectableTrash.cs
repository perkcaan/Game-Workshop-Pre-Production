using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public float trashSize;
    private ClosedRoom parentRoom;
    void Start()
    {
        parentRoom = GetComponentInParent<ClosedRoom>();
        parentRoom.trashList.Add(this);
    }

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
            parentRoom.trashList.Remove(this);
            parentRoom.OpenRoom();
            Destroy(gameObject);
        }
    }
}
