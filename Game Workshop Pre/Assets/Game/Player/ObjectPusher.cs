using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPusher : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] float throwForce; // players push force when clicking space
    [SerializeField] float distanceFromPlayer;
    private List<PushableObject> objectsInRange = new List<PushableObject>();
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
                // sweep
            }
        }
    }

    void PushTrash()
    {
        playerController.spriteAnimator.SetBool("Sweeping", objectsInRange.Count > 1);
        Vector2 forwardDirection = Quaternion.Euler(0, 0, playerController.rotation - 90) * Vector2.up;
        foreach (PushableObject pushableObject in objectsInRange)
        {
            pushableObject.Throw(forwardDirection, throwForce);
        }
    }

    // When touching trash, add it to the list of trash in range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PushableObject pushableObject))
        {
            if (!objectsInRange.Contains(pushableObject))
            {
                objectsInRange.Add(pushableObject);
                playerController.AddWeight(pushableObject.weight);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PushableObject pushableObject))
        {
            if (objectsInRange.Contains(pushableObject))
            {
                objectsInRange.Remove(pushableObject);
                playerController.RemoveWeight(pushableObject.weight);
            }
        }
    }
    
}
    