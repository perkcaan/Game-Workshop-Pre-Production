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
        checkpoints.Clear(); // Makes sure list starts empty
    }

    private void Update()
    {
        if (!respawnChoice) { AutoSetActiveCheckpoint(); }
        
    }

    private void AutoSetActiveCheckpoint() // Automatically sets the active checkpoint
    {
        if (!respawnChoice && (activeCheckpoint != checkpoints[checkpoints.Count - 1]))
        {
            activeCheckpoint = checkpoints[checkpoints.Count - 1];
            //Debug.Log("New Active Checkpoint");
        }
    }

    public void SetActiveCheckpoint(Checkpoint c) // The player can select a checkpoint at any time
    {
        activeCheckpoint = c;
    }

}
