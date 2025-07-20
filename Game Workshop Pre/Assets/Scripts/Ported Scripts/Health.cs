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
