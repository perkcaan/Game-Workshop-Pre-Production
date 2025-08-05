using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallController : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] TrashBall trashBall;
    [SerializeField] float distanceFromPlayer;
    public float trashSize;
    private float trashScale;
    private Vector2 initialPosition;

    private void Start()
    {
       
        
    }
    void LateUpdate()
    {
        HandlePosition();
        ChangeTrashSize(0);

        


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
        playerController.SetWeight(trashSize);
        trashScale = (float)Math.Max(Math.Log(trashSize + 1), 0);
        transform.localScale = new Vector3(trashScale, trashScale, trashSize);
    }

    
}
