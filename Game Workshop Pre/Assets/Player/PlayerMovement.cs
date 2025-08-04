using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 playerVector;
    public Vector2 normalPlayer; // Normalized vector for player movement
    public float basicSpeed = 5f; // Speed of the player movement
    public Transform ballTransform;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        playerVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        normalPlayer = playerVector.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (normalPlayer.magnitude == 0)
        {
            if (rb.velocity.magnitude > 0.1f) // Check if the velocity is significant
            {
                rb.velocity = Vector2.Lerp(rb.velocity.normalized, Vector2.zero, Time.deltaTime * 3f); // Gradually reduce velocity
            }

            //rb.velocity = Vector2.zero;
        }
        else
        {
            if (normalPlayer.magnitude >= 1)
            {
                rb.velocity = normalPlayer;
            }
        }

        // Move the ball
        if (playerVector.magnitude > 0.1f)
        {
            Vector3 move = (Vector3)(playerVector.normalized * basicSpeed * Time.deltaTime);
            ballTransform.position += move;
        }


        Vector2 acceleration = Input.acceleration;

        acceleration += playerVector;


    }

    private void FixedUpdate()
    {
        if(TrashScore.score > 1)
        {
            basicSpeed = Hitbox.FindAnyObjectByType<Hitbox>().BaseSpeed += 1; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
        {
            
        }
    }
}
