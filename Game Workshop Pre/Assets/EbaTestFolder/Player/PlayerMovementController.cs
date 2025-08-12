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
    [SerializeField] float baseRotationSpeed;
    [Header("Movement with Ball")]
    [SerializeField] float speedReduction;
    [SerializeField] float accelerationReduction;
    [SerializeField] float rotationReduction;

    public float moveSpeed = 0;
    float acceleration = 0;
    float rotationSpeed = 0;
    private Vector2 inputVector = Vector2.zero;

    public  Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private Rigidbody2D rb;
    private Camera mainCamera;
    public TrashBallController trashBallController;

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
        inputVector = new Vector2(horizontalInput, verticalInput); // Store inputVector

        if (inputVector.sqrMagnitude > 0)
        {
            targetVelocity = inputVector.normalized * moveSpeed;
        }
        else
        {
            targetVelocity = Vector2.zero;
        }

        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, moveSpeed / acceleration * Time.deltaTime);
        spriteAnimator.SetFloat("Speed", currentVelocity.magnitude);
        rb.velocity = currentVelocity;
    }

    
    void HandleRotation()
    {
        if (inputVector.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            rotation = Mathf.LerpAngle(rotation, targetAngle, rotationSpeed * Time.deltaTime);

            rotation %= 360;
            if (rotation <= -180) rotation += 360;
            else if (rotation > 180) rotation -= 360;

            spriteAnimator.SetFloat("Rotation", rotation);
        }
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

        if (collision.CompareTag("EndPoint"))
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

    

    
}
