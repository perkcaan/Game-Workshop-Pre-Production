using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public TextMeshProUGUI healthText;
    public Collider2D playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"Health: {currentHealth}/{ maxHealth}";

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player has died!");
            // You can add more logic here, like restarting the level or showing a game over screen
            Destroy(gameObject); // Destroy the player object
            Application.Quit(); // Quit the application (for standalone builds)
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision involves the player's specific collider
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider == playerCollider)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    currentHealth--;
                }
                break;
            }
        }
    }


}
