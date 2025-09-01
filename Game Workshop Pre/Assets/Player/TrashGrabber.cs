using System;
using System.Collections.Generic;
using UnityEngine;

public class TrashGrabber : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] float throwForce; // players push force when clicking space
    [SerializeField] float distanceFromPlayer;
    private List<CollectableTrash> trashInRange = new List<CollectableTrash>();
    private CircleCollider2D grabCollider;

    void Start()
    {
        grabCollider = GetComponent<CircleCollider2D>();
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
        transform.localPosition = (Vector3)direction * distanceFromPlayer;
    }

    void PlayerInput()
    {
        // when holding down the mouse push trash forward
        if (Input.GetMouseButton(0))
        {
            PushTrash();
            if (Input.GetKey(KeyCode.Space))
            {
                ThrowTrash();
            }
        }
    }

    void PushTrash()
    {
        playerController.spriteAnimator.SetBool("Sweeping", trashInRange.Count > 1);
        Vector2 forwardDirection = Quaternion.Euler(0, 0, playerController.rotation - 90) * Vector2.up;
        foreach (CollectableTrash trash in trashInRange)
        {
            trash.Throw(forwardDirection, throwForce);
        }
    }

    void ThrowTrash()
    {
        Vector2 forwardDirection = Quaternion.Euler(0, 0, playerController.rotation - 90) * Vector2.up;
        foreach (CollectableTrash trash in trashInRange)
        {
            trash.Throw(forwardDirection, throwForce);
        }
    }

    // When touching trash, add it to the list of trash in range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            if (!trashInRange.Contains(collectableTrash))
            {
                trashInRange.Add(collectableTrash);
                playerController.AddWeight(collectableTrash.trashSize);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            if (trashInRange.Contains(collectableTrash))
            {
                trashInRange.Remove(collectableTrash);
                playerController.RemoveWeight(collectableTrash.trashSize);
            }
        }
    }
    
}
    