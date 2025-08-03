using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 StartingPoint;
    public int speed = 5;
    private Rigidbody2D rb;

    // Reference to the Health component
    public Health playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        StartingPoint = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(this.transform.position, StartingPoint) > 4)
        {
            speed = -speed;
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, speed);
        }
        else
        {
            speed = 5;
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
               StartCoroutine(hitDelay());
            
        }
    }

    private IEnumerator hitDelay()
    {
        playerHealth.currentHealth -= 1;
        Debug.Log("Player hit! Current health: " + playerHealth.currentHealth);
        yield return new WaitForSeconds(0.5f);
        
    }
}
