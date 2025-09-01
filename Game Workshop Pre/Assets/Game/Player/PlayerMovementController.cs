using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public Animator spriteAnimator;
    [Header("Base Movement")]
    [SerializeField] float baseMaxSpeed;
    [SerializeField] float baseAcceleration;
    [SerializeField] float baseRotationSpeed;
    [Header("Movement with Ball")]
    [SerializeField] float speedReduction;
    [SerializeField] float accelerationReduction;
    [SerializeField] float rotationReduction;

    float maxSpeed = 0;
    float acceleration = 0;
    float rotationSpeed = 0;

    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Rigidbody2D rb;
    private Camera mainCamera;
    public float rotation;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
        rotationSpeed = baseRotationSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput).normalized;

        Vector2 force = inputVector * acceleration;

        // Apply the force to the rigidbody
        rb.AddForce(force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }

        // Update the animator
        spriteAnimator.SetFloat("Speed", rb.velocity.magnitude);
    }

    // makes the player gradually rotates towards the mouse
    void HandleRotation()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotation = Mathf.LerpAngle(rotation, targetAngle, rotationSpeed * Time.deltaTime);

        rotation %= 360;
        if (rotation <= -180) rotation += 360;
        else if (rotation > 180) rotation -= 360;

        spriteAnimator.SetFloat("Rotation", rotation);
    }

    // multiplies moveSpeed, acceleration and rotationSpeed
    public void AddWeight(float weight)
    {
        //maxSpeed += weight;
    }

    public void RemoveWeight(float weight)
    {
        //maxSpeed -= weight;
    }
}
