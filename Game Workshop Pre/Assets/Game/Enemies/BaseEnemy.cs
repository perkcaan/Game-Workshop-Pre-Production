using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class BaseEnemy : MonoBehaviour
{

   
    [Header("Enemy Movement")]

    private Rigidbody2D rb;

    public float direction;

    public float movementSpeed;

    // Fixed Movement
    public bool leftright; // Does the enemy move left and right, or up and down? (Irreveleant if seeksPlayer is active)d

    // Seeking Movement
    public bool seeksPlayer; // Does the enemy seek the player?

    private bool followingPlayer; // Is the enemy currently following the player?

    private Transform playerTransform;


    [Header("Enemy Stats")]

    public float attackForce; // The amount of force the enemy inflicts upon the player

    public float tumblingTimer; // For how long the tumbling status afflicts the player

    public float size; // The enemy's size

    public float resistance; // How much resistance the enemy puts up against an unattended trash ball


    [Header("Enemy Animation")]

    public Animator enemyAnimator;

    //public string moveAnimationName;

    public AnimationClip moveAnimation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (direction > 0 || direction == 0)
        {
            direction = 1;
        }
        else if (direction < 0)
        {
            direction = -1;
        }
    }

    private void FixedUpdate()
    {

        if (seeksPlayer)
        {
            SeekPlayerMovement();
        }
        else
        {
            FixedMovement();
        }

        AnimateEnemy();
    }


    #region FIXED_MOVEMENT
    private void FixedMovement()
    {
        if (leftright)
        {
            // Move left and right
            rb.velocity = new Vector2(direction * movementSpeed, 0);
        }
        else
        {
            // Move up and down
            rb.velocity = new Vector2(0, direction * movementSpeed);
        }
    }

    #endregion

    #region SEEKING_MOVEMENT
    private void SeekPlayerMovement()
    {
        direction = 0;
        if (followingPlayer)
        {
            // Follows Player
            Vector2 followPos = Vector2.MoveTowards(rb.position, playerTransform.position, movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(followPos);
        }
        // Otherwise, remain in place

    }
    #endregion

    #region ANIMATION
    private void AnimateEnemy()
    {
        // Player enemy animation
        enemyAnimator.Play(moveAnimation.name);
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!seeksPlayer && collision.gameObject.tag == "Wall")
        {
            // Reverse direction upon wall collision
            direction = -direction;

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        followingPlayer = true;
        Debug.Log(followingPlayer);
        playerTransform = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        followingPlayer = false;
        Debug.Log(followingPlayer);
        playerTransform = null;

    }
}
