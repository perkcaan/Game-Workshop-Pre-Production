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
        else if (collision.gameObject.TryGetComponent(out BallCollisionHandler ballHandler))
        {
            
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

        if (trashSize < 1)
        {
            trashRb.AddForce(launchDirection * 200f * (trashSize * 4));
        }
        else
        {
            trashRb.AddForce(launchDirection * 200f / (trashSize));
        }
        // Set drag for smooth deceleration
        trashRb.drag = 10f; // Adjust this value for faster/slower stop
        trashRb.angularDrag = 10f;
        trashRb.gravityScale = 0; // Ensure gravity is off
        Debug.Log("Trash swiped and launched");
        
    }


    public void OnSwipeEnd()
    {
        StartCoroutine(SwipedEndCoroutine());
    }



    public IEnumerator SwipedEndCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        trashRb.velocity = Vector2.zero;
        trashRb.angularVelocity = 0f;
        trashRb.bodyType = RigidbodyType2D.Kinematic;
        trashRb.drag = 0f; // Reset drag for next time
        trashRb.angularDrag = 0f;
        trashRb.gravityScale = 1; // Ensure gravity is off
        Debug.Log("Object stopped after swipe.");
    }

}


