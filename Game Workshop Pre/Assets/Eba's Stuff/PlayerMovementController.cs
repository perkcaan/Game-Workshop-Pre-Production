using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationTime;
    Vector2 currentVelocity = Vector2.zero; // Current velocity
    Vector2 targetVelocity = Vector2.zero;  // Target velocity
    float accelerationRate;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        accelerationRate = moveSpeed / accelerationTime;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        targetVelocity = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, accelerationRate * Time.deltaTime);

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        rb.velocity = currentVelocity;
    }
}
