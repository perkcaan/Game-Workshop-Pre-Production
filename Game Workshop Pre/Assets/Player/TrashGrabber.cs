using System;
using System.Collections.Generic;
using UnityEngine;

public class TrashGrabber : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] float distanceFromPlayer; // distance the trash ball is from the player
    [SerializeField] float trashBallCompaction; // how much the trash ball compacts into an actual ball
    [SerializeField] float maxGrabForce; // strength of the grabber gravitation pull
    [SerializeField] float throwForce; // players throwing force after charging up
    public float trashSize; 
    private float trashScale;
    private float chargingForce = 0;
    private float trashCompacting = 1;
    private List<CollectableTrash> trashInRange = new List<CollectableTrash>();
    private CircleCollider2D grabCollider;

    void Start()
    {
        grabCollider = GetComponent<CircleCollider2D>();
        SetTrashSize(1);
    }

    void Update()
    {
        HandlePosition();
        PlayerInput();
    }

    void HandlePosition()
    {
        float angle = playerController.rotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        transform.localPosition = (Vector3)direction * (distanceFromPlayer + (trashScale/2));
    }

    void PlayerInput()
    {
        // when holding down the mouse, collect trash around you
        if (Input.GetMouseButton(0))
        {
            GrabTrash();
            // when holding space start charging up a throw
            if (Input.GetKey(KeyCode.Space))
            {
                chargingForce += Time.deltaTime;
                chargingForce = Mathf.Min(chargingForce, 1.5f);
            }
            // after releasing space throw the trash forward
            if (Input.GetKeyUp(KeyCode.Space))
            {
                ThrowTrash();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DropTrash();
        }
    }

    void GrabTrash()
    {
        Vector2 grabCenter = transform.position;
        float grabRadius = grabCollider.radius * transform.lossyScale.x;
        float totalTrashSize = 1;
        
        foreach (CollectableTrash trash in trashInRange)
        {
            if (!trash.isThrown)
            {
                trash.PickUp();
                Rigidbody2D trashRb = trash.rb;
                totalTrashSize += trash.trashSize;
                trash.trashCollider.radius = trash.baseRadius / trashCompacting;

                float distance = Vector2.Distance(trashRb.position, grabCenter);
                float t = Mathf.Clamp01(distance / grabRadius);
                float grabForce = Mathf.Lerp(0, maxGrabForce, t);

                Vector2 direction = (grabCenter - trashRb.position).normalized;
                trashRb.velocity = direction * grabForce;
            }
        }
        SetTrashSize(totalTrashSize); 
    }

    void DropTrash()
    {
        foreach (CollectableTrash trash in trashInRange)
        {
            if (!trash.isThrown) trash.trashCollider.radius = trash.baseRadius;
        }   
        SetTrashSize(1);
    }

    void ThrowTrash()
    {
        if (chargingForce > 0f)
        {
            Vector2 forwardDirection = Quaternion.Euler(0, 0, playerController.rotation - 90) * Vector2.up;
            foreach (CollectableTrash trash in trashInRange)
            {
                trash.Throw(forwardDirection, chargingForce * throwForce);
            }
            chargingForce = 0;
        }
        SetTrashSize(1);
    }
    
    // When touching trash, add it to the list of trash in range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            if (!trashInRange.Contains(collectableTrash))
            {
                collectableTrash.isThrown = false;
                trashInRange.Add(collectableTrash);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            if (trashInRange.Contains(collectableTrash))
            {
                if (!collectableTrash.isThrown) collectableTrash.trashCollider.radius = collectableTrash.baseRadius;
                trashInRange.Remove(collectableTrash);
            }
        }
    }

    void SetTrashSize(float newSize)
    {
        trashSize = newSize;
        playerController.SetWeight(trashSize);
        trashScale = (float)Math.Max(Math.Log(trashSize + 1), 0);
        trashCompacting = 1 + (trashScale + chargingForce) * trashBallCompaction;
        transform.localScale = new Vector3(trashScale, trashScale, trashSize);
    }
}
    