using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomCollisionHandler : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    private Vector2 inputVector = Vector2.zero;
    float horizontalInput;
    float verticalInput;
    [HideInInspector] public float rotation;
    float rotationSpeed = 8;
    [SerializeField] Animator playerAnimator;

    // Start is called before the first frame update  
    void Start()
    {

    }

    // Update is called once per frame  
    void Update()
    {
        float playerRotation = playerAnimator.GetFloat("Rotation");

        float angleRad = playerRotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        // Set the swipe box position relative to the player  
        transform.position = playerController.gameObject.transform.position + (Vector3)offset / 2;

        transform.rotation = Quaternion.Euler(0, 0, playerRotation + 90f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        var swipeable = collision.gameObject.GetComponent<Swipeable>();

        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            float angle = playerController.SweepForce * Mathf.Deg2Rad; // Updated to use SweepForce
            Vector2 launchDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            swipeable.OnSwiped();
            //if(ct) rb.AddForce(launchDirection * 1f/ct.trashSize);  
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        var swipeable = collision.gameObject.GetComponent<Swipeable>();
        if (rb)
        {
            //rb.gravityScale = 1;  
            rb.bodyType = RigidbodyType2D.Kinematic;
            swipeable.OnSwipeEnd();

        }
    }
}
