using System.Collections;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public float trashSize;
    public Rigidbody2D rb;
    public CircleCollider2D trashCollider;
    public PhysicsMaterial2D bouncyMaterial;
    public float baseRadius;
    public bool isThrown = false;
    public bool isPickedUp = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trashCollider = GetComponent<CircleCollider2D>();
        baseRadius = trashCollider.radius;
    }

    public void PickUp()
    {
        rb.sharedMaterial = null;
    }

    public void Throw(Vector2 direction, float strength)
    {
        isThrown = true;
        rb.velocity = direction * strength;
        trashCollider.radius = baseRadius;
        rb.sharedMaterial = bouncyMaterial;
    }
}
