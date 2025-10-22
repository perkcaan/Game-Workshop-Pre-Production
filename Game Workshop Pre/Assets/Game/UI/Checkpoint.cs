using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Room assignedRoom;

    private CheckpointManager Checkpoint_Manager;

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
        if (assignedRoom.Cleanliness >= 1)
        {
            Checkpoint_Manager.checkpoints.Add(this);
        }
    }

}