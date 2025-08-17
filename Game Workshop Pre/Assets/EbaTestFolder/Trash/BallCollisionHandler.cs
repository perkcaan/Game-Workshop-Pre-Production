using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 velocity = Vector2.zero; // Initial velocity, can be set from outside
    public bool collide;
    public TrashBallController trashBallController;
    public PlayerMovementController PlayerMovementController;


    void Start()
    {
        //rb = trashBallController.GetComponent<Rigidbody2D>();
        
        
    }

    void Update()
    {

        

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //collide = true;
            Bounce();
            StartCoroutine(BodyReset());


        }
        

        if (collision.gameObject.CompareTag("Player"))
        {

        }

        if(collision.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            
            if (trashBallController.isAttached) return;
            
            trashBallController.AddCollectableTrash(trash);
            Destroy(collision.gameObject);
            

        }
    }

    private IEnumerator BodyReset()
    {
        yield return new WaitForSeconds(1f);
        rb.bodyType = RigidbodyType2D.Kinematic; 
        rb.velocity = Vector2.zero; 
        rb.angularVelocity = 0f; 
        rb.gravityScale = 0f;
        //collide = false;
    }

    public void Bounce()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f; // Keeps the ball from falling
        
        
        if (trashBallController.isAttached)
        {
            
            float maxBounceSpeed = 4f; 

            // When setting the player's velocity after a bounce:
            PlayerMovementController.currentVelocity = Vector2.ClampMagnitude(PlayerMovementController.currentVelocity, maxBounceSpeed);

            PlayerMovementController.currentVelocity *= -PlayerMovementController.currentVelocity.magnitude * (trashBallController.trashSize / 2);
        }

        if(PlayerMovementController.currentVelocity.magnitude <= 1f)
        {
            
            
            PlayerMovementController.currentVelocity = Vector2.zero;
        }
        




    }
}
    
        
