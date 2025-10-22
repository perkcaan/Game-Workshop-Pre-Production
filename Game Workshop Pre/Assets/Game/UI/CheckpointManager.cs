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
        else { ManualCheckpointSelect(); }
    }

    private void SetActiveCheckpoint() // Automatically sets the active checkpoint
    {
        activeCheckpoint = checkpoints[checkpoints.Count - 1];
    }

    private void ManualCheckpointSelect() // The player chooses their own checkpoint
    {
        // Open menu with list of checkpoints****
        // Freeze player controls****
        // Player selects from one of the checkpoints****
    }

}
