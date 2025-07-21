using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationTime;
    [SerializeField] float rotationSpeed;
    [SerializeField] Animator spriteAnimator;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private float accelerationRate;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private float rotation;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        accelerationRate = moveSpeed / accelerationTime;
        mainCamera = Camera.main;
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

        targetVelocity = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, accelerationRate * Time.deltaTime);

        spriteAnimator.SetFloat("Speed", currentVelocity.magnitude);
        rb.velocity = currentVelocity;
    }

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
    
}
