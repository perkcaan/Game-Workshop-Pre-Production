using System.Collections;
using UnityEngine;

public class CollectableTrash : MonoBehaviour,Swipeable
{
    public float trashSize;
    private Rigidbody2D trashRb;
    [SerializeField] PlayerMovementController playerController;

    private void Start()
    {
        trashRb = GetComponent<Rigidbody2D>();
        trashRb.bodyType = RigidbodyType2D.Kinematic; // Start as kinematic
        playerController = FindObjectOfType<PlayerMovementController>();
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
            Destroy(gameObject);
        }
    }

    public void OnSwiped()
    {
        trashRb.bodyType = RigidbodyType2D.Dynamic;

        float angle = playerController.rotation * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        trashRb.AddForce(launchDirection * trashSize * 200f); // Adjust force multiplier as needed
        Debug.Log("Trash swiped and launched");
        StartCoroutine(SwipedEndCoroutine());
    }
        

    public void OnSwipeEnd()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator SwipedEndCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        trashRb.bodyType = RigidbodyType2D.Kinematic;
        Debug.Log("Trash swipe ended, set to kinematic");


    }
}

