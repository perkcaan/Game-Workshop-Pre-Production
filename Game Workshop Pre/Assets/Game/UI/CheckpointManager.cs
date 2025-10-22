using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour // One for each level
{
    public List<Checkpoint> checkpoints;

    public Checkpoint activeCheckpoint;

    public bool respawnChoice; // If true, the player chooses where to respawn

    private void Start()
    {
        activeCheckpoint = checkpoints[0];
    }

    private void Update()
    {
        if (!respawnChoice) { SetActiveCheckpoint(); }
        
    }

    private void SetActiveCheckpoint() // Automatically sets the active checkpoint
    {
        activeCheckpoint = checkpoints[checkpoints.Count - 1];


        Debug.Log("New Checkpoint Gained");
    }

    public void ManualCheckpointSelect(Checkpoint c) // The player can select a checkpoint at any time
    {
        activeCheckpoint = c;
    }

}
