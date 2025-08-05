using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Animator spriteAnimator;
    [Header("Base Movement")]
    [SerializeField] float baseMoveSpeed;
    [SerializeField] float baseAcceleration;
    [SerializeField] float baseRotationSpeed;
    [Header("Movement with Ball")]
    [SerializeField] float speedReduction;
    [SerializeField] float accelerationReduction;
    [SerializeField] float rotationReduction;

    float moveSpeed = 0;
    float acceleration = 0;
    float rotationSpeed = 0;

    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Rigidbody2D rb;
    private Camera mainCamera;
    [HideInInspector] public float rotation;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        SetWeight(0);
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
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);

        if (inputVector.sqrMagnitude > 0) {
            targetVelocity = inputVector.normalized * moveSpeed;
        } else {
            targetVelocity = Vector2.zero;
        }

        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, moveSpeed / acceleration * Time.deltaTime);
        spriteAnimator.SetFloat("Speed", currentVelocity.magnitude);
        rb.velocity = currentVelocity;
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
    public void SetWeight(float weight)
    {
        moveSpeed = baseMoveSpeed / (1 + weight * speedReduction);
        acceleration = baseAcceleration / (1 + weight * accelerationReduction);
        rotationSpeed = baseRotationSpeed / (1 + weight * rotationReduction);
        spriteAnimator.SetBool("Sweeping", weight > 0);
        
    }
}
