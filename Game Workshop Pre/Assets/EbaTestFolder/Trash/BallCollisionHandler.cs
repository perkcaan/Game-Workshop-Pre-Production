using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 velocity = Vector2.zero; // Initial velocity, can be set from outside
    private CircleCollider2D collide;
    public TrashBallController trashBallController;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.bodyType = RigidbodyType2D.Kinematic; // Not affected by physics, but still detects collisions
        
    }

    void Update()
    {
        // Move the ball manually
        rb.velocity = PlayerMovementController.currentVelocity;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 0f; // Keeps the ball from falling
            PlayerMovementController.currentVelocity *= -rb.velocity.magnitude * (trashBallController.trashSize/2); // Reverse and reduce speed upon collision
            StartCoroutine(BodyReset()); // Reset the body type after a short delay
            Debug.Log("Ball collided with wall and bounced off.");
        }

        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }

    private IEnumerator BodyReset()
    {
        yield return new WaitForSeconds(0.5f);
        rb.bodyType = RigidbodyType2D.Kinematic; // Reset to kinematic after a short delay
        rb.velocity = Vector2.zero; // Reset velocity
        rb.angularVelocity = 0f; // Reset angular velocity
        rb.gravityScale = 0f; // Ensure gravity is still off
    }
}
    
        
