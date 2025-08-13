using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallController : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] public GameObject trashBall;
    [SerializeField] float distanceFromPlayer;
    public float trashSize;
    private float trashScale;
    private Vector2 initialPosition;
    public BallCollisionHandler BallCollisionHandler;
    [SerializeField] public Rigidbody2D trashRb;
    public bool isAttached = true;

    private void Start()
    {
        trashRb = trashBall.GetComponent<Rigidbody2D>();

    }
    void LateUpdate()
    {
        if (isAttached)
        {
            HandlePosition();
            ChangeTrashSize(0);
            // Keep the controller and ball together while attached
            trashBall.transform.position = transform.position;
            //trashBall.transform.localScale = new Vector2(1, 1);
            

        }
        
        LaunchTrash();
    }

    void HandlePosition()
    {


        float angle = playerController.rotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        transform.localPosition = (Vector3)direction * (distanceFromPlayer + (trashScale / 2));


    }

    public void AddCollectableTrash(CollectableTrash trash)
    {
        
        
        ChangeTrashSize(trash.trashSize);
        
    }

    void ChangeTrashSize(float sizeChange)
    {

        trashSize += sizeChange;
        if (isAttached)
        {
            playerController.SetWeight(trashSize);
        }
        trashScale = (float)Math.Max(Math.Log(trashSize + 1), 0);

        // Set the controller's scale
        transform.localScale = new Vector3(trashScale, trashScale, trashSize);

        SyncBallScale();
    }

    void LaunchTrash()
    {
        if (trashSize > 0 && isAttached)
        {
            if (Input.GetMouseButtonDown(0))
            {
                trashRb.bodyType = RigidbodyType2D.Dynamic;

                float angle = playerController.rotation * Mathf.Deg2Rad;
                Vector2 launchDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                if(trashSize > 4)
                {
                    float launchSpeed = trashSize/2f;
                    trashRb.velocity = launchDirection * launchSpeed;

                }
                else
                {
                    float launchSpeed = trashSize * 4f; // Adjust the launch speed based on trash size
                    trashRb.velocity = launchDirection * launchSpeed;

                }


                trashBall.transform.parent = null;
                SyncBallScale(); 

                

                playerController.SetWeight(0f);
                transform.localScale = new Vector3(trashScale, trashScale, trashSize);

                isAttached = false;
            }
        }
    }

   
    public void SyncBallScale()
    {
        if (trashBall != null)
        {
            if (trashBall.transform.parent == transform)
            {
                
                trashBall.transform.localScale = Vector3.one;
            }
            else
            {
                
                trashBall.transform.localScale = transform.localScale;
            }
        }
    }
}
