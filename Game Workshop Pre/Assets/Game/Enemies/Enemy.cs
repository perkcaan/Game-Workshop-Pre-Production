using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,Swipeable
{
    public Vector2 StartingPoint;
    public int speed = 5;
    private Rigidbody2D rb;
    [SerializeField] PlayerMovementController playerController;

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

    public void OnSwiped()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        float angle = playerController.rotation * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        rb.AddForce(launchDirection * 10f); // Adjust force multiplier as needed
    }

    public void OnSwipeEnd()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator SwipedEndCoroutine()
    {
        throw new System.NotImplementedException();
    }
}
