using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Animator spriteAnimator;
    [Header("Base Movement")]
    [SerializeField] public float baseMoveSpeed;
    [SerializeField] float baseAcceleration;
    [SerializeField] float baseRotationSpeed = 10f; 
    [Header("Movement with Ball")]
    [SerializeField] float speedReduction;
    [SerializeField] float accelerationReduction;
    [SerializeField] float rotationReduction;

    public float moveSpeed = 0;
    float acceleration = 0;
    float rotationSpeed = 0;
    private Vector2 moveInputVector = Vector2.zero;
    public Vector2 rotateInputVector = Vector2.zero; 


    public  Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Rigidbody2D rb;
    private Camera mainCamera;
    public TrashBallController trashBallController;
    public Collision2D playerCollider;

    [HideInInspector] public float rotation;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        SetWeight(0);
        rotationSpeed = baseRotationSpeed; // Initialize rotation speed
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        




    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxisRaw("Vertical"); 

        // Calculate the forward direction based on the player's rotation (in degrees)
        float angleRad = rotation * Mathf.Deg2Rad;
        Vector2 forward = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        // Only add force if there is input
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
           
            rb.AddForce(forward * verticalInput * moveSpeed, ForceMode2D.Force);
        }

        // Clamp velocity to prevent excessive speed
        float maxPlayerSpeed = 8f;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxPlayerSpeed);
        currentVelocity = rb.velocity;

        spriteAnimator.SetFloat("Speed", rb.velocity.magnitude);
    }

    
    void HandleRotation()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            
            float lerpFactor = Mathf.Clamp01(rotationSpeed * Time.deltaTime); 
            float targetAngle = rotation + horizontalInput * 90f;
            rotation -= horizontalInput * rotationSpeed * Time.deltaTime;

        }

        // Clamp rotation to [-180, 180]
        rotation %= 360;
        if (rotation <= -180) rotation += 360;
        else if (rotation > 180) rotation -= 360;

        spriteAnimator.SetFloat("Rotation", rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrashBall"))
        {
            Debug.Log("Reattached to ball");
            trashBallController.isAttached = true;
            trashBallController.trashBall.transform.parent = transform;
            trashBallController.trashRb.bodyType = RigidbodyType2D.Kinematic;
            trashBallController.SyncBallScale();
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EndPoint"))
        {
            SceneManager.LoadScene("RebuildScene");
        }
    }

    // multiplies moveSpeed, acceleration and rotationSpeed
    public void SetWeight(float weight)
    {
        moveSpeed = baseMoveSpeed / (1 + weight * speedReduction);
        acceleration = baseAcceleration / (1 + weight * accelerationReduction);
        rotationSpeed = baseRotationSpeed / (1 + weight * rotationReduction);
        spriteAnimator.SetBool("Sweeping", weight > 0);
        
    }

    //private IEnumerator RotationDelay()
    //{
    //    float horizontalInput = Input.GetAxisRaw("Horizontal");
    //    float lerpFactor = Mathf.Clamp01(rotationSpeed * Time.deltaTime);

    //    Vector2 direction = rotateInputVector - (Vector2)transform.position;
    //    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    //    if (Mathf.Abs(horizontalInput) > 0.5f)
    //    {
            
    //        rotation += Mathf.LerpAngle(rotation, targetAngle, rotationSpeed * Time.deltaTime);
            
    //    }


    //    rotation %= 360;

    //    if (rotation <= -180) rotation += 360;


    //    else if (rotation > 180) rotation -= 360;
            
        


    //    spriteAnimator.SetFloat("Rotation", rotation);
        

    //}



}
