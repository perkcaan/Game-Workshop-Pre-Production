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
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        




    }

    void HandleMovement()
    {
        
        float verticalInput = Input.GetAxisRaw("Vertical");
        moveInputVector = new Vector2(0, verticalInput); // Store inputVector

        if (moveInputVector.sqrMagnitude > 0)
        {
            targetVelocity = moveInputVector.normalized * moveSpeed;
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
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rotateInputVector = new Vector2(horizontalInput, 0); 
        //Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = rotateInputVector - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if(Mathf.Abs(horizontalInput) > 0.1f)
        {
            
            rotation += Mathf.LerpAngle(rotation, targetAngle, rotationSpeed * Time.deltaTime);
            
        }
        

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

    private IEnumerator RotationDelay()
    {
        yield return new WaitForSeconds(1f);
    }



}
