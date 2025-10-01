using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCollisionHandler : MonoBehaviour
{
    private Vector2 inputVector = Vector2.zero;
    float horizontalInput;
    float verticalInput;
    [HideInInspector] public float rotation;
    float rotationSpeed = 8;
    [SerializeField] Animator playerAnimator;

    [SerializeField] Transform playerTransform; // Assign in Inspector
    
    
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
        transform.position = playerTransform.position + ((Vector3)offset * 0.75f);

        transform.rotation = Quaternion.Euler(0, 0, playerRotation + 90f);
    }



    public void OnCollisionEnter2D(Collision2D collision)
    {
        var swipeable = collision.gameObject.GetComponent<Swipeable>();
        
        if (swipeable != null)
        {
            swipeable.OnSwiped();

        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        var swipeable = collision.gameObject.GetComponent<Swipeable>();
        var rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (swipeable != null && rb != null)
        {
            swipeable.OnSwipeEnd();
        }
    }

    private IEnumerator SwipeEndCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

    }
}

