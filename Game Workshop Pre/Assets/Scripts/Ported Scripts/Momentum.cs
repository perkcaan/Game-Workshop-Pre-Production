using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    public float mass = 1.0f; 
    public Vector2 velocity = Vector2.zero; 
    public float force = 20.0f; 
    public float maxVelocity = 0.1f;
    public float drag = 0.5f; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * force * Time.deltaTime / mass;

        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
            velocity.x *= Time.deltaTime;
            velocity.y *= Time.deltaTime;

            transform.position += (Vector3)velocity;

            
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash") || collision.CompareTag("BigTrash"))
        {
            mass += 0.5f; // Increase mass when hitting trash
        }
    }
}
