using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Animator spriteAnimator;
    [SerializeField] AudioSource parryReady;// Added

    
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
    public Collision2D playerCollider;
    public bool swiping; //Added
    public GameObject swipeBox; //Added
    public GameObject BroomBox; //Added
    public bool sweeping; // Added

    [HideInInspector] public float rotation;

    [SerializeField] float swipeCooldownDuration = 1f; //Added
    public bool canSwipe = true;//Added

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
        Swipe();
        Sweep();

       
        if (!canSwipe)
        {
            swipeCooldownDuration -= Time.deltaTime;
            if (swipeCooldownDuration <= 0f)
            {
                canSwipe = true;
                Debug.Log("Swipe ready");
                swipeCooldownDuration = 3f; // Reset cooldown
                if(canSwipe)
                FMODUnity.RuntimeManager.PlayOneShot("event:/Swipe Recharge");
            }
        }


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

    public void Swipe()
    {
        if (canSwipe && Input.GetKeyDown(KeyCode.Space))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Swipe/Swipe");
            swiping = true;
            swipeBox.SetActive(true);
            canSwipe = false;
            StartCoroutine(StopSwiping());

        }
    }

    public IEnumerator StopSwiping()
    {
        yield return new WaitForSeconds(0.5f);
        swiping = false;
        swipeBox.SetActive(false);

        
    }

    public void Sweep()
    {
        if (Input.GetKeyDown(KeyCode.E) && !sweeping)
        {
            sweeping = true;
            BroomBox.gameObject.SetActive(true);
            spriteAnimator.SetBool("Sweeping", true);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            sweeping = false;
            BroomBox.gameObject.SetActive(false);
            spriteAnimator.SetBool("Sweeping", false);
        }
    }


}
