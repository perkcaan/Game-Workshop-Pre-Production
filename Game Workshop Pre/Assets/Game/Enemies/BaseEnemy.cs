using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IAbsorbable
{


    [Header("Enemy Movement")]

    private Rigidbody2D rb;

    public float direction;

    public float movementSpeed;

    // Fixed Movement
    public bool patrol; // Does the enemy move back and forth>

    public bool leftright; // Does the enemy move left and right, or up and down?

    // Seeking Movement
    public bool seeksPlayer; // Does the enemy seek the player?

    private bool followingPlayer; // Is the enemy currently following the player?

    private Transform playerTransform;


    [Header("Enemy Stats")]

    public float attackForce; // The amount of force the enemy inflicts upon the player

    public float tumblingTimer; // For how long the tumbling status afflicts the player

    public float size; // The enemy's size

    public float resistance; // How much resistance the enemy puts up against an unattended trash ball

    public float trashBallEscapingPower;


    [Header("Trash Spawning")]

    public bool canMakeTrash;

    public float trashSpawnTimer;

    public CollectableTrash[] trash;


    [Header("Projectile")]

    public bool canFireProjs;

    public float shootTime;

    public EnemyProjectile proj;


    [Header("Enemy Animation")]

    public Animator enemyAnimator;

    //public string moveAnimationName;

    public AnimationClip moveAnimation;
    private bool isAbsorbed;


    [Header("Score Parameters")]
    [SerializeField] int absorbScoreVal;
    [SerializeField] int destroyScoreVal;

    public static event Action<int> ScoreEvent;

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

        if (canMakeTrash)
        {
            StartCoroutine(TrashSpawner());
        }
        if (canFireProjs)
        {
            StartCoroutine(FireProjs());
        }
    }

    private void FixedUpdate()
    {
        if (!isAbsorbed)
        {
            if (seeksPlayer && !patrol)
            {
                SeekPlayerMovement();
            }
            else if (!seeksPlayer && patrol)
            {
                FixedMovement();
            }
            else if (seeksPlayer && patrol)
            {
                //Debug.Log("Fixed and Search");
                SeekAndFixedMovement();
            } 
            AnimateEnemy();
        }
    }

    private void OnDestroy()
    {
        if (!isAbsorbed)
            ScoreEvent.Invoke(destroyScoreVal);
        else
            ScoreEvent.Invoke(destroyScoreVal * 2);
    }


    #region MOVEMENT
    private void FixedMovement()
    {
        if (leftright)
        {
            //Debug.Log("Moving Left and Right");
            // Move left and right
            rb.velocity = new Vector2(direction * movementSpeed, 0);
        }
        else
        {
            // Move up and down
            rb.velocity = new Vector2(0, direction * movementSpeed);
        }
    }

    private void SeekPlayerMovement()
    {
        if (!patrol)
        {
            direction = 0;
        }

        if (followingPlayer)
        {

            // Follows Player
            Vector2 followPos = Vector2.MoveTowards(rb.position, playerTransform.position, movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(followPos);
        }
        // Otherwise, remain in place

    }

    private void SeekAndFixedMovement()
    {
        if (followingPlayer)
        {
            SeekPlayerMovement();
        }
        else if (!followingPlayer)
        {
            //Debug.Log("Fixed Active");
            FixedMovement();
        }
    }

    #endregion

    #region ANIMATION
    private void AnimateEnemy()
    {
        // Player enemy animation
        enemyAnimator.Play(moveAnimation.name);
    }
    #endregion

    #region

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!seeksPlayer && collision.gameObject.tag == "Wall"
            || seeksPlayer && patrol && collision.gameObject.tag == "Wall")
        {
            // Reverse direction upon wall collision
            direction = -direction;

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        followingPlayer = true;
        //Debug.Log(followingPlayer);
        playerTransform = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        followingPlayer = false;
        //Debug.Log(followingPlayer);
        playerTransform = null;

    }


    private IEnumerator TrashSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(trashSpawnTimer); // wait 4 seconds

            // Define directions
            Vector2[] directions = new Vector2[]
            {
                Vector2.right,
                Vector2.left,
                Vector2.up,
                Vector2.down
            };

            bool trashDetected = false;

            // Check each direction with a raycast
            foreach (Vector2 dir in directions)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f);
                // 1f = distance, adjust to how close trash can be

                if (hit.collider != null && LayerMask.LayerToName(hit.collider.gameObject.layer) == "Trash")
                {
                    trashDetected = true;
                    break;
                }
            }

            // If no trash detected nearby, spawn one
            if (!trashDetected)
            {
                //Debug.Log("Spawn");
                int randomType = UnityEngine.Random.Range(0, trash.Length);
                Instantiate(trash[randomType], transform.position, Quaternion.identity);
            }
        }
    }

    private IEnumerator FireProjs()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootTime);

            if (playerTransform != null)
            {
                Instantiate(proj, transform.position, Quaternion.identity);
            }
        }

    }

    // eba changes

    public void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        if (forcedAbsorb || absorbingPower > resistance)
        {
            ScoreEvent?.Invoke(absorbScoreVal);
            trashBall.absorbedObjects.Add(this);
            isAbsorbed = true;
            gameObject.SetActive(false);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        transform.position = trashBall.transform.position;
        float explosionForce = 1 + (1 * trashBall.absorbedObjects.Count);
        Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce));

        rb.AddForce(randomForce);
    }

    public void OnTrashBallIgnite()
    {
        Destroy(gameObject);
    }

    #endregion
}
