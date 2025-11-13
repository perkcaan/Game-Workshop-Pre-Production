using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyProjectile : MonoBehaviour
{
    private Transform playerTransform;

    Rigidbody2D rb;

    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        Vector2 targetPos = playerTransform.position;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        rb.velocity = direction * speed;

        Destroy(gameObject, 5f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Apply force in the player's direction

            Destroy(gameObject);
        }
    }

}
