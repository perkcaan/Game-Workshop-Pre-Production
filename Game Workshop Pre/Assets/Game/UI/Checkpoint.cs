using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Room assignedRoom;

    private CheckpointManager Checkpoint_Manager;

    private bool checkGained = false;

    public bool starterPoint;

    private void Start()
    {
        Checkpoint_Manager = FindObjectOfType<CheckpointManager>();

        if (Checkpoint_Manager == null)
        {
            Debug.LogWarning("No CheckpointManager found");
        }
    }

    private void Update()
    {
        if ((assignedRoom.Cleanliness == 1 && !checkGained) || (starterPoint && !checkGained))
        {
            
            Checkpoint_Manager.checkpoints.Add(this);
            checkGained = true;

            Debug.Log("Checkpoint Gained");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Checkpoint_Manager.respawnChoice && Checkpoint_Manager.activeCheckpoint != this)
        {
            Checkpoint_Manager.ManualCheckpointSelect(this);
        }
    }

}